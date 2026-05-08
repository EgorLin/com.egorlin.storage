using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using EgorLin.Storage.Save;
using EgorLin.Storage.Serializer;
using EgorLin.Storage.Services;
using EgorLin.Storage.StateMigrations;

namespace EgorLin.Storage
{
    public sealed class StateHandle<TState> : IStateHandle where TState : struct, IState
    {
        private readonly ISchedulerSave _schedulerSave;

        private TState _value;
        private StateAttribute _attribute;
        private List<Migration<TState>> _migrationSteps;

        private StateAttribute StateAttribute  => _attribute ??= typeof(TState).GetCustomAttribute<StateAttribute>();

        public SaveDomain Domain => StateAttribute.Domain;
        public int CurrentVersion => StateAttribute.Version;
        public string Key => StateAttribute.Key;
        
        public Type StateType => typeof(TState);

        private bool _isLoaded;

        public StateHandle(ISchedulerSave schedulerSave, IEnumerable<Migration<TState>> migrationSteps)
        {
            _schedulerSave = schedulerSave;
            _migrationSteps = GetMigrationSteps(migrationSteps);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref readonly TState GetReadonlyRef()
        {
            if (!_isLoaded)
            {
                Load();
            }
            
            return ref _value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref TState GetWriteRef()
        {
            if (!_isLoaded)
            {
                Load();
            }
            
            return ref _value;
        }

        public string Save(ISerializerSave serializer)
        {
            return serializer.Serialize(_value);
        }

        public void Load(ISerializerSave serializer, string value)
        {
            var deserializedValue = serializer.Deserialize<TState>(value);

            _value = deserializedValue;
        }

        internal void MarkDirty()
        {
            _schedulerSave.Save(this);
        }

        public void Migrate(StateMigration stateMigration)
        {
            foreach (var migrationStep in _migrationSteps)
            {
                if (stateMigration.Version < migrationStep.Version)
                {
                    migrationStep.Migrate(ref _value);
                }

                stateMigration.Version = migrationStep.Version;
            }
        }

        private void Load()
        {
            _schedulerSave.Load(this);

            _isLoaded = true;
        }

        private List<Migration<TState>> GetMigrationSteps(IEnumerable<Migration<TState>> migrationSteps)
        {
            var steps = new List<Migration<TState>>(migrationSteps);

            for (var i = 1; i < steps.Count; i++)
            {
                var current = steps[i];
                var j = i - 1;
                while (j >= 0 && steps[j].Version > current.Version)
                {
                    steps[j + 1] = steps[j];
                    j--;
                }
                steps[j + 1] = current;
            }

            return steps;
        }
    }
}
