using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lift : MonoBehaviour
{
    public GameObject lift;
    public GameObject downPos;
    public GameObject upPos;
    public AudioSource audioSourcePlayer;
    public AudioClip liftArriveSound;
    public AudioClip liftTravelSound;
    public float liftWaitPeriod = 1f;
    public float timeOfLastLiftTrigger;
    public float liftSpeed = 50f;
    public bool liftIsUp = false;
    public bool liftTriggered = false;

    private Vector3 down;
    private Vector3 up;

    float step;

    void Awake()
    {
        down = downPos.transform.position;
        up = upPos.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Ground")
            liftTriggered = true;
    }

    private void OnTriggerStay(Collider other)
    {
        liftTriggered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        liftTriggered = false;
    }

    void Update()
    {
        //if (liftIsUp && Time.time > timeOfLastLiftTrigger + liftWaitPeriod)
        //    LiftDown();

        // Move our position a step closer to the target.
        step = liftSpeed * Time.time; // calculate distance to move

        // open door if trigered by bool
        if (!liftIsUp && liftTriggered)
            LiftUp();
        else if (liftIsUp && !liftTriggered) // close door if past a certain time and nothing in triggered area
            LiftDown();

        // Detect if doors are open or closed and play appropriate sounds
        if (Vector3.Distance(lift.transform.position, up) < 0.001f) // CHECK IF UP
        {
            lift.transform.position = up;
            liftIsUp = true;
            //audioSourcePlayer.Stop();
            //playedOneShotDown = false;
            //if (!playedOneShotUp)
            //{
            //    playedOneShotUp = true;
            //    audioSourcePlayer.PlayOneShot(liftArriveSound);
            //}
        }
        else if (Vector3.Distance(lift.transform.position, down) < 0.001f) // CHECK IF DOWN
        {
            lift.transform.position = down;
            liftIsUp = false;
            //audioSourcePlayer.Stop();
            //playedOneShotUp = false;
            //if (!playedOneShotDown)
            //{
            //    playedOneShotDown = true;
            //    audioSourcePlayer.PlayOneShot(liftArriveSound);
            //}
        }
    }

    public void LiftUp()
    {
        timeOfLastLiftTrigger = Time.time;
        liftIsUp = true;
        //audioSourcePlayer.clip = liftTravelSound;
        //audioSourcePlayer.Play();
        //lift.transform.position = Vector3.Lerp(down, up, liftSpeed);
        lift.transform.position = Vector3.MoveTowards(down, up, step);
        //audioSourcePlayer.Stop();
        //audioSourcePlayer.PlayOneShot(liftArriveSound);
    }

    public void LiftDown()
    {
        liftIsUp = false;
        //audioSourcePlayer.clip = liftTravelSound;
        //audioSourcePlayer.Play();
        //lift.transform.position = Vector3.Lerp(up, down, liftSpeed);
        lift.transform.position = Vector3.MoveTowards(up, down, step);
        //audioSourcePlayer.Stop();
        //audioSourcePlayer.PlayOneShot(liftArriveSound);
    }
}
