using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManagerScript : MonoBehaviour
{
    public GameObject worldOb;
    public GameObject playerManagerLocal;
    public GameObject NETWORK;
    public GameObject PlayerManagerNetwork;
    public GameObject LOCAL;
    public GameObject charPrefab;

    private Stopwatch preWorldGenStopwatch;

    void Awake()
    {
        preWorldGenStopwatch = new Stopwatch();
        preWorldGenStopwatch.Start();

        // these values are set immediately and overwritten later if necessary to match server properties

        if (Settings.OnlinePlay)
        {
            worldOb.SetActive(false); // later enabled by CustomNetworkManager when player selects host or join
            NETWORK.SetActive(true); // activate NetworkMenu where player selects host or join
            PlayerManagerNetwork.SetActive(true);

            LOCAL.SetActive(false); // not needed for online play (i.e. cannot do splitscreen/online play at same time)
        }
        else
            Setup();
        FileSystemExtension.SaveSettings(); // saved changed settings to file
    }

    public void Setup() // called by NetworkMenu for online play
    {
        // valid gameplay modes
        // splitscreen (pc = 0, console = 1)
        // network (pc = 0, console = 1)
        // mobile singleplayer

        SettingsStatic.LoadedSettings = SettingsStatic.LoadSettings();

        if (Settings.Platform == 2)
        {
            // values set ahead of world gameObject activation
            
        }

        if (Settings.OnlinePlay) // network online multiplayer
        {
            // order of events is important for network ids to be generated correctly

            if (Settings.Platform == 2) // mobile singleplayer network play
            {

            }
            else // console (1) and pc (0) singleplayer network play
            {

            }
        }
        else // local
        {
            NETWORK.SetActive(false);
            PlayerManagerNetwork.SetActive(false);

            if (Settings.Platform == 2) // mobile singleplayer
            {
                LOCAL.SetActive(false);
            }
            else // console (1) and pc (0) splitscreen
            {
                playerManagerLocal.GetComponent<PlayerInputManager>().playerPrefab = charPrefab;
                LOCAL.SetActive(true);
            }
        }
    }
}
