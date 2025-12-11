using UnityEngine;

/// <summary>
/// Antislime controller — robust: uses SendMessage to call Enable/Disable on behaviour components
/// and passes the actual Greenslime instance to GameManager when collisions happen.
/// </summary>
[RequireComponent(typeof(Movement))]
public class Antislime : MonoBehaviour
{
    public Movement movement { get; private set; }

    // these can stay strongly typed so you can assign specific behaviour components in Inspector
    public AntiHome home { get; private set; }
    public AntiScatter scatter { get; private set; }
    public AntiChase chase { get; private set; }
    public AntiScared scared { get; private set; }
    public AntiBehaviour initialBehaviour;

    public Transform target;
    public int points = 200;

    // duration to pass when enabling behaviours via SendMessage
    [SerializeField] private float behaviourEnableDuration = 5f;

    private void Awake()
    {
        // correct type usage (capital M)
        this.movement = GetComponent<Movement>();

        // get behaviour components if present (these may be null)
        this.home = GetComponent<AntiHome>();
        this.scatter = GetComponent<AntiScatter>();
        this.chase = GetComponent<AntiChase>();
        this.scared = GetComponent<AntiScared>();
    }

    public void ResetState()
    {
        this.gameObject.SetActive(true);

        if (this.movement != null) this.movement.ResetState();

        // Use SendMessage to safely call Disable/Enable on components without compile-time dependency.
        // If a component doesn't implement the method or expects a different signature, nothing breaks.
        if (this.scared != null)
            this.scared.gameObject.SendMessage("Disable", SendMessageOptions.DontRequireReceiver);

        if (this.chase != null)
            this.chase.gameObject.SendMessage("Disable", SendMessageOptions.DontRequireReceiver);

        if (this.scatter != null)
            this.scatter.gameObject.SendMessage("Enable", behaviourEnableDuration, SendMessageOptions.DontRequireReceiver);

        if (this.home != null && this.initialBehaviour != null && this.home != this.initialBehaviour)
            this.home.gameObject.SendMessage("Disable", SendMessageOptions.DontRequireReceiver);

        if (this.initialBehaviour != null)
            this.initialBehaviour.gameObject.SendMessage("Enable", behaviourEnableDuration, SendMessageOptions.DontRequireReceiver);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Make sure the layer name matches exactly (no trailing space)
        if (collision.gameObject.layer == LayerMask.NameToLayer("Greenslime"))
        {
            var gm = FindObjectOfType<GameManager>();

            // If this antislime is frightened (scared behaviour enabled) it can be eaten
            if (this.scared != null && this.scared.enabled)
            {
                if (gm != null) gm.AntiSlimeEaten(this);
                return;
            }

            // Otherwise the player (Greenslime) got hit — pass the Greenslime instance to GameManager
            Greenslime greenslime = collision.gameObject.GetComponent<Greenslime>();
            if (greenslime != null)
            {
                if (gm != null) gm.GreenslimeEaten(greenslime);
            }
            else
            {
                Debug.LogWarning($"Antislime collided with object on Greenslime layer but no Greenslime component found on {collision.gameObject.name}");
            }
        }
    }
}
