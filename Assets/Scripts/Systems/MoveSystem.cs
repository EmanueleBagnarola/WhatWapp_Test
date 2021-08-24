using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSystem 
{
    /// <summary>
    /// The current dragging card reference
    /// </summary>
    private GUICard _draggingCard = null;

    /// <summary>
    /// The card the player is trying to move the dragging card onto
    /// </summary>
    private GUICard _pointerEnterCard = null;

    /// <summary>
    /// The pile the player is trying to move the dragging card onto
    /// </summary>
    private PileHandler _pointerEnterPile = null;

    /// <summary>
    /// The drop destination parent that the dragging card has to be set into
    /// </summary>
    private Transform _destinationParent = null;

    /// <summary>
    /// The card data the player is trying to move the dragging card onto
    /// </summary>
    private CardData draggingCardData = null;

    /// <summary>
    /// The card data the player is trying to move the dragging card onto
    /// </summary>
    private CardData pointerEnterCardData = null;

    public MoveSystem()
    {
        InitEvents();
    }

    /// <summary>
    /// Handles what happens after the player dropped a card anywhere on the table. Manages the moves rules.
    /// </summary>
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

                _draggingCard = null;
                _pointerEnterCard = null;
                _pointerEnterPile = null;
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

                    _draggingCard = null;
                    _pointerEnterCard = null;
                    _pointerEnterPile = null;
                    return;
                }
            }
        }

        //Debug.Log("Trying to drop [" + draggingCardData.Rank + " of " + draggingCardData.Suit + "] on " +
        //"" + "[" + pointerEnterCardData.Rank + " of " + pointerEnterCardData.Suit + "]");

        // Check for multiple cards dragging
        if (_draggingCard.AppendedCards.Count > 0)
        {
            // Set the score of the next move
            int moveScore = 0;

            // Check if the last moved top card had a hidden card previous in his list. If so, add 5 points
            if (_draggingCard.IsLastFrontCardInPile(_draggingCard.AppendedCards.Count + 1))
                moveScore = 5;

            // Move the first card of the dragging cards list
            MoveCommand(_draggingCard, _destinationParent, true, moveScore);

            for (int i = 0; i < _draggingCard.AppendedCards.Count; i++)
            {
                GUICard appendedCard = _draggingCard.AppendedCards[i];

                MoveCommand(appendedCard, _destinationParent, true, 0);
            }

            // Clear the appended card list
            _draggingCard.ReleaseAppendedCards();
        }
        else
        {
            int moveScore = 0;

            if (_draggingCard.IsLastFrontCardInPile(1))
                moveScore = 5;

            MoveCommand(_draggingCard, _destinationParent, false, moveScore);
        }

        _draggingCard = null;
        _pointerEnterCard = null;
        _pointerEnterPile = null;
    }

    /// <summary>
    /// Calls the MoveCommand for the current dragging card
    /// </summary>
    /// <param name="movedCard"></param>
    /// <param name="destinationParent"></param>
    /// <param name="isMultipleMove"></param>
    /// <param name="moveScore"></param>
    private void MoveCommand(GUICard movedCard, Transform destinationParent, bool isMultipleMove, int moveScore)
    {
        //Debug.Log("Added MoveCommand");
        ICommand moveCommand = new MoveCommand(movedCard, destinationParent, isMultipleMove, moveScore);
        GameManager.Instance.CommandHandler.AddCommand(moveCommand);
        moveCommand.Execute();
    }

    /// <summary>
    /// Handles what happens if the player dropped a card and any rule was applied
    /// </summary>
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
