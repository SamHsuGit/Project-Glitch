using Mirror;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Controller : NetworkBehaviour
{
    public World world;

    [SyncVar(hook = nameof(SetName))] public string playerName = "PlayerName";

    // Server Values (server generates these values upon start, all clients get these values from server upon connecting)
    [SyncVar] private string versionServer;
    readonly private SyncList<string> playerNamesServer = new SyncList<string>();

    [SyncVar(hook = nameof(SetCurrentWeaponPrimaryIndex))] public int currentWeaponPrimaryIndex = 0;
    [SyncVar(hook = nameof(SetCurrentWeaponSecondaryIndex))] public int currentWeaponSecondaryIndex = 0;

    [Header("Debug States")]
    [SerializeField] float collisionDamage;
    public bool isGrounded;
    [SyncVar(hook = nameof(SetIsMoving))] public bool isMoving = false;
    public bool options = false;
    public float m_Speed = 20f;
    public float m_SpeedAir = 10f;
    public float _jumpForce = 350f;
    public int maxJumps = 2;
    public int currentJumps = 0;
    public bool jump;
    public bool shoot;
    public bool grenade;
    public bool melee;
    public int batteries;
    public bool isThrowingGrenade = false;

    [SerializeField] float _lookVelocity = 1f;

    [Header("GameObject References")]
    public Player player;
    public GameObject charModelOrigin;
    public GameObject charModelHead;
    public GameObject charModelTorso;
    public GameObject charModelLegs;
    public GameObject gameMenu;
    public GameObject nametag;
    public GameObject backgroundMask;
    public GameObject playerCamera;
    public GameObject playerHUD;
    public GameObject CinematicBars;
    public GameObject reticle;
    public GameObject target;
    public GameObject sceneObjectPrefab;
    public GameObject charModel;
    public Material[] charMaterials;
    public Animator[] animators;
    public GameObject projectilePrimaryOrigin;
    public GameObject projectileSecondaryOrigin;
    public GameObject[] weaponsObsPrimary;
    public bool[] inventoryWeaponsPrimary;
    public WeaponPrimary[] weaponsPrimary;
    public bool[] inventoryWeaponsSecondary;
    public GameObject[] weaponsObsSecondary;
    public WeaponSecondary[] weaponsSecondary;
    

    //Components
    private GameManagerScript gameManager;
    private GameObject playerCameraOrigin;
    private CapsuleCollider cc;
    private Rigidbody _rb;
    private Animator animator;
    private PlayerInput playerInput;
    private InputHandler _inputHandler;
    private Health health;
    private Gun gun;
    private CanvasGroup backgroundMaskCanvasGroup;
    private GameMenu gameMenuComponent;
    private BoxCollider playerCameraBoxCollider;
    private PhysicMaterial physicMaterial;
    private CustomNetworkManager customNetworkManager;
    private RaycastHit raycastHit;

    //Initializers & Constants
    private Vector3 velocityPlayer;
    private Vector3 direction;
    private Vector2 move;
    private Vector2 look;
    private float colliderHeight;
    private float colliderRadius;
    private Vector3 colliderCenter;
    private float sphereCastRadius;
    private float _rotationY = 0f;
    private float _rotationX = 0f;
    private float _maxLookVelocity = 5f;
    private float _maxCamAngle = 90f;
    private float _minCamAngle = -90f;
    private float fireRate = 2f;
    private float nextTimeToFire = 0f;
    private int[] startingPrimaryWeapons = new int[] {0,23}; // mining laser, hand
    private int[] startingSecondaryWeapons = new int[] {0,8}; // coring charge, hand

    // THE ORDER OF EVENTS IS CRITICAL FOR MULTIPLAYER!!!
    // Order of network events: https://docs.unity3d.com/Manual/NetworkBehaviourCallbacks.html
    // Order of SyncVars: https://mirror-networking.gitbook.io/docs/guides/synchronization/syncvars
    // The state of SyncVars is applied to game objects on clients before OnStartClient() is called, so the state of the object is always up - to - date inside OnStartClient().

    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        world = gameManager.worldOb.GetComponent<World>();
        customNetworkManager = gameManager.PlayerManagerNetwork.GetComponent<CustomNetworkManager>();
        NamePlayer();

        cc = GetComponent<CapsuleCollider>();
        _rb = GetComponent<Rigidbody>();
        _inputHandler = GetComponent<InputHandler>();
        health = GetComponent<Health>();
        gun = GetComponent<Gun>();
        
        backgroundMaskCanvasGroup = backgroundMask.GetComponent<CanvasGroup>();
        gameMenuComponent = gameMenu.GetComponent<GameMenu>();
        playerCameraOrigin = playerCamera.transform.parent.gameObject;
        playerCameraBoxCollider = playerCamera.GetComponent<BoxCollider>();

        health.isAlive = true;

        CinematicBars.SetActive(false);

        // init arrays
        inventoryWeaponsPrimary = new bool[24];
        inventoryWeaponsSecondary = new bool[9];
    }

    void SetInventory()
    {
        // Empty inventory
        for(int i = 0; i < inventoryWeaponsPrimary.Length; i++)
            inventoryWeaponsPrimary[i] = false;
        for (int i = 0; i < inventoryWeaponsSecondary.Length; i++)
            inventoryWeaponsSecondary[i] = false;

        // Give starting weapons
        for (int i = 0; i < startingPrimaryWeapons.Length; i++)
            GiveWeaponPrimary(i);
        for (int i = 0; i < startingSecondaryWeapons.Length; i++)
            GiveWeaponSecondary(i);
        SetCurrentWeaponPrimaryIndex(0, 0);
        SetCurrentWeaponSecondaryIndex(0, 0);
    }

    void NamePlayer()
    {
        {
            // set this object's name from saved settings so it can be modified by the world script when player joins
            playerName = SettingsStatic.LoadedSettings.playerName;

            player = new Player(gameObject, playerName); // create a new player, try to load player stats from save file
        }
    }

    private void Start()
    {
        world.JoinPlayer(gameObject); // must NamePlayer and initialize world before this can be run

        InputComponents();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        playerCamera.GetComponent<Camera>().nearClipPlane = 0.01f;

        // position nametag procedurally based on imported char model size
        nametag.transform.localPosition = new Vector3(0, colliderCenter.y + colliderHeight * 0.55f, 0);

        foreach (Animator anims in animators) // play initial respawn animation
            anims.Play("Respawn");

        if (!Settings.OnlinePlay)
        {
            SetName(playerName, playerName);
            nametag.SetActive(false); // disable nametag for singleplayer/splitscreen play

           world.gameObject.SetActive(true);
        }

        SetInventory();
    }

    void InputComponents()
    {
        if (gameObject.GetComponent<PlayerInput>() != null) { playerInput = gameObject.GetComponent<PlayerInput>(); }
        if (gameObject.GetComponent<InputHandler>() != null) { _inputHandler = gameObject.GetComponent<InputHandler>(); }
        if (!Settings.OnlinePlay)
        {
            playerInput.enabled = true;
            _inputHandler.enabled = true;
        }
        else
        {
            if (isLocalPlayer)
            {
                playerInput.enabled = false; // for online play, tricks playerInput component to accepting the online player as newest joined player even when more than one player
                playerInput.enabled = true;
                _inputHandler.enabled = true;
            }
            else
            {
                playerInput.enabled = false;
                _inputHandler.enabled = false;
            }
        }
    }

    public override void OnStartServer() // Only called on Server and Host
    {
        base.OnStartServer();

        // SET SERVER VALUES FROM HOST CLIENT
        //planetNumberServer = SettingsStatic.LoadedSettings.planetSeed;
        //seedServer = SettingsStatic.LoadedSettings.worldCoord;

        versionServer = Application.version;

        //SetServerChunkStringSyncVar(); // Server sends initially loaded chunks as chunkStringSyncVar to clients (DISABLED, send chunks over internet manually, figure out how to send data not as strings)

        //customNetworkManager.InitWorld();
    }
    
    public override void OnStartClient() // Only called on Client and Host
    {
        base.OnStartClient();

        // Check if client version matches versionServer SyncVar (SyncVars are updated before OnStartClient()
        if (isClientOnly)
        {
            if (Application.version != versionServer) // if client version does not match server version, show error.
                ErrorMessage.Show("Error: Version mismatch. " + Application.version + " != " + versionServer + ". Client game version must match host. Disconnecting Client.");

            foreach (string name in playerNamesServer) // check new client playername against existing server player names to ensure it is unique for savegame purposes
            {
                if (SettingsStatic.LoadedSettings.playerName == name)
                    ErrorMessage.Show("Error: Non-Unique Player Name. Client name already exists on server. Player names must be unique. Disconnecting Client.");
            }
        }

        SetName(playerName, playerName); // called on both clients and host

        //if (isClientOnly)
        //    customNetworkManager.InitWorld(); // activate world only after getting syncVar latest values from server
    }

    public void SetName(string oldValue, string newValue)
    {
        // update the player name using the SyncVar pushed from the server to clients
        if (playerName == null)
        {
            Debug.Log("No string found for playerName");
            return;
        }

        playerName = newValue;
        nametag.GetComponent<TextMesh>().text = newValue;
    }

    public void SetIsMoving(bool oldValue, bool newValue)
    {
        isMoving = newValue;
    }

    public void SetCurrentWeaponPrimaryIndex(int oldValue, int newValue)
    {
        currentWeaponPrimaryIndex = newValue;
    }

    public void SetCurrentWeaponSecondaryIndex(int oldValue, int newValue)
    {
        currentWeaponSecondaryIndex = newValue;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == null)
            return;

        GameObject ob = collision.gameObject;
        // hazards hurt player
        if (ob.tag == "Hazard")
        {
            if (ob.GetComponent<Projectile>() == null)
                return;

            // subtract health by damage value of projectile instead of -1 every time
            health.EditSelfHealth(-ob.GetComponent<Projectile>().damage);
        }
        else if(ob.tag == "Pickup")
        {
            if (ob.GetComponent<PickupObject>() == null)
                return;
            PickupObject pickup = ob.GetComponent<PickupObject>();
            pickup.audioSourcePlayer.clip = pickup.pickupSound1;
            pickup.audioSourcePlayer.Play();

            switch (pickup.type)
            {
                case 0: // PRIMARY WEAPON
                    {
                        if (ob.GetComponent<WeaponPrimary>() != null)
                            break;
                        GiveWeaponPrimary(ob.GetComponent<WeaponPrimary>().index);
                        break;
                    }
                case 1: // SECONDARY WEAPON
                    {
                        if (ob.GetComponent<WeaponSecondary>() == null)
                            break;
                        GiveWeaponSecondary(ob.GetComponent<WeaponSecondary>().index);
                        break;
                    }
                case 2: // power up (energy, upgrades, washers)
                    {
                        switch(pickup.index)
                        {
                            case 0: //energy
                                {
                                    health.EditSelfHealth(health.batteryMaxHP); // energy fills one battery's worth of hp
                                    break;
                                }
                            case 1: //mega energy
                                {
                                    health.EditSelfHealth(health.hpMax - health.hp); // mega energy fills hp to max
                                    break;
                                }
                            case 2://shield
                                {

                                    break;
                                }
                            case 3://speed
                                {

                                    break;
                                }
                            case 4://EUK2
                                {
                                    //Advance current weapon level by 1
                                    break;
                                }
                            case 5://EUK3
                                {
                                    //Advance current weapon level by 2???
                                    break;
                                }
                            case 6://Battery
                                {

                                    break;
                                }
                            case 7://Washer
                                {

                                    break;
                                }
                            case 8://Mega Washer
                                {

                                    break;
                                }
                            case 9://Det pak
                                {

                                    break;
                                }
                            case 10://Chip
                                {

                                    break;
                                }
                            case 11://Secret Chip
                                {

                                    break;
                                }
                            case 12://Fire Buddy
                                {

                                    break;
                                }
                        }
                        break;
                    }
            }
            Destroy(pickup.gameObject);
        }
    }

    private void GiveWeaponPrimary(int index)
    {
        inventoryWeaponsPrimary[index] = true;
        weaponsPrimary[index].ammo = weaponsPrimary[index].maxAmmo;
    }

    private void GiveWeaponSecondary(int index)
    {
        inventoryWeaponsSecondary[index] = true;
        weaponsSecondary[index].ammo = weaponsSecondary[index].maxAmmo;
    }

    private void Update()
    {
        if (!Settings.WorldLoaded) return; // don't do anything until world is loaded

        if (options)
            gameMenuComponent.OnOptions();
        else if (!options)
            gameMenuComponent.ReturnToGame();

        if(!options)
        {
            CacheInput();
            Look();
        }
    }

    void FixedUpdate()
    {
        if (!Settings.WorldLoaded) return; // don't do anything until world is loaded

        //disable virtual camera and exit from FixedUpdate if this is not the local player
        if (Settings.OnlinePlay && !isLocalPlayer)
        {
            Animate();
            playerCamera.SetActive(false);
            return;
        }

        isGrounded = CheckGroundedCollider();
        if (isGrounded)
            currentJumps = 0;

        if (!options)
        {
            Move();
            RbForceJump();
            Animate();
        }
    }

    public void PressedShoot()
    {
        CmdSpawnObject(0, 0, projectilePrimaryOrigin.transform.position);
    }

    public void PressedGrenade()
    {
        //if (!isThrowingGrenade)
        {
            isThrowingGrenade = true;
            CmdSpawnObject(1, 0, projectileSecondaryOrigin.transform.position);
        }
    }

    void SpawnVoxelRbFromWorld(Vector3 position, byte blockID)
    {
        if (blockID == 0 || blockID == 1) // if the blockID at position is air, barrier, base, procGenVBO, then skip to next position
            return;

        //EditVoxel(position, 0, true); // destroy voxel at position
        if (Settings.OnlinePlay)
            CmdSpawnObject(0, blockID, position);
        else
            SpawnObject(0, blockID, position);
    }

    public void CmdSpawnObject(int type, int item, Vector3 pos)
    {
        switch(type)
        {
            case (0):
            {
                if (weaponsPrimary[currentWeaponPrimaryIndex].projectile != null)
                {
                    // create target vector from projectile origin
                    Vector3 targetVector = target.transform.position - pos;
                    GameObject ob = Instantiate(weaponsPrimary[currentWeaponPrimaryIndex].projectile, pos, Quaternion.LookRotation(projectilePrimaryOrigin.transform.forward, Vector3.up));
                    Rigidbody rb = ob.GetComponent<Rigidbody>();
                    rb.velocity = targetVector.normalized * weaponsPrimary[currentWeaponPrimaryIndex].projectileVelocity;
                    ob.transform.Rotate(Vector3.right, 90f);
                    Destroy(ob, 120);
                }
                break;
            }
            case 1:
            {
                if (weaponsSecondary[currentWeaponSecondaryIndex].projectile != null)
                {
                    Vector3 grenadeTarget = target.transform.position + target.transform.up * 3; // grenade should have slight upwards velocity
                    // create target vector from projectile origin
                    Vector3 targetVector = grenadeTarget - pos;
                    GameObject ob = Instantiate(weaponsSecondary[currentWeaponSecondaryIndex].projectile, pos, Quaternion.Euler(targetVector));
                    Rigidbody rb = ob.GetComponent<Rigidbody>();
                    rb.velocity = targetVector.normalized * weaponsPrimary[currentWeaponSecondaryIndex].projectileVelocity;
                    Destroy(ob, 120); // grenade should destroy itself before 2 min (120 sec), but just in case, clean up scene
                }
                break;
            }
        }
        
        
    }

    public void SpawnObject(int type, int item, Vector3 pos, GameObject obToSpawn = null)
    {
        Vector3 spawnDir;
        // all other camera modes, spawn object in direction of playerObject
            spawnDir = transform.forward;

        GameObject ob = Instantiate(sceneObjectPrefab, pos, Quaternion.identity);
        Rigidbody rb;

        ob.transform.rotation = Quaternion.LookRotation(spawnDir); // orient forwards in direction of camera
        rb = ob.GetComponent<Rigidbody>();
        rb.mass = health.piecesRbMass;
        rb.isKinematic = false;
        rb.velocity = spawnDir * 25; // give some velocity away from where player is looking

        SceneObject sceneObject = ob.GetComponent<SceneObject>();
        // IF VOXEL
        sceneObject.SetEquippedItem(type, item); // set the child object on the server
        sceneObject.typeProjectile = item; // set the SyncVar on the scene object for clients
        BoxCollider VoxelBc = ob.AddComponent<BoxCollider>();
        VoxelBc.material = physicMaterial;
        VoxelBc.center = new Vector3(0.5f, 0.5f, 0.5f);
        ob.tag = "voxelRb";
        sceneObject.controller = this;

        if (Settings.OnlinePlay)
        {
            ob.GetComponent<NetworkIdentity>().enabled = true;
            if (ob.GetComponent<NetworkTransform>() == null)
                ob.AddComponent<NetworkTransform>();
            ob.GetComponent<NetworkTransform>().enabled = true;
            customNetworkManager.SpawnNetworkOb(ob);
        }
        Destroy(ob, 30); // clean up objects after 30 seconds
    }

    bool CheckGroundedCollider()
    {
        float rayLength;
        Vector3 rayStart = transform.position;

        // cast a ray starting from within the capsule collider down to outside the capsule collider.
        rayLength = cc.height * 0.5f + 0.01f;

        sphereCastRadius = cc.radius * 0.5f;

        // Debug tools
        Debug.DrawRay(rayStart, Vector3.down * rayLength, Color.red, 0.02f);

        // check if the char is grounded by casting a ray from rayStart down extending rayLength
        if (Physics.SphereCast(rayStart, sphereCastRadius, Vector3.down, out RaycastHit hit, rayLength))
            return true;
        else
            return false;
    }

    private void CacheInput()
    {
        move = _inputHandler.move;
        if (move != Vector2.zero)
            isMoving = true;
        else
            isMoving = false;

        look = _inputHandler.look;
        jump = _inputHandler.jump;


        shoot = _inputHandler.shoot;
        grenade = _inputHandler.grenade;

        melee = _inputHandler.melee;
    }

    private void Look()
    {
        Vector2 rotation = CalculateRotation();

        if (isGrounded)
        {
            //charModelOrigin.transform.eulerAngles = Vector3.zero;

            charModelHead.transform.eulerAngles = new Vector3(0, playerCameraOrigin.transform.rotation.eulerAngles.y, 0); // rotate char model head to face same y direction as camera
            charModelTorso.transform.eulerAngles = new Vector3(0, playerCameraOrigin.transform.rotation.eulerAngles.y, 0); // rotate char model torso to face same y direction as camera

            if (isMoving && direction != Vector3.zero)
            {
                charModelLegs.transform.forward = direction; // rotate char model legs to face same y direction as movement
                charModelLegs.transform.eulerAngles = new Vector3(0, charModelLegs.transform.eulerAngles.y, 0);
            }
        }
        else // if in air
        {
            // rotate entire char model to face direction of camera (must occur before camera is moved again)
            charModelOrigin.transform.eulerAngles = new Vector3(0f, playerCameraOrigin.transform.eulerAngles.y, 0f);
        }

        // rotate cameraOrigin around player model (has to occur after rotating charModelOrigin to match camera origin rotation in mid-air)
        playerCameraOrigin.transform.eulerAngles = new Vector3(rotation.y, rotation.x, 0f);
    }

    private void Move()
    {
        direction = playerCameraOrigin.transform.right * move.x + playerCameraOrigin.transform.forward * move.y; // move in direction camera is facing
        direction = new Vector3(direction.x, 0, direction.z); // remove y component (no vertical movement)

        float speed = m_Speed;
        if (!isGrounded)
            speed = m_SpeedAir;
        _rb.AddForce(direction * speed, ForceMode.Impulse);
    }

    private void RbForceJump()
    {
        //jump timer
        if (Time.time < nextTimeToFire)
            return;

        float force = _jumpForce;

        if (jump && (isGrounded || currentJumps < 2))
        {
            nextTimeToFire = Time.time + 1f / fireRate;

            // rotate char model legs to face same y direction as torso
            charModelLegs.transform.eulerAngles = new Vector3(0, charModelTorso.transform.eulerAngles.y, 0);

            if (currentJumps > 0) // 2nd jump has more force
            {
                force = _jumpForce * 2;
            }

            _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
            currentJumps++;
            isGrounded = false;
        }
    }

    private Vector2 CalculateRotation()
    {
        if (look.x != 0f && look.y != 0f)
        {
            if (_lookVelocity > _maxLookVelocity)
                _lookVelocity = _maxLookVelocity;
            else
                _lookVelocity += SettingsStatic.LoadedSettings.lookAccel;
        }
        else
            _lookVelocity = 1f;

        // rotate camera left/right, multiply by lookVelocity so controller players get look accel
        _rotationX += look.x * _lookVelocity * SettingsStatic.LoadedSettings.lookSpeed * 0.5f;

        if (!SettingsStatic.LoadedSettings.invertY)
            _rotationY += -look.y * _lookVelocity * SettingsStatic.LoadedSettings.lookSpeed * 0.5f;
        else
            _rotationY += look.y * _lookVelocity * SettingsStatic.LoadedSettings.lookSpeed * 0.5f;

        // limit transform so player cannot look up or down past the specified angles
        _rotationY = Mathf.Clamp(_rotationY, _minCamAngle, _maxCamAngle);

        return new Vector2(_rotationX, _rotationY);
    }

    public void ToggleOptions()
    {
        options = !options;
    }

    void Animate()
    {
        foreach (Animator anims in animators)
        {
            // set animation speed of walk anim to match normalized speed of character.
            anims.SetBool("isMoving", isMoving);
            anims.SetBool("isGrounded", isGrounded);
            anims.SetBool("Shoot", _inputHandler.shoot);
            anims.SetBool("Grenade", _inputHandler.grenade);
            anims.SetInteger("Jumps", currentJumps);
            anims.SetBool("Melee", _inputHandler.melee);
        }

        if (!health.isAlive)
            foreach (Animator anims in animators)
                anims.Play("Respawn");
    }
}