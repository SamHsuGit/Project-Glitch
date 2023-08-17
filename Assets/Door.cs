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
    public AudioClip doorCloseSound;

    public float doorWaitPeriod;
    public float timeOfLastDoorTrigger;
    public float doorSpeed;
    public bool doorOpen = false;

    private Vector3 closedLeft;
    private Vector3 closedRight;
    private Vector3 openLeft;
    private Vector3 openRight;

    private void Awake()
    {
        closedLeft = closedLeftPos.transform.position;
        closedRight = closedRightPos.transform.position;
        openLeft = openLeftPos.transform.position;
        openRight = openRightPos.transform.position;
    }

    void Update()
    {
        if (doorOpen && Time.time > timeOfLastDoorTrigger + doorWaitPeriod)
            CloseDoor();
    }

    public void CloseDoor()
    {
        doorOpen = false;
        doorLeft.transform.position = Vector3.Lerp(openLeft, closedLeft, doorSpeed);
        doorRight.transform.position = Vector3.Lerp(openRight, closedRight, doorSpeed);
        audioSourcePlayer.PlayOneShot(doorCloseSound);
    }

    private void OnTriggerEnter(Collider other)
    {
        OpenDoor();
    }

    public void OpenDoor()
    {
        timeOfLastDoorTrigger = Time.time;
        doorOpen = true;
        doorLeft.transform.position = Vector3.Lerp(closedLeft, openLeft, doorSpeed);
        doorRight.transform.position = Vector3.Lerp(closedRight, openRight, doorSpeed);
        audioSourcePlayer.PlayOneShot(doorOpenSound);
    }
}
