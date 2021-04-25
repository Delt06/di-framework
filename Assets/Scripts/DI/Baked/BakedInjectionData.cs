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
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod]
#endif
        public static void Bake()
        {
            BakedInjection.DataExists = true;
            BakedInjection.Clear();
            
            BakedInjection.Bake(typeof(PerformanceTest.Scripts.DI.Dependent1), InjectionFunction_0);
            BakedInjection.Bake(typeof(PerformanceTest.Scripts.DI.Dependent12), InjectionFunction_1);
            BakedInjection.Bake(typeof(PerformanceTest.Scripts.DI.Dependent123), InjectionFunction_2);
            BakedInjection.Bake(typeof(PerformanceTest.Scripts.DI.Dependent13), InjectionFunction_3);
            BakedInjection.Bake(typeof(PerformanceTest.Scripts.DI.Dependent2), InjectionFunction_4);
            BakedInjection.Bake(typeof(PerformanceTest.Scripts.DI.Dependent3), InjectionFunction_5);
            BakedInjection.Bake(typeof(Demo.Scripts.InjectablePoco), InstantiationFunction_6);
            BakedInjection.Bake(typeof(Demo.Scripts.InjectablePoco2), InstantiationFunction_7);
            BakedInjection.Bake(typeof(Demo.Scripts.SimpleInjectablePoco), InstantiationFunction_8);
            BakedInjection.Bake(typeof(Demo.Scripts.TwoConstructorsClass), InjectionFunction_9);
            BakedInjection.Bake(typeof(Demo.Scripts.UI.PointerControls), InjectionFunction_10);
            BakedInjection.Bake(typeof(Demo.Scripts.Turrets.Turret), InjectionFunction_11);
            BakedInjection.Bake(typeof(Demo.Scripts.Turrets.TurretAim), InjectionFunction_12);
            BakedInjection.Bake(typeof(Demo.Scripts.TestPocoBaking.TestPoco), InstantiationFunction_13);
            BakedInjection.Bake(typeof(Demo.Scripts.TestPocoBaking.TestPocoDependency), InstantiationFunction_14);
            BakedInjection.Bake(typeof(Demo.Scripts.TestPocoBaking.TestPocoDependant), InjectionFunction_15);
            BakedInjection.Bake(typeof(Demo.Scripts.Movement.Movement), InjectionFunction_16);
            BakedInjection.Bake(typeof(Demo.Scripts.Effects.DestructionEffect), InjectionFunction_17);
            BakedInjection.Bake(typeof(Demo.Scripts.Bullets.BulletHitTrigger), InjectionFunction_18);

        }

        private static void InjectionFunction_0(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (PerformanceTest.Scripts.DI.Dependent1) component;
            obj.Construct((PerformanceTest.Scripts.DI.Dependency1) resolve(component, typeof(PerformanceTest.Scripts.DI.Dependency1)));
        }

        private static void InjectionFunction_1(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (PerformanceTest.Scripts.DI.Dependent12) component;
            obj.Construct((PerformanceTest.Scripts.DI.Dependency1) resolve(component, typeof(PerformanceTest.Scripts.DI.Dependency1)), (PerformanceTest.Scripts.DI.Dependency2) resolve(component, typeof(PerformanceTest.Scripts.DI.Dependency2)));
        }

        private static void InjectionFunction_2(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (PerformanceTest.Scripts.DI.Dependent123) component;
            obj.Construct((PerformanceTest.Scripts.DI.Dependency1) resolve(component, typeof(PerformanceTest.Scripts.DI.Dependency1)), (PerformanceTest.Scripts.DI.Dependency2) resolve(component, typeof(PerformanceTest.Scripts.DI.Dependency2)), (PerformanceTest.Scripts.DI.Dependency3) resolve(component, typeof(PerformanceTest.Scripts.DI.Dependency3)));
        }

        private static void InjectionFunction_3(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (PerformanceTest.Scripts.DI.Dependent13) component;
            obj.Construct((PerformanceTest.Scripts.DI.Dependency1) resolve(component, typeof(PerformanceTest.Scripts.DI.Dependency1)), (PerformanceTest.Scripts.DI.Dependency3) resolve(component, typeof(PerformanceTest.Scripts.DI.Dependency3)));
        }

        private static void InjectionFunction_4(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (PerformanceTest.Scripts.DI.Dependent2) component;
            obj.Construct((PerformanceTest.Scripts.DI.Dependency2) resolve(component, typeof(PerformanceTest.Scripts.DI.Dependency2)));
        }

        private static void InjectionFunction_5(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (PerformanceTest.Scripts.DI.Dependent3) component;
            obj.Construct((PerformanceTest.Scripts.DI.Dependency3) resolve(component, typeof(PerformanceTest.Scripts.DI.Dependency3)));
        }

        private static object InstantiationFunction_6(PocoResolutionFunction resolve)
        {
            return new Demo.Scripts.InjectablePoco((System.Text.StringBuilder) resolve(typeof(System.Text.StringBuilder)));
        }

        private static object InstantiationFunction_7(PocoResolutionFunction resolve)
        {
            return new Demo.Scripts.InjectablePoco2((System.Text.StringBuilder) resolve(typeof(System.Text.StringBuilder)), (Demo.Scripts.SimpleInjectablePoco) resolve(typeof(Demo.Scripts.SimpleInjectablePoco)));
        }

        private static object InstantiationFunction_8(PocoResolutionFunction resolve)
        {
            return new Demo.Scripts.SimpleInjectablePoco();
        }

        private static void InjectionFunction_9(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (Demo.Scripts.TwoConstructorsClass) component;
            obj.Construct((UnityEngine.Rigidbody) resolve(component, typeof(UnityEngine.Rigidbody)));
            obj.Construct((UnityEngine.Rigidbody2D) resolve(component, typeof(UnityEngine.Rigidbody2D)));
        }

        private static void InjectionFunction_10(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (Demo.Scripts.UI.PointerControls) component;
            obj.Construct((UnityEngine.Camera) resolve(component, typeof(UnityEngine.Camera)), (Demo.Scripts.Movement.IMovement) resolve(component, typeof(Demo.Scripts.Movement.IMovement)));
        }

        private static void InjectionFunction_11(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (Demo.Scripts.Turrets.Turret) component;
            obj.Construct((Demo.Scripts.Bullets.IBulletFactory) resolve(component, typeof(Demo.Scripts.Bullets.IBulletFactory)), (Demo.Scripts.Shooting.IShootFrom) resolve(component, typeof(Demo.Scripts.Shooting.IShootFrom)), (Demo.Scripts.Shooting.IShootingTarget) resolve(component, typeof(Demo.Scripts.Shooting.IShootingTarget)));
        }

        private static void InjectionFunction_12(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (Demo.Scripts.Turrets.TurretAim) component;
            obj.Construct((Demo.Scripts.Shooting.IShootingTarget) resolve(component, typeof(Demo.Scripts.Shooting.IShootingTarget)));
        }

        private static object InstantiationFunction_13(PocoResolutionFunction resolve)
        {
            return new Demo.Scripts.TestPocoBaking.TestPoco((Demo.Scripts.TestPocoBaking.TestPocoDependency) resolve(typeof(Demo.Scripts.TestPocoBaking.TestPocoDependency)));
        }

        private static object InstantiationFunction_14(PocoResolutionFunction resolve)
        {
            return new Demo.Scripts.TestPocoBaking.TestPocoDependency();
        }

        private static void InjectionFunction_15(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (Demo.Scripts.TestPocoBaking.TestPocoDependant) component;
            obj.Construct((Demo.Scripts.TestPocoBaking.TestPoco) resolve(component, typeof(Demo.Scripts.TestPocoBaking.TestPoco)));
        }

        private static void InjectionFunction_16(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (Demo.Scripts.Movement.Movement) component;
            obj.Construct((UnityEngine.Rigidbody) resolve(component, typeof(UnityEngine.Rigidbody)));
        }

        private static void InjectionFunction_17(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (Demo.Scripts.Effects.DestructionEffect) component;
            obj.Construct((Demo.Scripts.Damage.IDestroyable) resolve(component, typeof(Demo.Scripts.Damage.IDestroyable)));
        }

        private static void InjectionFunction_18(MonoBehaviour component, ResolutionFunction resolve)
        {
            var obj = (Demo.Scripts.Bullets.BulletHitTrigger) component;
            obj.Construct((Demo.Scripts.Bullets.Bullet) resolve(component, typeof(Demo.Scripts.Bullets.Bullet)));
        }


    }
}
