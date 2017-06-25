using Microsoft.CSharp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sncf.NetworkBench.Model.Scripting;
using Sncf.NetworkBench.Model.Tcms;
using Sncf.NetworkBench.Scripting.Exceptions;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Sncf.NetworkBench.Scripting
{
    public class ScriptingService : IScriptingService
    {
        #region Fields

        readonly Config.ScriptingServiceConfiguration configuration;
        readonly ILoggerFactory logFactory;
        readonly ILogger<ScriptingService> log;
        readonly DirectoryInfo root;
        readonly ITcmsService tcms;

        #endregion

        #region Properties

        public List<IScript> Scripts { get; }

        #endregion

        public ScriptingService(ILoggerFactory logger,
                                IOptions<Config.ScriptingServiceConfiguration> config,
                                ITcmsService tcms)
        {
            configuration = config.Value;
            logFactory = logger;
            log = logFactory.CreateLogger<ScriptingService>();
            root = new DirectoryInfo(Environment.ExpandEnvironmentVariables(configuration.Root));
            Scripts = new List<IScript>();
            this.tcms = tcms;

            if (!root.Exists)
            {
                log.LogWarning("Directory {root} does not exist. No scripts will be provided by the ScriptingService module.", root.FullName);
            }
            else
            {
                if (!RebuildScripts())
                    throw new Exception("Could not create every type from the compiled assembly");
            }
        }

        #region Methods

        IEnumerable<string> GetAssemblyFiles(Assembly assembly)
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assemblyName in assembly.GetReferencedAssemblies())
                yield return loadedAssemblies.SingleOrDefault(a => a.FullName == assemblyName.FullName)?.Location;
        }

        Assembly CompileScripts(string[] scriptsFilesToBuild)
        {
            using (log.BeginScope("Build scripts"))
            {
                var provider = new CSharpCodeProvider();
                var compilerParameters = new CompilerParameters
                {
                    GenerateInMemory = true,
                    TreatWarningsAsErrors = false,
                };
                var appRoot = AppDomain.CurrentDomain.BaseDirectory;

                compilerParameters.ReferencedAssemblies.AddRange(new string[]
                {
                    "System.dll",
                    "System.Runtime.dll",
                    Path.Combine(appRoot, "Microsoft.Extensions.Logging.Abstractions.dll"),
                    Path.Combine(appRoot, "Sncf.NetworkBench.dll"),
                    Path.Combine(appRoot, "Sncf.NetworkBench.Scripting.dll"),
                });
                
                var compilationResult = provider.CompileAssemblyFromFile(compilerParameters, scriptsFilesToBuild);

                using (log.BeginScope("Compilation errors"))
                {
                    foreach (CompilerError error in compilationResult.Errors)
                    {
                        var str = error.ToString();

                        log.LogError(str);
                    }
                }

                if (compilationResult.Errors.Count > 0)
                    throw new CompilationFailedException($"{compilationResult.Errors.Count} errors occured during compilation.");

                return compilationResult.CompiledAssembly;
            }
        }

        string[] CollectScripts()
        {
            var scriptsToBuild = new List<string>();

            using (log.BeginScope("Collecting scripts"))
            {
                foreach (var scriptFile in root.EnumerateFiles("*.csx", SearchOption.AllDirectories))
                {
                    if (true)
                    {
                        scriptsToBuild.Add(scriptFile.FullName);
                    }
                }
            }

            return scriptsToBuild.ToArray();
        }

        public bool RebuildScripts()
        {
            bool errors = false;
            var newScripts = new List<IScript>();

            try
            {
                foreach (var type in CompileScripts(CollectScripts())
                            .ExportedTypes
                            .Where(t => t.ImplementsInterface(typeof(IScript))))
                {
                    try
                    {
                        var script = (IScript)Activator.CreateInstance(type,
                            new ScriptContext(logFactory.CreateLogger(type), tcms));

                        script.Running = true;
                        newScripts.Add(script);
                    }
                    catch (Exception activatorExc)
                    {
                        errors = true;
                        log.LogError(12345, activatorExc, "Could not create instance of {type}", type);
                    }
                }
            }
            catch (Exception exc)
            {
                errors = true;
                log.LogError(12346, exc, "Error while compilating assembly");
            }

            if (errors)
                return false;

            Scripts.Clear();
            Scripts.AddRange(newScripts);
            return true;
        }

        #endregion
    }
}
