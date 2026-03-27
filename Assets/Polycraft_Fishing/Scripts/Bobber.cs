using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bobber : MonoBehaviour
{
    public Animator anim;
    public GameObject fish;
    public Camera cam;
    public LineRenderer rodString;
    public ParticleSystem splashParticle;
    public ParticleSystem expParticle;
    //timer
    static bool timer;
    public float tTime;
    static float tVal = 0;
    //right gauge
    static bool nibble;
    public float nGaugeTime;
    static float nGaugeVal = 0;
    //balance gauge
    static bool hook;
    public float hSpeed = 1.0f;
    static float hVal = 0;
    static int hDir = 1;
    //balance count
    public float clickTime;
    static float cVal = 0;

    static bool win;
    public float fishLandingSpeed = 1.0f;

    public Text countTex;
    public RectTransform btnGroup;
    public RectTransform gaugeGroup;

    void Start()
    {
        //SetBtn(1); //0-Empty, 1-Cast, 2-Hook, 3-Balance, 4-Get Fish
        cVal = clickTime;
    }
    void Update()
    {
        //click event
        DetectObjectWithRaycast();

        if (timer)
        {
            TimerCount();
        }

        //rod string manage
        if (fish.activeSelf)
        {
            Vector3 mid = Vector3.Lerp(fish.transform.position, rodString.GetPosition(2), 0.35f);

            rodString.SetPosition(0, fish.transform.GetChild(0).transform.position);
            rodString.SetPosition(1, mid);
            //bobber position
            transform.position = mid + new Vector3(0, -0.2f, 0); //offset
        }
        else
        {
            rodString.SetPosition(0, transform.position + new Vector3(0, -0.5f, 0));
            rodString.SetPosition(1, transform.position);
        }

        //fish moveing manage
        if (win)
        {
            Vector3 a = new Vector3(0, 1, 0);
            float dist = Vector3.Distance(a, fish.transform.position);

            if (dist > 0.1f)
            {
                fish.transform.position = Vector3.Lerp(fish.transform.position, a, fishLandingSpeed);
            }
        }
        
        if (nibble)
        {
            //gauge active
            if (nGaugeVal <= nGaugeTime)
            {
                nGaugeVal += Time.deltaTime;
                gaugeGroup.transform.GetChild(0).transform.GetChild(0).transform.localScale = new Vector3(nGaugeVal / nGaugeTime, 1, 1);
            }
            else
            {
                //lose fish - right gauge over
                anim.SetTrigger("Gone");
                GaugeReset(0, 0);
                BobberReset();

                nibble = false;
            }
        }

        if (hook)
        {
            //timer start
            if (cVal > 0)
            {
                float limit = hVal * hSpeed;
                if (0 <= hVal && limit <= 1)
                {
                    cVal -= Time.deltaTime;
                    hVal += Time.deltaTime * hDir;

                    gaugeGroup.transform.GetChild(1).transform.GetChild(0).localScale = new Vector3(hVal * hSpeed, 1, 1);
                    gaugeGroup.transform.GetChild(1).transform.GetChild(1).localScale = new Vector3(cVal / clickTime, 1, 1);

                    //fish moveing
                    Vector3 a = cam.transform.position;
                    a.y = 0;

                    fish.transform.position = Vector3.Lerp(fish.transform.position, a, 0.01f);

                    //ripple moving
                    ParticleSystem.EmissionModule rippleEmission = splashParticle.emission;
                    rippleEmission.rateOverTime = 5.0f;
                    splashParticle.transform.position = fish.transform.position;
                }
                else
                {
                    //lose fish
                    anim.SetTrigger("Gone");
                    GaugeReset(1, 0);
                    BobberReset();

                    //timer reset & end hook
                    cVal = clickTime;
                    hook = false;
                }

            }
            else
            {
                //you Win!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                win = true;
                hook = false;
                GaugeReset(1, 0);
                SetBtn(4); //0-Empty, 1-Cast, 2-Hook, 3-Balance, 4-Get Fish

                fish.transform.rotation = Quaternion.Euler(-90, 0, 0);
                splashParticle.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                splashParticle.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
                //timer reset & end hook
                cVal = clickTime;
                hook = false;
            }
        }
    }
    public void DetectObjectWithRaycast()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (win && hit.collider.name == "Fish_Bass00")
                {
                    expParticle.transform.position = hit.transform.position;
                    expParticle.Play();
                    anim.SetTrigger("Gone");
                    BobberReset();
                    win = false;
                }
            }
        }
    }
    public void BtnFunction()
    {
        if (nibble)
        {
            //decision
            nibble = false;

            float point = nGaugeVal;
            int score = (int)(point / nGaugeTime * 100);
            if (80 <= score && score <= 90)
            {
                //success
                anim.SetTrigger("Catch");
                if (!fish.activeSelf) fish.SetActive(true);
                SetBtn(3); //0-Empty, 1-Cast, 2-Hook, 3-Balance, 4-Get Fish
                hook = true;
            }
            else
            {
                //fail
                //print("fail, score = " + score);
                anim.SetTrigger("Gone");
                BobberReset();
                SetBtn(1); //0-Empty, 1-Cast, 2-Hook, 3-Balance, 4-Get Fish
            }

            GaugeReset(0, 0); ; //gauge reset
        }
        else if (hook)
        {
            float point = hVal;
            int score = (int)(point / clickTime * 100);
            if (0 <= score && score <= 100)
            {
                //success
                //print("success, score = " + score + "Count" + cCount);
                hDir = -hDir;
            }
            else
            {
                //Lose fish
                print("----------------------------------------------game over, score = " + score);
                anim.SetTrigger("Gone");
                BobberReset();
                GaugeReset(1,0);
                hook = false;
            }

            GaugeReset(0, 0);
        }
        else
        {
            //cast start
            Rigidbody bobberRig = transform.GetComponent<Rigidbody>();
            if (bobberRig.isKinematic)
            {
                bobberRig.useGravity = true;
                bobberRig.isKinematic = false;
            }
            SetBtn(0); //0-Empty, 1-Cast, 2-Hook, 3-Balance, 4-Get Fish
        }
    }
    public void GaugeReset(int gaugeCount, float resetValue)
    {
        gaugeGroup.transform.GetChild(gaugeCount).GetChild(0).transform.localScale = new Vector3(resetValue, 1, 1);
        gaugeGroup.transform.GetChild(gaugeCount).gameObject.SetActive(false);
        if (gaugeCount == 0)
        {
            nGaugeVal = 0;
        }
        else if (gaugeCount == 1)
        {
            hVal = 0;
            hDir = 1;
            cVal = 0;
        }
    }
    public void BobberReset()
    {
        transform.position = new Vector3(0, 2, 0); //bobber position reset
        SetBtn(1); //0-Empty, 1-Cast, 2-Hook, 3-Balance, 4-Get Fish
        if (fish.activeSelf)
        {
            fish.SetActive(false);
            fish.transform.SetPositionAndRotation(new Vector3(2, -0.5f, 2), Quaternion.Euler(0, 45, 0));
        }
        if (splashParticle.isPlaying)
        {
            splashParticle.Stop();
        }
    }
    public void TimerCount()
    {
        if (tVal <= tTime)
        {
            tVal += Time.deltaTime;
            float inverseCount = tTime - tVal;

            countTex.text = inverseCount.ToString("F1");
        }
        else
        {
            //Guage on and nibble state start
            SetBtn(2); //0-Empty, 1-Cast, 2-Hook, 3-Balance, 4-Get Fish
            nibble = true;
            anim.SetTrigger("Nibble");

            tVal = 0;
            timer = false;
        }
    }
    public void SetBtn(int btnCount)
    {
        //hide other
        for (int i = 0; i < 5; i++)
        {
            if (btnGroup.transform.GetChild(i).gameObject.activeSelf)
            {
                btnGroup.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < 2; i++)
        {
            if (gaugeGroup.transform.GetChild(i).gameObject.activeSelf)
            {
                gaugeGroup.transform.GetChild(i).gameObject.SetActive(false);
                if (i == 0)
                {
                    GaugeReset(i, 0.0f);
                }
                else
                {
                    GaugeReset(i, 0.5f);
                }
            }
        }

        if (btnCount == 0)
        {
            //Empty
        }
        else if (btnCount == 1)
        {
            //Casting
            if (!btnGroup.transform.GetChild(btnCount).gameObject.activeSelf)
            {
                //on cast btn
                btnGroup.transform.GetChild(btnCount).gameObject.SetActive(true);
            }
        }
        else if (btnCount == 2)
        {
            //Snatch & Right Zone gauge
            if (!btnGroup.transform.GetChild(btnCount).gameObject.activeSelf)
            {
                btnGroup.transform.GetChild(btnCount).gameObject.SetActive(true);
                gaugeGroup.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
        else if (btnCount == 3)
        {
            //Tug of war & Balance gauge
            if (!btnGroup.transform.GetChild(btnCount).gameObject.activeSelf)
            {
                btnGroup.transform.GetChild(btnCount).gameObject.SetActive(true);
                gaugeGroup.transform.GetChild(1).gameObject.SetActive(true);
            }
        }
        else if (btnCount == 4)
        {
            //Get Fish
            if (!btnGroup.transform.GetChild(btnCount).gameObject.activeSelf)
            {
                btnGroup.transform.GetChild(btnCount).gameObject.SetActive(true);
            }
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "WaterCube")
        {
            anim.SetTrigger("Casting");
            Rigidbody bobberRig = transform.GetComponent<Rigidbody>();
            if (!bobberRig.isKinematic)
            {
                bobberRig.useGravity = false;
                bobberRig.isKinematic = true;
            }
            //Timer Count
            if (!timer) timer = true; //TimerCount();

            //Start Ripple
            if (splashParticle != null && !splashParticle.isPlaying)
            {
                splashParticle.transform.position = transform.position;

                //ParticleSystem.MainModule ripple = splashParticle.main;
                //if (!ripple.loop) ripple.loop = true;
                ParticleSystem.EmissionModule rippleEmission = splashParticle.emission;
                rippleEmission.rateOverTime = 2.0f;

                splashParticle.Play();
            }
            else
            {
                print("PLZ attech splashParticle or particle was played");
            }
        }
        else if (other.name == "Fish_Bass00")
        {
            splashParticle.Stop();
        }
        
    }
}
