using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSecondary", menuName = "MetalArms/WeaponSecondary")]
public class WeaponSecondary : ScriptableObject
{
    [Header("Weapon Attributes")]
    public int damage;
    public int splashRadius;
    public int fireRate;
    public int roundsPerFire;
    public int clipSize;
    public int maxAmmo;
    public AudioClip shootSound;
    public AudioClip lockOn;
    public AudioClip grenadeExplodeSound;
    public AudioClip ScopeZoom;
    public AudioClip wrenchAssemble;
    public AudioClip wrenchDisassemble;
}