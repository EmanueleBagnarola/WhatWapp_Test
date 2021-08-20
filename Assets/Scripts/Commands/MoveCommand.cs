using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveCommand : ICommand
{
    public bool IsMultipleMove = false;

    private GUICard _guiCard = null;
    private Transform _destinationParent = null;

    private Transform _sourceParent = null;
    private CardArea _sourceArea;

    public MoveCommand(GUICard guiCard, Transform destinationParent, bool _isMultipleMove)
    {
        _guiCard = guiCard;
        _destinationParent = destinationParent;

        _sourceParent = guiCard.transform.parent;
        _sourceArea = guiCard.CardArea;

        IsMultipleMove = _isMultipleMove;
    }

    public void Execute()
    {
        EventsManager.Instance.OnCardMove.Invoke(_guiCard, _destinationParent);
    }

    public void Undo()
    {
        EventsManager.Instance.OnUndoCardMove.Invoke(_guiCard, _sourceParent);
        _guiCard.SetCardArea(_sourceArea);
    }
}
