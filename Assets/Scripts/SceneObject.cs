using System.Collections;
using UnityEngine;
using Mirror;

public class SceneObject : NetworkBehaviour
{
    [SyncVar(hook = nameof(SetTool))] public int typeTool;
    [SyncVar(hook = nameof(SetProjectileInt))] public int typeProjectile;

    public GameObject[] voxel;
    public GameObject[] tool;
    public GameObject[] projectile;
    public Controller controller;

    void SetTool(int oldValue, int newValue)
    {
        StartCoroutine(ChangeEquipment(1, newValue));
    }

    void SetProjectileInt(int oldValue, int newValue)
    {
        StartCoroutine(ChangeEquipment(2, newValue));
    }

    // Since Destroy is delayed to the end of the current frame, we use a coroutine
    // to clear out any child objects before instantiating the new one
    IEnumerator ChangeEquipment(int type, int newEquippedItem)
    {
        while (transform.childCount > 0)
        {
            Destroy(transform.GetChild(0).gameObject);
            yield return null;
        }
        // Use the new value, not the SyncVar property value
        SetEquippedItem(type, newEquippedItem);
    }
    // SetEquippedItem is called on the client from OnChangeEquipment (above),
    // and on the server from CmdDropItem in the PlayerEquip script.
    public void SetEquippedItem(int type, int typeItem)
    {
        GameObject[] array = voxel;
        switch (type)
        {
            case 0:
                {
                    array = tool; // not currently used
                    break;
                }
            case 1:
                {
                    array = projectile;
                    break;
                }
        }
        
        GameObject ob = Instantiate(array[typeItem], transform.position, Quaternion.identity);

        // manually remove any unwanted -submodel objects (messy, need to improve by preventing submodel from spawning in first place)
        if (Settings.OnlinePlay)
        {
            foreach (Transform child in ob.transform)
                if (child.name.Contains("-submodel"))
                    Destroy(child.gameObject);
        }

        if (ob.GetComponent<BoxCollider>() != null)
            ob.GetComponent<BoxCollider>().enabled = true;
        ob.SetActive(true);
        if (type == 3 && ob.transform.localScale != new Vector3(2.5f, 2.5f, 2.5f)) // adjust scale for voxelBits
            ob.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
        ob.transform.rotation = Quaternion.LookRotation(transform.forward); // orient forwards in direction of camera
        ob.transform.parent = transform;
    }
}
