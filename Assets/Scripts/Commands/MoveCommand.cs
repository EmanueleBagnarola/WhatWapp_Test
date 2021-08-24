using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Command called when the player moved one card 
/// </summary>
public class MoveCommand : ICommand
{
    /// <summary>
    /// Check if the MoveCommand was called after the player moved a stacked pile of cards
    /// </summary>
    public bool IsMultipleMove = false;

    private GUICard _guiCard = null;

    /// <summary>
    /// The parent transform where the card will be set in
    /// </summary>
    private Transform _destinationParent = null;

    /// <summary>
    /// The parent transform where the card was set before the MoveCommand
    /// </summary>
    private Transform _sourceParent = null;

    /// <summary>
    /// The table area where the card was before the MoveCommand
    /// </summary>
    private CardArea _sourceArea;

    /// <summary>
    /// The score to add (or undo) after the MoveCommand
    /// </summary>
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

        AudioManager.Instance.PlayOneShot("MoveCommand");

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
