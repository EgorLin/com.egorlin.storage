using System;
using EgorLin.Storage.Save;

namespace EgorLin.Storage
{
	[AttributeUsage(AttributeTargets.Struct)]
	public sealed class StateAttribute : Attribute {
		public SaveDomain Domain { get; }
		public string Key { get;}
		public int Version { get; }
		
		public StateAttribute(SaveDomain domain, string key, int version = 1) {
			Domain = domain;
			Key = key;
			Version = version;
		}
	}
}
