using UnityEngine;

public class PaddleController : MonoBehaviour
{
    public GameObject orbPrefab;
    public int playerID;
    public float movementSpeed;
    public float projectileForceCoefficient;
    public float projectileMaxMass;

    private GameObject projectileOrb;
    private float projectChargeTimeRemaining;
    private float projectileOrbFloatRange = 0.7f;
    private bool growingOrb = false;

    // Start is called before the first frame update
    void Start()
    {
        movementSpeed = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        // Controls player input based on playerID
        transform.Translate(new Vector3(0, Input.GetAxis($"P{this.playerID} Move") * movementSpeed));
        if (Input.GetAxisRaw($"P{this.playerID} Fire") == 1)
        {
            if (!growingOrb)
            {
                CreateProjectileOrb();
            }
            else
            {
                ChargeProjectileOrb();
            }
        }
        else
        {
            if (growingOrb)
            {
                ShootProjectileOrb();
            }
        }
        // Keeps paddles within play area
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -10, 10));
    }

    void CreateProjectileOrb()
    {
        projectileOrb = Instantiate(orbPrefab, transform);
        projectileOrb.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        projectileOrb.GetComponent<OrbController>().SetScaleAndMassUsingMass(0f);
        GetComponentInParent<PlayingFieldController>().orbList.Add(projectileOrb);
        projectileOrb.transform.localPosition = projectileOrbFloatRange * Vector3.forward;
        growingOrb = true;
    }

    void ChargeProjectileOrb()
    {
        if (projectileOrb.GetComponent<Rigidbody>().mass <= projectileMaxMass)
        {
            projectileOrb.GetComponent<OrbController>().IncrementMass();
        }
        projectileOrb.transform.localPosition = projectileOrbFloatRange * Vector3.forward;
    }

    void ShootProjectileOrb()
    {
        projectileOrb.transform.SetParent(transform.parent);
        projectileOrb.GetComponent<Rigidbody>().AddForce(projectileForceCoefficient * Vector3.Normalize(projectileOrb.transform.position - transform.position));
        growingOrb = false;
    }

}
