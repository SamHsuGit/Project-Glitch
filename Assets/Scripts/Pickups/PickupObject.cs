using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObject : MonoBehaviour
{
    public int type;
    public int index;
    public int level;
    public int ammoPickupQty;
    public int range;
    public int splashDamage;
    public int fireRate;
    public int projectileVelocity;
    public int roundsPerFire;
    public int clips;
    public int ammo;
    public int clipSize;
    public int maxAmmo;
    public bool isPickup;
    public AudioSource audioSourcePlayer;
    public AudioClip pickupSound1;
    public AudioClip ammoPickupSound;
    public AudioClip weaponFireSound;
    public AudioClip weaponReloadSound;
    public AudioClip weaponChargeSound;
    public AudioClip clipEjectSound;
    public AudioClip clipInsertSound;
    public AudioClip ripperSawLoop;
    public AudioClip ripperSawWindDown;
    public AudioClip lockOn;
    public AudioClip controlTetherControlOn;
    public AudioClip controlTetherLostControl;
    public AudioClip toasterFireLoop;
    public AudioClip toasterFireLoopStop;
    public AudioClip wrenchDeconstruct;
    public AudioClip wrenchAssemble;
    public AudioClip grenadeExplode;
    public AudioClip hitSound;
    public GameObject projectile;

    private void Awake()
    {
        
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isPickup)
        {
            transform.position = new Vector3(transform.position.x, Mathf.Sin(Mathf.Deg2Rad* 0.5f * Time.deltaTime), transform.position.z);
            transform.Rotate(new Vector3(0, 360, 0) * 25f);
        }
    }
}
