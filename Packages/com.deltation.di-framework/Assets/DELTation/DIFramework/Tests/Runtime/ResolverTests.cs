using DELTation.DIFramework.Resolution;
using DELTation.DIFramework.Tests.Runtime.Components;
using DELTation.DIFramework.Tests.Runtime.Components.Benchmark;
using DELTation.DIFramework.Tests.Runtime.Containers;
using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;

namespace DELTation.DIFramework.Tests.Runtime
{
    [TestFixture]
    public sealed class ResolverTests : TestFixtureBase
    {
        [Test]
        public void Resolver_WhenActivated_ConstructIsCalled()
        {
            var go = NewInactiveGameObject();
            var checker = go.AddComponent<ConstructChecker>();
            go.AddComponent<Resolver>();

            go.SetActive(true);

            Assert.That(checker.Constructed, Is.True);
        }

        [Test]
        public void AddResolver_WithOtherComponent_ResolverIsCalledFirst()
        {
            var go = NewInactiveGameObject();
            var componentBefore = go.AddComponent<DefaultExecutionOrderScript>();
            go.AddComponent<Resolver>();
            var componentAfter = go.AddComponent<DefaultExecutionOrderScript>();

            go.SetActive(true);

            Assert.That(componentBefore.AwakenWhenConstructed, Is.False);
            Assert.That(componentAfter.AwakenWhenConstructed, Is.False);
        }

        [Test]
        public void CreateObject_WithTwoResolvers_ConstructOnlyCalledOnce()
        {
            var root = NewInactiveGameObject();
            root.AddComponent<Resolver>();
            var child = NewGameObject();
            child.transform.parent = root.transform;
            child.AddComponent<Resolver>();
            var counter = child.AddComponent<ResolutionCounter>();

            root.SetActive(true);

            Assert.That(counter.Count, Is.EqualTo(1));
        }

        [Test]
        public void Resolver_ResolverByInterfaceInGameObject_Resolved()
        {
            var go = NewInactiveGameObject();
            var component = go.AddComponent<InterfaceDependencyComponent>();
            var implementation = go.AddComponent<InterfaceImplementation>();
            go.AddComponent<Resolver>();

            go.SetActive(true);

            Assert.That(component.Dependency, Is.EqualTo(implementation));
        }

        [Test]
        public void CreateObject_WithSeveralConstructors_ResolveAll()
        {
            var go = NewInactiveGameObject();
            var component = go.AddComponent<ComponentsWithSeveralConstructors>();
            go.AddComponent<Resolver>();
            CreateContainerWith<CustomContainer>();

            go.SetActive(true);

            Assert.That(component.FirstCalled);
            Assert.That(component.SecondCalled);
        }

        [Test]
        public void CreateObject_WithDependenceOnSimpleTypeWithConstructor_ResolveRecursively()
        {
            var go = NewInactiveGameObject();
            var component = go.AddComponent<CtorInjectionContainer.StringDependentComponent>();
            go.AddComponent<Resolver>();
            CreateContainerWith<CtorInjectionContainer>();

            go.SetActive(true);

            Assert.That(component.Dependent, Is.Not.Null);
            Assert.That(component.Dependent.Dependent, Is.Not.Null);
            Assert.That(component.Dependent.Dependent.S, Is.Not.Null);
        }

        [Test, Performance, TestCase(false), TestCase(true)]
        public void Benchmark_CreateObject_WithDependenceOnSimpleTypeWithConstructor_ResolveRecursively(bool warmUp)
        {
            var prefab = NewInactiveGameObject();
            prefab.AddComponent<CtorInjectionContainer.StringDependentComponent>();

            prefab.AddComponent<Component1>();
            prefab.AddComponent<Component2>();
            prefab.AddComponent<Component3>();
            prefab.AddComponent<Component4>();
            prefab.AddComponent<Component5>();
            prefab.AddComponent<Component6>();
            prefab.AddComponent<Component7>();
            prefab.AddComponent<Component8>();
            prefab.AddComponent<Component9>();
            prefab.AddComponent<Component10>();
            prefab.AddComponent<Component11>();
            prefab.AddComponent<Component12>();
            prefab.AddComponent<Component13>();
            prefab.AddComponent<Component14>();
            prefab.AddComponent<Component15>();
            prefab.AddComponent<Component16>();
            prefab.AddComponent<Component17>();
            prefab.AddComponent<Component18>();
            prefab.AddComponent<Component19>();
            prefab.AddComponent<Component20>();

            prefab.AddComponent<Resolver>();

            Injection.InvalidateCache();

            if (warmUp)
            {
                Injection.WarmUp(prefab);
            }

            Measure.Method(() =>
                {
                    for (var i = 0; i < 10; i++)
                    {
                        var go = Object.Instantiate(prefab);
                        var component = go.GetComponent<CtorInjectionContainer.StringDependentComponent>();
                        CreateContainerWith<CtorInjectionContainer>();

                        go.SetActive(true);

                        Assert.That(component.Dependent, Is.Not.Null);
                        Assert.That(component.Dependent.Dependent, Is.Not.Null);
                        Assert.That(component.Dependent.Dependent.S, Is.Not.Null);
                    }

                })
                .MeasurementCount(100)
                .IterationsPerMeasurement(5)
                .GC()
                .Run();
        }
    }
}