using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject doorLeft;
    public GameObject doorRight;
    public GameObject closedLeftPos;
    public GameObject closedRightPos;
    public GameObject openLeftPos;
    public GameObject openRightPos;
    public AudioSource audioSourcePlayer;
    public AudioClip doorOpenSound;
    public AudioClip doorOpenArriveSound;
    public AudioClip doorTravelSound;
    public AudioClip doorCloseSound;
    public AudioClip doorCloseArriveSound;

    public float doorSpeed = 50f;
    public bool doorOpen = false;
    public bool doorTriggered = false;

    private bool playedOneShotOpen = false;
    private bool playedOneShotClosed = false;
    private Vector3 closedLeft;
    private Vector3 closedRight;
    private Vector3 openLeft;
    private Vector3 openRight;

    float step;

    private void Awake()
    {
        closedLeft = closedLeftPos.transform.position;
        closedRight = closedRightPos.transform.position;
        openLeft = openLeftPos.transform.position;
        openRight = openRightPos.transform.position;

    }

    private void OnTriggerEnter(Collider other)
    {
        doorTriggered = true;
    }

    private void OnTriggerStay(Collider other)
    {
        doorTriggered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        doorTriggered = false;
    }

    void Update()
    {

        // Move our position a step closer to the target.
        step = doorSpeed * Time.time; // calculate distance to move

        // open door if trigered by bool
        if (!doorOpen && doorTriggered)
            OpenDoor();
        else if (doorOpen && !doorTriggered) // close door if past a certain time and nothing in triggered area
            CloseDoor();

        // Detect if doors are open or closed and play appropriate sounds
        if(Vector3.Distance(doorLeft.transform.position, openLeft) < 0.001f && Vector3.Distance(doorRight.transform.position, openRight) < 0.001f) // CHECK IF OPEN
        {
            doorLeft.transform.position = openLeft;
            doorRight.transform.position = openRight;
            doorOpen = true;
            audioSourcePlayer.Stop();
            //playedOneShotClosed = false;
            //if (!playedOneShotOpen)
            //{
            //    playedOneShotOpen = true;
            //    audioSourcePlayer.PlayOneShot(doorOpenSound);
            //    audioSourcePlayer.PlayOneShot(doorOpenArriveSound);
            //}
        }
        else if (Vector3.Distance(doorLeft.transform.position, closedLeft) < 0.001f && Vector3.Distance(doorRight.transform.position, closedRight) < 0.001f) // CHECK IF CLOSED
        {
            doorLeft.transform.position = closedLeft;
            doorRight.transform.position = closedRight;
            doorOpen = false;
            audioSourcePlayer.Stop();
            //playedOneShotOpen = false;
            //if (!playedOneShotClosed)
            //{
            //    playedOneShotClosed = true;
            //    audioSourcePlayer.PlayOneShot(doorCloseSound);
            //    audioSourcePlayer.PlayOneShot(doorCloseArriveSound);
            //}
        }
    }

    public void OpenDoor()
    {
        audioSourcePlayer.clip = doorTravelSound;
        audioSourcePlayer.Play();
        doorLeft.transform.position = Vector3.MoveTowards(closedLeft, openLeft, step);
        doorRight.transform.position = Vector3.MoveTowards(closedRight, openRight, step);
    }

    public void CloseDoor()
    {
        audioSourcePlayer.clip = doorTravelSound;
        audioSourcePlayer.Play();
        doorLeft.transform.position = Vector3.MoveTowards(openLeft, closedLeft, step);
        doorRight.transform.position = Vector3.MoveTowards(openRight, closedRight, step);
    }
}
