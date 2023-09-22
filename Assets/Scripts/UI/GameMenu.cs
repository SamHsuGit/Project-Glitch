using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using Mirror;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class GameMenu : MonoBehaviour
{
    public Slider hpSlider;
    public Slider volumeSlider;
    public Slider lookSpeedSlider;
    public Slider lookAccelerationSlider;
    public Slider fovSlider;
    public Dropdown graphicsQualityDropdown;
    public Toggle fullScreenToggle;
    public Toggle invertYToggle;
    public GameObject player;
    public GameObject playerCamera;
    public AudioMixer masterAudioMixer;
    public GameObject GameMenuCanvas;
    public GameObject backgroundMask;
    public GameObject playerHUD;
    public GameObject optionsMenu;
    public GameObject debugText;
    public TextMeshProUGUI PlanetSeedWorldCoordText;
    public AudioSource buttonSound;
    public GameObject basicControlsText;
    public bool showControls;
    public GameObject[] PrimaryWeaponIcons;
    public GameObject[] SecondaryWeaponIcons;
    public GameObject[] wIconPrimaryGUI;
    public GameObject[] wIconSecondaryGUI;
    public TextMeshProUGUI AmmoCountPrimary;
    public TextMeshProUGUI ClipCountPrimary;
    public TextMeshProUGUI AmmoCountSecondary;
    public Slider laserAmmoSlider;
    public GameObject[] levelPrimaryWeaponIcons;
    public GameObject[] pickupsWeaponPrimary;
    public GameObject[] pickupsWeaponSecondary;
    public GameObject[] pickupsPowerUps;
    

    public GameObject currentPickupGameObject;
    public bool setNavigate = false;

    private int previousGraphicsQuality;
    private World world;
    private MotionBlur motionBlur;

    CanvasGroup backgroundMaskCanvasGroup;
    CanvasGroup playerHUDCanvasGroup;
    CanvasGroup optionsMenuCanvasGroup;
    Controller controller;
    Canvas canvas;
    Health health;
    CustomNetworkManager customNetworkManager;
    InputHandler inputHandler;

    //UnityEngine.Rendering.HighDefinition.UniversalAdditionalCameraData uac;

    private void Awake()
    {
        backgroundMaskCanvasGroup = backgroundMask.GetComponent<CanvasGroup>();
        playerHUDCanvasGroup = playerHUD.GetComponent<CanvasGroup>();
        optionsMenuCanvasGroup = optionsMenu.GetComponent<CanvasGroup>();
        controller = player.GetComponent<Controller>();
        world = controller.world;
        canvas = GetComponent<Canvas>();
        health = player.GetComponent<Health>();
        showControls = SettingsStatic.LoadedSettings.showControls;
        inputHandler = controller._inputHandler;

        MotionBlur tmpBlur;
        if (controller.gameManager.levelVolumeProfile.TryGet<MotionBlur>(out tmpBlur))
            motionBlur = tmpBlur;
    }

    private void Start()
    {
        // these must happen in start since world is not instantiated until after Awake...
        customNetworkManager = world.customNetworkManager;

        // set settings from loaded saved file
        volumeSlider.value = SettingsStatic.LoadedSettings.volume;
        lookSpeedSlider.value = SettingsStatic.LoadedSettings.lookSpeed;
        lookAccelerationSlider.value = SettingsStatic.LoadedSettings.lookAccel;
        fovSlider.value = SettingsStatic.LoadedSettings.fov;
        graphicsQualityDropdown.value = SettingsStatic.LoadedSettings.graphicsQuality;
        QualitySettings.SetQualityLevel(SettingsStatic.LoadedSettings.graphicsQuality);
        fullScreenToggle.isOn = SettingsStatic.LoadedSettings.fullscreen;
        invertYToggle.isOn = SettingsStatic.LoadedSettings.invertY;

        //uac = playerCamera.GetComponent<Camera>().GetComponent<UnityEngine.Rendering.HighDefinition.UniversalAdditionalCameraData>();

        // initialize objects when starting the game
        playerHUDCanvasGroup.alpha = 1;
        playerHUDCanvasGroup.interactable = true;
        optionsMenuCanvasGroup.alpha = 0;
        optionsMenuCanvasGroup.interactable = false;
        debugText.SetActive(false);
        basicControlsText.SetActive(false);

        UpdateHP();
        UpdateWeaponIcons();
        UpdateAmmoCounts();
    }

    public void toggleNavigate()
    {
        setNavigate = true;
    }

    private void Update()
    {
        CheckSplitscreenCanvasRenderMode();

        if (SettingsStatic.LoadedSettings.graphicsQuality == 0)
            motionBlur.active = false;
        else
            motionBlur.active = true;

        if (showControls)
            basicControlsText.SetActive(true);
        else
            basicControlsText.SetActive(false);

        UpdateWeaponIcons();
        UpdateAmmoCounts();
        UpdateHP();

        if (currentPickupGameObject != null)
            currentPickupGameObject.transform.Rotate(new Vector3(0, Mathf.Deg2Rad * 100, 0));

        if (optionsMenuCanvasGroup.alpha != 1 && controller.switchPrimary && (setNavigate || inputHandler.scrollWheel != Vector2.zero))
        {
            int currentWeaponPrimaryIndex = controller.currentWeaponPrimaryIndex;
            int weaponsPrimaryCount = controller.wPrimaryModels.Length;

            if (setNavigate)
                setNavigate = false;

            if (inputHandler.navUp || inputHandler.scrollWheel.y > 0)
                currentWeaponPrimaryIndex++;

            if (inputHandler.navDown || inputHandler.scrollWheel.y < 0)
                currentWeaponPrimaryIndex--;

            // index out of bounds check
            if (currentWeaponPrimaryIndex > weaponsPrimaryCount - 1)
                currentWeaponPrimaryIndex = 0;
            if (currentWeaponPrimaryIndex < 0)
                currentWeaponPrimaryIndex = weaponsPrimaryCount - 1;

            controller.SetPrimaryWeaponIndex(currentWeaponPrimaryIndex);
        }
        else if (optionsMenuCanvasGroup.alpha != 1 && controller.switchSecondary && (setNavigate || inputHandler.scrollWheel != Vector2.zero))
        {
            int currentWeaponSecondaryIndex = controller.currentWeaponSecondaryIndex;
            int weaponsSecondaryCount = controller.wSecondaryModels.Length;

            if (setNavigate)
                setNavigate = false;

            if (inputHandler.navUp || inputHandler.scrollWheel.y > 0)
                currentWeaponSecondaryIndex++;

            if (inputHandler.navDown || inputHandler.scrollWheel.y < 0)
                currentWeaponSecondaryIndex--;

            // index out of bounds check
            if (currentWeaponSecondaryIndex > weaponsSecondaryCount - 1)
                currentWeaponSecondaryIndex = 0;
            if (currentWeaponSecondaryIndex < 0)
                currentWeaponSecondaryIndex = weaponsSecondaryCount - 1;

            controller.SetSecondaryWeaponIndex(currentWeaponSecondaryIndex);
        }
    }

    public void UpdateHP()
    {
        hpSlider.value = (float)health.hp;
    }

    public void UpdateWeaponIcons()
    {
        int w_Index_P = controller.currentWeaponPrimaryIndex;
        int w_Index_S = controller.currentWeaponSecondaryIndex;

        for (int i = 0; i < PrimaryWeaponIcons.Length; i++)
            PrimaryWeaponIcons[i].SetActive(false);
        for (int i = 0; i < SecondaryWeaponIcons.Length; i++)
            SecondaryWeaponIcons[i].SetActive(false);
        for (int i = 0; i < levelPrimaryWeaponIcons.Length; i++)
            levelPrimaryWeaponIcons[i].SetActive(false);
        PrimaryWeaponIcons[w_Index_P].SetActive(true);
        SecondaryWeaponIcons[w_Index_S].SetActive(true);
        levelPrimaryWeaponIcons[controller.wPrimaryPickupObjects[w_Index_P].level - 1].SetActive(true);

        laserAmmoSlider.gameObject.SetActive(false);
        AmmoCountPrimary.gameObject.SetActive(true);
        ClipCountPrimary.gameObject.SetActive(true);
        if (w_Index_P == 0 || w_Index_P == 1 || w_Index_P == 2) // only show laser weapon ammo counter if laser weapon equipped
        {
            AmmoCountPrimary.gameObject.SetActive(false);
            ClipCountPrimary.gameObject.SetActive(false);
            laserAmmoSlider.gameObject.SetActive(true);
        }
    }

    public void UpdateAmmoCounts()
    {
        // laser ammo counter
        if (controller.currentWeaponPrimaryIndex < 3 && controller.shoot)
            laserAmmoSlider.value = controller.wPrimaryPickupObjects[controller.currentWeaponPrimaryIndex].ammo;
        else if (controller.currentWeaponPrimaryIndex < 3)
        {
            laserAmmoSlider.value = laserAmmoSlider.maxValue;
            controller.wPrimaryPickupObjects[controller.currentWeaponPrimaryIndex].ammo = controller.wPrimaryPickupObjects[controller.currentWeaponPrimaryIndex].maxAmmo;
        }

        //primary weapon counter
        AmmoCountPrimary.text = controller.wPrimaryPickupObjects[controller.currentWeaponPrimaryIndex].ammo.ToString();
        ClipCountPrimary.text = controller.wPrimaryPickupObjects[controller.currentWeaponPrimaryIndex].ammoReserve.ToString();

        // secondary weapon counter
        AmmoCountSecondary.text = controller.wSecondaryPickupObjects[controller.currentWeaponSecondaryIndex].ammo.ToString();
    }

    public void ShowPickupItem(int type, int index)
    {
        //set all as hidden when picking up multiple items
        for (int i = 0; i < pickupsWeaponPrimary.Length; i++)
            pickupsWeaponPrimary[i].SetActive(false);
        for(int i = 0; i < pickupsWeaponSecondary.Length; i++)
            pickupsWeaponSecondary[i].SetActive(false);
        for(int i = 0; i < pickupsPowerUps.Length; i++)
            pickupsPowerUps[i].SetActive(false);

        switch (type)
        {
            case 0:
                {
                    currentPickupGameObject = pickupsWeaponPrimary[index];
                    break;
                }
            case 1:
                {
                    currentPickupGameObject = pickupsWeaponSecondary[index];
                    break;
                }
            case 2:
                {
                    currentPickupGameObject = pickupsPowerUps[index];
                    break;
                }
        }

        currentPickupGameObject.SetActive(true);
        Invoke("HidePickupItem", 3f);
    }

    private void HidePickupItem()
    {
        currentPickupGameObject.SetActive(false);
    }

    void CheckSplitscreenCanvasRenderMode()
    {
        if (world.playerCount < 3)
            canvas.renderMode = RenderMode.ScreenSpaceOverlay; // if only 1 player (+ 1 worldPlayer dummy player) then set options to render infront of other objects
        else
            canvas.renderMode = RenderMode.ScreenSpaceCamera; // if multiplayer splitscreen, need to keep UI canvas as screenspace camera so splitscreen ui can work.
    }

    public void OnOptions()
    {
        if (backgroundMaskCanvasGroup.alpha == 1) // if menu already shown
            return;
        SettingsStatic.LoadedSettings = SettingsStatic.LoadSettings();

        //uac.renderPostProcessing = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        backgroundMaskCanvasGroup.alpha = 1;
        playerHUDCanvasGroup.alpha = 0;
        playerHUDCanvasGroup.interactable = false;
        optionsMenuCanvasGroup.alpha = 1;
        optionsMenuCanvasGroup.interactable = true;
    }

    public void ReturnToGame()
    {
        if (backgroundMaskCanvasGroup.alpha == 0) // if menu already disabled
            return;
        buttonSound.Play();

        //uac.renderPostProcessing = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        backgroundMaskCanvasGroup.alpha = 0;
        playerHUDCanvasGroup.alpha = 1;
        playerHUDCanvasGroup.interactable = true;
        optionsMenuCanvasGroup.alpha = 0;
        optionsMenuCanvasGroup.interactable = false;

        SaveSettings(); // save settings when exiting menu
    }

    public void SaveSettings()
    {
        // Save setttings when this function is called, otherwise settings will load from latest settings file upon game start
        SettingsStatic.LoadedSettings.showControls = showControls;

        FileSystemExtension.SaveSettings();
        SettingsStatic.LoadSettings();
    }

    public void Quit()
    {
        buttonSound.Play();
        SaveSettings(); // save settings

        // if splitscreen play and more than one player and the first player is not this player, destroy the gameObject
        if (!Settings.OnlinePlay && world.playerCount > 2 && world.players[1].playerGameObject != player.gameObject)
        {
            world.playerCount--;
            Destroy(player);
            return;
        }

        Settings.WorldLoaded = false;

        // stop host if host mode
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            customNetworkManager.StopHost();
        }
        // stop client if client-only
        else if (NetworkClient.isConnected)
        {
            customNetworkManager.StopClient();
        }
        // stop server if server-only
        else if (NetworkServer.active)
        {
            customNetworkManager.StopServer();
        }

        SceneManager.LoadScene(0);
    }

    public void SetVolume(float value)
    {
        SettingsStatic.LoadedSettings.volume = value;
        masterAudioMixer.SetFloat("masterVolume", Mathf.Log10(value) * 20); // uses log base 10 for log scale of master mixer volume (decibels)
    }

    public void SetLookSensitivity(float value)
    {
        SettingsStatic.LoadedSettings.lookSpeed = value;
    }

    public void SetLookAccel(float value)
    {
        SettingsStatic.LoadedSettings.lookAccel = value;
    }

    public void SetFoV(float value)
    {
        SettingsStatic.LoadedSettings.fov = value;
        playerCamera.GetComponent<Camera>().fieldOfView = value;
    }

    public void SetGraphicsQuality(int value)
    {
        previousGraphicsQuality = SettingsStatic.LoadedSettings.graphicsQuality;
        SettingsStatic.LoadedSettings.graphicsQuality = value;
        QualitySettings.SetQualityLevel(value, true);
    }

    public void SetFullScreen (bool value)
    {
        buttonSound.Play();
        Screen.fullScreen = value;
        SettingsStatic.LoadedSettings.fullscreen = value;
    }

    public void SetInvertY (bool value)
    {
        buttonSound.Play();
        SettingsStatic.LoadedSettings.invertY = value;
    }

    public void ToggleControls()
    {
        showControls = !showControls;
    }

    public void ToggleDebug()
    {
        debugText.SetActive(!debugText.activeSelf);
    }
}