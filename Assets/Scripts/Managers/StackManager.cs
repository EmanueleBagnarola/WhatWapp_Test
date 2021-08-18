using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackManager : MonoBehaviour 
{
    /// <summary>
    /// Singleton instance reference
    /// </summary>
    public static StackManager Instance = null;

    [SerializeField]
    private GUICard _draggingCard = null;
    [SerializeField]
    private GUICard _pointerEnterCard = null;

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
        InitEvents();
    }

    public void CheckIfStackable()
    {
        if (_draggingCard == null || _pointerEnterCard == null)
        {
            EventsManager.Instance.OnCardStacked.Invoke(null, false, null);
            return;
        }

        CardData draggingCardData = _draggingCard.CardDataReference;
        CardData pointerEnterCardData = _pointerEnterCard.CardDataReference;

        //Debug.Log("Trying to drop [" + draggingCardData.Rank + " of " + draggingCardData.Suit + "] on " +
        //    "" + "[" + pointerEnterCardData.Rank + " of " + pointerEnterCardData.Suit + "]");

        // Check if cards that user is trying to stack are of the same color
        if (draggingCardData.GetCardColor() == CardColor.Black && pointerEnterCardData.GetCardColor() == CardColor.Black
            || draggingCardData.GetCardColor() == CardColor.Red && pointerEnterCardData.GetCardColor() == CardColor.Red)
        {
            EventsManager.Instance.OnCardStacked.Invoke(null, false, null);
            return;
        }

        // Check if rank's cards that user is trying to stack are compatible
        if (draggingCardData.Rank > pointerEnterCardData.Rank || pointerEnterCardData.Rank - draggingCardData.Rank > 1 || pointerEnterCardData.Rank - draggingCardData.Rank == 0)
        {
            EventsManager.Instance.OnCardStacked.Invoke(null, false, null);
            return;
        }

        // Check if I am trying to stack a card on top of the card it's already stacked on
        if (_draggingCard.transform.parent == _pointerEnterCard.transform.parent)
        {
            EventsManager.Instance.OnCardStacked.Invoke(null, false, null);
            return;
        }

        EventsManager.Instance.OnCardStacked.Invoke(_draggingCard, true, _pointerEnterCard.transform.parent);

        _draggingCard = null;
        _pointerEnterCard = null;
    }

    #region Events Handlers
    private void InitEvents()
    {
        EventsManager.Instance.OnCardDragging.AddListener(HandleEventCardDragging);
        EventsManager.Instance.OnCardDropped.AddListener(HandleEventCardDropped);
        EventsManager.Instance.OnCardPointerEnter.AddListener(HandleEventCardPointerEnter);
        EventsManager.Instance.OnCardPointerExit.AddListener(HandleEventCardPointerExit);
    }
    private void HandleEventCardDragging(GUICard guiCard)
    {
        _draggingCard = guiCard;
    }

    private void HandleEventCardDropped()
    {
        CheckIfStackable();
    }

    private void HandleEventCardPointerEnter(GUICard guiCard)
    {
        _pointerEnterCard = guiCard;
    }

    private void HandleEventCardPointerExit()
    {
        _pointerEnterCard = null;
    }
    #endregion
}
