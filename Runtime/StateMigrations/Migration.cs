namespace EgorLin.Storage.StateMigrations
{
	public abstract class Migration<TState> where TState : struct, IState
	{
		public abstract int Version { get; }
		public abstract void Migrate(ref TState state);
	}
}
