using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Health : NetworkBehaviour
{
    public AudioSource audioSourcePlayer;
    public AudioClip[] hurtClips;
    public int currentHurtClipIndex;
    public AudioClip deathSound;
    public PhysicMaterial physicMaterial;
    
    private int pieceCount;
    [SyncVar(hook = nameof(UpdateHP))] public int hp; // uses hp SyncVar hook to syncronize # pieces an object has across all online players when hp value changes
    public int hpMax;
    public int batteryMaxHP = 100;
    public float piecesRbMass = 0.0001f;
    public bool isAlive = false;
    int lastPlayerPos = 0;

    Controller controller;
    GameObject ob;
    public List<GameObject> modelPieces;
    readonly SyncList<GameObject> modelPiecesSyncList = new SyncList<GameObject>();

    void Awake()
    {
        if (gameObject.GetComponent<Controller>() != null)
            controller = gameObject.GetComponent<Controller>();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        if (!isAlive && modelPieces.Count != 0)
        {
            for (int i = 0; i < modelPieces.Count; i++)
                modelPiecesSyncList.Add(modelPieces[i]);
        }
    }

    private void Start()
    {

        hpMax = batteryMaxHP * controller.batteries;
        hp = hpMax;

        if (isAlive)
        {
            lastPlayerPos = Mathf.FloorToInt(gameObject.transform.position.magnitude);
        }  
    }

    private void FixedUpdate()
    {
        if (gameObject.layer == 11) // if it is a player
        {
            //Hunger(); // Disabled (causes hp issues upon spawning which breaks online multiplayer)

            // Respawn
            if (hp < 1)
            {
                audioSourcePlayer.PlayOneShot(deathSound);
                if (Settings.OnlinePlay && hasAuthority)
                    Invoke("CmdRespawn",3f);
                else
                    Invoke("Respawn", 3f);
            }
        }

        if (transform.position.y < -20 && hp > 0) // hurt if falling below world
        {
            if (Settings.OnlinePlay && hasAuthority)
                CmdEditSelfHealth(-1);
            if (!Settings.OnlinePlay)
                EditSelfHealth(-1);
        }
    }

    public void RequestEditSelfHealth(int amount) // gameObjects can only call this for their own hp
    {
        if (Settings.OnlinePlay && hasAuthority)
            CmdEditSelfHealth(amount);
        if (!Settings.OnlinePlay)
            EditSelfHealth(amount);
    }

    [Command]
    public void CmdEditSelfHealth(int amount)
    {
        EditSelfHealth(amount); // calls server to update SyncVar hp which then pushes updates to clients automatically
        RpcUpdateHP(); // after hp update on server, need to update pieces on all clients
    }

    [ClientRpc]
    public void RpcUpdateHP() // server tells all clients to update pieces based on new hp value from server
    {
        UpdateHP(hp, hp);
    }

    public void EditSelfHealth(int amount) // Server updates hp and then updates pieces
    {
        if (hp + amount > hpMax)
        {
            hp = hpMax;
            return;
        }
        else
        {
            hp = hp + amount;
            UpdateHP(hp, hp);
        }
    }

    // runs from gun script when things are shot, runs in this script when object falls below certain height
    public void UpdateHP(int oldValue, int newValue)
    {
        hp = newValue;
        controller.gameMenu.GetComponent<GameMenu>().UpdateHP();
        SetModelPieceVisibility(modelPieces);
    }

    public void PlayHurtSound(int weaponHitIndex)
    {
        audioSourcePlayer.PlayOneShot(controller.wPrimaryPickupObjects[controller.currentWeaponPrimaryIndex].hitSound);

        if (currentHurtClipIndex > hurtClips.Length)
            currentHurtClipIndex = 0;
        audioSourcePlayer.PlayOneShot(hurtClips[currentHurtClipIndex]);
        currentHurtClipIndex++;
    }

    void SetModelPieceVisibility(List<GameObject> modelPartsList)
    {
        if (hp > hpMax)
            hp = hpMax;
        if (hp < 0)
            hp = 0;
        if (modelPartsList.Count > 1)
        {
            for (int i = 0; i < modelPartsList.Count; i++) // for all modelParts
            {
                GameObject obToSpawn = modelPartsList[i];
                if (i >= hp && obToSpawn.GetComponent<MeshRenderer>() != null && obToSpawn.GetComponent<MeshRenderer>().enabled) // if modelPart index >= hp and not hidden, hide it
                {
                    // cannot spawn voxel bits because this code cannot run on clients, gave error cannot spawn objects without active server...
                    // cannot spawn a copy of the character model piece that was shot because cannot pass gameobject to server command CmdSpawnObject

                    // turn off components, do not disable gameobject since multiplayer networking needs a reference to the object and disabling gameobject breaks this reference!
                    if (obToSpawn.GetComponent<MeshRenderer>() != null)
                        obToSpawn.GetComponent<MeshRenderer>().enabled = false;
                    if (obToSpawn.GetComponent<BoxCollider>() != null)
                        obToSpawn.GetComponent<BoxCollider>().enabled = false;
                }
            }
        }
    }

    [Command]
    public void CmdRespawn()
    {
        RpcRespawn();
    }

    [ClientRpc]
    public void RpcRespawn()
    {
        Respawn();
    }

    public void Respawn()
    {
        // teleport player to spawn point
        transform.position = Settings.DefaultSpawnPosition;

        hp = hpMax;

        // turn on components again, do not disable gameobject since multiplayer networking needs a reference to the object and disabling gameobject breaks this reference!
        for (int i = 0; i < modelPieces.Count; i++)
        {
            GameObject ob = modelPieces[i];
            if (ob.GetComponent<MeshRenderer>() != null)
                ob.GetComponent<MeshRenderer>().enabled = true;
            if (ob.GetComponent<BoxCollider>() != null)
                ob.GetComponent<BoxCollider>().enabled = true;
        }
    }

}