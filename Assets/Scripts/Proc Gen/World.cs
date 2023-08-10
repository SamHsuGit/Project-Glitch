using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Diagnostics;
using Unity.Profiling;

public class World : MonoBehaviour
{
    // PUBLIC VARIABLES
    [Header("Shown For Debug")]
    public int playerCount = 0;

    // Other Values
    [HideInInspector] public bool worldLoaded = false;
    [HideInInspector] public List<Player> players = new List<Player>();

    [Header("Public References")]
    public GameObject mainCameraGameObject;
    public GameObject loadingText;
    public GameObject loadingBackground;
    public CustomNetworkManager customNetworkManager;
    public GameObject worldPlayer;
    public PhysicMaterial physicMaterial;
    public GameObject[] pickups;
    [HideInInspector] public static World Instance { get { return _instance; } }

    // PRIVATE VARIABLES


    private Dictionary<Player, GameObject> playerGameObjects = new Dictionary<Player, GameObject>();

    private Camera mainCamera;
    private Vector3 defaultSpawnPosition;

    private static World _instance;

    private void Awake()
    {
        defaultSpawnPosition = Settings.DefaultSpawnPosition;
        mainCamera = mainCameraGameObject.GetComponent<Camera>();

        playerCount = 0;

        // If the instance value is not null and not *this*, we've somehow ended up with more than one World component.
        // Since another one has already been assigned, delete this one.
        if (_instance != null && _instance != this)
            Destroy(gameObject);
        // Else set this to the instance.
        else
            _instance = this;
    }

    private void Start()
    {
        worldLoaded = false;

        worldPlayer.transform.position = defaultSpawnPosition;

        JoinPlayer(worldPlayer); // needed to load world before player joins?

        if (Settings.OnlinePlay)
        {
            loadingText.SetActive(false);
            loadingBackground.GetComponent<CanvasGroup>().alpha = 0;
        }

        if(Settings.Platform != 2)
            mainCamera.enabled = false;

        Settings.WorldLoaded = true;

        for (int i = 0; i < pickups.Length; i++)
            pickups[i].SetActive(true);
    }

    public void JoinPlayer(GameObject playerGameObject)
    {
        Player player;

        if (playerGameObject == worldPlayer)
        {
            player = new Player(playerGameObject, "WorldPlayer"); // world player is needed to generate the world before the player is added
            players.Add(player);
        }
        else if (Settings.Platform != 2)
        {
            player = playerGameObject.GetComponent<Controller>().player;
            players.Add(player);
        }
        else
        {
            player = new Player(playerGameObject, "VR Player");
            players.Add(player);
        }

        playerGameObjects.Add(player, player.playerGameObject);

        playerGameObject.transform.position = defaultSpawnPosition; // spawn at world spawn point

        playerCount++;
    }
}