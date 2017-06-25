using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sncf.NetworkBench.Model.Scripting;
using Sncf.NetworkBench.Model.Simulator;
using Sncf.NetworkBench.Model.Tcms;
using Sncf.NetworkBench.Server.Model.Simulator;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sncf.NetworkBench.Server
{
    class Program
    {
        #region Fields

        readonly CommandLineApplication app;
        readonly CommandArgument fileArgument;
        readonly ILogger<Program> log;

        #endregion

        Program()
        {
            app = new CommandLineApplication { Invoke = Run, FullName = "Sncf.NetworkBench.Core", Name = "server.exe" };
            app.HelpOption("-? | -h | --help");
            app.VersionOption("-v | --version", "1.0.0.0");
            fileArgument = app.Argument("file", "config file to use for simulation");

            log = new LoggerFactory().AddConsole(LogLevel.Information, includeScopes: true).CreateLogger<Program>();
        }

        #region Methods

        int Execute(string[] args)
        {
            return app.Execute(args);
        }

        IConfiguration GetConfiguration(FileInfo file)
        {
            log.LogInformation("Loading config file \"{filepath}\"", file.FullName);
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["Logging:IncludeScopes"] = "true",
                    ["Logging:LogLevel:Default"] = "Information"
                })
                .AddJsonFile(file.FullName)
                .Build();
            log.LogInformation("Loaded config file \"{filepath}\"", file.FullName);

            Directory.SetCurrentDirectory(file.Directory.FullName);
            log.LogInformation("Set working directory to \"{dir}\"", file.Directory.FullName);

            return config;
        }

        IServiceProvider GetServices(IConfiguration config)
        {
            var ioc = new ServiceCollection()
                    .AddLogging()
                    .AddOptions()
                    .Configure<Scripting.Config.ScriptingServiceConfiguration>(config.GetSection("scripting"))
                    .Configure<Tcms.Config.TcmsServiceConfiguration>(config.GetSection("tcms"))
                    .AddSingleton<ITcmsService, Tcms.TcmsService>()
                    .AddSingleton<IScriptingService, Scripting.ScriptingService>()
                    .AddSingleton<ISimulator, Simulator>()
                    .BuildServiceProvider();

            // Add console logging and enable logging scopes
            ioc.GetService<ILoggerFactory>()
                .AddConsole(config.GetSection("Logging"));

            return ioc;
        }

        int Run()
        {
            app.ShowVersion();

            // No file argument
            if (fileArgument.Value is null)
            {
                app.ShowHint();
                return -1;
            }

            IServiceProvider ioc;

            // Read file contents and setup Dependency Injection
            try
            {
                var filepath = Environment.ExpandEnvironmentVariables(fileArgument.Value);
                var file = new FileInfo(filepath);
                if (!file.Exists)
                {
                    log.LogError("File does not exist: {filepath}", file.FullName);
                    return -1;
                }

                var config = GetConfiguration(file);
                ioc = GetServices(config);
            }
            catch (Exception e)
            {
                log.LogError(0, e, "Cannot read file : {filepath}", fileArgument.Value);
                return -2;
            }

            // Run simulator
            try
            {
                var simulator = ioc.GetService<ISimulator>();
                if (!simulator.Start())
                {
                    log.LogError(1, "Cannot start simulator");
                    return -3;
                }

                simulator.Tick();
                simulator.Tick();
                simulator.Tick();

                Console.ReadKey();

                log.LogInformation("Rebuilding scripts");
                if (simulator.Scripting.RebuildScripts())
                {
                    simulator.Tick();
                    simulator.Tick();
                    simulator.Tick();
                }

                simulator.Stop();
                log.LogInformation("Simulator stopped");

            }
            catch (Exception simulatorException)
            {
                log.LogError(2, simulatorException, "Error while simulating");
                return -4;
            }

            return 0;
        }

        #endregion

        static int Main(string[] args)
        {
            return new Program().Execute(args);
        }
    }
}
