using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem.UI;

public class MainMenu : MonoBehaviour
{
    
    public GameObject setupMenu;
    public AudioSource buttonSound;
    public Transform cameraPivotPoint;
    public TMP_Text versionText;
    public MultiplayerEventSystem mainMenuMultiplayerEventSystem;
    public MultiplayerEventSystem setupMenuMultiplayerEventSystem;

    private void Awake()
    {
        if (Settings.Platform == 2)
            Settings.AppSaveDataPath = Application.dataPath;
        else
            Settings.AppSaveDataPath = Application.persistentDataPath;

        SettingsStatic.LoadedSettings = SettingsStatic.LoadSettings();
        versionText.text = Application.version;
        Screen.fullScreen = SettingsStatic.LoadedSettings.fullscreen;
    }

    private void OnGUI()
    {
        
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    public void Play()
    {
        buttonSound.Play();
        //SceneManager.LoadScene(2);

        setupMenu.SetActive(true);
        setupMenuMultiplayerEventSystem.enabled = true;
        mainMenuMultiplayerEventSystem.enabled = false;
        gameObject.SetActive(false);
    }

    public void Quit()
    {
        buttonSound.Play();
        Application.Quit();
    }

    public void Credits()
    {
        buttonSound.Play();
        SceneManager.LoadScene(1);
    }

    private void Update()
    {
        cameraPivotPoint.Rotate(new Vector3(0, -Mathf.Deg2Rad * 1, 0));
    }
}
