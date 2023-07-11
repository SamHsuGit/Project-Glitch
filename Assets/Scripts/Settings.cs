using UnityEngine;
using System.IO;

// a static class that can be accesssed from anywhere
public static class SettingsStatic
{
    private static Settings loadedSettings;

    // a static variable called LoadedSettings that can be accessed from anywhere
    public static Settings LoadedSettings
    {
        get { return loadedSettings; }
        set { loadedSettings = value; }
    }

    // public static function to load settings
    public static Settings LoadSettings()
    {
        Settings settings = new Settings();

        // performance
        settings.graphicsQuality = 0;

        // customization
        settings.playerName = "PlayerName";
        settings.ipAddress = "localhost";
        settings.volume = 0.5f;
        settings.lookSpeed = 0.1f;
        settings.lookAccel = 0.1f;
        settings.fov = 90f;
        settings.invertY = false;
        settings.fullscreen = true;
        settings.showControls = true;

        string path;
        if (Settings.Platform == 2)
            path = Settings.AppSaveDataPath + "/settings.cfg";
        else
            path = Application.streamingAssetsPath + "/settings.cfg";
        if (File.Exists(path))
        {
            string JsonImport = File.ReadAllText(path);
            settings = JsonUtility.FromJson<Settings>(JsonImport);
        }

        return settings;
    }
}

[System.Serializable]
public class Settings
{
    // private static variables
    private static bool _worldLoaded = true; // set to false to prevent players from moving or opening menus upon world load
    private static bool _networkPlay = false;
    private static string _appPath;

    // performance
    public int graphicsQuality;

    // customization
    public string playerName;
    public string ipAddress;
    [Range(0.0001f, 1f)] public float volume;
    [Range(0.001f, 10f)] public float lookSpeed;
    public float lookAccel;
    public float fov;
    public bool invertY;
    public bool fullscreen;
    public bool showControls;


    public static bool WorldLoaded
    {
        get { return _worldLoaded; }
        set { _worldLoaded = value; }
    }

    public static bool OnlinePlay
    {
        get { return _networkPlay; }
        set {  _networkPlay = value; }
    }

    public static int Platform
    {
        // available gameplay options
        // pc singleplayer
        // pc network
        // console splitscreen
        // console network
        // mobile singleplayer
        // mobile network
        get
        {
            //return 1; // console
            //return 2; // mobile
            if (Application.isMobilePlatform)
                return 2;
            else if (Application.isConsolePlatform)
                return 1;
            else
                return 0; // then must be pc
        }
    }

    public static Vector3 DefaultSpawnPosition
    {
        // player default spawn position is centered slightly above floor
        get { return new Vector3(0, 0, 0); }
    }

    public static string AppSaveDataPath
    {
        get { return _appPath; }
        set { _appPath = value; }
    }
}

public static class FileSystemExtension
{
    public static void SaveSettings()
    {
        string path;
        if (Settings.Platform == 2)
            path = Settings.AppSaveDataPath + "/settings.cfg";
        else
            path = Application.streamingAssetsPath + "/settings.cfg";
        SaveStringToFile(JsonUtility.ToJson(SettingsStatic.LoadedSettings), path);
    }

    public static void SaveStringToFile(string jsonExport, string savePathAndFileName)
    {
        File.WriteAllText(savePathAndFileName, jsonExport);
    }
}