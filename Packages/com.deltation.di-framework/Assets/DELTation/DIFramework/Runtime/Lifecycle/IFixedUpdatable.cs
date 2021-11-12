namespace DELTation.DIFramework.Lifecycle
{
    public interface IFixedUpdatable
    {
        /// <summary>
        ///     Executed by
        ///     <see cref="DELTation.DIFramework.ContainerLifecycle" />
        ///     at Unity's FixedUpdate callback.
        /// </summary>
        void OnFixedUpdate();
    }
}