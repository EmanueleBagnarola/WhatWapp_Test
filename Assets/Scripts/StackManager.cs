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
            Debug.LogWarning("drag card or pointer enter card null");
            EventsManager.Instance.OnCardStacked.Invoke(null, false, null);
            return;
        }

        CardData draggingCardData = _draggingCard.CardDataReference;
        CardData pointerEnterCardData = _pointerEnterCard.CardDataReference;

        Debug.Log("Trying to drop [" + draggingCardData.Rank + " of " + draggingCardData.Suit + "] on " +
            "" +  "[" + pointerEnterCardData.Rank + " of " + pointerEnterCardData.Suit + "]");

        if (draggingCardData.GetCardColor() == CardColor.Black && pointerEnterCardData.GetCardColor() == CardColor.Black)
        {
            EventsManager.Instance.OnCardStacked.Invoke(null, false, null);
            return;
        }

        if (draggingCardData.Rank > pointerEnterCardData.Rank)
        {
            EventsManager.Instance.OnCardStacked.Invoke(null, false, null);
            return;
        }

        EventsManager.Instance.OnCardStacked.Invoke(_draggingCard, true, _pointerEnterCard.ColumnTransformReference);
        //_draggingCard.StackCard(_pointerEnterCard.ColumnTransformReference);
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
