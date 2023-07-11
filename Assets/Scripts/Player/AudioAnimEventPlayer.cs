using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAnimEventPlayer : MonoBehaviour
{
    public AudioSource footStepLeft;
    public AudioSource footStepRight;
    public AudioSource landing;
    public AudioSource meleeHit;
    public AudioSource grenadeToss;
    public GameObject[] grenadeObs;
    public Controller controller;

    public void Update()
    {
        if (grenadeToss.isPlaying)
            grenadeObs[controller.currentWeaponSecondaryIndex].SetActive(true);
        else
        {
            foreach (GameObject ob in grenadeObs)
                ob.SetActive(false);
        }
    }

    public void PlayFootStepLeft()
    {
        footStepLeft.Play();
    }

    public void PlayFootStepRight()
    {
        footStepRight.Play();
    }

    public void PlayLanding()
    {
        landing.Play();
    }

    public void PlayMeleeHit()
    {
        meleeHit.Play();
    }

    public void PlayGrenadeToss()
    {
        grenadeToss.Play();
    }
}
