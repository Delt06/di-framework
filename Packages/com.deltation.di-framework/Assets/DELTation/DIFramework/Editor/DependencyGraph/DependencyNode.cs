using System;
using UnityEditor.Experimental.GraphView;

namespace DELTation.DIFramework.Editor.DependencyGraph
{
    public class DependencyNode : Node
    {
        public DependencyNode()
        {
            DependantsPort = GeneratePort(Direction.Output, Port.Capacity.Multi);
            DependantsPort.portName = "";
            outputContainer.Add(DependantsPort);
        }

        private Port DependantsPort { get; }

        private Port CreateDependencyPort(Type type)
        {
            var dependencyPort = GeneratePort(Direction.Input, Port.Capacity.Single);
            dependencyPort.portName = type.GetFriendlyName();
            inputContainer.Add(dependencyPort);
            return dependencyPort;
        }

        private Port GeneratePort(Direction portDirection, Port.Capacity capacity) =>
            InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));

        public Edge AddDependency(DependencyNode dependencyNode, Type requiredDependencyType)
        {
            var dependencyPort = CreateDependencyPort(requiredDependencyType);
            var edge = dependencyPort.ConnectTo(dependencyNode.DependantsPort);
            return edge;
        }

        public void AddUnresolvedDependencyPort(Type requiredDependencyType)
        {
            var port = CreateDependencyPort(requiredDependencyType);
            port.portName = $"(UNRESOLVED) {port.portName}";
        }
    }
}