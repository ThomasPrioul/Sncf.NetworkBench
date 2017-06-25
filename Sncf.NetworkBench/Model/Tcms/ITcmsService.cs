using Sncf.NetworkBench.Model.Simulator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sncf.NetworkBench.Model.Tcms
{
    public interface ITcmsService : ISimulatorComponent
    {
        //List<IMvbDriver> MvbDrivers { get; }

        IEnumerable<ITcmsVariable> Variables { get; }

        ITcmsVariable this[string variablePath] { get; }
    }
}
