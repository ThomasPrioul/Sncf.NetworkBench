namespace Sncf.NetworkBench.Model.Scripting
{
    public interface IScript
    {
        bool Running { get; set; }
        void Tick(ulong tick);
    }
}
