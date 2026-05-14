using EgorLin.Storage.Save;

namespace EgorLin.Storage.Data.Configs
{
	public class ConfigSaveService
	{
		public readonly SaveDomain Domain;
		public readonly float Cooldown;
		public readonly string Key;

		public ConfigSaveService(SaveDomain domain, float cooldown, string key)
		{
			Domain = domain;
			Cooldown = cooldown;
			Key = key;
		}
	}
}
