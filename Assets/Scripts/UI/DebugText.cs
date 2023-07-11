using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugText : MonoBehaviour
{
    public GameObject player;

    TextMeshProUGUI text;
    string oldDebugText;

    float frameRate;
    float timer;
    
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (player == null)
            return;

        string debugText = "Digital Bricks v" + Application.version;
        debugText += "\n";
        debugText += frameRate + " fps";
        debugText += "\n";
        debugText += "CPU: " + SystemInfo.processorType + " RAM: " + SystemInfo.systemMemorySize + " Mb  OS: " + SystemInfo.operatingSystem;
        debugText += "\n";
        debugText += "GPU: " + SystemInfo.graphicsDeviceName + " VRAM: " + SystemInfo.graphicsMemorySize + " Mb";

        if(debugText != oldDebugText)
        {
            text.text = debugText;
            oldDebugText = debugText;
        }

        if (timer > 1f)
        {

            frameRate = (int)(1f / Time.unscaledDeltaTime);
            timer = 0;

        }
        else
            timer += Time.deltaTime;
    }
}