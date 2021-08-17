using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventsManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance reference
    /// </summary>
    public static EventsManager Instance = null;

    public Events.EventStartGame OnStartGame = null;
    public Events.EventShuffleEnded OnShuffleEnded = null;

    private void Awake()
    {
        // Init Singleton ------
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        //----------------------
    }
}

public class Events
{
    [System.Serializable] public class EventStartGame : UnityEvent { };
    [System.Serializable] public class EventShuffleEnded : UnityEvent<List<CardData>> { };
}

