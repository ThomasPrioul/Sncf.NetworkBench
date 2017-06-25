using Microsoft.Extensions.Logging;
using Sncf.NetworkBench.Model.Tcms;

namespace Sncf.NetworkBench.Scripting
{
    public class ScriptContext
    {
        public ILogger Log { get; }

        public ITcmsService Tcms { get; }

        public ScriptContext(ILogger log, ITcmsService tcms)
        {
            Log = log;
            Tcms = tcms;
        }
    }
}
