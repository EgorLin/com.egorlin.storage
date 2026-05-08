namespace EgorLin.Storage.StateMigrations
{
	public class MigrationDummy<TState> : Migration<TState> where TState : struct, IState
	{
		public override int Version => 1;
		
		public override void Migrate(ref TState state)
		{
		}
	}
}
