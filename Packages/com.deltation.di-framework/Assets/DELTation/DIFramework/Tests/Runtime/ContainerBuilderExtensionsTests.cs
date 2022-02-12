using System.Collections.Generic;
using System.Text;
using DELTation.DIFramework.Containers;
using NUnit.Framework;
using UnityEngine;

namespace DELTation.DIFramework.Tests.Runtime
{
    public class ContainerBuilderExtensionsTests : TestFixtureBase
    {
        [Test]
        public void GivenRegisterFromMethod_WhenResolving_ThenCanResolveAll()
        {
            // Arrange
            CreateContainerWith<RegisterFromMethodContainer>();

            // Act
            var canResolveS = Di.TryResolveGlobally(out string _);
            var canResolveSb = Di.TryResolveGlobally(out StringBuilder _);
            var canResolveL = Di.TryResolveGlobally(out List _);
            var canResolveLi = Di.TryResolveGlobally(out List<int> _);
            var canResolveLf = Di.TryResolveGlobally(out List<float> _);
            var canResolveLd = Di.TryResolveGlobally(out List<double> _);

            // Assert
            Assert.That(canResolveS);
            Assert.That(canResolveSb);
            Assert.That(canResolveL);
            Assert.That(canResolveLi);
            Assert.That(canResolveLf);
            Assert.That(canResolveLd);
        }


        [Test]
        public void GivenRegisterFromResources_WhenFound_ThenCanBeResolved()
        {
            // Arrange
            var diSettingsContainer = CreateContainerWith<DiSettingsContainer>();
            diSettingsContainer.Path = "DI Settings";

            // Act
            var resolved = Di.TryResolveGlobally(out DiSettings _);

            // Assert
            Assert.That(resolved);
        }

        [Test]
        public void GivenRegisterFromResources_WhenNotFound_ThenThrowsArgumentNullException()
        {
            // Arrange
            var diSettingsContainer = CreateContainerWith<DiSettingsContainer>();
            diSettingsContainer.Path = "Some other asset path";

            // Act
            bool Code() => Di.TryResolveGlobally(out DiSettings _);

            // Assert
            Assert.That(Code, Throws.ArgumentNullException);
        }


        [Test]
        public void GivenRegisterIfNotNull_WhenNull_ThenNoExceptions()
        {
            // Arrange
            CreateContainerWith<ContainerRegisteringNull>();

            // Act
            var resolved = Di.TryResolveGlobally<object>(out _);

            // Assert
            Assert.IsFalse(resolved);
        }

        [Test]
        public void GivenRegisterIfNotNull_WhenUnityNull_ThenNoExceptions()
        {
            // Arrange
            CreateContainerWith<ContainerRegisteringUnityNull>();

            // Act
            var resolved = Di.TryResolveGlobally<Collider>(out _);

            // Assert
            Assert.IsFalse(resolved);
        }

        [Test]
        public void GivenRegisterIfNotNull_WhenNotNull_ThenRegistered()
        {
            // Arrange
            CreateContainerWith<ContainerRegisteringNotNull>();

            // Act
            var resolved = Di.TryResolveGlobally<object>(out _);

            // Assert
            Assert.IsTrue(resolved);
        }

        [Test]
        public void GivenRegisterIfNotNull_WhenNotNullUnityObj_ThenRegistered()
        {
            // Arrange
            CreateContainerWith<ContainerRegisteringNotNullUnityObj>();

            // Act
            var resolved = Di.TryResolveGlobally<Collider>(out _);

            // Assert
            Assert.IsTrue(resolved);
        }

        [Test]
        public void GivenTryResolveGloballyAndRegister_WhenResolved_ThenRegistered()
        {
            // Arrange
            CreateContainerWith<ContainerRegisteringString>();
            CreateContainerWith<ContainerResolvingAndRegisteringString>();

            // Act
            var resolved = Di.TryResolveGlobally<string>(out _);

            // Assert
            Assert.IsTrue(resolved);
        }

        [Test]
        public void GivenTryResolveGloballyAndRegister_WhenNotResolved_ThenNotRegistered()
        {
            // Arrange
            CreateContainerWith<ContainerResolvingAndRegisteringString>();

            // Act
            var resolved = Di.TryResolveGlobally<string>(out _);

            // Assert
            Assert.IsFalse(resolved);
        }

        public class RegisterFromMethodContainer : DependencyContainerBase
        {
            protected override void ComposeDependencies(ICanRegisterContainerBuilder builder)
            {
                builder
                    .RegisterFromMethod(() => "abc")
                    .RegisterFromMethod((string s) => new StringBuilder())
                    .RegisterFromMethod((string s, StringBuilder sb) => new List())
                    .RegisterFromMethod((string s, StringBuilder sb, List l) => new List<int>())
                    .RegisterFromMethod((string s, StringBuilder sb, List l, List<int> li) => new List<float>())
                    .RegisterFromMethod((string s, StringBuilder sb, List l, List<int> li, List<float> lf) =>
                        new List<double>()
                    );
            }
        }

        public class DiSettingsContainer : DependencyContainerBase
        {
            public string Path;

            protected override void ComposeDependencies(ICanRegisterContainerBuilder builder)
            {
                builder.RegisterFromResources<DiSettings>(Path);
            }
        }

        private class ContainerRegisteringNull : DependencyContainerBase
        {
            protected override void ComposeDependencies(ICanRegisterContainerBuilder builder)
            {
                builder.RegisterIfNotNull(null);
            }
        }

        private class ContainerRegisteringUnityNull : DependencyContainerBase
        {
            protected override void ComposeDependencies(ICanRegisterContainerBuilder builder)
            {
                var c = gameObject.AddComponent<BoxCollider>();
                DestroyImmediate(c); // using immediate so that object is null right away
                builder.RegisterIfNotNull(c);
            }
        }

        private class ContainerRegisteringNotNull : DependencyContainerBase
        {
            protected override void ComposeDependencies(ICanRegisterContainerBuilder builder)
            {
                builder.RegisterIfNotNull(new object());
            }
        }

        private class ContainerRegisteringNotNullUnityObj : DependencyContainerBase
        {
            protected override void ComposeDependencies(ICanRegisterContainerBuilder builder)
            {
                var c = gameObject.AddComponent<BoxCollider>();
                builder.RegisterIfNotNull(c);
            }
        }

        private class ContainerResolvingAndRegisteringString : DependencyContainerBase
        {
            protected override void ComposeDependencies(ICanRegisterContainerBuilder builder)
            {
                builder.TryResolveGloballyAndRegister<string>();
            }
        }

        private class ContainerRegisteringString : DependencyContainerBase
        {
            protected override void ComposeDependencies(ICanRegisterContainerBuilder builder)
            {
                builder.Register("Some string");
            }
        }
    }
}