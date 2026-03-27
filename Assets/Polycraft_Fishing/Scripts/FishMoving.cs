using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(CubeEnemy))]
public class FishMoving : MonoBehaviour
{
    //private Transform target;
    public Transform target;
    public Vector2 fishSpeedRange = new Vector2(1, 2);
    float fishSpeed;
    public float rotateSpeed = 1;


    void Start()
    {
        fishSpeed = Random.Range(fishSpeedRange.x, fishSpeedRange.y);
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 dir = target.position - transform.position;
            transform.Translate(dir.normalized * fishSpeed * Time.deltaTime, Space.World);

            var targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }
}
