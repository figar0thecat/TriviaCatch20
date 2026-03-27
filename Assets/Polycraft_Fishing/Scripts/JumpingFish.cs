using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingFish : MonoBehaviour
{
    public bool delay;
    public Vector2 randomMinMax = new Vector2(0, 1);
    public float randomTime;

    
    public Vector2 xMinMax = new Vector2(0f, 1f);
    public Vector2 zMinMax = new Vector2(0f, 1f);
    public Vector3 randomPos;

    // Start is called before the first frame update
    void Start()
    {
        randomTime = Random.Range(1.5f, (int)randomMinMax.y);

        randomPos = transform.position;
        randomPos.x = Random.Range(xMinMax.x, xMinMax.y);
        randomPos.z = Random.Range(zMinMax.x, zMinMax.y);

        transform.position = randomPos;

        delay = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (delay)
        {
            if (randomTime >= 0)
            {
                randomTime -= Time.deltaTime;
            }
            else
            {
                randomTime = Random.Range((int)randomMinMax.x, (int)randomMinMax.y);
                if (randomTime < 1)
                {
                    randomTime = Random.Range((int)randomMinMax.x, (int)randomMinMax.y);
                }

                randomPos.x = Random.Range(xMinMax.x, xMinMax.y);
                randomPos.z = Random.Range(zMinMax.x, zMinMax.y);
                transform.position = randomPos;

                Vector3 euler = transform.eulerAngles;
                euler.y = Random.Range(0f, 360f);
                transform.eulerAngles = euler;

                transform.GetComponent<Animator>().SetBool("Jumping", true);
                delay = false;
            }
        }
    }
    public void EndAnimation()
    {
        transform.GetComponent<Animator>().SetBool("Jumping", false);
        delay = true;
    }
}
