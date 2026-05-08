namespace EgorLin.Storage
{
    /// <summary>
    /// Read-only state view injected by the container.
    /// Assignment to <see cref="Read"/> fields is a compile error (CS8332).
    /// </summary>
    public readonly struct StateRO<TState> where TState : struct, IState {
        private readonly StateHandle<TState> _handle;
        
        /// <summary>Zero-copy ref readonly. Compiler blocks writes.</summary>
        public ref readonly TState Read => ref _handle.GetReadonlyRef();
        
        public StateRO(StateHandle<TState> handle)
        {
            _handle = handle;
        }
    }
}
