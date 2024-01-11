using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementTrack : MonoBehaviour
{
    [Tooltip("Offset from start position")]
    [SerializeField] private List<Vector3> pointOffsets;
    private List<Vector3> truePoints;
    private Vector3 startPoint;
    private int target;
    [Range(0, 255)]
    [SerializeField] private float speed;

    [Tooltip("Does the object reverse trajectory when it reaches an endpoint?")]
    [SerializeField] private bool bounce;
    private bool ended;
    private Direction direction;

    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        startPoint = transform.position;

        if (pointOffsets.Count < 1)
            Debug.LogWarning("Your movement track is missing points!");

        CalculateTruePoints();

        target = 0;
        direction = Direction.Forward;

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        ended = false;
        StepTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (!ended)
            if ((truePoints[target] - transform.position).magnitude < speed * Time.deltaTime * 2)
                StepTarget();
    }

    /*
     * Calculates the world location of the points provided.
     */
    private void CalculateTruePoints()
    {
        truePoints = new List<Vector3>();
        truePoints.Add(startPoint);
        for (int i = 0; i < pointOffsets.Count; i++)
        {
            truePoints.Add(startPoint + pointOffsets[i]);
        }
    }

    private void StepTarget()
    {
        // Flip direction if needed
        if (target + (int)direction < 0 || target + (int)direction >= truePoints.Count)
        {
            if (bounce)
                direction = (Direction)(-(int)direction);
            else
            {
                rb.velocity = Vector3.zero;
                ended = true;
                return;
            }
        }
        target += (int)direction;

        Vector3 aim = (truePoints[target] - transform.position).normalized;
        rb.velocity = aim * speed;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player") && bounce)
        {
            direction = (Direction)(-(int)direction);
            StepTarget();
        }
    }

    public enum Direction
    {
        Forward = 1,
        Backward = -1
    }
}
