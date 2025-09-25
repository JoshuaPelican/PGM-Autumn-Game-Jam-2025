using System.Collections.Generic;
using UnityEngine;

public class Constellation : MonoBehaviour
{
    public const float SCALE = 0.15f;
    public const float LINE_RESOLUTION = 0.1f;

    [SerializeField] string ConstellationID;
    [SerializeField] Transform StarPrefab;
    [SerializeField] LineRenderer LinePrefab;


    ConstellationData data;
    List<Vector2> linePoints;
    public List<Vector2> LinePoints => linePoints;

    private void Awake()
    {
        GetConstellationData();
        SpawnConstellation();
    }

    void GetConstellationData()
    {
        data = ConstellationDatabase.Constellations[ConstellationID];
        CalcAllLinePoints();
    }

    void SpawnConstellation()
    {
        foreach (StarData starData in data.Stars)
        {
            Transform star = Instantiate(StarPrefab, transform.position + (Vector3)(starData.Position * SCALE), Quaternion.identity, transform);
            star.localScale *= starData.Magnitude;
        }
        int n = 0;
        foreach ((int, int) line in data.Lines)
        {
            Vector2 pos1 = data.Stars[line.Item1].Position * SCALE;
            Vector2 pos2 = data.Stars[line.Item2].Position * SCALE;

            LineRenderer lineRend = Instantiate(LinePrefab, transform);
            lineRend.name = $"Line {++n}";
            lineRend.positionCount = 2;
            lineRend.SetPositions(new Vector3[] { pos1, pos2 });

            float dist = Vector2.Distance(pos1, pos2);

            int numLinePoints = Mathf.CeilToInt(dist / LINE_RESOLUTION);

            Vector2[] linePoints = new Vector2[numLinePoints];
            for (int i = 0; i < numLinePoints; i++)
            {
                linePoints[i] = Vector2.Lerp(pos1, pos2, i / (float)numLinePoints);
            }
        }
    }

    public void CalcAllLinePoints()
    {
        linePoints = new List<Vector2>();

        foreach ((int, int) line in data.Lines)
        {
            Vector2 pos1 = data.Stars[line.Item1].Position * SCALE;
            Vector2 pos2 = data.Stars[line.Item2].Position * SCALE;

            float dist = Vector2.Distance(pos1, pos2);

            int numLinePoints = Mathf.CeilToInt(dist / LINE_RESOLUTION);

            for (int i = 0; i < numLinePoints; i++)
            {
                linePoints.Add(Vector2.Lerp(pos1, pos2, i / (float)numLinePoints));
            }
        }
    }
}

public class ConstellationData
{
    public List<StarData> Stars;
    public List<(int, int)> Lines;

    public ConstellationData(List<StarData> stars, List<(int, int)> lines)
    {
        Stars = stars;
        Lines = lines;
    }
}

public class StarData
{
    public Vector2 Position;
    public float Magnitude; // Used for size, brightness, etc.

    public StarData(Vector2 position, float magnitude)
    {
        Position = position;
        Magnitude = magnitude;
    }
}


public static class ConstellationDatabase
{
    public static Dictionary<string, ConstellationData> Constellations = new Dictionary<string, ConstellationData>
    {
        // SIMPLE CONSTELLATIONS (3-5 stars) - Great for learning controls

        ["The Triangle Portal"] = new ConstellationData(
            new List<StarData>
            {
                new StarData(new Vector2(0, 60), 1.5f),      // Top
                new StarData(new Vector2(-50, -30), 1.8f),   // Bottom left
                new StarData(new Vector2(50, -30), 1.6f)     // Bottom right
            },
            new List<(int, int)> { (0, 1), (1, 2), (2, 0) }
        ),

        ["The Cosmic Bowtie"] = new ConstellationData(
            new List<StarData>
            {
                new StarData(new Vector2(-40, 40), 1.4f),    // Top left
                new StarData(new Vector2(0, 0), 1.2f),       // Center
                new StarData(new Vector2(40, -40), 1.6f),    // Bottom right
                new StarData(new Vector2(-40, -40), 1.8f),   // Bottom left
                new StarData(new Vector2(40, 40), 1.5f)      // Top right
            },
            new List<(int, int)> { (0, 1), (1, 2), (1, 3), (1, 4) }
        ),

        ["The Star Cross"] = new ConstellationData(
            new List<StarData>
            {
                new StarData(new Vector2(0, 0), 1.3f),       // Center
                new StarData(new Vector2(0, 70), 1.8f),      // North
                new StarData(new Vector2(70, 0), 1.9f),      // East
                new StarData(new Vector2(0, -70), 1.7f),     // South
                new StarData(new Vector2(-70, 0), 2.0f)      // West
            },
            new List<(int, int)> { (0, 1), (0, 2), (0, 3), (0, 4) }
        ),

        // MODERATE CONSTELLATIONS (6-8 stars) - Fun traversal patterns

        ["The Orbital Ring"] = new ConstellationData(
            new List<StarData>
            {
                new StarData(new Vector2(0, 80), 1.6f),      // Top
                new StarData(new Vector2(56, 56), 1.8f),     // Top right
                new StarData(new Vector2(80, 0), 1.7f),      // Right
                new StarData(new Vector2(56, -56), 1.9f),    // Bottom right
                new StarData(new Vector2(0, -80), 1.8f),     // Bottom
                new StarData(new Vector2(-56, -56), 2.0f),   // Bottom left
                new StarData(new Vector2(-80, 0), 1.7f),     // Left
                new StarData(new Vector2(-56, 56), 1.9f)     // Top left
            },
            new List<(int, int)> { (0, 1), (1, 2), (2, 3), (3, 4), (4, 5), (5, 6), (6, 7), (7, 0) }
        ),

        ["The Zigzag Lightning"] = new ConstellationData(
            new List<StarData>
            {
                new StarData(new Vector2(-100, 80), 1.5f),   // Start
                new StarData(new Vector2(-40, 40), 1.8f),    // Zig 1
                new StarData(new Vector2(-80, 0), 2.0f),     // Zag 1
                new StarData(new Vector2(-20, -40), 1.7f),   // Zig 2
                new StarData(new Vector2(-60, -80), 1.9f),   // Zag 2
                new StarData(new Vector2(0, -40), 1.6f),     // Zig 3
                new StarData(new Vector2(60, -80), 2.1f),    // End
            },
            new List<(int, int)> { (0, 1), (1, 2), (2, 3), (3, 4), (4, 5), (5, 6) }
        ),

        ["The Swing Bridge"] = new ConstellationData(
            new List<StarData>
            {
                new StarData(new Vector2(-120, 40), 1.4f),   // Left anchor
                new StarData(new Vector2(-80, 20), 2.0f),    // Support 1
                new StarData(new Vector2(-40, -10), 2.2f),   // Low point 1
                new StarData(new Vector2(0, -20), 2.3f),     // Lowest point
                new StarData(new Vector2(40, -10), 2.1f),    // Low point 2
                new StarData(new Vector2(80, 20), 1.9f),     // Support 2
                new StarData(new Vector2(120, 40), 1.5f)     // Right anchor
            },
            new List<(int, int)> { (0, 1), (1, 2), (2, 3), (3, 4), (4, 5), (5, 6) }
        ),

        ["The Spiral Galaxy"] = new ConstellationData(
            new List<StarData>
            {
                new StarData(new Vector2(0, 0), 1.2f),       // Center
                new StarData(new Vector2(20, 0), 1.8f),      // Arm start
                new StarData(new Vector2(30, 25), 2.0f),     // Spiral 1
                new StarData(new Vector2(15, 50), 2.1f),     // Spiral 2
                new StarData(new Vector2(-20, 45), 2.2f),    // Spiral 3
                new StarData(new Vector2(-50, 15), 1.9f),    // Spiral 4
                new StarData(new Vector2(-40, -30), 2.0f),   // Spiral 5
                new StarData(new Vector2(0, -60), 2.1f)      // Spiral end
            },
            new List<(int, int)> { (0, 1), (1, 2), (2, 3), (3, 4), (4, 5), (5, 6), (6, 7) }
        ),

        // COMPLEX CONSTELLATIONS (9-15 stars) - Challenging navigation

        ["The Quantum Web"] = new ConstellationData(
            new List<StarData>
            {
                new StarData(new Vector2(0, 0), 1.3f),       // Center hub
                new StarData(new Vector2(-60, 60), 1.8f),    // NW outer
                new StarData(new Vector2(0, 80), 1.7f),      // N outer
                new StarData(new Vector2(60, 60), 1.9f),     // NE outer
                new StarData(new Vector2(80, 0), 1.6f),      // E outer
                new StarData(new Vector2(60, -60), 2.0f),    // SE outer
                new StarData(new Vector2(0, -80), 1.8f),     // S outer
                new StarData(new Vector2(-60, -60), 1.9f),   // SW outer
                new StarData(new Vector2(-80, 0), 1.7f),     // W outer
                new StarData(new Vector2(-30, 30), 2.2f),    // Inner NW
                new StarData(new Vector2(30, 30), 2.1f),     // Inner NE
                new StarData(new Vector2(30, -30), 2.3f),    // Inner SE
                new StarData(new Vector2(-30, -30), 2.2f)    // Inner SW
            },
            new List<(int, int)> { (0,1), (0,2), (0,3), (0,4), (0,5), (0,6), (0,7), (0,8),
                                 (1,9), (2,9), (3,10), (4,10), (5,11), (6,11), (7,12), (8,12),
                                 (9,10), (10,11), (11,12), (12,9) }
        ),

        ["The Asteroid Belt"] = new ConstellationData(
            new List<StarData>
            {
                new StarData(new Vector2(-100, 20), 2.0f),   // Belt 1
                new StarData(new Vector2(-70, -10), 2.3f),   // Belt 2
                new StarData(new Vector2(-40, 15), 1.9f),    // Belt 3
                new StarData(new Vector2(-10, -20), 2.1f),   // Belt 4
                new StarData(new Vector2(20, 10), 2.4f),     // Belt 5
                new StarData(new Vector2(50, -15), 1.8f),    // Belt 6
                new StarData(new Vector2(80, 25), 2.2f),     // Belt 7
                new StarData(new Vector2(110, -5), 2.0f),    // Belt 8
                new StarData(new Vector2(-60, 50), 2.5f),    // Stray 1
                new StarData(new Vector2(30, 60), 2.3f),     // Stray 2
                new StarData(new Vector2(0, -60), 2.1f),     // Stray 3
                new StarData(new Vector2(70, -50), 2.4f)     // Stray 4
            },
            new List<(int, int)> { (0,1), (1,2), (2,3), (3,4), (4,5), (5,6), (6,7),
                                 (2,8), (4,9), (3,10), (5,11) }
        ),

        ["The Fractal Tree"] = new ConstellationData(
            new List<StarData>
            {
                new StarData(new Vector2(0, -80), 1.4f),     // Root
                new StarData(new Vector2(0, -40), 1.6f),     // Trunk
                new StarData(new Vector2(0, 0), 1.5f),       // Main branch
                new StarData(new Vector2(-30, 30), 1.8f),    // Left main
                new StarData(new Vector2(30, 30), 1.7f),     // Right main
                new StarData(new Vector2(-50, 60), 2.0f),    // Left branch 1
                new StarData(new Vector2(-10, 60), 2.1f),    // Left branch 2
                new StarData(new Vector2(10, 60), 1.9f),     // Right branch 1
                new StarData(new Vector2(50, 60), 2.0f),     // Right branch 2
                new StarData(new Vector2(-70, 80), 2.3f),    // Left leaf 1
                new StarData(new Vector2(-40, 85), 2.4f),    // Left leaf 2
                new StarData(new Vector2(-20, 85), 2.2f),    // Left leaf 3
                new StarData(new Vector2(20, 85), 2.3f),     // Right leaf 1
                new StarData(new Vector2(40, 85), 2.1f),     // Right leaf 2
                new StarData(new Vector2(70, 80), 2.4f)      // Right leaf 3
            },
            new List<(int, int)> { (0,1), (1,2), (2,3), (2,4), (3,5), (3,6), (4,7), (4,8),
                                 (5,9), (5,10), (6,11), (7,12), (8,13), (8,14) }
        ),

        // EXTREME COMPLEXITY (15+ stars) - For advanced players

        ["The Neural Network"] = new ConstellationData(
            new List<StarData>
            {
                // Input layer
                new StarData(new Vector2(-120, 60), 2.0f),   // Input 1
                new StarData(new Vector2(-120, 20), 2.1f),   // Input 2  
                new StarData(new Vector2(-120, -20), 1.9f),  // Input 3
                new StarData(new Vector2(-120, -60), 2.0f),  // Input 4
                
                // Hidden layer 1
                new StarData(new Vector2(-40, 80), 1.7f),    // Hidden 1
                new StarData(new Vector2(-40, 40), 1.6f),    // Hidden 2
                new StarData(new Vector2(-40, 0), 1.8f),     // Hidden 3
                new StarData(new Vector2(-40, -40), 1.5f),   // Hidden 4
                new StarData(new Vector2(-40, -80), 1.7f),   // Hidden 5
                
                // Hidden layer 2
                new StarData(new Vector2(40, 50), 1.8f),     // Hidden 6
                new StarData(new Vector2(40, 10), 1.6f),     // Hidden 7
                new StarData(new Vector2(40, -30), 1.9f),    // Hidden 8
                
                // Output layer
                new StarData(new Vector2(120, 30), 1.4f),    // Output 1
                new StarData(new Vector2(120, -10), 1.3f)    // Output 2
            },
            new List<(int, int)> { 
                // Input to Hidden 1 connections
                (0,4), (0,5), (0,6), (1,4), (1,5), (1,6), (1,7),
                (2,5), (2,6), (2,7), (2,8), (3,6), (3,7), (3,8),
                // Hidden 1 to Hidden 2 connections
                (4,9), (4,10), (5,9), (5,10), (5,11), (6,9), (6,10), (6,11),
                (7,10), (7,11), (8,11),
                // Hidden 2 to Output connections
                (9,12), (9,13), (10,12), (10,13), (11,12), (11,13)
            }
        ),

        ["The Cosmic Labyrinth"] = new ConstellationData(
            new List<StarData>
            {
                // Outer walls
                new StarData(new Vector2(-100, 100), 1.8f),  new StarData(new Vector2(0, 100), 1.9f),
                new StarData(new Vector2(100, 100), 1.7f),   new StarData(new Vector2(100, 0), 2.0f),
                new StarData(new Vector2(100, -100), 1.8f),  new StarData(new Vector2(0, -100), 1.9f),
                new StarData(new Vector2(-100, -100), 1.7f), new StarData(new Vector2(-100, 0), 2.0f),
                
                // Inner maze walls
                new StarData(new Vector2(-60, 60), 2.1f),    new StarData(new Vector2(-20, 60), 2.2f),
                new StarData(new Vector2(20, 60), 2.1f),     new StarData(new Vector2(60, 60), 2.2f),
                new StarData(new Vector2(-60, 20), 2.3f),    new StarData(new Vector2(-20, 20), 2.1f),
                new StarData(new Vector2(20, 20), 2.2f),     new StarData(new Vector2(60, 20), 2.3f),
                new StarData(new Vector2(-60, -20), 2.1f),   new StarData(new Vector2(-20, -20), 2.2f),
                new StarData(new Vector2(20, -20), 2.1f),    new StarData(new Vector2(60, -20), 2.2f),
                new StarData(new Vector2(-60, -60), 2.3f),   new StarData(new Vector2(-20, -60), 2.1f),
                new StarData(new Vector2(20, -60), 2.2f),    new StarData(new Vector2(60, -60), 2.3f),
                
                // Center goal
                new StarData(new Vector2(0, 0), 1.2f)        // Goal star
            },
            new List<(int, int)> { 
                // Outer perimeter
                (0,1), (1,2), (2,3), (3,4), (4,5), (5,6), (6,7), (7,0),
                // Maze paths (creating a solvable maze pattern)
                (8,9), (10,11), (12,16), (13,17), (14,18), (15,19), (20,21), (22,23),
                (9,13), (11,15), (16,20), (18,22), (17,21), (19,23),
                // Path to center
                (13,24), (18,24)
            }
        ),

        // SPECIALTY CONSTELLATIONS - Unique traversal challenges

        ["The Pendulum Chain"] = new ConstellationData(
            new List<StarData>
            {
                new StarData(new Vector2(-80, 80), 1.5f),    // Anchor 1
                new StarData(new Vector2(-60, 40), 2.0f),    // Chain 1
                new StarData(new Vector2(-30, 0), 2.1f),     // Chain 2
                new StarData(new Vector2(0, -30), 2.2f),     // Chain 3
                new StarData(new Vector2(30, -50), 2.1f),    // Chain 4
                new StarData(new Vector2(60, -60), 2.0f),    // Chain 5
                new StarData(new Vector2(80, -80), 1.8f),    // Weight
                new StarData(new Vector2(40, 80), 1.6f),     // Anchor 2
                new StarData(new Vector2(20, 50), 2.3f),     // Counter chain 1
                new StarData(new Vector2(-10, 20), 2.4f),    // Counter chain 2
                new StarData(new Vector2(-40, -10), 2.2f)    // Counter weight
            },
            new List<(int, int)> { (0, 1), (1, 2), (2, 3), (3, 4), (4, 5), (5, 6), (7, 8), (8, 9), (9, 10) }
        ),

        ["The Orbital Dance"] = new ConstellationData(
            new List<StarData>
            {
                new StarData(new Vector2(0, 0), 1.1f),       // Central star
                // Inner orbit
                new StarData(new Vector2(40, 0), 1.8f),      new StarData(new Vector2(0, 40), 1.9f),
                new StarData(new Vector2(-40, 0), 1.7f),     new StarData(new Vector2(0, -40), 1.8f),
                // Middle orbit  
                new StarData(new Vector2(70, 0), 2.0f),      new StarData(new Vector2(50, 50), 2.1f),
                new StarData(new Vector2(0, 70), 2.0f),      new StarData(new Vector2(-50, 50), 2.1f),
                new StarData(new Vector2(-70, 0), 2.0f),     new StarData(new Vector2(-50, -50), 2.1f),
                new StarData(new Vector2(0, -70), 2.0f),     new StarData(new Vector2(50, -50), 2.1f),
                // Outer orbit
                new StarData(new Vector2(100, 0), 2.2f),     new StarData(new Vector2(0, 100), 2.3f),
                new StarData(new Vector2(-100, 0), 2.2f),    new StarData(new Vector2(0, -100), 2.3f)
            },
            new List<(int, int)> { 
                // Center to inner orbit
                (0,1), (0,2), (0,3), (0,4),
                // Inner to middle orbit connections
                (1,5), (2,7), (3,9), (4,11), (1,12), (2,6), (3,8), (4,10),
                // Middle to outer orbit
                (5,13), (7,14), (9,15), (11,16), (6,14), (8,15), (10,16), (12,13)
            }
        )
    };
}