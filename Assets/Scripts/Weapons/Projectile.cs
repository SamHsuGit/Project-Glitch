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
    public GameObject[] particles;

    private bool exploded = false;
    private float grenadeFuseTime = 3.38f;

    private void OnCollisionEnter(Collision collision)
    {
        collisionCount++;
        if (ricochet != null)
            ricochet.Play();

        if (collisionCount > collisionCountThreshold)
            Explode();
    }

    private void Start()
    {
        Invoke("Explode", grenadeFuseTime); // timer for grenades
    }

    private void Explode()
    {
        exploded = true; // ensures this is only called once

        if (!exploded)
        {
            if(explosion != null)
            explosion.Play();

            for(int i = 0; i < particles.Length; i++)
            {
                GameObject ob = Instantiate(particles[i], transform.position, Quaternion.identity);
                Rigidbody rb = ob.GetComponent<Rigidbody>();
                rb.velocity = Vector3.up * 3f;
            }

            Destroy(this.gameObject);
        }
    }
}
