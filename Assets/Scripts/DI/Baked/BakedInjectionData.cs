// Generated automatically by DI Framework

using DELTation.DIFramework.Baking;
using UnityEngine;

namespace DI.Baked
{
    /// <summary>
    /// Baked injection data
    /// </summary>
    internal static class BakedInjectionData
    {
        [RuntimeInitializeOnLoadMethod]
        public static void Bake()
        {
            BakedInjection.Bake(typeof(PerformanceTest.Scripts.DI.Dependent1), InjectionFunction_0);
            BakedInjection.Bake(typeof(PerformanceTest.Scripts.DI.Dependent12), InjectionFunction_1);
            BakedInjection.Bake(typeof(PerformanceTest.Scripts.DI.Dependent123), InjectionFunction_2);
            BakedInjection.Bake(typeof(PerformanceTest.Scripts.DI.Dependent13), InjectionFunction_3);
            BakedInjection.Bake(typeof(PerformanceTest.Scripts.DI.Dependent2), InjectionFunction_4);
            BakedInjection.Bake(typeof(PerformanceTest.Scripts.DI.Dependent3), InjectionFunction_5);
            BakedInjection.Bake(typeof(Demo.Scripts.UI.PointerControls), InjectionFunction_6);
            BakedInjection.Bake(typeof(Demo.Scripts.Turrets.Turret), InjectionFunction_7);
            BakedInjection.Bake(typeof(Demo.Scripts.Turrets.TurretAim), InjectionFunction_8);
            BakedInjection.Bake(typeof(Demo.Scripts.Movement.Movement), InjectionFunction_9);
            BakedInjection.Bake(typeof(Demo.Scripts.Effects.DestructionEffect), InjectionFunction_10);
            BakedInjection.Bake(typeof(Demo.Scripts.Bullets.BulletHitTrigger), InjectionFunction_11);

        }

        private static void InjectionFunction_0(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (PerformanceTest.Scripts.DI.Dependent1) component;
        }

        private static void InjectionFunction_1(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (PerformanceTest.Scripts.DI.Dependent12) component;
        }

        private static void InjectionFunction_2(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (PerformanceTest.Scripts.DI.Dependent123) component;
        }

        private static void InjectionFunction_3(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (PerformanceTest.Scripts.DI.Dependent13) component;
        }

        private static void InjectionFunction_4(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (PerformanceTest.Scripts.DI.Dependent2) component;
        }

        private static void InjectionFunction_5(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (PerformanceTest.Scripts.DI.Dependent3) component;
        }

        private static void InjectionFunction_6(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (Demo.Scripts.UI.PointerControls) component;
        }

        private static void InjectionFunction_7(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (Demo.Scripts.Turrets.Turret) component;
        }

        private static void InjectionFunction_8(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (Demo.Scripts.Turrets.TurretAim) component;
        }

        private static void InjectionFunction_9(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (Demo.Scripts.Movement.Movement) component;
        }

        private static void InjectionFunction_10(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (Demo.Scripts.Effects.DestructionEffect) component;
        }

        private static void InjectionFunction_11(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (Demo.Scripts.Bullets.BulletHitTrigger) component;
        }


    }
}
