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
    public Events.EventCardsDealed OnCardsDealed = null;
    public Events.EventCardDropped OnCardDropped = null;
    public Events.EventCardPointerEnter OnCardPointerEnter = null;
    public Events.EventCardPointerExit OnCardPointerExit = null;
    public Events.EventCardDragging OnCardDragging = null;
    public Events.EventCardStacked OnCardStacked = null;
    public Events.EventUndoDeckCard OnUndoDeckCard = null;

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
    [System.Serializable] public class EventCardsDealed : UnityEvent<List<CardData>> { };
    [System.Serializable] public class EventCardDragging : UnityEvent<GUICard> { };
    [System.Serializable] public class EventCardDropped : UnityEvent { };
    [System.Serializable] public class EventCardPointerEnter : UnityEvent<GUICard> { };
    [System.Serializable] public class EventCardPointerExit : UnityEvent { };
    [System.Serializable] public class EventCardStacked : UnityEvent<GUICard, bool, Transform> { }
    [System.Serializable] public class EventUndoDeckCard : UnityEvent<GUICard> { }
}

