namespace EgorLin.Storage.Storage
{
	public interface IStorageLocal
	{
		void Set(string key, string value);
		string Get(string key);
		void Save();
		bool Has(string key);
	}
}
