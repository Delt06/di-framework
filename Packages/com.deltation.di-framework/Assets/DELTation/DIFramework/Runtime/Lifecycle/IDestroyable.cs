namespace DELTation.DIFramework.Lifecycle
{
    public interface IDestroyable
    {
        /// <summary>
        ///     Executed by
        ///     <see cref="DELTation.DIFramework.ContainerLifecycle" />
        ///     at Unity's OnDestroy callback.
        /// </summary>
        void OnDestroy();
    }
}