using UnityEngine;

public class RotateArrow : MonoBehaviour
{
    [Header("Rotation")]
    public float rotationSpeed = 50f; // degrees per second

    [Header("Bob Movement")]
    public float bobHeight = 0.5f;     // how high it moves up/down
    public float bobSpeed = 2f;        // how fast it bobs

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Rotate around Y axis
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);

        // Bob up and down
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}
