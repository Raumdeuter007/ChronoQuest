// MovingPlatformMover.cs
using UnityEngine;

public class MovingPlatformMover : MonoBehaviour
{
    public float speed = 2f;
    public float moveDistance = 3f;

    private Rigidbody2D rb;
    private Vector3 startPos;
    private Vector3 lastPos;

    [HideInInspector]
    public Vector2 velocity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
        lastPos = startPos;
    }

    void FixedUpdate()
    {
        float movement = Mathf.Sin(Time.time * speed) * moveDistance;
        Vector3 targetPos = startPos + new Vector3(movement, 0f, 0f);
        velocity = (targetPos - lastPos) / Time.fixedDeltaTime;
        rb.MovePosition(targetPos);
        lastPos = targetPos;
    }
}
