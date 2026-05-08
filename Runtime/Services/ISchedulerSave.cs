namespace EgorLin.Storage.Services
{
	public interface ISchedulerSave
	{
		void Save(IStateHandle handle);
		void Load(IStateHandle handle);
	}
}
