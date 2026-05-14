namespace EgorLin.Storage.Storage
{
	public interface IStorage
	{
		void Set(string key, string value);
		string Get(string key);
		void Save();
		bool Has(string key);
	}
}
