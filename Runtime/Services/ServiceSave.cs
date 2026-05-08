using System;
using System.Collections.Generic;
using EgorLin.Storage.Save;
using EgorLin.Storage.Serializer;
using EgorLin.Storage.StateMigrations;
using EgorLin.Storage.Storage;
using UnityEngine;

namespace EgorLin.Storage.Services
{
	public class ServiceSave : IServiceSave, ISchedulerSave, ITickable
	{
		private const float TotalCooldownLocal = 1f;
		private const string SaveDocumentLocal = "save_document";

		private readonly IStorageLocal _storageLocal;
		private readonly ISerializerSave _serializerSave;
		private readonly HashSet<IStateHandle> _handlersSaveLocal = new();

		private SaveDocument _saveDocumentLocal;
		
		private float _currentCooldownLocal;

		public ServiceSave(IStorageLocal storageLocal, ISerializerSave serializerSave)
		{
			_storageLocal = storageLocal;
			_serializerSave = serializerSave;
		}

		public void LoadDocument()
		{
			if (_storageLocal.Has(SaveDocumentLocal))
			{
				var rawSaveDocument = _storageLocal.Get(SaveDocumentLocal);
				_saveDocumentLocal = _serializerSave.Deserialize<SaveDocument>(rawSaveDocument);
			}
			else
			{
				_saveDocumentLocal = new SaveDocument();
			}
		}
			
		public void Save(IStateHandle handle)
		{
			if (handle.Domain == SaveDomain.Local)
			{
				_handlersSaveLocal.Add(handle);
			}
		}

		public void Load(IStateHandle handle)
		{
			if (_saveDocumentLocal.MapStates.TryGetValue(handle.Key, out var data))
			{
				handle.Load(_serializerSave, data);
				
				var stateMigration = _saveDocumentLocal.MapMigration[handle.Key];

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

		public void Tick()
		{
			_currentCooldownLocal += Time.deltaTime;

			if (_currentCooldownLocal >= TotalCooldownLocal && _handlersSaveLocal.Count > 0)
			{
				_currentCooldownLocal = 0f;

				SaveStatesLocal();
			}
		}

		private void SaveStatesLocal()
		{
			foreach (var stateHandle in _handlersSaveLocal)
			{
				var data = stateHandle.Save(_serializerSave);
					
				_saveDocumentLocal.MapStates[stateHandle.Key] = data;
			}

			_saveDocumentLocal.BinaryTimestampUtc = DateTime.UtcNow.ToBinary();
				
			var saveDocumentSerialized = _serializerSave.Serialize(_saveDocumentLocal);
				
			_storageLocal.Set(SaveDocumentLocal, saveDocumentSerialized);
			_storageLocal.Save();
				
			_handlersSaveLocal.Clear();
		}

		private void Migrate(IStateHandle stateHandle, StateMigration stateMigration)
		{
			stateHandle.Migrate(stateMigration);
					
			_saveDocumentLocal.MapMigration[stateHandle.Key] = stateMigration;
					
			_handlersSaveLocal.Add(stateHandle);
		}
	}
}