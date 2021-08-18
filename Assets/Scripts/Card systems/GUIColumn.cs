using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIColumn : MonoBehaviour
{
    private CommandHandler _commandHandler = new CommandHandler();

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
        GUICard firstCard = _guiCards[_guiCards.Count - 1];

        if (firstCard.CurrentSide == CardSide.Back)
        {
            firstCard.FlipCard(CardSide.Front);
        }
    }

    public void CheckUndoCommand(UndoAction columnAction)
    {
        GUICard firstCard = _guiCards[_guiCards.Count - 1];

        switch (columnAction)
        {
            case UndoAction.Add:
                // Check if the second card is front sided too.
                if (_guiCards.Count > 2)
                {
                    GUICard secondCard = _guiCards[_guiCards.Count - 2];

                    if (secondCard.CurrentSide == CardSide.Front)
                        return;
                }

                //If there is no second card front sided, then first one has to be back side
                if (firstCard.CurrentSide == CardSide.Front)
                {
                    firstCard.FlipCard(CardSide.Back);
                }
                break;

            case UndoAction.Remove:
                if (firstCard.CurrentSide == CardSide.Back)
                {
                    firstCard.FlipCard(CardSide.Front);
                }
                break;
        }
    }
}
