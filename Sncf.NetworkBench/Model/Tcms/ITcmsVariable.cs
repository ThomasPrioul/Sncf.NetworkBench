namespace Sncf.NetworkBench.Model.Tcms
{
    public interface ITcmsVariable
    {
        //bool IsMvb { get; }
        //bool IsIP { get; }

        bool BoolValue { get; set; }
        string Name { get; }
        string FullName { get; }
        ulong Value { get; set; }
    }
}
