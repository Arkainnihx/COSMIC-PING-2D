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
        FindGameOverScreen();
    }

    void FindGameOverScreen()
    {
        RectTransform[] uiArray = Resources.FindObjectsOfTypeAll<RectTransform>();
        foreach (RectTransform uiElement in uiArray)
        {
            if (uiElement.gameObject.name == "Game Over Menu")
            {
                gameOverScreen = uiElement.gameObject;
                break;
            }
        }
    }

    public void StartGame()
    {
        winArray = new int[totalPlayers];
        InitalGameSetup();
        Rematch();
    }

    void InitalGameSetup()
    {
        gameOverScreen.SetActive(true);
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
        gameOverScreen.SetActive(false);
    }

    public void Rematch()
    {
        gameOverScreen.SetActive(false);
        StateController.currentState = State.PlayingMatch;
        GetComponent<OrbSimulationController>().ResetOrbSimulation();
        foreach (GameObject paddle in paddleArray) paddle.GetComponent<PaddleController>().ResetPaddle();

        // Create initial energy orb
        StartCoroutine(GetComponent<OrbSimulationController>().SmoothCreateOrb(1f, Vector3.zero, 3f * Random.insideUnitCircle));
        //GetComponent<OrbSimulationController>().ChaosMode();
    }

    public void GameOver(GameObject losingPaddle)
    {
        StateController.currentState = State.GameOver;
        int winnerArrayIndex = UpdateWinCount(losingPaddle);
        SetAndShowGameOverScreen(winnerArrayIndex, paddleArray[winnerArrayIndex].GetComponent<PaddleController>().playerColour);
        losingPaddle.SetActive(false);
    }

    int UpdateWinCount(GameObject losingPaddle)
    {
        int winnerArrayIndex = losingPaddle.GetComponent<PaddleController>().playerID - 1 == 1 ? 0 : 1;
        winArray[winnerArrayIndex]++;
        return winnerArrayIndex;
    }

    void SetAndShowGameOverScreen(int winnerArrayIndex, Color winnerColour)
    {
        gameOverScreen.transform.Find("Winner Text").gameObject.GetComponent<Text>().text = $"P{winnerArrayIndex + 1} WINS!";
        gameOverScreen.transform.Find("Winner Text").gameObject.GetComponent<Text>().color = winnerColour;
        gameOverScreen.transform.Find("Score Text").Find("P1 Score").gameObject.GetComponent<Text>().text = winArray[0].ToString();
        gameOverScreen.transform.Find("Score Text").Find("P2 Score").gameObject.GetComponent<Text>().text = winArray[1].ToString();
        gameOverScreen.SetActive(true);
    }

    public void ReturnToMenu()
    {
        GetComponent<OrbSimulationController>().ResetOrbSimulation();
        foreach (GameObject paddle in paddleArray)
        {
            Destroy(paddle);
        }
        paddleArray = null;
        winArray = null;
        GameObject.Find("P1 Health Bar").GetComponent<Image>().fillAmount = 0f;
        GameObject.Find("P2 Health Bar").GetComponent<Image>().fillAmount = 0f;
        gameOverScreen.SetActive(false);
    }
}
