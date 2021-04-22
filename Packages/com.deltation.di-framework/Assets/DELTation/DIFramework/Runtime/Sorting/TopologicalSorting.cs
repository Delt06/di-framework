﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace DELTation.DIFramework.Sorting
{
    public static class TopologicalSorting
    {
        public static void Sort([NotNull] IReadOnlyList<IReadOnlyList<int>> graph, int nodesCount,
            [NotNull] LinkedList<int> result, out bool loop)
        {
            if (graph == null) throw new ArgumentNullException(nameof(graph));
            if (result == null) throw new ArgumentNullException(nameof(result));

            var colors = new NodeColor[nodesCount];
            loop = false;

            for (var i = 0; i < nodesCount; i++)
            {
                colors[i] = NodeColor.White;
            }

            for (var i = 0; i < nodesCount; i++)
            {
                SortStep(colors, i, graph, result, out loop);
                if (loop)
                    return;
            }
        }

        private static void SortStep([NotNull] NodeColor[] colors, int index,
            [NotNull] IReadOnlyList<IReadOnlyList<int>> graph, [NotNull] LinkedList<int> result, out bool loop)
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
                    result.AddFirst(index);
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