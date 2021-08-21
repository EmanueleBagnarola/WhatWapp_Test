using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandSystem
{
    private List<ICommand> _commandList = new List<ICommand>();
    private int _index;

    public void AddCommand(ICommand command)
    {
        if (_index < _commandList.Count)
            _commandList.RemoveRange(_index, _commandList.Count - _index);

        _commandList.Add(command);

        //command.Execute();

        _index++;
    }

    public void UndoCommand()
    {
        if (_commandList.Count == 0)
            return;

        ICommand lastCommand = _commandList[_index - 1];

        if (_index > 0)
        {
            _commandList[_index - 1].Undo();
            _commandList.RemoveAt(_index - 1);
            _index--;
        }

        if(lastCommand is MoveCommand)
        {
            MoveCommand moveCommand = (MoveCommand)lastCommand;

            if (moveCommand.IsMultipleMove == false)
                return;

            else
            {
                for (int i = _commandList.Count - 1; i > 0; i--)
                {
                    MoveCommand multipleMoveCommand = _commandList[i] as MoveCommand;
                    if (multipleMoveCommand.IsMultipleMove)
                    {
                        multipleMoveCommand.Undo();
                        _commandList.Remove(multipleMoveCommand);
                        _index--;
                    }
                }
            }
        }

        // If last undo command was a pick command, undo also the move command previous to it, called by the card move
        if (lastCommand is PickCommand)
        {
            if (_index > 0)
            {
                if (_commandList[_index - 1] is MoveCommand)
                {
                    _commandList[_index - 1].Undo();
                    _commandList.RemoveAt(_index - 1);
                    _index--;
                }

            }
        }
    }
}

