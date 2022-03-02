using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbController : MonoBehaviour
{
    public float volumeCoefficient = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Orb":

                break;
            case "Player":

                break;
        }
    }

    public void SetScaleAndMassUsingScale(float scale)
    {
        transform.localScale = volumeCoefficient * scale * Vector3.one;
        GetComponent<Rigidbody>().mass = Mathf.Pow(scale, 3f);
    }

    public void SetScaleAndMassUsingMass(float mass)
    {
        GetComponent<Rigidbody>().mass = mass;
        transform.localScale = volumeCoefficient * Mathf.Pow(mass, 1f / 3f) * Vector3.one;
    }
}
