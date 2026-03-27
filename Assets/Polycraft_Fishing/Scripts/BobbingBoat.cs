/*
**	Bobbing Boat.cs
**
**	By Neil Carter <n.carter@nether.org.uk>
*/

using UnityEngine;
using System.Collections;

public class BobbingBoat : MonoBehaviour
{
    public float amplitude = 0.06f;
    public float offset = -0.15f;
    void Update()
    {
        // This is just here to make the boat do something.  Don't actually do
        // a bobbing boat like this, it's a terrible way of doing it.

        Vector3 position = transform.position;
        //position.y = (Mathf.Cos(Time.time) * 0.06f) - 0.15f;
        position.y = (Mathf.Cos(Time.time) * amplitude) + offset;
        transform.position = position;

        Vector3 angles = transform.eulerAngles;
        angles.x = Mathf.Sin(Time.time) * 5.0f;
        angles.z = Mathf.Sin(Time.time * 0.8f) * 5.0f;
        transform.eulerAngles = angles;
    }
}

