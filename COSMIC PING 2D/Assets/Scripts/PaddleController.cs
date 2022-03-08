using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleController : MonoBehaviour
{
    public GameObject orbPrefab;
    public int playerID;
    public Color playerColour;
    public float movementSpeed;
    public float projectileForceCoefficient;
    public float projectileForceMinimum;
    public float projectileMaxMass;

    private GameObject projectileOrb;
    private float projectileOrbFloatRange = 0.7f;
    private float chargeTime = 0f;
    private float startTime = 0f;
    private float endTime = 0f;
    private float cooldown = 0.5f;
    private float cooldownBound = 0.5f;
    private bool growingOrb = false;

    private Light[] lights;
    //private Gradient lightsChargeGradient;
    //private float playerHue;
    //private float playerSaturation;
    //private float playerValue;

    // Start is called before the first frame update
    void Start()
    {
        movementSpeed = 0.1f;
        lights = lights = transform.GetChild(2).GetComponentsInChildren<Light>();
        StartCoroutine(SetLights());
    }

    // Update is called once per frame
    void Update()
    {
        // Controls player input based on playerID
        transform.Translate(new Vector3(0, Input.GetAxis($"P{playerID} Move") * movementSpeed));
        if (Input.GetAxisRaw($"P{playerID} Fire") == 1)
        {
            if (cooldown >= cooldownBound)
            {
                if (!growingOrb)
                {
                    startTime = CreateProjectileOrb();
                }
                else
                {
                    chargeTime = ChargeProjectileOrb(startTime);
                }
            }
        }
        // Keeps paddles within play area
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -9.5f, 9.5f));

        if (cooldown < cooldownBound)
        {
            cooldown += Time.deltaTime;
            //if (cooldown / cooldownBound < 0.5f)
            //{
            //    SetBackLightsColour(lightsChargeGradient.Evaluate(cooldown / cooldownBound));
            //    Debug.Log($"Back: {cooldown / cooldownBound}");
            //} else
            //{
            //    SetFrontLightsColour(lightsChargeGradient.Evaluate(cooldown / cooldownBound));
            //    Debug.Log($"Front: {cooldown / cooldownBound}");
            //}
        }
    }

    void FixedUpdate()
    {
        if (Input.GetAxisRaw($"P{playerID} Fire") == 0 && growingOrb)
        {
            ShootProjectileOrb(chargeTime);
        }
    }

    IEnumerator SetLights()
    {
        yield return new WaitForSeconds(0.1f);
        SetLightsColour(playerColour);
        //Color.RGBToHSV(playerColour, out playerHue, out playerSaturation, out playerValue);

        //lightsChargeGradient = new Gradient();
        //GradientColorKey[] colourKeys = new GradientColorKey[6];
        //GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        //colourKeys[0].color = Color.white;
        //colourKeys[0].time = 0f;
        //colourKeys[1].color = Color.HSVToRGB(playerHue, playerSaturation / 2, playerValue);
        //colourKeys[1].time = 0.489f;
        //colourKeys[2].color = playerColour;
        //colourKeys[2].time = 0/49f;
        //colourKeys[3].color = Color.white;
        //colourKeys[3].time = 0.5f;
        //colourKeys[4].color = Color.HSVToRGB(playerHue, playerSaturation / 2, playerValue);
        //colourKeys[4].time = 0.989f;
        //colourKeys[5].color = playerColour;
        //colourKeys[5].time = 0.99f;
        //alphaKeys[0].alpha = 1f;
        //alphaKeys[0].time = 0f;
        //alphaKeys[1].alpha = 1f;
        //alphaKeys[1].time = 1f;
        //lightsChargeGradient.SetKeys(colourKeys, alphaKeys);    

    }

    float CreateProjectileOrb()
    {
        projectileOrb = GetComponentInParent<PlayingFieldController>().CreateOrb(0f);
        projectileOrb.transform.SetParent(transform);
        projectileOrb.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        projectileOrb.transform.localPosition = projectileOrbFloatRange * Vector3.forward;
        growingOrb = true;

        return Time.time;
    }

    float ChargeProjectileOrb(float startTime)
    {
        if (projectileOrb.GetComponent<Rigidbody>().mass < projectileMaxMass)
        {
            endTime = Time.time;
            projectileOrb.GetComponent<OrbController>().IncrementMass();
        }
        projectileOrb.transform.localPosition = projectileOrbFloatRange * Vector3.forward;
        return endTime - startTime;
    }

    void ShootProjectileOrb(float chargeTime)
    {
        if (projectileOrb != null)
        {
            projectileOrb.transform.SetParent(transform.parent);
            projectileOrb.GetComponent<Rigidbody>().AddForce((projectileForceMinimum + (chargeTime * projectileForceCoefficient)) * Vector3.Normalize(projectileOrb.transform.position - transform.position));
        }
        cooldownBound = 0.5f + chargeTime / 4;
        cooldown = 0f;
        growingOrb = false;
    }

    void SetLightsColour(Color colour)
    {
        foreach (Light light in lights)
        {
            light.color = colour;
        }
    }

    //void SetBackLightsColour(Color colour)
    //{
    //    lights[0].color = colour;
    //    lights[1].color = colour;
    //}

    //void SetFrontLightsColour(Color colour)
    //{
    //    lights[2].color = colour;
    //    lights[3].color = colour;
    //}

}
