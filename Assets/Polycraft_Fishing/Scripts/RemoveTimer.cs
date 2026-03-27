using UnityEngine;
using System.Collections;

public class RemoveTimer : MonoBehaviour
{
    //public GameObject target;
    public float timer = 3;
    float time;

    void Start()
    {
        time = timer;
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            timer = time;
            //do Something.
            Destroy(gameObject);
        }
        
    }
}
