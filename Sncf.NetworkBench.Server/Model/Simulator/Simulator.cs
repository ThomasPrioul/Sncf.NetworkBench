using Microsoft.Extensions.Logging;
using Sncf.NetworkBench.Model.Simulator;
using Sncf.NetworkBench.Model.Tcms;
using System;
using Sncf.NetworkBench.Model.Scripting;

namespace Sncf.NetworkBench.Server.Model.Simulator
{
    public class Simulator : ISimulator
    {
        #region Fields

        readonly object runLock = new object();
        readonly ILogger<Simulator> log;
        ulong tick;

        #endregion

        #region Properties

        public bool Running { get; private set; }

        public IScriptingService Scripting { get; }

        public ITcmsService Tcms { get; }

        #endregion

        public Simulator(ILogger<Simulator> log, ITcmsService tcms, IScriptingService scriptRepository)
        {
            this.log = log;
            Scripting = scriptRepository;
            Tcms = tcms;
            tick = 0;
        }

        #region Methods

        public bool Start()
        {
            if (Running)
                throw new InvalidOperationException("Cannot start an already started simulator");

            lock (runLock)
            {
                // TODO : provide lots of checks
                bool success = Tcms.Start();

                if (success)
                {
                    Running = true;
                }

                return success;
            }
        }

        public void Stop()
        {
            lock (runLock)
            {
                Tcms.Stop();
                Running = false;
            }
        }

        public void Tick()
        {
            foreach (var script in Scripting.Scripts)
            {
                if (script.Running)
                {
                    script.Tick(tick);
                }
            }

            tick++;
        }

        #endregion
    }
}
