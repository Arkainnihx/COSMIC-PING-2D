using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchController : MonoBehaviour
{
    public GameObject paddlePrefab;
    public GameObject[] paddleArray;

    private int totalPlayers = 2;
    private int[] winArray;
    private GameObject gameOverScreen;

    // Start is called before the first frame update
    void Start()
    {
        winArray = new int[totalPlayers];
        gameOverScreen = GameObject.Find("Game Over Menu");
        InitalGameSetup();
        Rematch();
    }

    void InitalGameSetup()
    {
        // Create and position two paddles, assign player IDs
        paddleArray = new GameObject[totalPlayers];
        paddleArray[0] = Instantiate(paddlePrefab, new Vector3(15f, 0f), Quaternion.Euler(0, -90f, 0f), transform);
        paddleArray[0].GetComponent<PaddleController>().playerID = 1;
        paddleArray[0].GetComponent<PaddleController>().playerColour = Color.green;
        paddleArray[0].GetComponent<PaddleController>().SetHealthBar(GameObject.Find("P1 Health Bar"));
        gameOverScreen.transform.Find("Score Text").Find("P1 Score").GetComponent<Text>().color = paddleArray[0].GetComponent<PaddleController>().playerColour;
        paddleArray[1] = Instantiate(paddlePrefab, new Vector3(-15f, 0f), Quaternion.Euler(180f, -90f, 0f), transform);
        paddleArray[1].GetComponent<PaddleController>().playerID = 2;
        paddleArray[1].GetComponent<PaddleController>().playerColour = Color.red;
        paddleArray[1].GetComponent<PaddleController>().SetHealthBar(GameObject.Find("P2 Health Bar"));
        gameOverScreen.transform.Find("Score Text").Find("P2 Score").GetComponent<Text>().color = paddleArray[1].GetComponent<PaddleController>().playerColour;
    }

    public void GameOver(GameObject losingPaddle)
    {
        int winnerArrayIndex = UpdateWinCount(losingPaddle);
        SetAndShowGameOverScreen(winnerArrayIndex, paddleArray[winnerArrayIndex].GetComponent<PaddleController>().playerColour);
        losingPaddle.SetActive(false);
    }

    void SetAndShowGameOverScreen(int winnerArrayIndex, Color winnerColour)
    {
        gameOverScreen.transform.Find("Winner Text").gameObject.GetComponent<Text>().text = $"P{winnerArrayIndex + 1} WINS!";
        gameOverScreen.transform.Find("Winner Text").gameObject.GetComponent<Text>().color = winnerColour;
        gameOverScreen.transform.Find("Score Text").Find("P1 Score").gameObject.GetComponent<Text>().text = winArray[0].ToString();
        gameOverScreen.transform.Find("Score Text").Find("P2 Score").gameObject.GetComponent<Text>().text = winArray[1].ToString();
        gameOverScreen.SetActive(true);
    }

    int UpdateWinCount(GameObject losingPaddle)
    {
        int winnerArrayIndex = losingPaddle.GetComponent<PaddleController>().playerID - 1 == 1 ? 0 : 1;
        winArray[winnerArrayIndex]++;
        return winnerArrayIndex;
    }

    public void Rematch()
    {
        gameOverScreen.SetActive(false);
        GetComponent<OrbSimulationController>().ResetOrbSimulation();
        foreach (GameObject paddle in paddleArray) paddle.GetComponent<PaddleController>().ResetPaddle();

        // Create initial energy orb
        //GetComponent<OrbSimulationController>().CreateOrb(1f);
        GetComponent<OrbSimulationController>().ChaosMode();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
