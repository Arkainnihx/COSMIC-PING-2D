using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbController : MonoBehaviour
{

    public float volumeCoefficient;

    private Vector3 bounceDirection;
    private bool paddleBounceToResolve = false;
    private float massIncrement = 0.001f;

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
        if (paddleBounceToResolve)
        {
            float speed = GetComponent<Rigidbody>().velocity.magnitude;
            GetComponent<Rigidbody>().velocity = speed * bounceDirection;
            paddleBounceToResolve = false;
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
                bounceDirection = Vector3.Normalize(transform.position - collision.transform.Find("Bounce Angle Point").position);
                paddleBounceToResolve = true;
                break;
        }
    }

    public void IncrementMass()
    {
        SetScaleAndMassUsingMass(gameObject.GetComponent<Rigidbody>().mass + massIncrement);
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
