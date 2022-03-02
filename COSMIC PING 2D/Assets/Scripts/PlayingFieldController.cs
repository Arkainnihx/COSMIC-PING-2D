using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingFieldController : MonoBehaviour
{
    public GameObject paddlePrefab;
    public GameObject orbPrefab;
    public int totalPlayers = 2;
    public float gravityCoefficient = 10f;

    private GameObject[] paddleArray;
    private List<GameObject> orbList;
    // Start is called before the first frame update
    void Start()
    {
        orbList = new List<GameObject>();
        if (totalPlayers == 2)
        {
            SetUpTwoPlayerGame();
        }
        //SetUpOrbGravityTest();
    }

    void SetUpTwoPlayerGame()
    {
        // Create and position two paddles, assign player IDs
        paddleArray = new GameObject[2];
        paddleArray[0] = Instantiate(paddlePrefab, new Vector3(15f, 0f), Quaternion.Euler(0f, -90f, 0f));
        paddleArray[0].GetComponent<PaddleController>().playerID = 1;
        paddleArray[1] = Instantiate(paddlePrefab, new Vector3(-15f, 0f), Quaternion.Euler(180f, -90f, 0f));
        paddleArray[1].GetComponent<PaddleController>().playerID = 2;
        // Create initial energy orb
        CreateOrb(0.5f, Vector3.zero, 5 * Random.insideUnitCircle);
    }
    void SetUpOrbGravityTest()
    {
        CreateOrb(Random.Range(0.01f, 2f), new Vector3(6, 6), Random.insideUnitCircle);
        CreateOrb(Random.Range(0.01f, 2f), new Vector3(6, -6), Random.insideUnitCircle);
        CreateOrb(Random.Range(0.01f, 2f), new Vector3(-6, 6), Random.insideUnitCircle);
        CreateOrb(Random.Range(0.01f, 2f), new Vector3(-6, -6), Random.insideUnitCircle);
    }

    // Update is called once per frame
    void Update()
    {
        if (orbList.Count > 1)
        {
            ApplyGraviticForcesToOrbs();
        }
    }

    void ApplyGraviticForcesToOrbs()
    {
        foreach (GameObject orbToApplyForceTo in orbList)
        {
            Vector3 forceToApply = Vector3.zero;
            foreach (GameObject otherOrb in orbList)
            {
                if (otherOrb == orbToApplyForceTo) continue;
                forceToApply += (gravityCoefficient * orbToApplyForceTo.GetComponent<Rigidbody>().mass * otherOrb.GetComponent<Rigidbody>().mass * (otherOrb.transform.position - orbToApplyForceTo.transform.position)) / Mathf.Pow(Vector3.Distance(otherOrb.transform.position, orbToApplyForceTo.transform.position), 3);
            }
            orbToApplyForceTo.GetComponent<Rigidbody>().AddForce(forceToApply);
        }
    }

    void CreateOrb(float mass, Vector3 position, Vector3 velocity)
    {
        GameObject orb = Instantiate(orbPrefab, position, Quaternion.identity);
        orb.GetComponent<OrbController>().SetScaleAndMassUsingMass(mass);
        orb.GetComponent<Rigidbody>().velocity = velocity;
        orbList.Add(orb);

    }

    void CreateOrb(float mass, Vector3 position)
    {
        CreateOrb(mass, position, Vector3.zero);
    }

    void CreateOrb(float mass)
    {
        CreateOrb(mass, Vector3.zero);
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}
