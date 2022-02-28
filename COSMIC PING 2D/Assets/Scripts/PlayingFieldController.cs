using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingFieldController : MonoBehaviour
{
    public GameObject paddlePrefab;
    public GameObject orbPrefab;
    public int totalPlayers = 2;

    private GameObject[] paddleArray;
    // Start is called before the first frame update
    void Start()
    {
        if (totalPlayers == 2)
        {
            paddleArray = new GameObject[2];
            paddleArray[0] = Instantiate(paddlePrefab, new Vector3(15, 0), Quaternion.Euler(0, -90f, 0));
            paddleArray[0].GetComponent<PaddleController>().playerID = 1;
            paddleArray[1] = Instantiate(paddlePrefab, new Vector3(-15, 0), Quaternion.Euler(180, -90f, 0));
            paddleArray[0].GetComponent<PaddleController>().playerID = 2;
        }
        Instantiate(orbPrefab, Vector3.zero, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
