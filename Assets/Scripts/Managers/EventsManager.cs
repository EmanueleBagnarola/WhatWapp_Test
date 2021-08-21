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
    public Events.EventTablePilePointerEnter OnTablePilePointerEnter = null;
    public Events.EventTablePilePointerExit OnTablePilePointerExit = null;
    public Events.EventCardMove OnCardMove = null;
    public Events.EventUndoCardMove OnUndoCardMove = null;
    public Events.EventCardDragging OnCardDragging = null;
    public Events.EventCardFailMove OnCardFailMove = null;
    public Events.EventPick OnPick = null;
    public Events.EventUndoPick OnUndoPick = null;
    public Events.EventUndoDraw OnUndoDraw = null;
    public Events.EventDeckEmpty OnDeckEmpty = null;
    public Events.EventReset OnReset = null;
    public Events.EventUndoReset OnUndoReset = null;

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
    [System.Serializable] public class EventTablePilePointerEnter : UnityEvent<TablePileHandler> { };
    [System.Serializable] public class EventTablePilePointerExit : UnityEvent { };
    [System.Serializable] public class EventCardMove : UnityEvent<GUICard, Transform> { };
    [System.Serializable] public class EventUndoCardMove : UnityEvent<GUICard, Transform> { };
    [System.Serializable] public class EventCardFailMove : UnityEvent<GUICard> { };
    [System.Serializable] public class EventPick : UnityEvent<GUICard> { }
    [System.Serializable] public class EventUndoPick : UnityEvent<GUICard, int> { }
    [System.Serializable] public class EventUndoDraw : UnityEvent { }
    [System.Serializable] public class EventDeckEmpty : UnityEvent { }
    [System.Serializable] public class EventReset : UnityEvent { }
    [System.Serializable] public class EventUndoReset : UnityEvent { }
}

