namespace EgorLin.Storage.Serializer
{
	public interface ISerializerSave
	{
		string Serialize<T>(T value);
		T Deserialize<T>(string raw);
	}
}
