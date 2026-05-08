namespace EgorLin.Storage
{
    /// <summary>
    /// Read-write state view injected by the container.
    /// </summary>
    public readonly struct StateRW<TState> where TState : struct, IState
    {
        private readonly StateHandle<TState> _handle;

        /// <summary>Read-only view — same underlying ref, no copy.</summary>
        public ref readonly TState Read => ref _handle.GetReadonlyRef();

        /// <summary>
        /// Tracked mutable ref. Sets dirty flag, returns the ref.
        /// Use for any write that should trigger save scheduling or reactive systems.
        /// </summary>
        public ref TState Write {
            get {
                _handle.MarkDirty();
                return ref _handle.GetWriteRef();
            }
        }

        public StateRW(StateHandle<TState> handle)
        {
            _handle = handle;
        }

        /// <summary>Downcast so RW passes wherever RO is expected.</summary>
        public static implicit operator StateRO<TState>(StateRW<TState> rw)
        {
            return new StateRO<TState>(rw._handle);
        }
    }
}
