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
