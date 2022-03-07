using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbController : MonoBehaviour
{

    public float volumeCoefficient;
    public float bounceCoefficient;

    private Vector3 bounceDirection;
    private bool bounceToResolve = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (bounceToResolve)
        {
            float speed = GetComponent<Rigidbody>().velocity.magnitude;
            GetComponent<Rigidbody>().velocity = speed * bounceDirection;
            bounceToResolve = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Orb":
                GetComponentInParent<PlayingFieldController>().HandleHalfAnOrbCollision(gameObject, collision.gameObject);
                break;
            case "Player":
                bounceDirection = Vector3.Normalize(transform.position - collision.transform.Find("BounceAnglePoint").position);
                bounceToResolve = true;
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
