using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    public float speed = 8.0f;
    public float speedMultiplier = 1.0f;
    public UnityEngine.Vector2 initialDirection;
    public LayerMask obstacleLayer;

    public new Rigidbody2D rigidbody { get; private set; }
    public UnityEngine.Vector2 direction { get; private set; }
    public UnityEngine.Vector2 nextDirection { get; private set; }
    public UnityEngine.Vector3 startingPosition { get; private set; }

    private void Awake()
    {
        this.rigidbody = GetComponent<Rigidbody2D>();
        this.startingPosition = this.transform.position;
    }

    private void Start()
    {
        ResetState();
    }

    // Allow external callers (e.g., Greenslime) to update the stored spawn position
    public void ResetStartingPosition(Vector3 pos)
    {
        this.startingPosition = pos;
    }

    public void ResetState()
    {
        this.speedMultiplier = 1.0f;
        this.direction = this.initialDirection;
        this.nextDirection = UnityEngine.Vector2.zero;

        // Reset physics body to the stored starting position
        if (this.rigidbody != null)
        {
            this.rigidbody.position = new UnityEngine.Vector2(this.startingPosition.x, this.startingPosition.y);
            this.rigidbody.velocity = UnityEngine.Vector2.zero;
            this.rigidbody.rotation = 0f;
            // Depending on how you use physics, set isKinematic appropriately
            this.rigidbody.isKinematic = false;
        }

        // Also ensure transform is aligned
        this.transform.position = this.startingPosition;

        this.enabled = true;
    }

    private void Update()
    {
        if (this.nextDirection != UnityEngine.Vector2.zero)
        {
            SetDirection(this.nextDirection);
        }
    }

    private void FixedUpdate()
    {
        UnityEngine.Vector2 position = this.rigidbody.position;
        UnityEngine.Vector2 translation = this.direction * this.speed * this.speedMultiplier * Time.fixedDeltaTime;

        this.rigidbody.MovePosition(position + translation);
    }

    public void SetDirection(UnityEngine.Vector2 direction, bool forced = false)
    {
        if (forced || !Occupied(direction))
        {
            this.direction = direction;
            this.nextDirection = UnityEngine.Vector2.zero;
        }
        else
        {
            this.nextDirection = direction;
        }
    }

    // use rigidbody.position as origin 
    // and use smaller cast size/distance tuned to typical tile size.
    public bool Occupied(UnityEngine.Vector2 direction)
    {
        Vector2 origin = this.rigidbody != null ? this.rigidbody.position : (Vector2)transform.position;

        // Make the cast box slightly smaller than the visual sprite to avoid early hits.
        Vector2 castSize = UnityEngine.Vector2.one * 0.5f; // tweak between 0.3 - 0.8 aslong as its balanced ofcourse
        float castDist = 0.9f; // if your nodes are 1 unit apart, change to 0.9 just in case :)

        RaycastHit2D hit = Physics2D.BoxCast(
            origin,
            castSize,
            0.0f,
            direction.normalized,
            castDist,
            this.obstacleLayer
        );

        return hit.collider != null;
    }
}
