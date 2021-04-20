using DELTation.DIFramework.Tests.Runtime.Components;
using DELTation.DIFramework.Tests.Runtime.Containers;
using FluentAssertions;
using NUnit.Framework;

namespace DELTation.DIFramework.Tests.Runtime
{
    [TestFixture]
    public sealed class CustomDependencyContainerTests : TestFixtureBase
    {
        [Test]
        public void CreateObject_WithDependency_Resolved()
        {
            CreateContainerWith<CustomContainer>();

            var component = NewGameObject().AddComponent<StringDependencyComponent>();
            component.gameObject.AddComponent<Resolver>();

            component.String.Should().BeEquivalentTo(CustomContainer.String);
        }

        [Test]
        public void CreateObject_WithIgnoredDependency_NotResolved()
        {
            CreateContainerWith<CustomContainer>();

            var component = NewGameObject().AddComponent<StringDependencyComponent>();
            component.gameObject.AddComponent<Resolver>();

            component.String.Should().BeEquivalentTo(CustomContainer.String);
        }

        [Test]
        public void CreateObject_WithParentDependency_ResolvedViaChild()
        {
            CreateContainerWith<CustomContainer>();

            var component = NewGameObject().AddComponent<ParentDependencyComponent>();
            component.gameObject.AddComponent<Resolver>();

            component.Parent.Should().BeAssignableTo<Child>();
        }
    }
}