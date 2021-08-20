using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSystem 
{
    private GUICard _draggingCard = null;
    private GUICard _pointerEnterCard = null;

    public MoveSystem()
    {
        InitEvents();
    }

    public void CheckIfStackable()
    {
        if (_draggingCard == null || _pointerEnterCard == null)
        {
            CallCardStackFail();
            return;
        }

        CardData draggingCardData = _draggingCard.CardDataReference;
        CardData pointerEnterCardData = _pointerEnterCard.CardDataReference;

        //Debug.Log("Trying to drop [" + draggingCardData.Rank + " of " + draggingCardData.Suit + "] on " +
        //"" + "[" + pointerEnterCardData.Rank + " of " + pointerEnterCardData.Suit + "]");

        //Check if cards that user is trying to stack are of the same color
        //if (draggingCardData.GetCardColor() == CardColor.Black && pointerEnterCardData.GetCardColor() == CardColor.Black
        //    || draggingCardData.GetCardColor() == CardColor.Red && pointerEnterCardData.GetCardColor() == CardColor.Red)
        //{
        //    CallCardStackFail();
        //    return;
        //}

        //// Check if rank's cards that user is trying to stack are compatible
        //if (draggingCardData.Rank > pointerEnterCardData.Rank || pointerEnterCardData.Rank - draggingCardData.Rank > 1 || pointerEnterCardData.Rank - draggingCardData.Rank == 0)
        //{
        //    CallCardStackFail();
        //    return;
        //}

        // Check if the player is trying to stack a card on top of the card it's already stacked on
        if (_draggingCard.transform.parent == _pointerEnterCard.transform.parent)
        {
            Debug.Log("CallCardStackFail");
            CallCardStackFail();
            return;
        }

        // Check if the player is trying to stack a table card on a draw pile card
        if(_pointerEnterCard.CardArea == CardArea.DrawPile)
        {
            CallCardStackFail();
            return;
        }


        // Check for multiple cards dragging
        if (_draggingCard.AppendedCards.Count > 0)
        {
            MoveCommand(_draggingCard, true);

            for (int i = 0; i < _draggingCard.AppendedCards.Count; i++)
            {
                GUICard appendedCard = _draggingCard.AppendedCards[i];

                MoveCommand(appendedCard, true);

                _draggingCard.ReleaseAppendedCard(appendedCard);
            }
        }
        else
        {
            MoveCommand(_draggingCard, false);
        }

        _draggingCard = null;
        _pointerEnterCard = null;
    }

    private void MoveCommand(GUICard movedCard, bool isMultipleMove)
    {
        Debug.Log("Added MoveCommand");
        ICommand moveCommand = new MoveCommand(movedCard, _pointerEnterCard.transform.parent, isMultipleMove);
        GameManager.Instance.CommandHandler.AddCommand(moveCommand);
        moveCommand.Execute();
    }

    private void CallCardStackFail()
    {
        EventsManager.Instance.OnCardFailMove.Invoke(_draggingCard);
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
