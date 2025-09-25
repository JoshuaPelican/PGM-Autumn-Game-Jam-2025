using UnityEngine;

public class SampledPoints : MonoBehaviour
{
    const float EDGE_PADDING = 0.75f;

    [SerializeField] float width;
    [SerializeField] float height;
    [SerializeField] int numPoints;
    [Space]
    [SerializeField] Transform debugPoint;


    private void Start()
    {
        Vector2[] samples = SamplePoints(width, height, numPoints);
        foreach (Vector2 sample in samples)
        {
            Transform point = Instantiate(debugPoint, sample, Quaternion.identity, transform);
            int starType = StarUtil.GetWeightedRandomStarType();
            float starBrightness = ((starType + 1) / 6f) + Random.Range(-0.24f, 0.24f);
            point.localScale *= Mathf.Lerp(2f, 0.5f, Random.value);

            SpriteRenderer rend = point.GetComponent<SpriteRenderer>();
            rend.color = StarUtil.StarColors[starType] * new Color(1, 1, 1, Mathf.Lerp(0.5f, 1f, starBrightness));
        }
    }

    public Vector2[] SamplePoints(float width, float height, int numPoints)
    {
        Vector2[] sampledPoints = new Vector2[numPoints];

        for (int i = 0; i < numPoints; i++)
        {
            sampledPoints[i] = Sample(sampledPoints, width, height, 10);
        }

        return sampledPoints;
    }

    Vector2 Sample(Vector2[] samples, float width, float height, int numCandidates)
    {
        Vector2 bestPoint = Vector2.zero;
        float bestDist = 0;

        for (int i = 0; i < numCandidates; i++)
        {
            Vector2 randomPoint = new Vector2(Random.Range(EDGE_PADDING, width - EDGE_PADDING), Random.Range(EDGE_PADDING, height - EDGE_PADDING)) - new Vector2(width / 2, height / 2);
            float dist = FindClosestSampleDist(samples, randomPoint);
            if(dist > bestDist)
            {
                bestDist = dist;
                bestPoint = randomPoint;
            }
        }

        return bestPoint;
    }

    float FindClosestSampleDist(Vector2[] samples, Vector2 from)
    {
        float bestDist = float.MaxValue;
        foreach (Vector2 point in samples)
        {
            float sqrDist = Vector2.SqrMagnitude(from - point);
            if (sqrDist < bestDist)
            {
                bestDist = sqrDist;
            }
        }

        return bestDist;
    }
}
