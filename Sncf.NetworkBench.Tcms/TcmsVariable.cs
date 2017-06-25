using Sncf.NetworkBench.Model.Tcms;
using System;

namespace Sncf.NetworkBench.Tcms
{
    public class TcmsVariable : ITcmsVariable
    {
        #region Fields

        readonly string fullname;
        readonly string name;

        ulong _value;

        #endregion

        #region Properties

        public bool BoolValue
        {
            get => _value > 0;
            set => _value = Convert.ToUInt64(value);
        }

        public string Name => name;

        public string FullName => fullname;

        public ulong Value
        {
            get => _value;
            set => _value = value;
        }

        #endregion

        public TcmsVariable(string name, string fullname)
        {
            this.name = name;
            this.fullname = fullname;
        }
    }
}
