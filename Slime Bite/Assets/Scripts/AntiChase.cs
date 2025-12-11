using UnityEngine;

[RequireComponent(typeof(Movement))]
public class AntiChase : AntiBehaviour
{
    public Transform target; // Assign the Greenslime in the Inspector

    private Movement movement;

    private void Awake()
    {
        movement = GetComponent<Movement>();
    }

    private void Update()
{
    if (target == null) return;

    // Determine raw direction toward target
    Vector2 desiredDirection = (target.position - transform.position).normalized;

    // Snap direction to nearest 4-way movement
    if (Mathf.Abs(desiredDirection.x) > Mathf.Abs(desiredDirection.y))
    {
        desiredDirection = new Vector2(Mathf.Sign(desiredDirection.x), 0f);
    }
    else
    {
        desiredDirection = new Vector2(0f, Mathf.Sign(desiredDirection.y));
    }

    // Force movement even if Unity thinks something is blocking it
    movement.SetDirection(desiredDirection, true);
}
}

