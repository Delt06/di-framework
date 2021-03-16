using DELTation.DIFramework.Baking;
using UnityEngine;

namespace DI.Baked
{
    /// <summary>
    /// Baked injection data
    /// </summary>
    
    internal static class BakedInjectionDataSample
    {
        [RuntimeInitializeOnLoadMethod]
        public static void Bake()
        {
            BakedInjection.Bake(typeof(Demo.Scripts.Movement.Movement), InjectionFunction_Movement_Movement);
        }

        private static void InjectionFunction_Movement_Movement(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (Demo.Scripts.Movement.Movement) component;
            obj.Construct((Rigidbody) resolve(component, typeof(Rigidbody)));
        }
    }
}