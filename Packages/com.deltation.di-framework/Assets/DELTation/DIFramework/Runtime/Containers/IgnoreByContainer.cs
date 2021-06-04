using UnityEngine;

namespace DELTation.DIFramework.Containers
{
    /// <summary>
    /// Makes containers ignore the object.
    /// </summary>
    public interface IIgnoreByContainer { }

    /// <inheritdoc cref="DELTation.DIFramework.Containers.IIgnoreByContainer" />
    public sealed class IgnoreByContainer : MonoBehaviour, IIgnoreByContainer { }
}