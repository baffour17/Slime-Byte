using UnityEngine;
public class PowerPellet : Pellet
{
    public float duration = 5f;
    private void Awake() { points = 50; } // if using base points
    protected override void Eat()
    {
        var gm = FindObjectOfType<GameManager>();
        if (gm != null) gm.PowerPelletEaten(this);
    }
}
