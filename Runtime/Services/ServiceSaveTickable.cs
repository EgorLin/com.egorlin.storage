#if STORAGE_DISOLATATED
using EgorLin.DIsolated.LifeCycle;

namespace EgorLin.Storage.Services
{
    public class ServiceSaveTickable :ITickable
    {
        private readonly IServiceSave _serviceSave;

        public ServiceSaveTickable(IServiceSave serviceSave)
        {
            _serviceSave = serviceSave;
        }
        
        public void Tick()
        {
            _serviceSave.Tick();
        }
    }
}
#endif