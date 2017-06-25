namespace Sncf.NetworkBench.Model.Simulator
{
    public interface ISimulatorComponent
    {
        bool Running { get; }

        bool Start();
        void Stop();

        void Read(ulong tick);
        void Write(ulong tick);
    }
}
