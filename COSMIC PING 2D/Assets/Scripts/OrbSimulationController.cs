using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbSimulationController : MonoBehaviour
{
    public GameObject orbPrefab;
    public float gravityCoefficient;
    public float explosionForceCoefficient;
    public List<GameObject> orbList;

    private List<(GameObject, GameObject)> orbCollsionList;
    // Start is called before the first frame update
    void Start()
    {
        orbList = new List<GameObject>();
        orbCollsionList = new List<(GameObject, GameObject)>();
        //SetUpOrbGravityTest();
        //RunNewOrbGravityTest();
    }

    //void SetUpOrbGravityTest()
    //{
    //    float orbDistance = 5f;
    //    CreateOrb(Random.Range(0.01f, 2f), new Vector3(orbDistance, orbDistance), Random.insideUnitCircle);
    //    CreateOrb(Random.Range(0.01f, 2f), new Vector3(orbDistance, -orbDistance), Random.insideUnitCircle);
    //    CreateOrb(Random.Range(0.01f, 2f), new Vector3(-orbDistance, orbDistance), Random.insideUnitCircle);
    //    CreateOrb(Random.Range(0.01f, 2f), new Vector3(-orbDistance, -orbDistance), Random.insideUnitCircle);
    //}

    //void RunNewOrbGravityTest()
    //{
    //    float positionRange = 8f;
    //    float velocityCoefficient = 4f;
    //    float maxMass = 0.5f;
    //    StartCoroutine(CreateRandomTestOrbs(positionRange, velocityCoefficient, maxMass));
    //}

    //IEnumerator CreateRandomTestOrbs(float positionRange, float velocityCoefficient, float maxMass)
    //{
    //    float nextActionTime = 0.25f;
    //    while (Time.time < 120f)
    //    {
    //        if (Time.time > nextActionTime)
    //        {
    //            nextActionTime += 0.2f;
    //            CreateOrb(Random.Range(0.01f, maxMass), positionRange * Random.insideUnitCircle, velocityCoefficient * Random.insideUnitCircle);
    //        }
    //        yield return null;
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.B))
        {
            bool value = transform.Find("Barriers").gameObject.activeInHierarchy;
            transform.Find("Barriers").gameObject.SetActive(!value);
        }
    }

    private void FixedUpdate()
    {
        if (orbList.Count > 1)
        {
            ApplyGraviticForcesToOrbs();
        }
    }

    public void ResetOrbSimulation()
    {
        orbCollsionList.Clear();
        foreach (GameObject orb in orbList)
        {
            Destroy(orb);
        }
        orbList.Clear();
    }

    void ApplyGraviticForcesToOrbs()
    {
        foreach (GameObject orbToApplyForceTo in orbList)
        {
            Vector3 forceToApply = Vector3.zero;
            foreach (GameObject otherOrb in orbList)
            {
                if (otherOrb == orbToApplyForceTo) continue;
                Rigidbody rb1 = orbToApplyForceTo.GetComponent<Rigidbody>();
                Rigidbody rb2 = otherOrb.GetComponent<Rigidbody>();
                Vector3 position1 = orbToApplyForceTo.transform.position;
                Vector3 position2 = otherOrb.transform.position;
                forceToApply += (gravityCoefficient * rb1.mass * rb2.mass * (position2 - position1)) / Mathf.Pow(Vector3.Distance(position2, position1), 3);
            }
            orbToApplyForceTo.GetComponent<Rigidbody>().AddForce(forceToApply);
        }
    }

    public void ApplyExplosionForceToOrbs(GameObject explodingOrb)
    {
        float mass = explodingOrb.GetComponent<Rigidbody>().mass;
        Vector3 position = explodingOrb.transform.position;
        orbList.Remove(explodingOrb);
        Destroy(explodingOrb);
        foreach (GameObject orb in orbList)
        {
            orb.GetComponent<Rigidbody>().AddExplosionForce(explosionForceCoefficient * Mathf.Pow(mass, 2), position, mass * 5f);
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

    public void HandleHalfAnOrbCollision(GameObject thisOrb, GameObject thatOrb)
    {
        // If there's an unresolved collision on the list, try to resolve it with this new collision, else add this unresolved collision to the list
        if (orbCollsionList.Count % 2 == 1)
        {
            foreach ((GameObject thisOrb, GameObject thatOrb) orbCollision in orbCollsionList)
            {
                if (thisOrb == orbCollision.thatOrb && thatOrb == orbCollision.thisOrb)
                {
                    orbCollsionList.Remove(orbCollision);
                    CombineOrbs(thisOrb, thatOrb);
                }
            }
        } else
        {
            orbCollsionList.Add((thisOrb, thatOrb));
        }
    }

    void CombineOrbs(GameObject orb1, GameObject orb2)
    {
        Rigidbody rb1 = orb1.GetComponent<Rigidbody>();
        Rigidbody rb2 = orb2.GetComponent<Rigidbody>();
        float newMass = rb1.mass + rb2.mass;
        Vector3 momentum = rb1.mass * rb1.velocity + rb2.mass * rb2.velocity;
        Vector3 newVelocity = momentum / newMass;
        orb1.GetComponent<OrbController>().SetProportionalValuesUsingMass(newMass);
        rb1.velocity = newVelocity;
        orbList.Remove(orb2);
        Destroy(orb2);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Orb")
        {
            if (other.transform.position.x > 0f)
            {
                GetComponent<MatchController>().paddleArray[0].GetComponent<PaddleController>().TakeDamage(other.attachedRigidbody.mass, other.attachedRigidbody.velocity.magnitude);
            }
            else
            {
                GetComponent<MatchController>().paddleArray[1].GetComponent<PaddleController>().TakeDamage(other.attachedRigidbody.mass, other.attachedRigidbody.velocity.magnitude);
            }
            orbList.Remove(other.gameObject);
            Destroy(other.gameObject);
        }
    }

}
