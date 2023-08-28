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
    public int ammoReserve;
    public int ammo;
    public int maxAmmo;
    public int maxAmmoReserve;
    public bool isPickup;
    public bool bounce = true;
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

    private float initialYPos;

    private MeshRenderer mr;
    private BoxCollider bc;

    private void Awake()
    {
        initialYPos = transform.position.y;
        mr = GetComponent<MeshRenderer>();
        bc = GetComponent<BoxCollider>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isPickup)
        {
            if(bounce)
                transform.position = new Vector3(transform.position.x, initialYPos + Mathf.Sin(Mathf.Deg2Rad* 360 * Time.time) * 0.25f, transform.position.z);
            transform.Rotate(new Vector3(0, Mathf.Deg2Rad * 100, 0));
        }
    }

    public void Respawn()
    {
        mr.enabled = false;
        bc.enabled = false;
        Invoke("Unhide", 20f);
    }

    private void Unhide()
    {
        mr.enabled = true;
        bc.enabled = true;
    }
}
