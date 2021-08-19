using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIColumn : MonoBehaviour
{
    private void Start()
    {
        InitEvents();
    }

    [SerializeField]
    private List<GUICard> _guiCards = new List<GUICard>();

    public void AddCard(GUICard _guiCard)
    {
        _guiCards.Add(_guiCard);
    }

    public void RemoveCard(GUICard _guiCard)
    {
        _guiCards.Remove(_guiCard);
    }

    public void CheckAddCommand()
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
                // Check if the second card is front sided too.
                GUICard secondCard = _guiCards[_guiCards.Count - 2];

                if (secondCard.CurrentSide == CardSide.Front)
                    return;

                //If there is no second card front sided, then first one has to be back side
                if (firstCard.CurrentSide == CardSide.Front)
                {
                    if (firstCard.CardDataReference.Rank - undoCard.Rank == 1)
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
    }

    private void HandleEventCardsDealed(List<CardData> cardsData)
    {
        StartCoroutine(FillGUICardsList());
    }
    #endregion
}
