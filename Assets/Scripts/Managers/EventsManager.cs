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

    public Events.EventGameStateChanged OnGameStateChanged = null;
    public Events.EventDeviceOrientationUpdate OnDeviceOrientationUpdate = null;
    public Events.EventShuffleEnded OnShuffleEnded = null;
    public Events.EventCardsDealed OnCardsDealed = null;
    public Events.EventCardDropped OnCardDropped = null;
    public Events.EventCardPointerEnter OnCardPointerEnter = null;
    public Events.EventCardPointerExit OnCardPointerExit = null;
    public Events.EventPilePointerEnter OnPilePointerEnter = null;
    public Events.EventPilePointerExit OnPilePointerExit = null;
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
    public Events.EventScore OnScore = null;
    public Events.EventUndoScore OnUndoScore = null;
    public Events.EventCommand OnCommand = null;
    public Events.EventGameWon OnGameWon = null;

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

    private void Start()
    {
        DontDestroyOnLoad(this);
    }
}

public class Events
{
    /// <summary>
    /// Event called when the pause button is pressed
    /// </summary>
    [System.Serializable] public class EventGameStateChanged : UnityEvent<GameState> { };
    /// <summary>
    /// Event called when the device orientation changed
    /// </summary>
    [System.Serializable] public class EventDeviceOrientationUpdate : UnityEvent<DeviceOrientation> { };
    /// <summary>
    /// Event called when the starting deck shuffle is completed
    /// </summary>
    [System.Serializable] public class EventShuffleEnded : UnityEvent<List<CardData>> { };
    /// <summary>
    /// Event called when all the card have been spawned onto the table
    /// </summary>
    [System.Serializable] public class EventCardsDealed : UnityEvent<List<CardData>> { };
    /// <summary>
    /// Event called when the player started to drag one card
    /// </summary>
    [System.Serializable] public class EventCardDragging : UnityEvent<GUICard> { };
    /// <summary>
    /// Event called when the player dropped the dragging card
    /// </summary>
    [System.Serializable] public class EventCardDropped : UnityEvent { };
    /// <summary>
    /// Event called when the player tries to move one card on top of another card
    /// </summary>
    [System.Serializable] public class EventCardPointerEnter : UnityEvent<GUICard> { };
    /// <summary>
    /// Event called when the player moves one card away from another card
    /// </summary>
    [System.Serializable] public class EventCardPointerExit : UnityEvent { };
    /// <summary>
    /// Event called when the player moves one card onto a pile
    /// </summary>
    [System.Serializable] public class EventPilePointerEnter : UnityEvent<PileHandler> { };
    /// <summary>
    /// Event called when the player moves one card away from a pile
    /// </summary>
    [System.Serializable] public class EventPilePointerExit : UnityEvent { };
    /// <summary>
    /// Event called when a MoveCommand has been called
    /// </summary>
    [System.Serializable] public class EventCardMove : UnityEvent<GUICard, Transform> { };
    /// <summary>
    /// Event called when a MoveCommand undo has been called
    /// </summary>
    [System.Serializable] public class EventUndoCardMove : UnityEvent<GUICard, Transform> { };
    /// <summary>
    /// Event called when the player dropped a card and any MoveCommand hasn't been called
    /// </summary>
    [System.Serializable] public class EventCardFailMove : UnityEvent<GUICard> { };
    /// <summary>
    /// Event called when the player moved a card from the drawn pile to the table pile or the aces pile
    /// </summary>
    [System.Serializable] public class EventPick : UnityEvent<GUICard> { }
    /// <summary>
    /// Event called when the moved card from the drawn pile to the table pile or the aces pile has been reverted
    /// </summary>
    [System.Serializable] public class EventUndoPick : UnityEvent<GUICard, int> { }
    /// <summary>
    /// Event called when drawn card position has been reverted
    /// </summary>
    [System.Serializable] public class EventUndoDraw : UnityEvent { }
    /// <summary>
    /// Event called when draw deck is empty
    /// </summary>
    [System.Serializable] public class EventDeckEmpty : UnityEvent { }
    /// <summary>
    /// Event called when the player clicked on the empty deck and called the deck reset
    /// </summary>
    [System.Serializable] public class EventReset : UnityEvent { }
    /// <summary>
    /// Event called when the deck reset has been reverted
    /// </summary>
    [System.Serializable] public class EventUndoReset : UnityEvent { }
    /// <summary>
    /// Event called when the player performed a move that granted a score 
    /// </summary>
    [System.Serializable] public class EventScore : UnityEvent<int> { }
    /// <summary>
    /// Event called when the player reverted a move that granted a score 
    /// </summary>
    [System.Serializable] public class EventUndoScore : UnityEvent<int> { }
    /// <summary>
    /// Event called when any command has been called. Used to update the moves count 
    /// </summary>
    [System.Serializable] public class EventCommand : UnityEvent { };
    /// <summary>
    /// Event called when the player stacked successfully every card on the aces pile
    /// </summary>
    [System.Serializable] public class EventGameWon : UnityEvent { };
}

