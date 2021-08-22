using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSystem 
{
    private GUICard _draggingCard = null;
    private GUICard _pointerEnterCard = null;
    private PileHandler _pointerEnterPile = null;

    private Transform _destinationParent = null;

    private CardData draggingCardData = null;
    private CardData pointerEnterCardData = null;

    public MoveSystem()
    {
        InitEvents();
    }

    public void CheckMove()
    {
        if (_draggingCard == null || _pointerEnterCard == null && _pointerEnterPile == null)
        {
            CallFailMove();
            return;
        }

        draggingCardData = _draggingCard.CardDataReference;
        
        // Check move done on another table card
        if(_pointerEnterCard != null)
        {
            pointerEnterCardData = _pointerEnterCard.CardDataReference;
            _destinationParent = _pointerEnterCard.transform.parent;

            // Check if the player is trying to stack a table card on a draw pile card
            if (_pointerEnterCard.CardArea == CardArea.DrawPile)
            {
                CallFailMove();
                return;
            }

            // Check if the player is trying to stack a table card on the aces pile
            if(_pointerEnterCard.CardArea == CardArea.AcesPile)
            {
                if(draggingCardData.Suit != pointerEnterCardData.Suit)
                {
                    CallFailMove();
                    return;
                }

                if(draggingCardData.Rank - pointerEnterCardData.Rank != 1)
                {
                    CallFailMove();
                    return;
                }

                //Debug.Log("CARTA SU PILA ASSI");
                MoveCommand(_draggingCard, _destinationParent, false, 10);
                return;
            }

            if (_draggingCard.transform.parent == _pointerEnterCard.transform.parent)
            {
                CallFailMove();
                return;
            }

            //Check if cards that user is trying to stack are of the same color
            if(GameManager.Instance.EnableTest == false)
            {
                if (draggingCardData.GetCardColor() == CardColor.Black && pointerEnterCardData.GetCardColor() == CardColor.Black
                    || draggingCardData.GetCardColor() == CardColor.Red && pointerEnterCardData.GetCardColor() == CardColor.Red)
                {
                    CallFailMove();
                    return;
                }

                // Check if rank's cards that user is trying to stack are compatible
                if (draggingCardData.Rank > pointerEnterCardData.Rank || pointerEnterCardData.Rank - draggingCardData.Rank != 1)
                {
                    CallFailMove();
                    return;
                }
            }
        } 

        // Check empty Table Pile Move
        if(_pointerEnterPile != null)
        {
            _destinationParent = _pointerEnterPile.transform;

            if(_pointerEnterPile.CardArea == CardArea.Table)
            {
                // If the dragging card isn't a Rank 13, the move is illegal
                if (draggingCardData.Rank != 13)
                {
                    CallFailMove();
                    return;
                }
            }

            if(_pointerEnterPile.CardArea == CardArea.AcesPile)
            {
                if(_pointerEnterPile.CardSuit != draggingCardData.Suit)
                {
                    CallFailMove();
                    return;
                }
                if(draggingCardData.Rank != 1)
                {
                    CallFailMove();
                    return;
                }
                else
                {
                    // Check if I am trying to position again a 1 of Ace
                    if (_pointerEnterPile.GUICards.Contains(_draggingCard))
                    {
                        CallFailMove();
                        return;
                    }

                    //Debug.Log("PRIMO ASSO POSIZIONATO");
                    MoveCommand(_draggingCard, _destinationParent, false, 10);
                    return;
                }
            }
        }

        //Debug.Log("Trying to drop [" + draggingCardData.Rank + " of " + draggingCardData.Suit + "] on " +
        //"" + "[" + pointerEnterCardData.Rank + " of " + pointerEnterCardData.Suit + "]");

        // Check for multiple cards dragging
        if (_draggingCard.AppendedCards.Count > 0)
        {
            int moveScore = 0;

            if (_draggingCard.IsLastFrontCardInPile())
                moveScore = 5;

            MoveCommand(_draggingCard, _destinationParent, true, moveScore);

            for (int i = 0; i < _draggingCard.AppendedCards.Count; i++)
            {
                GUICard appendedCard = _draggingCard.AppendedCards[i];

                MoveCommand(appendedCard, _destinationParent, true, 0);
                //_draggingCard.ReleaseAppendedCard(appendedCard);
            }

            //_draggingCard.AppendedCards.Clear();
            _draggingCard.ReleaseAppendedCards();
        }
        else
        {
            int moveScore = 0;

            if (_draggingCard.IsLastFrontCardInPile())
                moveScore = 5;

            MoveCommand(_draggingCard, _destinationParent, false, moveScore);
        }

        _draggingCard = null;
        _pointerEnterCard = null;
        _pointerEnterPile = null;
    }

    private void MoveCommand(GUICard movedCard, Transform destinationParent, bool isMultipleMove, int moveScore)
    {
        //Debug.Log("Added MoveCommand");
        ICommand moveCommand = new MoveCommand(movedCard, destinationParent, isMultipleMove, moveScore);
        GameManager.Instance.CommandHandler.AddCommand(moveCommand);
        moveCommand.Execute();
    }

    private void CallFailMove()
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
        EventsManager.Instance.OnPilePointerEnter.AddListener(HandleEventPilePointerEnter);
        EventsManager.Instance.OnPilePointerExit.AddListener(HandleEventPilePointerExit);
    }
    private void HandleEventCardDragging(GUICard guiCard)
    {
        _draggingCard = guiCard;
    }

    private void HandleEventCardDropped()
    {
        CheckMove();
    }

    private void HandleEventCardPointerEnter(GUICard guiCard)
    {
        _pointerEnterCard = guiCard;
    }

    private void HandleEventCardPointerExit()
    {
        _pointerEnterCard = null;
    }

    private void HandleEventPilePointerEnter(PileHandler tablePileHandler)
    {
        _pointerEnterPile = tablePileHandler;
    }

    private void HandleEventPilePointerExit()
    {
        _pointerEnterPile = null;
    }
    #endregion
}
