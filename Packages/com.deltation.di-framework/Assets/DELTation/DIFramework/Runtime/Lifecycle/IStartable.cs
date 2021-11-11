namespace DELTation.DIFramework.Lifecycle
{
    public interface IStartable
    {
        /// <summary>
        ///     Executed by
        ///     <see cref="DELTation.DIFramework.ContainerLifecycle" />
        ///     at Unity's Start callback.
        /// </summary>
        void OnStart();
    }
}