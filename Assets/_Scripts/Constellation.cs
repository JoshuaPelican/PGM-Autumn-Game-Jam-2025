using System.Collections.Generic;
using UnityEngine;

public class Constellation : MonoBehaviour
{
    public const float SCALE = 0.15f;
    public const float LINE_RESOLUTION = 0.1f;

    [SerializeField] ConstellationData constellationToSpawn;
    [SerializeField] Transform StarPrefab;
    [SerializeField] LineRenderer LinePrefab;

    List<Vector2> linePoints;
    public List<Vector2> LinePoints => linePoints;

    private void Awake()
    {
        GetConstellationData();
        SpawnConstellation();
    }

    void GetConstellationData()
    {
        //TEMP SOLUTION
        if (constellationToSpawn == null)
            constellationToSpawn = (ConstellationData)SceneManager.Instance.ScenePayload.payload.Find(x => x.Key == "ConstellationToLoad").Value;
        CalcAllLinePoints();
    }

    void SpawnConstellation()
    {
        foreach (StarData starData in constellationToSpawn.Stars)
        {
            Transform star = Instantiate(StarPrefab, (Vector3)(starData.Position * SCALE) + transform.position, Quaternion.identity, transform);
            star.localScale *= starData.Magnitude;
        }
        int n = 0;
        foreach (var line in constellationToSpawn.Lines)
        {
            Vector2 pos1 = constellationToSpawn.Stars[line.x].Position * SCALE + (Vector2)transform.position;
            Vector2 pos2 = constellationToSpawn.Stars[line.y].Position * SCALE + (Vector2)transform.position;

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

        foreach (var line in constellationToSpawn.Lines)
        {
            Vector2 pos1 = constellationToSpawn.Stars[line.x].Position * SCALE + (Vector2)transform.position;
            Vector2 pos2 = constellationToSpawn.Stars[line.y].Position * SCALE + (Vector2)transform.position;

            float dist = Vector2.Distance(pos1, pos2);

            int numLinePoints = Mathf.CeilToInt(dist / LINE_RESOLUTION);

            for (int i = 0; i < numLinePoints; i++)
            {
                linePoints.Add(Vector2.Lerp(pos1, pos2, i / (float)numLinePoints));
            }
        }
    }
}

public static class ConstellationDatabase
{
    public static Dictionary<string, ConstellationData> Constellations = new Dictionary<string, ConstellationData>();
}