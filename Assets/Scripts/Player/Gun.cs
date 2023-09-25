using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Gun : NetworkBehaviour
{
    public float hitScanDist = 100f;
    public float fireRatePrimary = 2f;
    public float fireRateSecondary = 2f;
    public float impactForce = 0.01f;
    private int damage = 1;
    public float nextTimeToFirePrimary = 0f;
    public float nextTimeToFireSecondary = 0f;
    public float sphereCastRadius = 0.1f;

    public Camera fpsCam;
    public AudioSource audioSourcePlayer;
    public GameObject reticleChangeColor;
    public GameObject backgroundMask;
    public Health target;
    public PickupObject currentWeaponPrimary;
    public PickupObject currentWeaponSecondary;

    InputHandler inputHandler;
    Controller controller;
    CanvasGroup backgroundMaskCanvasGroup;
    public RaycastHit hit;

    private Vector3 sphereCastStart;
    private Image image;
    private int currentPrimaryWeaponIndex;
    private int currentSecondaryWeaponIndex;
    private int maxWeaponInt;

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
        image = reticleChangeColor.GetComponent<Image>();
        controller = GetComponent<Controller>();
        backgroundMaskCanvasGroup = backgroundMask.GetComponent<CanvasGroup>();
        maxWeaponInt = controller.wPrimaryModels.Length;
    }

    //Disabled since we are using the shoot button to place bricks
private void FixedUpdate()
{
    if (Settings.OnlinePlay && !isLocalPlayer) return;

    target = null; // reset target
    target = FindTarget(); // get target gameObject
    sphereCastStart = controller.playerCameraGameObject.transform.parent.transform.position;

    currentPrimaryWeaponIndex = controller.currentWeaponPrimaryIndex;
    currentSecondaryWeaponIndex = controller.currentWeaponSecondaryIndex;
    currentWeaponPrimary = controller.wPrimaryPickupObjects[currentPrimaryWeaponIndex];
    currentWeaponSecondary = controller.wSecondaryPickupObjects[currentSecondaryWeaponIndex];
    fireRatePrimary = currentWeaponPrimary.fireRate * 0.375f;
    fireRateSecondary = currentWeaponSecondary.fireRate * 0.5f;

    if (Time.time >= nextTimeToFirePrimary && backgroundMaskCanvasGroup.alpha == 0 && (!currentWeaponPrimary.hasAmmoCount || currentWeaponPrimary.ammo > 0) && !controller.isReloading)
    {
        if (inputHandler.shoot)
        {
            if (currentPrimaryWeaponIndex > maxWeaponInt - 2)
            {
                //// causes error state
                //nextTimeToFirePrimary = Time.time + 1f / fireRatePrimary;
                //if (currentWeaponPrimary.weaponFireSound != null)
                //    audioSourcePlayer.PlayOneShot(currentWeaponPrimary.weaponFireSound);
                //HitRegCheck();

                controller.melee = true;
            }
            else
            {
                nextTimeToFirePrimary = Time.time + 1f / fireRatePrimary;

                if (currentWeaponPrimary.weaponFireSound != null)
                    audioSourcePlayer.PlayOneShot(currentWeaponPrimary.weaponFireSound);

                    if (currentWeaponPrimary.ammo - currentWeaponPrimary.roundsPerFire >= 0)
                currentWeaponPrimary.ammo -= currentWeaponPrimary.roundsPerFire;

                HitRegCheck();
                controller.PressedShoot(); // make projectile
            }
        }
    }
}

// if raycast hits a destructible object (with health but not this player), turn reticle red
public Health FindTarget() // use hitscan to detect if something is targeted by reticle
    {
        image.color = Color.HSVToRGB(0, 0, 50, true);

        //if hit something
        if (Physics.SphereCast(sphereCastStart, sphereCastRadius, fpsCam.transform.forward, out hit, hitScanDist))
        {
            if (hit.transform.GetComponent<Health>() != null)
                target = hit.transform.GetComponent<Health>();
            
            if (target != null && target.gameObject != gameObject && target.hp != 0) // if hits a model that is not this model
            {
                image.color = Color.HSVToRGB(0, 100, 50, true); // turn reticle red
                return target;
            }
            else
            {
                image.color = Color.HSVToRGB(0, 0, 50, true);
                return null;
            }
        }
        else
            return null;
    }

    // Server calculated shoot logic gives players the authority to change hp of other preregistered gameObjects
    public void HitRegCheck() // if hitscan weapon is used and hits something
    {
        //if (hit.transform != null) // hit anything (ground, walls)
        //{
        //    //audioSourcePlayer.clip = currentWeaponPrimary.hitSound;
        //    audioSourcePlayer.PlayOneShot(currentWeaponPrimary.hitSound);

        //    // spawn damage sparks effects (use particle system)
        //    Vector3 pos = hit.transform.position;
        //    if (Settings.OnlinePlay)
        //    {
        //        //controller.CmdSpawnObject(3, 3, new Vector3(pos.x + -0.25f, pos.y + 0, pos.z + 0.25f));
        //        //controller.CmdSpawnObject(3, 3, new Vector3(pos.x + -0.25f, pos.y + 0, pos.z - 0.25f));
        //        //controller.CmdSpawnObject(3, 3, new Vector3(pos.x + 0.25f, pos.y + 0, pos.z + 0.25f));
        //        //controller.CmdSpawnObject(3, 3, new Vector3(pos.x + 0.25f, pos.y + 0, pos.z - 0.25f));
        //    }
        //    else
        //    {
        //        //controller.SpawnObject(3, 3, new Vector3(pos.x + -0.25f, pos.y + 0, pos.z + 0.25f));
        //        //controller.SpawnObject(3, 3, new Vector3(pos.x + -0.25f, pos.y + 0, pos.z - 0.25f));
        //        //controller.SpawnObject(3, 3, new Vector3(pos.x + 0.25f, pos.y + 0, pos.z + 0.25f));
        //        //controller.SpawnObject(3, 3, new Vector3(pos.x + 0.25f, pos.y + 0, pos.z - 0.25f));
        //    }
        //}
        if (target != null) // if target was found (i.e. player)
        {
            //audioSourcePlayer.clip = currentWeaponPrimary.hitSound;
            audioSourcePlayer.PlayOneShot(currentWeaponPrimary.hitSound);

            if (Settings.OnlinePlay)
                CmdDamage(target);
            else
                Damage(target);
        }
    }

    [ServerCallback] // prevents clients from running this
    public void CmdDamage(Health target)
    {
        // player identity validation logic here
        target.hp -= damage; // update the hp value on the server (hp is a syncVar which will then propogate to all clients)
    }

    //[ClientRpc]
    //public void RpcDamage(Health target)
    //{
    //    Damage(target);
    //}

    public void Damage(Health target)
    {
        target.hp -= damage; // damage self or client
    }
}
