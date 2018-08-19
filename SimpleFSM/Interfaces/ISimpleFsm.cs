using System.Threading.Tasks;

namespace SimpleFSM.Interfaces
{
    public interface ISimpleFsm<in TPayload> where TPayload: class
    {
        void Start(TPayload payload);

        Task StartAsync(TPayload payload);
    }
}