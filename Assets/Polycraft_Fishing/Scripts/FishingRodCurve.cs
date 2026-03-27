using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRodCurve : MonoBehaviour {
    public bool debugRay;
    public float animPower = 1f;
    public GameObject fishingPole;
    public Transform phyTip;
    public Transform debugTip;

    Animator anim;
    public float poleVertical;
    public float poleHorizontal;
    
    void Start () {
        if (!phyTip)
        {
            print("You will need the tip of the fishing rod.");
        }
        anim = fishingPole.transform.GetComponent<Animator>();
    }
	
	
	void Update () {
        if (!phyTip)
        {
            return;
        }


        Vector3 dir = phyTip.position - debugTip.position;
        if (debugRay)
        {
            Debug.DrawRay(debugTip.position, dir, Color.red);
            Debug.DrawLine(transform.position, debugTip.position, Color.blue);
        }
        dir = transform.InverseTransformDirection(dir) * animPower;

        Vector2 curveBlend;
        curveBlend = new Vector2(phyTip.localPosition.z, phyTip.localPosition.x);
        poleVertical = curveBlend.x;
        poleVertical = curveBlend.y;

        anim.SetFloat("PoleVertical", -dir.z);
        anim.SetFloat("PoleHorizontal", -dir.x);
    }
}
