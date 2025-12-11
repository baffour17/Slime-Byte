using UnityEngine;
using System.Collections.Generic;

public class AntiScatter : AntiBehaviour
{
    private void OnDisable()
    {
        // When scatter behavior turns off, allow chase mode to turn on
        if (antislime != null && antislime.chase != null)
        {
            this.antislime.chase.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Node node = other.GetComponent<Node>();

        if (node != null && this.enabled && antislime != null && !antislime.scared.enabled)
        {
            int index = Random.Range(0, node.availableDirections.Count);

            if (node.availableDirections[index] == -antislime.movement.direction &&
                node.availableDirections.Count > 1)
            {
                index++;

                if (index >= node.availableDirections.Count)
                {
                    index = 0;
                }
            }

            antislime.movement.SetDirection(node.availableDirections[index]);
        }
    }
}
