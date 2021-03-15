using DELTation.DIFramework.Baking;
using UnityEditor;
using UnityEngine;

namespace Demo.Scripts.DI.Baked
{
    [InitializeOnLoad]
    internal static class BakedInjectionData
    {
        static BakedInjectionData()
        {
            BakedInjection.Bake(typeof(Movement.Movement), InjectionFunction_Movement_Movement);
        }

        private static void InjectionFunction_Movement_Movement(MonoBehaviour component, ResolutionFunction resolve)
        {
            Debug.Log("Inject");
            var obj = (Movement.Movement) component;
            obj.Construct((Rigidbody) resolve(component, typeof(Rigidbody)));
        }
    }
}