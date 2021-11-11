namespace DELTation.DIFramework.Lifecycle
{
    public interface IUpdatable
    {
        /// <summary>
        ///     Executed by
        ///     <see cref="DELTation.DIFramework.ContainerLifecycle" />
        ///     at Unity's Update callback.
        /// </summary>
        void OnUpdate();
    }
}