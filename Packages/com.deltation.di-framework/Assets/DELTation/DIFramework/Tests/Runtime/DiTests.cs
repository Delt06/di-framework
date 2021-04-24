using System.Collections.Generic;
using System.Linq;
using DELTation.DIFramework.Containers;
using DELTation.DIFramework.Exceptions;
using DELTation.DIFramework.Tests.Runtime.Components;
using DELTation.DIFramework.Tests.Runtime.Pocos;
using NUnit.Framework;
using UnityEngine;
using IInterface = DELTation.DIFramework.Tests.Runtime.Pocos.IInterface;
using Object = UnityEngine.Object;

namespace DELTation.DIFramework.Tests.Runtime
{
    public class DiTests : TestFixtureBase
    {
        [Test]
        public void GivenContainerWithRegisteredObject_WhenTryingToResolveGlobally_ThenReturnsTheRegisteredObjectIs()
        {
            // Arrange
            var dependency = NewGameObject().AddComponent<Rigidbody>();

            var gameObject = NewGameObject();
            gameObject.AddComponent<ListDependencyContainer>().Add(dependency);
            gameObject.AddComponent<RootDependencyContainer>();

            // Act
            var resolved = Di.TryResolveGlobally<Rigidbody>(out var resolvedDependency);

            // Assert
            Assert.That(resolved);
            Assert.That(resolvedDependency, Is.EqualTo(dependency));
        }

        [Test]
        public void GivenContainerWithRegisteredObject_WhenTryingToResolveGloballyOtherType_ThenReturnsFalse()
        {
            // Arrange
            var dependency = NewGameObject().AddComponent<Rigidbody>();

            var gameObject = NewGameObject();
            gameObject.AddComponent<ListDependencyContainer>().Add(dependency);
            gameObject.AddComponent<RootDependencyContainer>();

            // Act
            var resolved = Di.TryResolveGlobally<BoxCollider>(out _);

            // Assert
            Assert.That(resolved, Is.False);
        }

        [Test]
        public void GivenDi_WhenTryingToResolveGloballyNullType_ThenThrowsArgumentNullException()
        {
            // Arrange

            // Act

            // Assert
            Assert.That(() => Di.TryResolveGlobally(null, out _), Throws.ArgumentNullException);
        }

        [Test]
        public void GivenContainerWithRegisteredObject_WhenCheckingWhetherCanResolveGloballySafe_ThenReturnsTrue()
        {
            // Arrange
            var dependency = NewGameObject().AddComponent<Rigidbody>();

            var gameObject = NewGameObject();
            gameObject.AddComponent<ListDependencyContainer>().Add(dependency);
            gameObject.AddComponent<RootDependencyContainer>();

            // Act
            var resolved = Di.CanBeResolvedGloballySafe<Rigidbody>();

            // Assert
            Assert.That(resolved);
        }

        [Test]
        public void
            GivenContainerWithRegisteredObject_WhenCheckingWhetherCanResolveGloballySafeOtherType_ThenReturnsFalse()
        {
            // Arrange
            var dependency = NewGameObject().AddComponent<Rigidbody>();

            var gameObject = NewGameObject();
            gameObject.AddComponent<ListDependencyContainer>().Add(dependency);
            gameObject.AddComponent<RootDependencyContainer>();

            // Act
            var resolved = Di.CanBeResolvedGloballySafe<BoxCollider>();

            // Assert
            Assert.That(resolved, Is.False);
        }

        [Test]
        public void GivenDi_WhenCheckingWhetherCanResolveGloballySafeNullType_ThenThrowsArgumentNullException()
        {
            // Arrange

            // Act

            // Assert
            Assert.That(() => Di.CanBeResolvedGloballySafe(null), Throws.ArgumentNullException);
        }

        [Test]
        public void GivenDi_WhenGettingAllRegisteredObjects_ThenItIsNotNull()
        {
            // Arrange

            // Act
            var allRegisteredObjects = Di.GetAllRegisteredObjects();

            // Assert
            Assert.That(allRegisteredObjects, Is.Not.Null);
        }

        [Test]
        public void GivenDiWithNoRootContainers_WhenGettingAllRegisteredObjects_ThenItIsEmpty()
        {
            // Arrange

            // Act
            var allRegisteredObjects = Di.GetAllRegisteredObjects();

            // Assert
            Assert.That(allRegisteredObjects, Is.Empty);
        }

        [Test]
        public void GivenDiWithOneRootContainer_WhenGettingAllRegisteredObjects_ThenReturnsAllObjectsFromContainer()
        {
            // Arrange
            var objects = new List<Object>
            {
                NewGameObject().AddComponent<Rigidbody>(),
                NewGameObject().AddComponent<SphereCollider>(),
                NewGameObject().AddComponent<BoxCollider>(),
            };

            var listDependencyContainer = CreateContainerWith<ListDependencyContainer>();
            objects.ForEach(o => listDependencyContainer.Add(o));

            // Act
            var allRegisteredObjects = Di.GetAllRegisteredObjects();

            // Assert
            Assert.That(allRegisteredObjects, Is.EquivalentTo(objects));
        }

        [Test]
        public void
            GivenDiWithOneRootContainerAndTwoSubContainers_WhenGettingAllRegisteredObjects_ThenReturnsAllObjectsFromBothSubContainers()
        {
            // Arrange
            var objects1 = new List<Object>
            {
                NewGameObject().AddComponent<Rigidbody>(),
                NewGameObject().AddComponent<SphereCollider>(),
                NewGameObject().AddComponent<BoxCollider>(),
            };

            var objects2 = new List<Object>
            {
                NewGameObject().AddComponent<BoxCollider2D>(),
                NewGameObject().AddComponent<CharacterController>(),
            };

            var listDependencyContainer1 = CreateContainerWith<ListDependencyContainer>();
            var listDependencyContainer2 = listDependencyContainer1.gameObject.AddComponent<ListDependencyContainer>();
            objects1.ForEach(o => listDependencyContainer1.Add(o));
            objects2.ForEach(o => listDependencyContainer2.Add(o));

            // Act
            var allRegisteredObjects = Di.GetAllRegisteredObjects();

            // Assert
            var allObjects = objects1.Concat(objects2).Distinct().ToArray();
            Assert.That(allRegisteredObjects, Is.EquivalentTo(allObjects));
        }

        [Test]
        public void
            GivenDiWithTwoRootContainers_WhenGettingAllRegisteredObjects_ThenReturnsAllObjectsFromBothRootContainers()
        {
            // Arrange
            var objects1 = new List<Object>
            {
                NewGameObject().AddComponent<Rigidbody>(),
                NewGameObject().AddComponent<SphereCollider>(),
                NewGameObject().AddComponent<BoxCollider>(),
            };

            var objects2 = new List<Object>
            {
                NewGameObject().AddComponent<BoxCollider2D>(),
                NewGameObject().AddComponent<CharacterController>(),
            };

            var listDependencyContainer1 = CreateContainerWith<ListDependencyContainer>();
            var listDependencyContainer2 = CreateContainerWith<ListDependencyContainer>();
            objects1.ForEach(o => listDependencyContainer1.Add(o));
            objects2.ForEach(o => listDependencyContainer2.Add(o));

            // Act
            var allRegisteredObjects = Di.GetAllRegisteredObjects();

            // Assert
            var allObjects = objects1.Concat(objects2).Distinct().ToArray();
            Assert.That(allRegisteredObjects, Is.EquivalentTo(allObjects));
        }

        [Test]
        public void GivenDi_WhenCreatingInstanceWithoutExplicitConstructors_ThenItIsNotNull()
        {
            // Arrange

            // Act
            var instance = Di.Create<PocoDep2>();

            // Assert
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void GivenDi_WhenCreatingInstanceAndCantResolveDependency_ThenThrowsDependencyNotRegisteredException()
        {
            // Arrange

            // Act

            // Assert
            Assert.That(Di.Create<PocoDep1>, Throws.InstanceOf<DependencyNotRegisteredException>());
        }

        [Test]
        public void GivenDi_WhenCreatingInstanceOfAbstractClassOrInterface_ThenThrowsArgumentException()
        {
            // Arrange

            // Act

            // Assert
            Assert.That(Di.Create<AbstractClass>, Throws.ArgumentException);
            Assert.That(Di.Create<IInterface>, Throws.ArgumentException);
        }

        [Test]
        public void GivenDi_WhenCreatingInstanceAndCanResolveDependency_ThenInstanceIsCreatedAndDependencyIsInjected()
        {
            // Arrange
            CreateContainerWith<SingleInstanceContainer_PocoDep2>();

            // Act
            var instance = Di.Create<PocoDep1>();
            var resolvedPocoDep2 = Di.TryResolveGlobally<PocoDep2>(out var pocoDep2);

            // Assert
            Assert.That(instance, Is.Not.Null);
            Assert.That(resolvedPocoDep2);
            Assert.That(instance.Dep, Is.EqualTo(pocoDep2));
        }

        [Test]
        public void GivenDiAndNoComponents_WhenInjecting_ThenThrowsDependencyNotResolvedException()
        {
            // Arrange
            var gameObject = NewGameObject();
            gameObject.AddComponent<RigidbodyComponent>();

            // Act

            // Assert
            Assert.That(() => Di.Inject(gameObject), Throws.InstanceOf<DependencyNotResolvedException>());
        }


        [Test]
        public void GivenDiAndLocalComponent_WhenInjecting_ThenValueIsSet()
        {
            // Arrange
            var gameObject = NewGameObject();
            var rigidbodyComponent = gameObject.AddComponent<RigidbodyComponent>();
            var rigidbody = gameObject.AddComponent<Rigidbody>();

            // Act
            Di.Inject(gameObject);

            // Assert
            Assert.That(rigidbodyComponent.Rigidbody, Is.EqualTo(rigidbody));
        }

        [Test]
        public void GivenDiAndGlobalComponent_WhenInjecting_ThenValueIsSet()
        {
            // Arrange
            var listDependencyContainer = CreateContainerWith<ListDependencyContainer>();
            var rigidbody = NewGameObject().AddComponent<Rigidbody>();
            listDependencyContainer.Add(rigidbody);
            var gameObject = NewGameObject();
            var rigidbodyComponent = gameObject.AddComponent<RigidbodyComponent>();

            // Act
            Di.Inject(gameObject);

            // Assert
            Assert.That(rigidbodyComponent.Rigidbody, Is.EqualTo(rigidbody));
        }

        [Test]
        public void GivenDiAndGlobalComponent_WhenInjectingWithLocalSources_ThenThrowsDependencyNotResolvedException()
        {
            // Arrange
            var listDependencyContainer = CreateContainerWith<ListDependencyContainer>();
            var rigidbody = NewGameObject().AddComponent<Rigidbody>();
            listDependencyContainer.Add(rigidbody);
            var gameObject = NewGameObject();
            gameObject.AddComponent<RigidbodyComponent>();

            // Act

            // Assert
            Assert.That(() => Di.Inject(gameObject, DependencySource.Local),
                Throws.InstanceOf<DependencyNotResolvedException>()
            );
        }
    }
}