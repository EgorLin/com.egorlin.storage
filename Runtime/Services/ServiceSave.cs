using System.Collections.Generic;
using EgorLin.DIsolated.LifeCycle;
using EgorLin.Storage.Data.Configs;
using EgorLin.Storage.Save;
using EgorLin.Storage.Serializer;
using EgorLin.Storage.StateMigrations;
using EgorLin.Storage.Storage;
using EgorLin.Storage.Time;

namespace EgorLin.Storage.Services
{
	public class ServiceSave : IServiceSave, ITickable
	{
		private readonly ConfigSaveService _configSaveService;
		private readonly IStorage _storage;
		private readonly ISerializerSave _serializerSave;
		private readonly IProviderTime _providerTime;

		private readonly HashSet<IStateHandle> _handlersSave = new();
		
		private SaveDocument _saveDocument;
		
		private float _currentCooldown;

		public SaveDomain Domain => _configSaveService.Domain;

		public ServiceSave(ConfigSaveService configSaveService, IStorage storage, ISerializerSave serializerSave,
			IProviderTime providerTime)
		{
			_configSaveService = configSaveService;
			_storage = storage;
			_serializerSave = serializerSave;
			_providerTime = providerTime;
		}

		public void Initialize()
		{
			if (_storage.Has(_configSaveService.Key))
			{
				var rawSaveDocument = _storage.Get(_configSaveService.Key);
				_saveDocument = _serializerSave.Deserialize<SaveDocument>(rawSaveDocument);
			}
			else
			{
				_saveDocument = new SaveDocument();
			}
		}

		public void Tick()
		{
			_currentCooldown += _providerTime.DeltaTick;

			if (_currentCooldown >= _configSaveService.Cooldown && _handlersSave.Count > 0)
			{
				_currentCooldown = 0f;

				SaveStates();
			}
		}

		public void Load(IStateHandle handle)
		{
			if (_saveDocument.MapStates.TryGetValue(handle.Key, out var data))
			{
				handle.Load(_serializerSave, data);

				if (!_saveDocument.MapMigration.TryGetValue(handle.Key, out var stateMigration))
				{
					stateMigration = new StateMigration();
				}

				if (stateMigration.Version < handle.CurrentVersion)
				{
					Migrate(handle, stateMigration);
				}
			}
			else
			{
				var stateMigration = new StateMigration();
				
				Migrate(handle, stateMigration);
			}
		}

		public void Save(IStateHandle handle)
		{
			_handlersSave.Add(handle);
		}

		private void SaveStates()
		{
			foreach (var stateHandle in _handlersSave)
			{
				var data = stateHandle.Save(_serializerSave);
					
				_saveDocument.MapStates[stateHandle.Key] = data;
			}

			_saveDocument.BinaryTimestampUtc = _providerTime.TimeNowUtc.ToBinary();
				
			var saveDocumentSerialized = _serializerSave.Serialize(_saveDocument);
				
			_storage.Set(_configSaveService.Key, saveDocumentSerialized);
			_storage.Save();
				
			_handlersSave.Clear();
		}

		private void Migrate(IStateHandle stateHandle, StateMigration stateMigration)
		{
			stateHandle.Migrate(stateMigration);
					
			_saveDocument.MapMigration[stateHandle.Key] = stateMigration;
					
			_handlersSave.Add(stateHandle);
		}
	}
}