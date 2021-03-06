﻿using UnityEngine;
using System.Collections.Generic;

public static class LevelManager
{
    public static int MaxLevelNo { get { return _levels.Count; } }

    public class LevelDefinition
    {
        public GraphType GraphType { get; set; }
        public IGraphProperties GraphSettings { get; set; }
        public int EnemiesCount { get; set; }
        public int[][] PowerDotLocations { get; set; }
        public int PlayerRespawnNodeIndex { get; set; }
        public int[] EnemiesRespawnNodeIndexes { get; set; }

        public LevelDefinition()
        {
            PlayerRespawnNodeIndex = 0;
        }
    }

    public static LevelDefinition GetLevelDefinition(int levelNo)
    {
        if (levelNo < 1 || levelNo > MaxLevelNo)
        {
            levelNo = 1;
        }
        return _levels[levelNo - 1];
    }

    public static void InitiateLevels(Vector3 seedPosition)
    {
        LevelDefinition cube1x1 = new LevelDefinition
        {
            GraphType = GraphType.Cubic,
            GraphSettings = new CubicGraphProperties
            {
                Location = seedPosition,
                PartsCount = 2,
                RandomizationPercentage = 1,
                Size = new Vector3(100, 75, 75)
            },
            EnemiesCount = 2,
            PowerDotLocations = new[] { new[] { 2, 6 }, new[] { 5, 1 } },
            EnemiesRespawnNodeIndexes = new [] { 3, 4 }
        };
        LevelDefinition square = new LevelDefinition
        {
            GraphType = GraphType.Conic,
            GraphSettings = new ConicGraphProperties
            {
                Location = seedPosition,
                RandomizationPercentage = 1,
                BaseRadius = 100,
                FloorsCount = 2,
                Height = 200,
                FloorsNodesCounts = new[] { 4, 0 }
            },
            EnemiesCount = 0,
        };
        LevelDefinition cube2x2 = new LevelDefinition
        {
            GraphType = GraphType.Cubic,
            GraphSettings = new CubicGraphProperties
            {
                Location = seedPosition,
                PartsCount = 3,
                RandomizationPercentage = 1,
                Size = new Vector3(200, 150, 150)
            },
            EnemiesCount = 3,
            PowerDotLocations = new[] { new[] { 4, 13 }, new[] { 25, 26 }, new[] { 18, 19 } },
            EnemiesRespawnNodeIndexes = new [] { 10, 21, 24 }
        };
        LevelDefinition tetrahedron = new LevelDefinition
        {
            GraphType = GraphType.Conic,
            GraphSettings = new ConicGraphProperties
            {
                Location = seedPosition,
                RandomizationPercentage = 1,
                BaseRadius = 100,
                FloorsCount = 2,
                Height = 200,
                FloorsNodesCounts = new[] { 4, 0 }
            },
            EnemiesCount = 1,
            PowerDotLocations = new[] { new[] { 0, 1 } },
            EnemiesRespawnNodeIndexes = new [] {2}
        };
        LevelDefinition conic1x3x2 = new LevelDefinition
        {
            GraphType = GraphType.Conic,
            GraphSettings = new ConicGraphProperties
            {
                Location = seedPosition,
                RandomizationPercentage = 1,
                BaseRadius = 200,
                FloorsCount = 3,
                Height = 400,
                FloorsNodesCounts = new[] { 2, 3, 1 }
            },
            EnemiesCount = 4,
            PowerDotLocations = new[] { new[] { 1, 4 }, new[] { 2, 5 } },
            EnemiesRespawnNodeIndexes = new [] { 2, 3, 4, 5 }
        };
        LevelDefinition spiral = new LevelDefinition
        {
            GraphType = GraphType.Spiral,
            GraphSettings = new SpiralGraphProperties
            {
                Location = seedPosition,
                RandomizationPercentage = 0f,
                BaseRadius = 200,
                FloorsCount = 3,
                Height = 400,
                EdgesProbability = 0.2f,
                FloorsNodesCounts = new[] { 10, 5, 1 }
            },
            EnemiesCount = 4,
            PowerDotLocations = new[] { new[] { 6, 7 }, new[] { 10, 11 }, new[] { 14, 15 } },
            EnemiesRespawnNodeIndexes = new [] {10,13,15, 6}
        };
        LevelDefinition spiral2 = new LevelDefinition
        {
            GraphType = GraphType.Spiral,
            GraphSettings = new SpiralGraphProperties
            {
                Location = seedPosition,
                RandomizationPercentage = 0f,
                BaseRadius = 250,
                FloorsCount = 6,
                Height = 600,
                EdgesProbability = 0.2f,
                FloorsNodesCounts = new[] { 15, 10, 10, 8, 7, 3 }
            },
            EnemiesCount = 4,
            PowerDotLocations = new[] { new[] { 0, 1 } },
            EnemiesRespawnNodeIndexes = new[] { 10, 13, 15, 6 }
        };

        _levels = new List<LevelDefinition>
        {
            square,
            tetrahedron,
            cube1x1,
            conic1x3x2,
            cube2x2,
            spiral,
            spiral2
        };
    }

    private static List<LevelDefinition> _levels;
}
