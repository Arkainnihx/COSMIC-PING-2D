using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleController : MonoBehaviour
{
    public int playerID;
    public float movementSpeed;

    // Start is called before the first frame update
    void Start()
    {
        movementSpeed = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        switch (playerID)
        {
            case 1:
                transform.Translate(new Vector3(0, Input.GetAxisRaw("P1 Move") * movementSpeed));
                break;
            case 2:
                transform.Translate(new Vector3(0, Input.GetAxisRaw("P2 Move") * movementSpeed));
                break;
        }
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -10, 10));
    }
}
