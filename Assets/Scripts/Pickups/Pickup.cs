using UnityEngine;

[CreateAssetMenu(fileName = "Pickup", menuName = "MetalArms/Pickup")]
public class Pickup : ScriptableObject
{
    [Header("Pickup Attributes")]
    public int type;
    public int index;
    public int weaponLevel;
    public int pickupQtySize;
    public bool hoverAbove;
    public AudioClip pickupSound;
    public AudioClip pickupSound2;
    public GameObject pickupGameObject;
}