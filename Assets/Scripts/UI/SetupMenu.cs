using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using UnityEngine.InputSystem.UI;

public class SetupMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public Transform cameraPivotPoint;
    public Slider loadingSlider;
    public TextMeshProUGUI loadingPercentageText;
    public TMP_InputField playerNameInputField;
    public GameObject modelsObjectToSpin;
    public GameObject levelLoaderObject;
    public GameObject playButton;
    public AudioSource buttonSound;

    public int index;

    public MultiplayerEventSystem multiplayerEventSystem;

    private LevelLoader levelLoader;

    private void Awake()
    {
        // import this player's char model as a preview before entering the game
        SettingsStatic.LoadedSettings = SettingsStatic.LoadSettings();

        levelLoader = levelLoaderObject.GetComponent<LevelLoader>();
    }

    private void Start()
    {
        //reset framerate of game for video animation
        Application.targetFrameRate = 60;
        
        playerNameInputField.text = SettingsStatic.LoadedSettings.playerName;
        loadingSlider.gameObject.SetActive(false);
    }

    private void Update()
    {
        //modelsObjectToSpin.transform.Rotate(new Vector3(0, 1, 0));
        cameraPivotPoint.Rotate(new Vector3(0, -Mathf.Deg2Rad * 1, 0));
    }

    public void Local()
    {
        buttonSound.Play();
        SaveSettings();

        Settings.OnlinePlay = false;

        // play loading animation?
        loadingSlider.gameObject.SetActive(true);

        //if (Settings.Platform == 2)
        //{
        //    SceneManager.LoadScene(5); // mobile VR loads smaller scene
        //}
        //else
        //{
            SceneManager.LoadScene(2);
        //}
    }

    public void Online()
    {
        buttonSound.Play();
        SaveSettings();

        Settings.OnlinePlay = true;

        //if (Settings.Platform == 2)
        //{
        //    SceneManager.LoadScene(5); // mobile VR loads smaller scene
        //}
        //else
        //{
            SceneManager.LoadScene(2);
        //}
    }

    public void Back()
    {
        buttonSound.Play();
        SaveSettings();
        //SceneManager.LoadScene(0);

        multiplayerEventSystem.SetSelectedGameObject(playButton);
        mainMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    public void SaveSettings()
    {
        SettingsStatic.LoadedSettings.playerName = playerNameInputField.text;

        // Save setttings when this function is called, otherwise settings will load from latest settings file upon game start
        FileSystemExtension.SaveSettings();
    }
}
