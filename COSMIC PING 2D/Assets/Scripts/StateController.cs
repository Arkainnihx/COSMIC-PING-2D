using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StateController
{
    public static State currentState = State.MainMenu;
}

public enum State
{
    MainMenu,
    PlayingMatch,
    Paused,
    GameOver
}
