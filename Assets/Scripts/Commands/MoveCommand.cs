using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveCommand : ICommand
{
    private Transform _cardTransform = null;
    private Transform _sourceParent = null;
    private Transform _destinationParent = null;
    private CardArea _previousCardArea = CardArea.Table;

    private GUICard _guiCardReference = null;

    public MoveCommand(Transform cardTransform, Transform sourceParent, Transform destinationParent, CardArea previousCardArea)
    {
        _cardTransform = cardTransform;
        _sourceParent = sourceParent;
        _destinationParent = destinationParent;
        _guiCardReference = cardTransform.GetComponent<GUICard>();
    }

    public void Execute()
    {
        GUIColumn sourceColumn = _sourceParent.GetComponent<GUIColumn>();
        GUIColumn destinationColumn = _destinationParent.GetComponent<GUIColumn>();

        sourceColumn?.RemoveCard(_guiCardReference);
        destinationColumn?.AddCard(_guiCardReference);

        sourceColumn?.CheckAddCommand();

        _cardTransform.SetParent(_destinationParent);

        // Set the moved card area as table
        _guiCardReference.SetCardArea(CardArea.Table);
    }

    public void Undo()
    {
        // the reference of the destination of the execute command (the column the player dragged on to)
        GUIColumn sourceColumn = _destinationParent.GetComponent<GUIColumn>();
        // the reference of the source of the execute command (the column the player dragged from)
        GUIColumn destinationColumn = _sourceParent.GetComponent<GUIColumn>();

        // if the first card of the undo destination column where we are ADDING a card is already front side (and the only one card front sided), then turn it
        destinationColumn?.CheckUndoCommand(_guiCardReference.CardDataReference, MoveUndoType.Add);

        sourceColumn?.RemoveCard(_guiCardReference);
        destinationColumn?.AddCard(_guiCardReference);

        // then chekcs if the first card 
        sourceColumn?.CheckUndoCommand(_guiCardReference.CardDataReference, MoveUndoType.Remove);

        _cardTransform.SetParent(_sourceParent);

        // Reset the previous card area
        _guiCardReference.SetCardArea(_previousCardArea);
    }
}
