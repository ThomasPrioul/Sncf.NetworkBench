using System.Collections.Generic;

namespace Sncf.NetworkBench.Tcms.Config
{
    public class TcmsServiceConfiguration
    {
        public Dictionary<string,string> Variables { get; set; }

        public TcmsServiceConfiguration()
        {
            Variables = new Dictionary<string, string>();
        }
    }
}
