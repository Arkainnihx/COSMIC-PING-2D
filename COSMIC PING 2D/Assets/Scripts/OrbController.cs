using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbController : MonoBehaviour
{

    public float volumeCoefficient;

    private Vector3 bounceDirection;
    private bool paddleBounceToResolve = false;
    private float massIncrement = 0.003f;
    private float critialMass = 10f;

    private void FixedUpdate()
    {
        if (paddleBounceToResolve)
        {
            float speed = GetComponent<Rigidbody>().velocity.magnitude;
            GetComponent<Rigidbody>().velocity = speed * bounceDirection;
            paddleBounceToResolve = false;
        }
        if (GetComponent<Rigidbody>().mass > critialMass)
        {
            StartCoroutine(GoingCritialMass());
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Orb":
                GetComponentInParent<OrbSimulationController>().HandleHalfAnOrbCollision(gameObject, collision.gameObject);
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

    public void SetProportionalValuesUsingMass(float mass)
    {
        GetComponent<Rigidbody>().mass = mass;
        transform.localScale = volumeCoefficient * Mathf.Pow(mass, 1f / 3f) * Vector3.one;

        GetComponentInChildren<Light>().range = 3 + (10 * mass);
        GetComponentInChildren<Light>().intensity = 1 + (4 * mass);
    }

    IEnumerator GoingCritialMass()
    {
        yield return new WaitForSeconds(3f);
        Explode();
    }

    void Explode()
    {
        GetComponentInParent<OrbSimulationController>().ApplyExplosionForceToOrbs(gameObject);
    }
}
