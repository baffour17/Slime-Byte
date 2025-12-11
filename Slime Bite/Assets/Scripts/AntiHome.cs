using UnityEngine;

public class AntiHome : MonoBehaviour
{
    public float duration = 7f;

    public void Enable(float overrideDuration = -1f)
    {
        float useDuration = overrideDuration > 0 ? overrideDuration : duration;
        this.enabled = true;
        Invoke(nameof(Disable), useDuration);
    }

    public void Disable()
    {
        this.enabled = false;
    }
}
