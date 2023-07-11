using UnityEngine;

[CreateAssetMenu(fileName = "WeaponPrimary", menuName = "MetalArms/WeaponPrimary")]
public class WeaponPrimary : ScriptableObject
{
    [Header("Weapon Attributes")]
    public int damage;
    public int splashDamage;
    public int fireRate;
    public int roundsPerFire;
    public int clipSize;
    public int maxAmmo;
    public AudioClip shootSound;
    public AudioClip clipEjectSound;
    public AudioClip clipInsertSound;
    public AudioClip reloadSound;
    public AudioClip chargeSound;
    public AudioClip lockOn;
    public AudioClip controlOn;
    public AudioClip lostControl;
    public AudioClip toasterFireLoopStop;
    public AudioClip hitSound;
    public GameObject projectile;
}