using System;
using UnityEngine;

namespace DELTation.DIFramework
{
    internal interface IResolver
    {
        void Resolve();
        bool CanBeResolvedSafe(MonoBehaviour component, Type type);
    }
}