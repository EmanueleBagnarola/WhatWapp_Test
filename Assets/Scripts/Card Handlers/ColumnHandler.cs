using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnHandler : MonoBehaviour
{
    private void Start()
    {
        InitEvents();
    }

    [SerializeField]
    private List<GUICard> _guiCards = new List<GUICard>();

    //public void AddCard(GUICard _guiCard)
    //{
    //    _guiCards.Add(_guiCard);
    //}

    //public void RemoveCard(GUICard _guiCard)
    //{
    //    _guiCards.Remove(_guiCard);
    //}

    public void CheckFirstCard()
    {
        if (_guiCards.Count <= 0)
            return;

        GUICard firstCard = _guiCards[_guiCards.Count - 1];

        if (firstCard.CurrentSide == CardSide.Back)
        {
            firstCard.FlipCard(CardSide.Front);
        }
    }

    public void CheckUndoCommand(CardData undoCard, MoveUndoType columnAction)
    {
        if (_guiCards.Count <= 0)
            return;

        GUICard firstCard = _guiCards[_guiCards.Count - 1];

        switch (columnAction)
        {
            case MoveUndoType.Add:

                if(_guiCards.Count > 1)
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

            case MoveUndoType.Remove:
                if (firstCard.CurrentSide == CardSide.Back)
                {
                    firstCard.FlipCard(CardSide.Front);
                }
                break;
        }
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

    #region Events Handlers
    private void InitEvents()
    {
        EventsManager.Instance.OnCardsDealed.AddListener(HandleEventCardsDealed);
        EventsManager.Instance.OnCardMove.AddListener(HandleEventCardMove);
        EventsManager.Instance.OnUndoCardMove.AddListener(HandleEventUndoCardMove);
    }

    private void HandleEventCardsDealed(List<CardData> cardsData)
    {
        StartCoroutine(FillGUICardsList());
    }

    private void HandleEventCardMove(GUICard guiCard, Transform destinationParent)
    {
        if (_guiCards.Contains(guiCard))
        {
            _guiCards.Remove(guiCard);
            CheckFirstCard();
        }
        else
        {
            if (destinationParent.GetComponent<ColumnHandler>() == this)
            {
                _guiCards.Add(guiCard);
                guiCard.transform.SetParent(transform);

                // Set the guiCard CardArea as Table area
                guiCard.SetCardArea(CardArea.Table);
            }
        }
    }

    private void HandleEventUndoCardMove(GUICard guiCard, Transform sourceParent)
    {
        if (_guiCards.Contains(guiCard))
        {
            _guiCards.Remove(guiCard);

            CheckUndoCommand(guiCard.CardDataReference, MoveUndoType.Remove);
        }

        if(sourceParent.GetComponent<ColumnHandler>() == this)
        {
            CheckUndoCommand(guiCard.CardDataReference, MoveUndoType.Add);

            _guiCards.Add(guiCard);
            guiCard.transform.SetParent(transform);

            guiCard.SetCardArea(CardArea.Table);
        }
    }
    #endregion
}
