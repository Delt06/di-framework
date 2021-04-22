using System;
using System.Collections.Generic;
using DELTation.DIFramework.Pooling;
using JetBrains.Annotations;

namespace DELTation.DIFramework.Sorting
{
    public static class TopologicalSorting
    {
        public static void Sort([NotNull] IReadOnlyList<IReadOnlyList<int>> graph, int nodesCount,
            [NotNull] ICollection<int> result, out bool loop)
        {
            if (graph == null) throw new ArgumentNullException(nameof(graph));
            if (result == null) throw new ArgumentNullException(nameof(result));

            var colors = ListPool<NodeColor>.Rent();
            loop = false;

            for (var i = 0; i < nodesCount; i++)
            {
                colors.Add(NodeColor.White);
            }

            for (var i = 0; i < nodesCount; i++)
            {
                SortStep(colors, i, graph, result, out loop);
                if (!loop) continue;

                ListPool<NodeColor>.Return(colors);
                return;
            }

            ListPool<NodeColor>.Return(colors);
        }

        private static void SortStep([NotNull] List<NodeColor> colors, int index,
            [NotNull] IReadOnlyList<IReadOnlyList<int>> graph, [NotNull] ICollection<int> result, out bool loop)
        {
            var color = colors[index];
            loop = false;

            switch (color)
            {
                case NodeColor.Black:
                    break;
                case NodeColor.Grey:
                    loop = true;
                    break;
                case NodeColor.White:
                    colors[index] = NodeColor.Grey;
                    var adjacentNodes = graph[index];

                    for (var adjacentIndex = 0; adjacentIndex < adjacentNodes.Count; adjacentIndex++)
                    {
                        SortStep(colors, adjacentNodes[adjacentIndex], graph, result, out loop);
                        if (loop)
                            return;
                    }

                    colors[index] = NodeColor.Black;
                    result.Add(index);
                    break;
            }
        }

        private enum NodeColor
        {
            Black,
            Grey,
            White,
        }
    }
}