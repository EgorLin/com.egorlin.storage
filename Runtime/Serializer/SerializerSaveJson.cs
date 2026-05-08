using Unity.Plastic.Newtonsoft.Json;

namespace EgorLin.Storage.Serializer
{
	public class SerializerSaveJson : ISerializerSave
	{
		public string Serialize<T>(T value)
		{
			return JsonConvert.SerializeObject(value);
		}

		public T Deserialize<T>(string raw)
		{
			return JsonConvert.DeserializeObject<T>(raw);
		}
	}
}
