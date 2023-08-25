using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    public int collisionCount;
    public int collisionCountThreshold;
    public AudioSource audioSourcePlayer;
    public AudioClip ricochet;
    public AudioClip explosion;
    private ParticleSystem particle_Sys;

    private float grenadeFuseTime = 4f;
    public bool isGrenade = false;

    private void Awake()
    {
        particle_Sys = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        Invoke("Explode", grenadeFuseTime); // timer for grenades
    }

    private void OnParticleCollision()
    {
        collisionCount++;
        if (ricochet != null)
            audioSourcePlayer.PlayOneShot(ricochet);

        if (isGrenade)
        {
            if (collisionCount > collisionCountThreshold)
                Explode();
        }
    }

    private void Explode()
    {
        if(explosion != null)
            audioSourcePlayer.PlayOneShot(explosion);
        if(isGrenade && particle_Sys != null)
            particle_Sys.Play(); // used for grenades
        Invoke("DestroyObject", grenadeFuseTime + 0.1f);
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
