﻿using UnityEngine;
using System.Collections.Generic;

public class CubicGraphGenerator : IGraphGenerator
{
    public Vector3[] GenerateGraph(IGraphProperties graphProperties, out int[][] edges)
    {
        List<Vector3> nodes = new List<Vector3>();
        List<int[]> edgesList = new List<int[]>();

        CubicGraphProperties cubicGraphProperties = graphProperties as CubicGraphProperties;;
        Vector3[,,] nodesMesh =
            new Vector3[cubicGraphProperties.PartsCount, cubicGraphProperties.PartsCount,
                cubicGraphProperties.PartsCount];

        Vector3 partSize = cubicGraphProperties.Size / (cubicGraphProperties.PartsCount - 1);

        GenerateNodes(cubicGraphProperties.Location, cubicGraphProperties.PartsCount,
            cubicGraphProperties.RandomizationPercentage, partSize, nodesMesh, nodes);
        GenerateEdges(cubicGraphProperties.PartsCount, edgesList, nodes, nodesMesh);

        edges = edgesList.ToArray();
        return nodes.ToArray();
    }

    private static void GenerateNodes(Vector3 location, int partsCount, float randomizationPercentage, Vector3 partSize,
        Vector3[,,] nodesMesh, List<Vector3> nodes)
    {
        for (int i = 0; i < partsCount; i++)
        {
            for (int j = 0; j < partsCount; j++)
            {
                for (int k = 0; k < partsCount; k++)
                {
                    Vector3 randomVector3 = GraphGeneratorHelper.GetRandomCartesianCoordinates(partSize, randomizationPercentage);

                    Vector3 node = new Vector3(location.x + i * partSize.x, location.y + j * partSize.y,
                        location.x + k * partSize.z) + randomVector3;

                    nodesMesh[i, j, k] = node;
                    nodes.Add(node);
                }
            }
        }
    }

    private static void GenerateEdges(int partsCount, List<int[]> edgesList, List<Vector3> nodes, Vector3[,,] nodesMesh)
    {
        for (int i = 0; i < partsCount; i++)
        {
            for (int j = 0; j < partsCount; j++)
            {
                for (int k = 0; k < partsCount; k++)
                {
                    if (i + 1 < partsCount)
                    {
                        edgesList.Add(new[] { nodes.IndexOf(nodesMesh[i, j, k]), nodes.IndexOf(nodesMesh[i + 1, j, k]) });
                    }
                    if (j + 1 < partsCount)
                    {
                        edgesList.Add(new[] { nodes.IndexOf(nodesMesh[i, j, k]), nodes.IndexOf(nodesMesh[i, j + 1, k]) });
                    }
                    if (k + 1 < partsCount)
                    {
                        edgesList.Add(new[] { nodes.IndexOf(nodesMesh[i, j, k]), nodes.IndexOf(nodesMesh[i, j, k + 1]) });
                    }
                }
            }
        }
    }
}
