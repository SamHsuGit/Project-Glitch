using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.SceneManagement;

public class NetworkMenu : MonoBehaviour
{
    public GameObject playerManagerNetwork;
    public TMP_InputField networkAddressInputField;
    public TMP_Text connectionStatus;
    public GameObject networkMenuElements;
    public AudioSource buttonSound;
    public GameObject background;
    public GameObject loadingText;
    public GameObject gameManagerObject;

    public AudioSource soundtrack;

    GameManagerScript gameManager;
    NetworkManager manager;
    CanvasGroup networkMenuElementsCanvasGroup;

    private void Awake()
    {
        manager = playerManagerNetwork.GetComponent<CustomNetworkManager>();

        networkMenuElementsCanvasGroup = networkMenuElements.GetComponent<CanvasGroup>();

        networkMenuElementsCanvasGroup.alpha = 1;
        networkMenuElementsCanvasGroup.interactable = true;
        networkAddressInputField.text = SettingsStatic.LoadedSettings.ipAddress;
        background.GetComponent<CanvasGroup>().alpha = 1;
        gameManager = gameManagerObject.GetComponent<GameManagerScript>();
    }

    public void OnHostClient()
    {
        networkMenuElementsCanvasGroup.alpha = 0;
        networkMenuElementsCanvasGroup.interactable = false;
        loadingText.SetActive(true); // in order for this text to show before world load, would need to change scene before loading next scene with world (like Setup Menu for Splitscreen)

        buttonSound.Play();
        FileSystemExtension.SaveSettings();

        if (!NetworkClient.active)
        {
            // Server + Client
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                manager.StartHost();
                manager.networkAddress = networkAddressInputField.text;
            }
        }
        else
        {
            // Connecting
            connectionStatus.text = "Connecting to " + manager.networkAddress + "..";
        }

        StatusLabels();
        gameManager.Setup(); // activate ldraw importer, etc.
        loadingText.SetActive(false);
        background.GetComponent<CanvasGroup>().alpha = 0;
        soundtrack.Stop();
    }

    public void OnClientOnly()
    {

        manager.networkAddress = networkAddressInputField.text;

        networkMenuElementsCanvasGroup.alpha = 0;
        networkMenuElementsCanvasGroup.interactable = false;
        loadingText.SetActive(true); // in order for this text to show before world load, would need to change scene before loading next scene with world (like Setup Menu for Splitscreen)

        buttonSound.Play();
        FileSystemExtension.SaveSettings();

        if (!NetworkClient.active)
        {
            // Client + IP
            manager.StartClient();
        }
        else
        {
            // Connecting
            connectionStatus.text = "Connecting to " + manager.networkAddress + "..";
        }

        StatusLabels();
        gameManager.Setup(); // activate ldraw importer, etc.
        loadingText.SetActive(false);
        background.GetComponent<CanvasGroup>().alpha = 0;
        soundtrack.Stop();
    }

    public void Back()
    {
        buttonSound.Play();
        FileSystemExtension.SaveSettings();
        SceneManager.LoadScene(0);
    }

    public void OnClientConnectToEvent()
    {
        manager.networkAddress = "98.235.185.75"; // ideally load from file

        networkMenuElementsCanvasGroup.alpha = 0;
        networkMenuElementsCanvasGroup.interactable = false;
        loadingText.SetActive(true);

        buttonSound.Play();
        FileSystemExtension.SaveSettings();

        if (!NetworkClient.active)
        {
            // Client + IP
            manager.StartClient();
        }
        else
        {
            // Connecting
            connectionStatus.text = "Connecting to Server";
        }

        StatusLabels();
        gameManager.Setup(); // activate ldraw importer, etc.
        loadingText.SetActive(false);
        background.GetComponent<CanvasGroup>().alpha = 0;
        soundtrack.Stop();
    }


    // Used to host servers without actually adding a player
    public void OnServerOnly()
    {
        networkMenuElementsCanvasGroup.alpha = 0;
        networkMenuElementsCanvasGroup.interactable = false;
        loadingText.SetActive(true); // in order for this text to show before world load, would need to change scene before loading next scene with world (like Setup Menu for Splitscreen)

        buttonSound.Play();
        FileSystemExtension.SaveSettings();

        if (!NetworkClient.active)
        {
            // Server Only
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                // cant be a server in webgl build
                connectionStatus.text = "(  WebGL cannot be server  )";
            }
            else
            {
                manager.StartServer();
                manager.networkAddress = networkAddressInputField.text;
            }
        }
        else
        {
            // Connecting
            connectionStatus.text = "Connecting to " + manager.networkAddress + "..";
        }

        StatusLabels();
        gameManager.Setup();
        loadingText.SetActive(false);
        background.GetComponent<CanvasGroup>().alpha = 0;
        soundtrack.Stop();
    }

    public void OnChangeNetworkAddress()
    {
        manager.networkAddress = networkAddressInputField.text;
        SettingsStatic.LoadedSettings.ipAddress = networkAddressInputField.text;
    }

    void StatusLabels()
    {
        // host mode
        // display separately because this always confused people:
        //   Server: ...
        //   Client: ...
        if (NetworkServer.active && NetworkClient.active)
        {
            connectionStatus.text = "Host " + manager.networkAddress + " running via " + Transport.activeTransport;
        }
        // server only
        else if (NetworkServer.active)
        {
            connectionStatus.text = "Server " + manager.networkAddress + " running via " + Transport.activeTransport;
        }
        // client only
        else if (NetworkClient.isConnected)
        {
            connectionStatus.text = "Client connected to " + manager.networkAddress + " via " + Transport.activeTransport;
        }
    }
}
