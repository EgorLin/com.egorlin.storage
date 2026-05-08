using UnityEngine;

namespace EgorLin.Storage.Storage
{
	public class StorageLocalPlayerPrefs : IStorageLocal
	{
		public bool Has(string key)
		{
			return PlayerPrefs.HasKey(key);
		}
		
		public void Set(string key, string value)
		{
			PlayerPrefs.SetString(key, value);
		}

		public string Get(string key)
		{
			return PlayerPrefs.GetString(key);
		}

		public void Save()
		{
			PlayerPrefs.Save();
		}
	}
}
