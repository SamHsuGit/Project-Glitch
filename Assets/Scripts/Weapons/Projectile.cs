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
    public ParticleSystem particleSystem;

    private bool exploded = false;
    private float grenadeFuseTime = 4f;
    public bool isGrenade = false;

    private void OnCollisionEnter(Collision collision)
    {
        collisionCount++;
        if (ricochet != null)
            ricochet.Play();

        //if (collision.gameObject.tag == "Player") // collides with self
        //    Explode();

        if (collisionCount > collisionCountThreshold)
            Explode();
    }

    private void Start()
    {
        Invoke("Explode", grenadeFuseTime); // timer for grenades
    }

    private void Explode()
    {
        explosion.Play();
        exploded = true; // ensures this is only called once
        //if (!exploded)
        if(isGrenade && particleSystem != null)
            particleSystem.Play();
        Invoke("DestroyObject", 5f);
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
