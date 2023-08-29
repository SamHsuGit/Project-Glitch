using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class SetupMenu : MonoBehaviour
{
    public GameObject menuElements;
    public Slider loadingSlider;
    public TextMeshProUGUI loadingPercentageText;
    public TMP_InputField playerNameInputField;
    public GameObject modelsObjectToSpin;
    public GameObject levelLoaderObject;
    
    public Slider worldRenderDistanceSlider;
    public TextMeshProUGUI worldRenderText;

    public AudioSource buttonSound;

    public int index;

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
    }

    public void SetRenderDistance()
    {
        //worldRenderText.text = worldRenderDistanceSlider.value.ToString();
    }

    public void Local()
    {
        buttonSound.Play();
        menuElements.SetActive(false);
        SaveSettings();

        Settings.OnlinePlay = false;

        // play loading animation?
        loadingSlider.gameObject.SetActive(true);

        //if (Settings.Platform == 2)
        //{
        //    SceneManager.LoadScene(5); // mobile VR loads smaller scene
        //    //levelLoader.LoadLevel(5, loadingSlider, loadingPercentageText); // doesn't work since most of level loading is done by world after scene is loaded
        //}
        //else
        //{
            SceneManager.LoadScene(1);
            //levelLoader.LoadLevel(3, loadingSlider, loadingPercentageText); // doesn't work since most of level loading is done by world after scene is loaded
        //}
    }

    public void Online()
    {
        buttonSound.Play();
        menuElements.SetActive(false);
        SaveSettings();

        Settings.OnlinePlay = true;

        //if (Settings.Platform == 2)
        //{
        //    SceneManager.LoadScene(5); // mobile VR loads smaller scene
        //    //levelLoader.LoadLevel(5, loadingSlider, loadingPercentageText); // doesn't work since most of level loading is done by world after scene is loaded
        //}
        //else
        //{
            SceneManager.LoadScene(1);
            //levelLoader.LoadLevel(3, loadingSlider, loadingPercentageText); // doesn't work since most of level loading is done by world after scene is loaded
        //}
    }

    public void Back()
    {
        buttonSound.Play();
        SaveSettings();
        SceneManager.LoadScene(0);
    }

    public void SaveSettings()
    {
        SettingsStatic.LoadedSettings.playerName = playerNameInputField.text;

        // Save setttings when this function is called, otherwise settings will load from latest settings file upon game start
        FileSystemExtension.SaveSettings();
    }
}
