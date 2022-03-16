using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public Text fpsText;

    private int framesSinceLastSecond = 0;
    private float thisSecond = 0f;

    // Update is called once per frame
    void Update()
    {
        framesSinceLastSecond++;
        if (Mathf.Floor(Time.time) > thisSecond)
        {
            fpsText.text = framesSinceLastSecond.ToString();
            framesSinceLastSecond = 0;
        }
        thisSecond = Mathf.Floor(Time.time);
    }
}
