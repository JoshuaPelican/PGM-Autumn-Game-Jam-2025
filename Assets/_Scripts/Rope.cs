using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Rope : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float nodeDistance = 0.2f;
    [SerializeField] int totalNodes = 50;
    [SerializeField] float ropeWidth = 0.1f;
    [SerializeField] int iterationCount = 80;
    [SerializeField] float dampingFactor = 0.95f;
    [SerializeField] Vector2 gravity = new Vector2(0f, -5f);
    [SerializeField] float collidionDetectRadius = 0.25f;
    [SerializeField] ContactFilter2D contactFilter;


    List<RopeNode> ropeNodes = new List<RopeNode>();

    LineRenderer lineRenderer;
    Vector3[] linePositions;
    public Vector3[] LinePositions => linePositions;

    RaycastHit2D[] RaycastHitBuffer = new RaycastHit2D[10];
    Collider2D[] ColliderHitBuffer = new Collider2D[10];


    void Start()
    {
        lineRenderer = this.GetComponent<LineRenderer>();

        // VERY HACKY TO FINISH GAME
        if(SceneManager.Instance.ScenePayload != null)
        {
            ConstellationData constellation = (ConstellationData)SceneManager.Instance.ScenePayload.payload.Find(x => x.Key == "ConstellationToLoad").Value;
            if (constellation != null)
            {
                float constLength = 0;
                foreach (Vector2Int line in constellation.Lines)
                {
                    constLength += Vector2.Distance(constellation.Stars[line.x].Position * 1.5f, constellation.Stars[line.y].Position * 1.5f);
                }
                constLength += constellation.Stars.Sum(x => x.Magnitude * 0.25f * 2 * Mathf.PI);
                totalNodes = Mathf.CeilToInt(constLength / nodeDistance);
            }
        }

        // Generate some rope nodes based on properties
        Vector3 startPosition = transform.position;
        for (int i = 0; i < totalNodes; i++)
        {
            RopeNode node = new RopeNode(startPosition);
            ropeNodes.Add(node);

            //startPosition.y -= nodeDistance/7f;
        }

        // for line renderer data
        linePositions = new Vector3[totalNodes];
    }


    void Update()
    {
        DrawRope();
    }

    private void FixedUpdate()
    {
        Simulate();

        // Higher iteration results in stiffer ropes and stable simulation
        for (int i = 0; i < iterationCount; i++)
        {
            ApplyConstraint();

            // Playing around with adjusting collisions at intervals - still stable when iterations are skipped
            if (i % 2 == 1)
                AdjustCollisions();
        }
    }

    private void Simulate()
    {
        // step each node in rope
        for (int i = 0; i < totalNodes; i++)
        {
            // derive the velocity from previous frame
            Vector2 velocity = ropeNodes[i].CurrentPosition - ropeNodes[i].PreviousPosition;
            ropeNodes[i].PreviousPosition = ropeNodes[i].CurrentPosition;

            // calculate new position
            Vector2 newPos = ropeNodes[i].CurrentPosition + velocity * dampingFactor;
            newPos += gravity * Time.fixedDeltaTime;
            Vector2 direction = ropeNodes[i].CurrentPosition - newPos;

            // cast ray towards this position to check for a collision
            int result = -1;
            result = Physics2D.CircleCast(ropeNodes[i].CurrentPosition, collidionDetectRadius, -direction.normalized, contactFilter, RaycastHitBuffer, direction.magnitude);

            if (result > 0)
            {
                for (int n = 0; n < result; n++)
                {
                    if (RaycastHitBuffer[n].collider.gameObject.layer == 9)
                    {
                        Vector2 collidercenter = new Vector2(RaycastHitBuffer[n].collider.transform.position.x, RaycastHitBuffer[n].collider.transform.position.y);
                        Vector2 collisionDirection = RaycastHitBuffer[n].point - collidercenter;
                        // adjusts the position based on a circle collider
                        Vector2 hitPos = collidercenter + collisionDirection.normalized * (RaycastHitBuffer[n].collider.transform.localScale.x / 2f + collidionDetectRadius);
                        newPos = hitPos;
                        //break;              //Just assuming a single collision to simplify the model
                    }
                }
            }

            ropeNodes[i].CurrentPosition = newPos;
        }
    }

    private void AdjustCollisions()
    {
        // Loop rope nodes and check if currently colliding
        for (int i = 0; i < totalNodes - 1; i++)
        {
            RopeNode node = this.ropeNodes[i];

            int result = -1;
            result = Physics2D.OverlapCircle(node.CurrentPosition, collidionDetectRadius, contactFilter, ColliderHitBuffer);

            if (result > 0)
            {
                for (int n = 0; n < result; n++)
                {
                    if (ColliderHitBuffer[n].gameObject.layer != 8)
                    {
                        // Adjust the rope node position to be outside collision
                        Vector2 collidercenter = ColliderHitBuffer[n].transform.position;
                        Vector2 collisionDirection = node.CurrentPosition - collidercenter;

                        Vector2 hitPos = collidercenter + collisionDirection.normalized * ((ColliderHitBuffer[n].transform.localScale.x / 2f) + collidionDetectRadius);
                        node.CurrentPosition = hitPos;
                        break;
                    }
                }
            }
        }
    }

    private void ApplyConstraint()
    {
        ropeNodes[0].CurrentPosition = transform.position;

        for (int i = 0; i < totalNodes - 1; i++)
        {
            RopeNode node1 = this.ropeNodes[i];
            RopeNode node2 = this.ropeNodes[i + 1];

            // Get the current distance between rope nodes
            float currentDistance = (node1.CurrentPosition - node2.CurrentPosition).magnitude;
            float difference = Mathf.Abs(currentDistance - nodeDistance);
            Vector2 direction = Vector2.zero;

            // determine what direction we need to adjust our nodes
            if (currentDistance > nodeDistance)
            {
                direction = (node1.CurrentPosition - node2.CurrentPosition).normalized;
            }
            else if (currentDistance < nodeDistance)
            {
                direction = (node2.CurrentPosition - node1.CurrentPosition).normalized;
            }

            // calculate the movement vector
            Vector2 movement = direction * difference;

            // apply correction
            node1.CurrentPosition -= (movement * 0.5f);
            node2.CurrentPosition += (movement * 0.5f);
        }
    }

    private void DrawRope()
    {
        lineRenderer.startWidth = ropeWidth;
        lineRenderer.endWidth = ropeWidth;

        for (int n = 0; n < totalNodes; n++)
        {
            linePositions[n] = ropeNodes[n].CurrentPosition; //new Vector3(RopeNodes[n].CurrentPosition.x, RopeNodes[n].CurrentPosition.y, 0);
        }

        lineRenderer.positionCount = linePositions.Length;
        lineRenderer.SetPositions(linePositions);
    }

}

public class RopeNode
{
    public Vector2 CurrentPosition;
    public Vector2 PreviousPosition;

    public RopeNode(Vector2 currentPos)
    {
        CurrentPosition = PreviousPosition = currentPos;
    }
}