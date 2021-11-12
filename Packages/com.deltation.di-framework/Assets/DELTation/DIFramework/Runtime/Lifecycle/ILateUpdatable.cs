namespace DELTation.DIFramework.Lifecycle
{
    public interface ILateUpdatable
    {
        /// <summary>
        ///     Executed by
        ///     <see cref="DELTation.DIFramework.ContainerLifecycle" />
        ///     at Unity's LateUpdate callback.
        /// </summary>
        void OnLateUpdate();
    }
}