using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbController : MonoBehaviour
{

    public float volumeCoefficient;
    public bool toExplode = false;
    public static float massIncrement = 0.001f;

    private Vector3 bounceDirection;
    private bool paddleBounceToResolve = false;
    private static float critialMass = 10f;

    private void FixedUpdate()
    {
        // Controls settings the orb's new velocity after bouncing off a paddle
        if (paddleBounceToResolve)
        {
            float speed = GetComponent<Rigidbody>().velocity.magnitude;
            GetComponent<Rigidbody>().velocity = speed * bounceDirection;
            paddleBounceToResolve = false;
        }

        // If orb has reached critical mass, set to explode
        if (GetComponent<Rigidbody>().mass > critialMass) StartCoroutine(GoingCritialMass());
    }

    void OnCollisionEnter(Collision collision)
    {
        // If orb has collided with another orb, register this half of the collision with the simulation controller
        // If orb has collided with a paddle, set the direction for the bounce force to be applied along, to be resolved in FixedUpdate
        switch (collision.gameObject.tag)
        {
            case "Orb":
                GetComponentInParent<OrbSimulationController>().RegisterHalfCollision(gameObject, collision.gameObject);
                break;
            case "Player":
                bounceDirection = Vector3.Normalize(transform.position - collision.transform.Find("Bounce Angle Point").position);
                paddleBounceToResolve = true;
                break;
        }
    }

    public void IncrementMass()
    {
        SetProportionalValuesUsingMass(gameObject.GetComponent<Rigidbody>().mass + massIncrement);
    }

    // Set mass, and size & brightness relative to mass
    public void SetProportionalValuesUsingMass(float mass)
    {
        GetComponent<Rigidbody>().mass = mass;
        transform.localScale = volumeCoefficient * Mathf.Pow(mass, 1f / 3f) * Vector3.one;

        GetComponentInChildren<Light>().intensity = 0.1f + Mathf.Pow(mass, 1.5f);
    }

    IEnumerator GoingCritialMass()
    {
        GetComponentInChildren<Light>().color = Color.cyan;
        yield return new WaitForSeconds(3f);
        toExplode = true;
    }
}
