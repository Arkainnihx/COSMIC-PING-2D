using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbSimulationController : MonoBehaviour
{
    public GameObject orbPrefab;
    public float gravityCoefficient;
    public float explosionForceCoefficient;
    public List<GameObject> orbList;
    public List<(GameObject thisOrb, GameObject thatOrb)> orbCollisionList;

    private float orbSpawnInterval = 1f;

    // Start is called before the first frame update
    void Start()
    {
        orbList = new List<GameObject>();
        orbCollisionList = new List<(GameObject, GameObject)>();
    }

    void FixedUpdate()
    {
        ResolveOrbCollisions();
        ApplyGraviticForcesToOrbs();
    }

    void ResolveOrbCollisions()
    {
        if (orbCollisionList.Count > 1)
        {
            for (int collisionIndex = orbCollisionList.Count - 1; collisionIndex >= 0; collisionIndex--)
            {
                orbCollisionList.RemoveAt(collisionIndex);
                CombineOrbs(orbCollisionList[collisionIndex].thisOrb, orbCollisionList[collisionIndex].thatOrb);
            }
        }
    }

    void CombineOrbs(GameObject orb1, GameObject orb2)
    {
        Rigidbody rb1 = orb1.GetComponent<Rigidbody>();
        Rigidbody rb2 = orb2.GetComponent<Rigidbody>();
        float newMass = rb1.mass + rb2.mass;
        Vector3 momentum = rb1.mass * rb1.velocity + rb2.mass * rb2.velocity;
        Vector3 newVelocity = momentum / newMass;
        if (rb1.mass > rb2.mass)
        {
            orb1.GetComponent<OrbController>().SetProportionalValuesUsingMass(newMass);
            rb1.velocity = newVelocity;
            orbList.Remove(orb2);
            Destroy(orb2);
        }
        else
        {
            orb2.GetComponent<OrbController>().SetProportionalValuesUsingMass(newMass);
            rb2.velocity = newVelocity;
            orbList.Remove(orb1);
            Destroy(orb1);
        }
    }

    void ApplyGraviticForcesToOrbs()
    {
        if (orbList.Count > 1)
        {
            for (int orbIndex = orbList.Count - 1; orbIndex >= 0; orbIndex--)
            {
                if (orbList[orbIndex].GetComponent<OrbController>().toExplode)
                {
                    HandleOrbExplosion(orbList[orbIndex]);
                    continue;
                }
                Vector3 forceToApply = Vector3.zero;
                foreach (GameObject otherOrb in orbList)
                {
                    if (otherOrb == orbList[orbIndex]) continue;
                    Rigidbody rb1 = orbList[orbIndex].GetComponent<Rigidbody>();
                    Rigidbody rb2 = otherOrb.GetComponent<Rigidbody>();
                    Vector3 position1 = orbList[orbIndex].transform.position;
                    Vector3 position2 = otherOrb.transform.position;
                    forceToApply += (gravityCoefficient * rb1.mass * rb2.mass * (position2 - position1)) / Mathf.Pow(Vector3.Distance(position2, position1), 3);
                }
                orbList[orbIndex].GetComponent<Rigidbody>().AddForce(forceToApply);
            }
        }
    }

    void HandleOrbExplosion(GameObject explodingOrb)
    {
        float mass = explodingOrb.GetComponent<Rigidbody>().mass;
        Vector3 position = explodingOrb.transform.position;
        orbList.Remove(explodingOrb);
        Destroy(explodingOrb);
        foreach (GameObject orb in orbList)
        {
            orb.GetComponent<Rigidbody>().AddExplosionForce(explosionForceCoefficient * Mathf.Pow(mass, 2f), position, mass * 8f);
        }
    }

    public void ResetOrbSimulation()
    {
        StopAllCoroutines();
        orbCollisionList.Clear();
        foreach (GameObject orb in orbList)
        {
            Destroy(orb);
        }
        orbList.Clear();
    }

    public IEnumerator SmoothCreateOrb(float mass, Vector3 position, Vector3 velocity)
    {
        float startingMass = 0.01f;
        GameObject orb = CreateOrb(startingMass, position, velocity);
        while (orb != null && orb.GetComponent<Rigidbody>().mass < mass)
        {
            orb.GetComponent<OrbController>().IncrementMass();
            yield return null;
        }
    }

    public GameObject CreateOrb(float mass, Vector3 position, Vector3 velocity)
    {
        GameObject orb = Instantiate(orbPrefab, position, Quaternion.identity, transform);
        orb.GetComponent<OrbController>().SetProportionalValuesUsingMass(mass);
        orb.GetComponent<Rigidbody>().velocity = velocity;
        orbList.Add(orb);
        return orb;
    }

    public GameObject CreateOrb(float mass, Vector3 position)
    {
        return CreateOrb(mass, position, Vector3.zero);
    }

    public GameObject CreateOrb(float mass)
    {
        return CreateOrb(mass, Vector3.zero);
    }

    public void RegisterHalfCollision(GameObject thisOrb, GameObject thatOrb)
    {
        bool alreadyRegistered = false;
        foreach ((GameObject thisOrb, GameObject thatOrb) orbCollision in orbCollisionList)
        {
            if (thisOrb == orbCollision.thisOrb && thatOrb == orbCollision.thisOrb)
            {
                alreadyRegistered = true;
                break;
            }
        }
        if (!alreadyRegistered)
        {
            orbCollisionList.Add((thisOrb, thatOrb));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If an orb exits the Playing Field trigger collider, destroy it and, depending on which side it left, deal damage to the appropriate player
        if (other.gameObject.tag == "Orb")
        {
            if (StateController.currentState == State.PlayingMatch)
            {
                if (other.transform.position.x > 0f)
                {
                    GetComponent<MatchController>().paddleArray[0].GetComponent<PaddleController>().TakeDamage(other.attachedRigidbody.mass, other.attachedRigidbody.velocity.magnitude);
                }
                else
                {
                    GetComponent<MatchController>().paddleArray[1].GetComponent<PaddleController>().TakeDamage(other.attachedRigidbody.mass, other.attachedRigidbody.velocity.magnitude);
                }
            }
            orbList.Remove(other.gameObject);
            Destroy(other.gameObject);
        }
    }

    // Alternative game mode, regularly spawns random orbs
    public void ChaosMode()
    {
        float positionRange = 5f;
        float velocityCoefficient = 3f;
        float maxMass = 0.8f;
        StartCoroutine(CreateRandomOrbs(positionRange, velocityCoefficient, maxMass));
    }

    IEnumerator CreateRandomOrbs(float positionRange, float velocityCoefficient, float maxMass)
    {
        while (StateController.currentState == State.PlayingMatch)
        {
            yield return new WaitForSeconds(orbSpawnInterval);
            StartCoroutine(SmoothCreateOrb(Random.Range(0.1f, maxMass), positionRange * Random.insideUnitCircle, velocityCoefficient * Random.insideUnitCircle));
        }
    }

}
