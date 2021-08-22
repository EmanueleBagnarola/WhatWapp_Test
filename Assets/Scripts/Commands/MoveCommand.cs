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

    private int _moveScore = 0;

    public MoveCommand(GUICard guiCard, Transform destinationParent, bool _isMultipleMove, int moveScore)
    {
        _guiCard = guiCard;
        _destinationParent = destinationParent;

        _sourceParent = guiCard.transform.parent;
        _sourceArea = guiCard.CardArea;

        IsMultipleMove = _isMultipleMove;

        _moveScore = moveScore;
    }

    public void Execute()
    {
        Debug.Log("EXECUTE MOVE COMMAND FROM: " + _sourceArea);

        //if (_sourceArea == CardArea.AcesPile)
        //{
        //    _moveScore = -15;
        //}

        EventsManager.Instance.OnCardMove.Invoke(_guiCard, _destinationParent);

        EventsManager.Instance.OnScore.Invoke(_moveScore);
    }

    public void Undo()
    {
        Debug.Log("UNDO MOVE COMMAND");
        EventsManager.Instance.OnUndoCardMove.Invoke(_guiCard, _sourceParent);
        EventsManager.Instance.OnUndoScore.Invoke(_moveScore);
        _guiCard.SetCardArea(_sourceArea);
    }
}
