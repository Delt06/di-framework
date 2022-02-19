using System;
using System.Collections.Generic;
using System.Linq;
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
        private static readonly int FakeNodeIndex = -1;

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
                var node = GenerateNode(title);
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

            var unresolvedDependencies = new HashSet<Type>();
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

            ApplyLayerBasedLayout(rawDependencies, dependencyNodes);

            foreach (var dependencyNode in dependencyNodes)
            {
                dependencyNode.RefreshExpandedState();
                dependencyNode.RefreshPorts();
            }

            deleteSelection = delegate { };
            Focus();
        }

        // The sugiyama algorithm
        // https://drive.google.com/file/d/1uAAch1SxLLVBJ53ZX-zX4AnwzwhcXcEM/view
        private static void ApplyLayerBasedLayout(DependencyWithMetadata[] rawDependencies,
            List<DependencyNode> dependencyNodes)
        {
            var nodeDataList = new NodeData[rawDependencies.Length];
            PopulateNodeFollowers(rawDependencies, nodeDataList);

            var visited = new HashSet<int>();
            for (var i = 0; i < rawDependencies.Length; i++)
            {
                AssignLayers(nodeDataList, visited, i);
            }

            var nodeDataIndicesByLayers = GetNodeDataIndicesByLayers(nodeDataList, out var maxNodesInLayer);
            SetNodePositions(dependencyNodes, nodeDataIndicesByLayers, maxNodesInLayer);
        }

        private static void PopulateNodeFollowers(DependencyWithMetadata[] rawDependencies, NodeData[] nodeDataList)
        {
            for (var nodeDataIndex = 0; nodeDataIndex < nodeDataList.Length; nodeDataIndex++)
            {
                var nodeData = new NodeData();
                nodeDataList[nodeDataIndex] = nodeData;
            }

            for (var nodeDataIndex = 0; nodeDataIndex < nodeDataList.Length; nodeDataIndex++)
            {
                var nodeData = nodeDataList[nodeDataIndex];

                for (var otherIndex = 0; otherIndex < rawDependencies.Length; otherIndex++)
                {
                    if (nodeDataIndex == otherIndex) continue;

                    var nodeDependency = rawDependencies[nodeDataIndex];
                    var otherDependency = rawDependencies[otherIndex];
                    if (DependencyUtils.DependsOn(otherDependency, nodeDependency))
                    {
                        nodeData.Followers.Add(otherIndex);
                        nodeDataList[otherIndex].Predecessors.Add(nodeDataIndex);
                    }
                }
            }
        }

        private static void AssignLayers(NodeData[] nodeDataList, HashSet<int> visited, int index, int layer = 0)
        {
            if (visited.Contains(index)) return;

            var nodeData = nodeDataList[index];
            nodeData.NodeLayer = nodeData.Predecessors.Count == 0
                ? layer
                : Mathf.Max(layer, nodeData.Predecessors.Max(p => nodeDataList[p].NodeLayer + 1));
            visited.Add(index);

            foreach (var followerIndex in nodeData.Followers)
            {
                AssignLayers(nodeDataList, visited, followerIndex, nodeData.NodeLayer + 1);
            }
        }

        private static List<int>[] GetNodeDataIndicesByLayers(NodeData[] nodeDataList, out int maxNodesInLayer)
        {
            var maxLayer = nodeDataList.Select(nd => nd.NodeLayer).Max();
            var nodeDataIndicesByLayers = new List<int>[maxLayer + 1];
            maxNodesInLayer = 0;

            for (var layer = 0; layer <= maxLayer; layer++)
            {
                var capturedLayer = layer;
                var nodeIndicesOnThisLayer = nodeDataList
                        .Select((nd, i) => (nd, i))
                        .Where(t => t.nd.NodeLayer == capturedLayer)
                        .Select(t => t.i)
                        .ToList()
                    ;
                nodeDataIndicesByLayers[layer] = nodeIndicesOnThisLayer;
                maxNodesInLayer = Mathf.Max(maxNodesInLayer, nodeIndicesOnThisLayer.Count);
            }

            for (var layer = 0; layer < maxLayer; layer++)
            {
                var nodeIndicesOnThisLayer = nodeDataIndicesByLayers[layer];

                foreach (var nodeIndex in nodeIndicesOnThisLayer)
                {
                    if (nodeIndex == FakeNodeIndex) continue;

                    var nodeData = nodeDataList[nodeIndex];
                    if (nodeData.Followers.All(f => nodeDataList[f].NodeLayer == nodeData.NodeLayer + 1)) continue;

                    // if there are some followers further from the next layer, insert fake node
                    var nodeIndicesOnNextLayer = nodeDataIndicesByLayers[layer + 1];
                    if (nodeIndex < nodeIndicesOnNextLayer.Count)
                        nodeIndicesOnNextLayer.Insert(nodeIndex, FakeNodeIndex);
                    else
                        nodeIndicesOnNextLayer.Add(FakeNodeIndex);
                }
            }

            return nodeDataIndicesByLayers;
        }

        private static void SetNodePositions(List<DependencyNode> dependencyNodes, List<int>[] nodeDataIndicesByLayers,
            int maxNodesInLayer)
        {
            for (var layer = 0; layer < nodeDataIndicesByLayers.Length; layer++)
            {
                var nodeDataIndices = nodeDataIndicesByLayers[layer];

                for (var indexOnLayer = 0; indexOnLayer < nodeDataIndices.Count; indexOnLayer++)
                {
                    var nodeDataIndex = nodeDataIndices[indexOnLayer];
                    if (nodeDataIndex == FakeNodeIndex) continue;

                    var node = dependencyNodes[nodeDataIndex];
                    var nodePosition = node.GetPosition();
                    nodePosition.x = layer * 300;
                    var t = indexOnLayer / (nodeDataIndices.Count - 1f);
                    nodePosition.y = 100 * maxNodesInLayer * t;
                    node.SetPosition(nodePosition);
                }
            }
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) => new List<Port>();

        private static DependencyNode GenerateNode(string title)
        {
            var node = new DependencyNode
            {
                title = title,
            };

            var position = Vector2.zero;
            var size = new Vector2(150, 100);

            node.SetPosition(new Rect(position, size));
            return node;
        }

        private class NodeData
        {
            public readonly List<int> Followers = new List<int>();
            public readonly List<int> Predecessors = new List<int>();
            public int NodeLayer;
        }
    }
}