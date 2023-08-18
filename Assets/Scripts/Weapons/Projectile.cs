using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    public int collisionCount;
    public int collisionCountThreshold;
    public AudioSource ricochet;
    public AudioSource explosion;
    private ParticleSystem particle_Sys;

    //private bool exploded = false;
    private float grenadeFuseTime = 4f;
    public bool isGrenade = false;

    private void Awake()
    {
        particle_Sys = GetComponent<ParticleSystem>();
        //if (isGrenade)
        //    GetComponent<PickupObject>().enabled = false;
    }

    private void Start()
    {
        Invoke("Explode", grenadeFuseTime); // timer for grenades
    }

    private void OnParticleCollision()
    {
        collisionCount++;
        if (ricochet != null)
            ricochet.Play();

        //if (collision.gameObject.tag == "Player") // collides with self
        //    Explode();

        if (collisionCount > collisionCountThreshold)
            Explode();
    }

    private void Explode()
    {
        explosion.Play();
        //exploded = true; // ensures this is only called once
        //if (!exploded)
        if(isGrenade && particle_Sys != null)
            particle_Sys.Play(); // used for grenades
        Invoke("DestroyObject", grenadeFuseTime + 0.1f);
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
