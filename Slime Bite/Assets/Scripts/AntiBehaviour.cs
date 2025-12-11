using UnityEngine;
[RequireComponent(typeof(Antislime))]

public abstract class AntiBehaviour : MonoBehaviour
{
    public Antislime antislime {get; private set; }

    public float duration;

    private void Awake()
    {
        this.antislime = GetComponent<Antislime>();
        this.enabled = false;
    }

//If powerpellet(Limebell) is eaten scared class will be enabled
    public virtual void Enable(float duration)
    {
        this.enabled = true;

        CancelInvoke();
        Invoke(nameof(Disable), duration);
    }

    public virtual void Disable()
    {
        this.enabled = false;

        CancelInvoke();
    }

}
