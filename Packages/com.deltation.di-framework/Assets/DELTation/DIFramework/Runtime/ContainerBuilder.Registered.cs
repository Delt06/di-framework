using System;
using System.Diagnostics;

namespace DELTation.DIFramework
{
    public interface IRegisteredContainerBuilder : ICanRegisterContainerBuilder
    {
        /// <summary>
        ///     Mark the last dependency as internal (disallow to resolve it outside the container).
        /// </summary>
        /// <returns>The same container builder.</returns>
        IRegisteredContainerBuilder AsInternal();
    }

    internal class InternalOnlyTag { }

    internal partial class ContainerBuilder
    {
        /// <inheritdoc />
        public IRegisteredContainerBuilder AsInternal()
        {
            if (WasAbleToRegisterLast)
            {
                EnsureNotEmpty();
                var index = DependenciesCount - 1;
                AddTag(index, typeof(InternalOnlyTag));
            }

            return this;
        }

        [Conditional("DEBUG")]
        private void EnsureNotEmpty()
        {
            if (DependenciesCount == 0)
                throw new InvalidOperationException("Container builder is empty.");
        }
    }
}