using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passage : MonoBehaviour
{
    // A Transform reference to the destination connection in the scene
    public Transform connection;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Teleport the other object to the connection's position
        Vector3 position = other.transform.position;
        position.x = this.connection.position.x;
        position.y = this.connection.position.y;
        other.transform.position = position;
    }
}
