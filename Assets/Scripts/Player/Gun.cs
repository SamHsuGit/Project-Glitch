using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Gun : NetworkBehaviour
{
    public float hitScanDist = 100f;
    public float fireRate = 2f;
    public float impactForce = 0.01f;
    private int damage = 1;
    public float nextTimeToFire = 0f;
    public float sphereCastRadius = 0.1f;

    public Camera fpsCam;
    public AudioSource weaponSounds;
    public AudioSource hitSound;
    public GameObject reticleChangeColor;
    public GameObject backgroundMask;
    public Health target;

    InputHandler inputHandler;
    Controller controller;
    CanvasGroup backgroundMaskCanvasGroup;
    public RaycastHit hit;

    private Vector3 sphereCastStart;
    private Image image;
    private int currentPrimaryWeaponIndex;

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
        image = reticleChangeColor.GetComponent<Image>();
        controller = GetComponent<Controller>();
        backgroundMaskCanvasGroup = backgroundMask.GetComponent<CanvasGroup>();
    }

    //Disabled since we are using the shoot button to place bricks
private void FixedUpdate()
{
    if (Settings.OnlinePlay && !isLocalPlayer) return;

    target = null; // reset target
    target = FindTarget(); // get target gameObject
    sphereCastStart = controller.playerCamera.transform.parent.transform.position;

    currentPrimaryWeaponIndex = controller.currentWeaponPrimaryIndex;
    WeaponPrimary currentWeaponPrimary = controller.weaponsPrimary[currentPrimaryWeaponIndex];
    fireRate = currentWeaponPrimary.fireRate * 0.5f;

    if (Time.time >= nextTimeToFire && backgroundMaskCanvasGroup.alpha == 0)
    {
        if (inputHandler.shoot)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            weaponSounds.clip = currentWeaponPrimary.shootSound;
            weaponSounds.Play();
            Shoot();
        }
    }
    if(Time.time >= nextTimeToFire && backgroundMaskCanvasGroup.alpha == 0)
    {
        if(inputHandler.grenade)
        {
            
        }
    }
}

// if raycast hits a destructible object (with health but not this player), turn outer reticle red
public Health FindTarget()
    {
        image.color = Color.HSVToRGB(0, 0, 50, true);

        //if hit something
        if (Physics.SphereCast(sphereCastStart, sphereCastRadius, fpsCam.transform.forward, out hit, hitScanDist))
        {
            if (hit.transform.GetComponent<Health>() != null)
                target = hit.transform.GetComponent<Health>();

            if (hit.transform.tag == "BaseObPiece") // else if targeting a base object
            {
                image.color = Color.HSVToRGB(0, 100, 50, true); // turn reticle red
                return null;
            }
            else if (target != null && target.gameObject != gameObject && target.hp != 0) // if hits a model that is not this model
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
    public void Shoot()
    {
        if (hit.transform != null && hit.transform.tag == "BaseObPiece") // hit base object
        {
            hitSound.Play();

            Vector3 pos = hit.transform.position;
            if (Settings.OnlinePlay)
            {
                controller.CmdSpawnObject(3, 3, new Vector3(pos.x + -0.25f, pos.y + 0, pos.z + 0.25f));
                controller.CmdSpawnObject(3, 3, new Vector3(pos.x + -0.25f, pos.y + 0, pos.z - 0.25f));
                controller.CmdSpawnObject(3, 3, new Vector3(pos.x + 0.25f, pos.y + 0, pos.z + 0.25f));
                controller.CmdSpawnObject(3, 3, new Vector3(pos.x + 0.25f, pos.y + 0, pos.z - 0.25f));
            }
            else
            {
                controller.SpawnObject(3, 3, new Vector3(pos.x + -0.25f, pos.y + 0, pos.z + 0.25f));
                controller.SpawnObject(3, 3, new Vector3(pos.x + -0.25f, pos.y + 0, pos.z - 0.25f));
                controller.SpawnObject(3, 3, new Vector3(pos.x + 0.25f, pos.y + 0, pos.z + 0.25f));
                controller.SpawnObject(3, 3, new Vector3(pos.x + 0.25f, pos.y + 0, pos.z - 0.25f));
            }
        }
        else if (target != null) // if target was found
        {
            hitSound.Play();

            if (Settings.OnlinePlay)
                CmdDamage(target);
            else
                Damage(target);
        }
    }

    [Command]
    // public function called when gun raycast hits target
    public void CmdDamage(Health target)
    {
        // player identity validation logic here
        RpcDamage(target);
    }

    [ClientRpc]
    public void RpcDamage(Health target)
    {
        Damage(target);
    }

    public void Damage(Health target)
    {
        target.hp -= damage; // only edit health on server which pushes syncVar updates to clients
        target.UpdateHP(target.hp, target.hp);
        if (target.isAlive)
            target.PlayHurtSound(currentPrimaryWeaponIndex);
    }
}
