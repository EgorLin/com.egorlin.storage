using EgorLin.Storage.Save;
using EgorLin.Storage.Serializer;
using EgorLin.Storage.StateMigrations;

namespace EgorLin.Storage
{
	public interface IStateHandle
	{
		SaveDomain Domain { get; }
		int CurrentVersion { get; }
		string Key { get; }
		void Migrate(StateMigration stateMigration);
		string Save(ISerializerSave serializer);
		void Load(ISerializerSave serializer, string value);
	}
}
