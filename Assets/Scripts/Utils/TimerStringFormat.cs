using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimerStringFormat
{
    public static string GetTimerString(float timer)
    {
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer - minutes * 60);
        string timerString = string.Format("{0:00}:{1:00}", minutes, seconds);

        return timerString;
    }
}