using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaddleController : MonoBehaviour
{
    public GameObject orbPrefab;
    public int playerID;
    public Color playerColour;
    public float movementSpeed;
    public float projectileForceCoefficient;
    public float projectileForceMinimum;
    public float projectileMaxMass;
    public float fullHealth;

    private GameObject healthBar;
    private float health;

    private GameObject projectileOrb;
    private float projectileOrbFloatRange = 0.7f;
    private float cooldown = 0.5f;
    private float cooldownBound = 0.5f;
    private bool growingOrb = false;

    private Light[] lights;

    // Start is called before the first frame update
    void Start()
    {
        health = fullHealth;
        lights = lights = transform.Find("Lights").GetComponentsInChildren<Light>();
        SetLightsColour(playerColour);
        healthBar.GetComponent<Image>().color = playerColour;
    }

    void FixedUpdate()
    {
        // Controls player input based on playerID
        transform.Translate(new Vector3(0f, Input.GetAxis($"P{playerID} Move") * movementSpeed));

        // Keeps paddles within play area
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -9.5f, 9.5f));

        // Controls all stages of shooting orbs
        if (Input.GetAxisRaw($"P{playerID} Fire") == 1f)
        {
            if (cooldown >= cooldownBound)
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
        }
        else if (growingOrb)
        {
            ShootProjectileOrb();
        }

        // Decrements cooldown remaining between shots
        if (cooldown < cooldownBound)
        {
            cooldown += Time.fixedDeltaTime;
        }
    }

    public void TakeDamage(float mass, float speed)
    {
        health -= speed * Mathf.Pow(mass, 2f);
        healthBar.GetComponent<Image>().fillAmount = health / fullHealth;
        if (health <= 0f)
        {
            GetComponentInParent<MatchController>().GameOver(gameObject);
        }
    }

    void CreateProjectileOrb()
    {
        projectileOrb = GetComponentInParent<OrbSimulationController>().CreateOrb(0f);
        projectileOrb.transform.SetParent(transform);
        projectileOrb.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        projectileOrb.transform.localPosition = projectileOrbFloatRange * Vector3.forward;
        growingOrb = true;
    }

    void ChargeProjectileOrb()
    {
        if (projectileOrb != null)
        {
            if (projectileOrb.GetComponent<Rigidbody>().mass < projectileMaxMass)
            {
                projectileOrb.GetComponent<OrbController>().IncrementMass();
            }
            projectileOrb.transform.localPosition = projectileOrbFloatRange * Vector3.forward;
        }
    }

    void ShootProjectileOrb()
    {
        if (projectileOrb != null)
        {
            projectileOrb.transform.SetParent(transform.parent);
            projectileOrb.GetComponent<Rigidbody>().AddForce((projectileForceMinimum + (projectileOrb.GetComponent<Rigidbody>().mass * projectileForceCoefficient)) * Vector3.Normalize(projectileOrb.transform.position - transform.position));
            cooldownBound = 0.5f + projectileOrb.GetComponent<Rigidbody>().mass / 4;
        } else
        {
            cooldownBound = 0.5f;
        }
        cooldown = 0f;
        growingOrb = false;
    }

    public void SetHealthBar(GameObject healthBar)
    {
        this.healthBar = healthBar;
    }

    public void ResetPaddle()
    {
        gameObject.SetActive(true);
        transform.position = new Vector3(playerID == 1 ? 15f : -15f, 0f);
        ResetHealth();
        projectileOrb = null;
        cooldown = 0.5f;
        cooldownBound = 0.5f;
        growingOrb = false;
    }

    private void ResetHealth()
    {
        health = fullHealth;
        healthBar.GetComponent<Image>().fillAmount = 1f;
    }

    void SetLightsColour(Color colour)
    {
        foreach (Light light in lights)
        {
            light.color = colour;
        }
    }

}
