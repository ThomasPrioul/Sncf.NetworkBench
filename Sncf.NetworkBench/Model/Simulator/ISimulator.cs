using Sncf.NetworkBench.Model.Scripting;
using Sncf.NetworkBench.Model.Tcms;

namespace Sncf.NetworkBench.Model.Simulator
{
    public interface ISimulator
    {
        bool Running { get; }
        IScriptingService Scripting { get; }
        ITcmsService Tcms { get; }

        bool Start();
        void Stop();
        void Tick();
    }
}
