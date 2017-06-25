using Microsoft.Extensions.Logging;
using Sncf.NetworkBench.Model.Scripting;
using Sncf.NetworkBench.Model.Tcms;

namespace Sncf.NetworkBench.Scripting
{
    public abstract class Script : IScript
    {
        protected readonly ILogger log;
        protected readonly ITcmsService tcms;

        public bool Running { get; set; }

        public Script(ScriptContext context)
        {
            log = context.Log;
            tcms = context.Tcms;
        }

        public abstract void Tick(ulong tick);
    }
}
