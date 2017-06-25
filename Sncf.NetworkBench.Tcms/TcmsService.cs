using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sncf.NetworkBench.Model.Tcms;
using Sncf.NetworkBench.Tcms.Config;
using System;
using System.Collections.Generic;

namespace Sncf.NetworkBench.Tcms
{
    public class TcmsService : ITcmsService
    {
        #region Fields

        readonly TcmsServiceConfiguration configuration;
        readonly ILogger<TcmsService> log;
        bool running;
        readonly SortedDictionary<string, TcmsVariable> variables;

        #endregion

        #region Properties

        public ITcmsVariable this[string variablePath] => variables[variablePath];

        public bool Running => running;

        public IEnumerable<ITcmsVariable> Variables => variables.Values;

        #endregion

        public TcmsService(ILogger<TcmsService> logger, IOptions<TcmsServiceConfiguration> config)
        {
            log = logger;
            configuration = config.Value;
            variables = new SortedDictionary<string, TcmsVariable>();

            foreach (var variable in configuration.Variables)
            {
                variables.Add(variable.Key, new TcmsVariable(variable.Key, variable.Value));
            }
        }

        #region Methods

        public void Read(ulong tick)
        {
            throw new NotImplementedException();
        }

        public bool Start()
        {
            running = true;
            return running;
        }

        public void Stop()
        {
            running = false;
        }

        public void Write(ulong tick)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
