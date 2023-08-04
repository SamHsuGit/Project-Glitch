using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debris : MonoBehaviour
{
    bool shrink = false;
    Vector3 scaleChange = new Vector3(-0.01f, -0.01f, -0.01f);

    // Start is called before the first frame update
    void Start()
    {
        Invoke("Shink", 5); // after 5 seconds shrink and destroy this object
    }

    void Shrink()
    {
        shrink = true;
    }

    private void Update()
    {
        if (shrink)
        {
            transform.localScale += scaleChange;
            if (transform.localScale.y < 0.1f)
                Destroy(this.gameObject);
        }
    }
}
