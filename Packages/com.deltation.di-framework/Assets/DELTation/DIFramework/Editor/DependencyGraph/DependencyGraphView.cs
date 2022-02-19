using System;
using System.Collections.Generic;
using DELTation.DIFramework.Containers;
using DELTation.DIFramework.Dependencies;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DELTation.DIFramework.Editor.DependencyGraph
{
    public class DependencyGraphView : GraphView
    {
        public DependencyGraphView([NotNull] DependencyContainerBase dependencyContainer)
        {
            if (dependencyContainer == null) throw new ArgumentNullException(nameof(dependencyContainer));

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var rawDependencies = dependencyContainer.GetRawDependencies();
            var dependencyNodes = new List<DependencyNode>();

            for (var index = 0; index < rawDependencies.Length; index++)
            {
                var rawDependency = rawDependencies[index];
                var typeName = rawDependency.GetResultingType().GetFriendlyName();
                var title = $"{typeName} ({rawDependency.GetInternalDependencyTypeName()})";
                var node = GenerateNode(index, title);
                dependencyNodes.Add(node);
                AddElement(node);
            }

            for (var i = 0; i < rawDependencies.Length; i++)
            {
                for (var j = 0; j < rawDependencies.Length; j++)
                {
                    var dependent = rawDependencies[i];
                    var dependency = rawDependencies[j];
                    if (!DependencyUtils.DependsOn(dependent, dependency, out var requiredDependencyType)) continue;

                    var dependentNode = dependencyNodes[i];
                    var dependencyNode = dependencyNodes[j];
                    var edge = dependentNode.AddDependency(dependencyNode, requiredDependencyType);
                    AddElement(edge);
                }
            }

            var unresolvedDependencies = new List<Type>();
            var possibleDependencyResolvers = new List<Type>();

            for (var index = 0; index < rawDependencies.Length; index++)
            {
                unresolvedDependencies.Clear();
                possibleDependencyResolvers.Clear();
                var dependency = rawDependencies[index];

                for (var i = 0; i < index; i++)
                {
                    possibleDependencyResolvers.Add(rawDependencies[i].GetResultingType());
                }

                if (DependencyUtils.DependenciesCanBeResolved(dependency, possibleDependencyResolvers,
                        unresolvedDependencies
                    )) continue;

                foreach (var unresolvedDependency in unresolvedDependencies)
                {
                    dependencyNodes[index].AddUnresolvedDependencyPort(unresolvedDependency);
                }
            }

            foreach (var dependencyNode in dependencyNodes)
            {
                dependencyNode.RefreshExpandedState();
                dependencyNode.RefreshPorts();
            }

            deleteSelection = delegate { };
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) => new List<Port>();

        private DependencyNode GenerateNode(int index, string title)
        {
            var node = new DependencyNode
            {
                title = title,
            };

            node.SetPosition(new Rect(100 + 400 * index, 200, 100, 150));
            return node;
        }
    }
}