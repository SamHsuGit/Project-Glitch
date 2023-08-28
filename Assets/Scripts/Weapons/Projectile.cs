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

    public float grenadeFuseTime = 2f;
    public float explosionLength = 2.01f;
    public bool isGrenade = false;

    private MeshRenderer mr;

    private void Awake()
    {
        particle_Sys = GetComponent<ParticleSystem>();
        if(isGrenade && GetComponent<MeshRenderer>() != null)
            mr = GetComponent<MeshRenderer>();
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
        if(isGrenade && mr != null)
            mr.enabled = false;

        if(explosion != null)
            audioSourcePlayer.PlayOneShot(explosion);

        if(isGrenade && particle_Sys != null)
            particle_Sys.Play(); // used for grenades

        Invoke("DestroyObject", explosionLength);
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
