using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PileHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region Public Variables References
    /// <summary>
    /// The list of this pile children GUICards
    /// </summary>
    public List<GUICard> GUICards
    {
        get
        {
            return _guiCards;
        }
    }

    /// <summary>
    /// The table area where this pile was set
    /// </summary>
    public CardArea CardArea
    {
        get
        {
            return _cardArea;
        }
    }

    /// <summary>
    /// In case of AcePile, set the suit of the pile
    /// </summary>
    public CardSuit CardSuit
    {
        get
        {
            return _cardSuit;
        }
    }
    #endregion

    #region Privave Variables References

    [SerializeField]
    private List<GUICard> _guiCards = new List<GUICard>();

    [SerializeField]
    private CardArea _cardArea = CardArea.Table;

    [SerializeField, Header("If this is a Ace pile")]
    private CardSuit _cardSuit = CardSuit.Empty;

    /// <summary>
    /// Set a parent reference for the GUICards list. if null, the parent will be this object.
    /// </summary>
    [SerializeField]
    private Transform _overrideParent = null;
    #endregion

    private void Start()
    {
        InitEvents();

        if (_overrideParent == null)
            _overrideParent = transform;
    }

    /// <summary>
    /// After all the cards are dealed, save any child GUICard reference in GUICards list
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// After any UndoCommand, check the right action to do with the first card of the list
    /// </summary>
    /// <param name="undoCard"></param>
    /// <param name="columnAction"></param>
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
    /// <summary>
    /// Call the pointer enter event when this pile is empty
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(_guiCards.Count <= 0)
        {
            EventsManager.Instance.OnPilePointerEnter.Invoke(this);
        }
    }

    /// <summary>
    /// Call the pointer exit event when this pile is empty
    /// </summary>
    /// <param name="eventData"></param>
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

    /// <summary>
    /// When the player started to drag a card in top of a stacked pile, append cards and set their UI sorting order
    /// </summary>
    /// <param name="guiCard"></param>
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

    /// <summary>
    /// Handle the MoveCommand called event
    /// </summary>
    /// <param name="guiCard"></param>
    /// <param name="destinationParent"></param>
    private void HandleEventCardMove(GUICard guiCard, Transform destinationParent)
    {
        // If the moved card reference was saved in the GUICards list, remove it 
        if (_guiCards.Contains(guiCard))
        {
            _guiCards.Remove(guiCard);

            if (_guiCards.Count <= 0)
                return;

            // If the first was faced back, turn it front side
            GUICard firstCard = _guiCards[_guiCards.Count - 1];

            if (firstCard.CurrentSide == CardSide.Back)
            {
                firstCard.FlipCard(CardSide.Front);
            }
        }
        else
        {
            // If the moved card reference has this pile as move destination, set this pile as its parent and add it to GUICards list
            if (destinationParent.GetComponent<PileHandler>() == this || destinationParent.GetComponentInParent<PileHandler>() == this)
            {
                _guiCards.Add(guiCard);
                guiCard.transform.SetParent(_overrideParent);

                // Set the guiCard CardArea as this Pile Card Area
                guiCard.SetCardArea(_cardArea);

                // If this pile is one of the AcePile, add one unit of the CompletedAcePileCount to detect the win conditoin
                if(CardArea == CardArea.AcesPile)
                {
                    if(_guiCards.Count >= 13)
                    {
                        GameManager.Instance.UpdateCompletedAcePileCount(OperationType.Add);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Handle the UndoCommand called event
    /// </summary>
    /// <param name="guiCard"></param>
    /// <param name="destinationParent"></param>
    private void HandleEventUndoCardMove(GUICard guiCard, Transform sourceParent)
    {
        // If the GUICards list contained the undo card, remove it
        if (_guiCards.Contains(guiCard))
        {
            // If this pile is one of the AcePile and the pile was completed (13 cards) remove one unit of the CompletedAcePileCount to detect the win conditoin
            if (CardArea == CardArea.AcesPile)
            {
                if (_guiCards.Count >= 13)
                {
                    GameManager.Instance.UpdateCompletedAcePileCount(OperationType.Remove);
                }
            }

            _guiCards.Remove(guiCard);

            CheckUndoCommand(guiCard.CardDataReference, OperationType.Remove);
        }

        if(sourceParent.GetComponent<PileHandler>() == this)
        {
            CheckUndoCommand(guiCard.CardDataReference, OperationType.Add);

            _guiCards.Add(guiCard);
            guiCard.transform.SetParent(_overrideParent);

            guiCard.SetCardArea(_cardArea);
        }
    }
    #endregion
}
