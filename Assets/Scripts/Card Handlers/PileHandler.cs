using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PileHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public List<GUICard> GUICards
    {
        get
        {
            return _guiCards;
        }
    }

    public CardArea CardArea
    {
        get
        {
            return _cardArea;
        }
    }

    public CardSuit CardSuit
    {
        get
        {
            return _cardSuit;
        }
    }

    [SerializeField]
    private List<GUICard> _guiCards = new List<GUICard>();

    [SerializeField]
    private CardArea _cardArea = CardArea.Table;

    [SerializeField, Header("If this is a Ace pile")]
    private CardSuit _cardSuit = CardSuit.Empty;

    [SerializeField]
    private Transform _overrideParent = null;

    private void Start()
    {
        InitEvents();

        if (_overrideParent == null)
            _overrideParent = transform;
    }

    private IEnumerator FillGUICardsList()
    {
        yield return new WaitForSeconds(0.1f);

        GUICard[] guiCardsArray = transform.GetComponentsInChildren<GUICard>();

        for (int i = 0; i < guiCardsArray.Length; i++)
        {
            GUICard guiCard = guiCardsArray[i];
            _guiCards.Add(guiCard);
        }
    }

    private void CheckUndoCommand(CardData undoCard, OperationType columnAction)
    {
        if (_guiCards.Count <= 0)
            return;

        GUICard firstCard = _guiCards[_guiCards.Count - 1];

        switch (columnAction)
        {
            case OperationType.Add:

                if (_guiCards.Count > 1)
                {
                    // Check if the second card is front sided too.
                    GUICard secondCard = _guiCards[_guiCards.Count - 2];

                    if (secondCard.CurrentSide == CardSide.Front)
                        return;
                }

                //If there is no second card front sided, then first one has to be back side
                if (firstCard.CurrentSide == CardSide.Front)
                {
                    if (firstCard.CardDataReference.Rank - undoCard.Rank == 1 && firstCard.CardDataReference.Suit != undoCard.Suit)
                        return;

                    firstCard.FlipCard(CardSide.Back);
                }
                break;

            case OperationType.Remove:
                if (firstCard.CurrentSide == CardSide.Back)
                {
                    firstCard.FlipCard(CardSide.Front);
                }
                break;
        }
    }

    #region Event System Handlers
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(_guiCards.Count <= 0)
        {
            EventsManager.Instance.OnPilePointerEnter.Invoke(this);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_guiCards.Count <= 0)
        {
            EventsManager.Instance.OnPilePointerExit.Invoke();
        }
    }
    #endregion

    #region Events Handlers
    private void InitEvents()
    {
        EventsManager.Instance.OnCardsDealed.AddListener(HandleEventCardsDealed);
        EventsManager.Instance.OnCardDragging.AddListener(HandleEventCardDragging);
        EventsManager.Instance.OnCardMove.AddListener(HandleEventCardMove);
        EventsManager.Instance.OnUndoCardMove.AddListener(HandleEventUndoCardMove);
    }

    private void HandleEventCardsDealed(List<CardData> cardsData)
    {
        StartCoroutine(FillGUICardsList());
    }

    private void HandleEventCardDragging(GUICard guiCard)
    {
        if (!_guiCards.Contains(guiCard))
            return;

        int draggingCardIndex = _guiCards.IndexOf(guiCard);

        // for list count, check if index + 1 has a gui card ref
        for (int i = draggingCardIndex + 1; i < _guiCards.Count; i++)
        {
            GUICard hangingCard = _guiCards[i];

            if (hangingCard == null)
                return;

            hangingCard.SetSortingOrder(5 + draggingCardIndex+i);

            guiCard.AppendDraggingCards(hangingCard);
        }
    }

    private void HandleEventCardMove(GUICard guiCard, Transform destinationParent)
    {
        if (_guiCards.Contains(guiCard))
        {
            _guiCards.Remove(guiCard);

            if (_guiCards.Count <= 0)
                return;

            GUICard firstCard = _guiCards[_guiCards.Count - 1];

            if (firstCard.CurrentSide == CardSide.Back)
            {
                firstCard.FlipCard(CardSide.Front);
            }
        }
        else
        {
            if (destinationParent.GetComponent<PileHandler>() == this || destinationParent.GetComponentInParent<PileHandler>() == this)
            {
                _guiCards.Add(guiCard);
                guiCard.transform.SetParent(_overrideParent);

                // Set the guiCard CardArea as this Pile Card Area
                guiCard.SetCardArea(_cardArea);
            }
        }
    }

    private void HandleEventUndoCardMove(GUICard guiCard, Transform sourceParent)
    {
        if (_guiCards.Contains(guiCard))
        {
            _guiCards.Remove(guiCard);

            CheckUndoCommand(guiCard.CardDataReference, OperationType.Remove);
        }

        if(sourceParent.GetComponent<PileHandler>() == this)
        {
            CheckUndoCommand(guiCard.CardDataReference, OperationType.Add);

            _guiCards.Add(guiCard);
            guiCard.transform.SetParent(_overrideParent);

            guiCard.SetCardArea(CardArea.Table);
        }
    }
    #endregion
}
