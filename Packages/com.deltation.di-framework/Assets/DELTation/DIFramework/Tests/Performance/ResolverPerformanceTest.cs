using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEditor;
using UnityEngine;

namespace DELTation.DIFramework.Tests.Performance
{
    [Explicit]
    public class ResolverPerformanceTest
    {
        private const int WarmupCount = 10;
        private const int MeasurementCount = 25;
        private const int IterationsPerMeasurement = 10;

        [Test, Performance]
        public void InstantiatePrefabWithDi()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PerformanceTest/Prefabs/With DI.prefab");

            Measure.Method(() => Object.Instantiate(prefab))
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void InstantiatePrefabWithoutDi()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PerformanceTest/Prefabs/Without DI.prefab");
            Measure.Method(() => Object.Instantiate(prefab))
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .GC()
                .Run();
        }
    }
}