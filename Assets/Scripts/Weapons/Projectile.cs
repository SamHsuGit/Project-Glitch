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

    private void OnCollisionEnter(Collision collision)
    {
        collisionCount++;
        if (ricochet != null)
            ricochet.Play();

        if (collisionCount > collisionCountThreshold)
        {
            if(explosion != null)
                explosion.Play();
            Destroy(this.gameObject);
        }
    }
}
