using EgorLin.DIsolated.Core;
using EgorLin.DIsolated.Core.Extensions;
using EgorLin.Storage.StateMigrations;

namespace EgorLin.Storage.Extensions
{
    public static class StateContainerExtensions
    {
        public static DiContainer BindStateRW<TState>(this DiContainer container) where TState : struct, IState 
        {
            container.BindSingleton(typeof(StateRW<TState>), typeof(StateRW<TState>));
            
            return container;
        }
        
        public static DiContainerMigration<TState> BindStateROWithMigration<TState>(this DiContainer container) where TState : struct, IState 
        {
            container.BindSingleton(typeof(StateHandle<TState>), typeof(StateHandle<TState>), typeof(IStateHandle));

            container.BindSingleton(typeof(StateRO<TState>), typeof(StateRO<TState>));

            return new DiContainerMigration<TState>(container);
        }
        
        public static void BindStateROWithoutMigration<TState>(this DiContainer container) where TState : struct, IState 
        {
            container.BindSingleton(typeof(StateHandle<TState>), typeof(StateHandle<TState>), typeof(IStateHandle));

            container.BindSingleton(typeof(StateRO<TState>), typeof(StateRO<TState>));
            
            container.BindSingleton(typeof(MigrationDummy<TState>), typeof(Migration<TState>));
        }
        
        public static DiContainerMigration<TState> BindStateWithMigration<TState>(this DiContainer container) where TState : struct, IState 
        {
            container.BindSingleton(typeof(StateHandle<TState>), typeof(StateHandle<TState>), typeof(IStateHandle));

            container.BindSingleton(typeof(StateRW<TState>), typeof(StateRW<TState>));
            container.BindSingleton(typeof(StateRO<TState>), typeof(StateRO<TState>));

            return new DiContainerMigration<TState>(container);
        }
        
        public static void BindStateWithoutMigration<TState>(this DiContainer container) where TState : struct, IState 
        {
            container.BindSingleton(typeof(StateHandle<TState>), typeof(StateHandle<TState>), typeof(IStateHandle));

            container.BindSingleton(typeof(StateRW<TState>), typeof(StateRW<TState>));
            container.BindSingleton(typeof(StateRO<TState>), typeof(StateRO<TState>));
            
            container.BindSingleton(typeof(MigrationDummy<TState>), typeof(Migration<TState>));
        }
    }

    public struct DiContainerMigration<TState> where TState : struct, IState
    {
        private readonly DiContainer Container;

        public DiContainerMigration(DiContainer container)
        {
            Container = container;
        }
        
        public DiContainerMigration<TState> BindMigration<TMigration>() where TMigration : Migration<TState>
        {
            Container.BindSingleton(typeof(TMigration), typeof(Migration<TState>));
            
            return this;
        }
    }
}
