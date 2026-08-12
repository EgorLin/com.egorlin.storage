using EgorLin.Storage.Save;

namespace EgorLin.Storage.Services
{
	public interface IServiceSave
	{
		SaveDomain Domain { get; }
		void Initialize();
		void Save(IStateHandle handle);
		void Load(IStateHandle handle);
		void Tick();
	}
}
