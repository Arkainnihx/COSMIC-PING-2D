using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public MatchController matchController;

    public void StartGame()
    {
        matchController.StartGame();
        gameObject.SetActive(false);
    }

    public void ReturnToMenu()
    {
        matchController.ReturnToMenu();
        gameObject.SetActive(true);
        StateController.currentState = State.MainMenu;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
