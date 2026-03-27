using UnityEngine;
using System.Collections;

public class FishingZone : MonoBehaviour
{
    bool playerCanFish = false;

    public Color gizmoColor = new Color(1, 0, 0, 0.5f);

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("You have entered a fishing zone.");
            playerCanFish = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("You have left a fishing zone.");
            playerCanFish = false;
        }
    }

    public bool PlayerCanFish()
    {
        return playerCanFish;
    }

    void OnDrawGizmosSelected()
    {
        // Draw a semitransparent blue cube at the transforms position
        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}
