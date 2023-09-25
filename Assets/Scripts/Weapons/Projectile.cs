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
    public SphereCollider explosionRadiusSphereCollider;

    public float grenadeFuseTime = 2f;
    public float explosionLength = 2.01f;
    public bool isGrenade = false;
    public bool exploded = false;

    public List<GameObject> damageObs;

    private MeshRenderer mr;

    private void Awake()
    {
        particle_Sys = GetComponent<ParticleSystem>();
        if(isGrenade && GetComponent<MeshRenderer>() != null)
            mr = GetComponent<MeshRenderer>();
        exploded = false;
    }

    private void Start()
    {
        Invoke("Explode", grenadeFuseTime); // timer for grenades
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!isGrenade || !exploded || collider.gameObject == null)
            return;

        GameObject ob = collider.gameObject;

        if (ob.layer == 11 || ob.tag == "Player" && ob.GetComponent<Health>() != null)
        {
            damageObs.Add(ob);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (!isGrenade || !exploded || collider.gameObject == null)
            return;

        GameObject ob = collider.gameObject;

        if (ob.layer == 11 || ob.tag == "Player" && ob.GetComponent<Health>() != null)
        {
            damageObs.Remove(ob);
        }
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

        exploded = true;

        Invoke("DestroyObject", explosionLength);

        foreach (GameObject ob in damageObs)
        {
            Health health = ob.GetComponent<Health>();
            if (Settings.OnlinePlay)
                health.CmdEditSelfHealth(-damage);
            else
                health.EditSelfHealth(-damage);
        }
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
