using UnityEngine;

namespace DELTation.DIFramework.Tests.Runtime.Components
{
    public class StringDependencyComponent : MonoBehaviour
    {
        public string String;

        public void Construct(string @string)
        {
            String = @string;
        }
    }
}