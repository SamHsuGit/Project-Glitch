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
    public float liftSpeed = 1f;
    public bool liftIsUp = false;

    private Vector3 down;
    private Vector3 up;

    void Awake()
    {
        down = downPos.transform.position;
        up = upPos.transform.position;
    }

    void Update()
    {
        if (liftIsUp && Time.time > timeOfLastLiftTrigger + liftWaitPeriod)
            LiftDown();
    }

    public void LiftDown()
    {
        liftIsUp = false;
        audioSourcePlayer.clip = liftTravelSound;
        audioSourcePlayer.Play();
        lift.transform.position = Vector3.Lerp(up, down, liftSpeed);
        audioSourcePlayer.Stop();
        audioSourcePlayer.PlayOneShot(liftArriveSound);
    }

    private void OnTriggerEnter(Collider other)
    {
        LiftUp();
    }

    public void LiftUp()
    {
        timeOfLastLiftTrigger = Time.time;
        liftIsUp = true;
        audioSourcePlayer.clip = liftTravelSound;
        audioSourcePlayer.Play();
        lift.transform.position = Vector3.Lerp(down, up, liftSpeed);
        audioSourcePlayer.Stop();
        audioSourcePlayer.PlayOneShot(liftArriveSound);
    }
}
