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
            BakedInjection.Clear();
            
            BakedInjection.Bake(typeof(PerformanceTest.Scripts.DI.Dependent1), InjectionFunction_0);
            BakedInjection.Bake(typeof(PerformanceTest.Scripts.DI.Dependent12), InjectionFunction_1);
            BakedInjection.Bake(typeof(PerformanceTest.Scripts.DI.Dependent123), InjectionFunction_2);
            BakedInjection.Bake(typeof(PerformanceTest.Scripts.DI.Dependent13), InjectionFunction_3);
            BakedInjection.Bake(typeof(PerformanceTest.Scripts.DI.Dependent2), InjectionFunction_4);
            BakedInjection.Bake(typeof(PerformanceTest.Scripts.DI.Dependent3), InjectionFunction_5);
            BakedInjection.Bake(typeof(Demo.Scripts.TwoConstructorsClass), InjectionFunction_6);
            BakedInjection.Bake(typeof(Demo.Scripts.UI.PointerControls), InjectionFunction_7);
            BakedInjection.Bake(typeof(Demo.Scripts.Turrets.Turret), InjectionFunction_8);
            BakedInjection.Bake(typeof(Demo.Scripts.Turrets.TurretAim), InjectionFunction_9);
            BakedInjection.Bake(typeof(Demo.Scripts.Movement.Movement), InjectionFunction_10);
            BakedInjection.Bake(typeof(Demo.Scripts.Effects.DestructionEffect), InjectionFunction_11);
            BakedInjection.Bake(typeof(Demo.Scripts.Bullets.BulletHitTrigger), InjectionFunction_12);

        }

        private static void InjectionFunction_0(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (PerformanceTest.Scripts.DI.Dependent1) component;
            obj.Construct((PerformanceTest.Scripts.Dependency1) resolve(component, typeof(PerformanceTest.Scripts.Dependency1)));
        }

        private static void InjectionFunction_1(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (PerformanceTest.Scripts.DI.Dependent12) component;
            obj.Construct((PerformanceTest.Scripts.Dependency1) resolve(component, typeof(PerformanceTest.Scripts.Dependency1)), (PerformanceTest.Scripts.Dependency2) resolve(component, typeof(PerformanceTest.Scripts.Dependency2)));
        }

        private static void InjectionFunction_2(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (PerformanceTest.Scripts.DI.Dependent123) component;
            obj.Construct((PerformanceTest.Scripts.Dependency1) resolve(component, typeof(PerformanceTest.Scripts.Dependency1)), (PerformanceTest.Scripts.Dependency2) resolve(component, typeof(PerformanceTest.Scripts.Dependency2)), (PerformanceTest.Scripts.Dependency3) resolve(component, typeof(PerformanceTest.Scripts.Dependency3)));
        }

        private static void InjectionFunction_3(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (PerformanceTest.Scripts.DI.Dependent13) component;
            obj.Construct((PerformanceTest.Scripts.Dependency1) resolve(component, typeof(PerformanceTest.Scripts.Dependency1)), (PerformanceTest.Scripts.Dependency3) resolve(component, typeof(PerformanceTest.Scripts.Dependency3)));
        }

        private static void InjectionFunction_4(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (PerformanceTest.Scripts.DI.Dependent2) component;
            obj.Construct((PerformanceTest.Scripts.Dependency2) resolve(component, typeof(PerformanceTest.Scripts.Dependency2)));
        }

        private static void InjectionFunction_5(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (PerformanceTest.Scripts.DI.Dependent3) component;
            obj.Construct((PerformanceTest.Scripts.Dependency3) resolve(component, typeof(PerformanceTest.Scripts.Dependency3)));
        }

        private static void InjectionFunction_6(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (Demo.Scripts.TwoConstructorsClass) component;
            obj.Construct((UnityEngine.Rigidbody) resolve(component, typeof(UnityEngine.Rigidbody)));
            obj.Construct((UnityEngine.Rigidbody2D) resolve(component, typeof(UnityEngine.Rigidbody2D)));
        }

        private static void InjectionFunction_7(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (Demo.Scripts.UI.PointerControls) component;
            obj.Construct((UnityEngine.Camera) resolve(component, typeof(UnityEngine.Camera)), (Demo.Scripts.Movement.IMovement) resolve(component, typeof(Demo.Scripts.Movement.IMovement)));
        }

        private static void InjectionFunction_8(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (Demo.Scripts.Turrets.Turret) component;
            obj.Construct((Demo.Scripts.Bullets.IBulletFactory) resolve(component, typeof(Demo.Scripts.Bullets.IBulletFactory)), (Demo.Scripts.Shooting.IShootFrom) resolve(component, typeof(Demo.Scripts.Shooting.IShootFrom)), (Demo.Scripts.Shooting.IShootingTarget) resolve(component, typeof(Demo.Scripts.Shooting.IShootingTarget)));
        }

        private static void InjectionFunction_9(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (Demo.Scripts.Turrets.TurretAim) component;
            obj.Construct((Demo.Scripts.Shooting.IShootingTarget) resolve(component, typeof(Demo.Scripts.Shooting.IShootingTarget)));
        }

        private static void InjectionFunction_10(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (Demo.Scripts.Movement.Movement) component;
            obj.Construct((UnityEngine.Rigidbody) resolve(component, typeof(UnityEngine.Rigidbody)));
        }

        private static void InjectionFunction_11(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (Demo.Scripts.Effects.DestructionEffect) component;
            obj.Construct((Demo.Scripts.Damage.IDestroyable) resolve(component, typeof(Demo.Scripts.Damage.IDestroyable)));
        }

        private static void InjectionFunction_12(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (Demo.Scripts.Bullets.BulletHitTrigger) component;
            obj.Construct((Demo.Scripts.Bullets.Bullet) resolve(component, typeof(Demo.Scripts.Bullets.Bullet)));
        }


    }
}
