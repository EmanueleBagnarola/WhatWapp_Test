using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickCommand : ICommand
{
    private GUICard _guiCardReference = null;
    private int _deckIndex = 0;

    public PickCommand(GUICard guiCardReference, int deckIndex)
    {
        _guiCardReference = guiCardReference;
        _deckIndex = deckIndex;
    }

    public void Execute()
    {
        DeckManager.Instance.PickCard(_guiCardReference);
    }

    public void Undo()
    {
        DeckManager.Instance.InsertCard(_guiCardReference, _deckIndex);
    }
}
