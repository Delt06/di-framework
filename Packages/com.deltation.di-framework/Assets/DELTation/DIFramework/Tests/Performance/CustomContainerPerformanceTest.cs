using DELTation.DIFramework.Containers;
using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;

namespace DELTation.DIFramework.Tests.Performance
{
    [Explicit]
    public class CustomContainerPerformanceTest
    {
        [Test, Performance]
        public void GivenCustomContainer_WhenComposingAndResolving_ThenMeasurePerformance()
        {
            Measure.Method(() =>
                    {
                        {
                            var gameObject = new GameObject();
                            gameObject.AddComponent<RootDependencyContainer>();
                            gameObject.AddComponent<CustomContainer>();
                            Di.TryResolveGlobally(out StringDepDep _);
                        }
                    }
                )
                .WarmupCount(25)
                .MeasurementCount(25)
                .IterationsPerMeasurement(10)
                .GC()
                .Run();
        }

        private class StringDep
        {
            private string _str;

            public StringDep(string str) => _str = str;
        }

        private class StringDepDep
        {
            private StringDep _stringDep;

            public StringDepDep(StringDep stringDep) => _stringDep = stringDep;
        }

        private class CustomContainer : DependencyContainerBase
        {
            protected override void ComposeDependencies(ICanRegisterContainerBuilder builder)
            {
                builder.Register("abc")
                    .Register<StringDep>()
                    .Register<StringDepDep>();
            }
        }
    }
}