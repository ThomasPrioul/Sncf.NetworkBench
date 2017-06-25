using System.Collections.Generic;

namespace Sncf.NetworkBench.Model.Scripting
{
    public interface IScriptingService
    {
        List<IScript> Scripts { get; }

        bool RebuildScripts();
    }
}
