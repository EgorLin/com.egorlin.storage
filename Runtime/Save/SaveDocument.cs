using System;
using System.Collections.Generic;
using EgorLin.Storage.StateMigrations;

namespace EgorLin.Storage.Save
{
	[Serializable]
	public class SaveDocument
	{
		public string SaveId = Guid.NewGuid().ToString();
		public int FormatVersion = 1;
		public long BinaryTimestampUtc;

		public Dictionary<string, string> MapStates = new();
		public Dictionary<string, StateMigration> MapMigration = new();
	}
}
