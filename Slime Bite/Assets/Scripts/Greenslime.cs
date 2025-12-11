using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Movement))]
public class Greenslime : MonoBehaviour
{
    public Movement movement { get; private set; }

    [SerializeField] float artAngleOffset = 180f; // tweak in Inspector if needed

    private Animator animator;
    private SpriteRenderer sprite;
    private Collider2D col;
    private Vector3 startingPosition;

    // Respawn tuning (tweak in Inspector)
    [SerializeField] private float respawnDelay = 1.2f;
    [SerializeField] private float overlapRadius = 0.35f;
    [SerializeField] private LayerMask respawnObstacleMask;

    private void Awake()
    {
        this.movement = GetComponent<Movement>();
        this.animator = GetComponent<Animator>();
        this.sprite = GetComponent<SpriteRenderer>();
        this.col = GetComponent<Collider2D>();
        this.startingPosition = transform.position;
    }

    private void Update()
    {
        // keys to set directions
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            this.movement.SetDirection(UnityEngine.Vector2.up);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            this.movement.SetDirection(UnityEngine.Vector2.down);
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            this.movement.SetDirection(UnityEngine.Vector2.left);
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            this.movement.SetDirection(UnityEngine.Vector2.right);
    }

    private void LateUpdate()
    {
        var dir = this.movement.direction;
        if (dir.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float angleDeg = angle + artAngleOffset;
            transform.rotation = Quaternion.AngleAxis(angleDeg, Vector3.forward);

            // makes sure sprite scale isn't accidentally flipped
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x),
                Mathf.Abs(transform.localScale.y),
                Mathf.Abs(transform.localScale.z));
        }
    }

    // Called by GameManager when slime is eaten
    public void OnEaten()
    {
        Debug.Log($"[Greenslime] OnEaten called for {name} at {Time.time:F2}");

        // disable visuals & physics immediately
        if (animator != null) animator.enabled = false;
        if (sprite != null) sprite.enabled = false;
        if (col != null) col.enabled = false;

        if (movement != null) movement.enabled = false;

        // deactivate object so it won't trigger collisions, GameManager will start respawn
        gameObject.SetActive(false);
    }

    // Coroutine to safely respawn this Greenslime
    public IEnumerator DoRespawn()
    {
        Debug.Log($"[Greenslime] DoRespawn started for {name} at {Time.time:F2}");

        // wait minimum delay
        yield return new WaitForSeconds(respawnDelay);

        // teleport to starting position (keeps it out of other places)
        transform.position = startingPosition;
        transform.rotation = Quaternion.identity;

        // Try to place only when spawn area is clear
        int safety = 0;
        bool placed = false;
        while (!placed && safety < 30)
        {
            Collider2D hit = Physics2D.OverlapCircle(startingPosition, overlapRadius, respawnObstacleMask);
            if (hit == null)
            {
                placed = true;
                break;
            }
            Debug.Log($"[Greenslime] spawn blocked by {hit.name}; waiting... {Time.time:F2}");
            yield return new WaitForSeconds(0.15f);
            safety++;
        }

        // finish reset: enable components and movement
        gameObject.SetActive(true);

        if (sprite != null) sprite.enabled = true;
        if (animator != null) animator.enabled = true;
        if (col != null) col.enabled = true;

        if (movement != null)
        {
            movement.ResetStartingPosition(startingPosition);
            movement.ResetState();
            movement.enabled = true;
        }

        Debug.Log($"[Greenslime] DoRespawn finished for {name} at {Time.time:F2}");
        yield break;
    }

    // Backwards-compatible ResetState that GameManager may call directly
    public void ResetState()
    {
        // teleport & restore immediately (no overlap checks)
        transform.position = startingPosition;
        transform.rotation = Quaternion.identity;

        gameObject.SetActive(true);

        if (sprite != null) sprite.enabled = true;
        if (animator != null) animator.enabled = true;
        if (col != null) col.enabled = true;

        if (movement != null)
        {
            movement.ResetStartingPosition(startingPosition);
            movement.ResetState();
            movement.enabled = true;
        }
    }

    // debug helper: draws spawn check radius
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, overlapRadius);
    }
}
