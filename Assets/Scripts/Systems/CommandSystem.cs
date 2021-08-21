using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandSystem
{
    private List<ICommand> _commandList = new List<ICommand>();

    public void AddCommand(ICommand command)
    {
        _commandList.Add(command);
    }

    public void UndoCommand()
    {
        if (_commandList.Count == 0)
            return;

        ICommand lastCommand = _commandList[_commandList.Count - 1];

        //lastCommand.Undo();
        ////_commandList.RemoveAt(_index - 1);
        //_commandList.Remove(lastCommand);

        if (lastCommand is MoveCommand)
        {
            MoveCommand moveCommand = (MoveCommand)lastCommand;

            if(moveCommand.IsMultipleMove)
            {
                List<MoveCommand> mulitpleMoveCommands = new List<MoveCommand>();

                for (int i = _commandList.Count - 1; i > 0; i--)
                {
                    MoveCommand multipleMoveCommand = _commandList[i] as MoveCommand;
                    if (multipleMoveCommand.IsMultipleMove)
                    {
                        mulitpleMoveCommands.Insert(0, multipleMoveCommand);
                    }
                    else
                    {
                        break;
                    }
                }

                for (int i = 0; i < mulitpleMoveCommands.Count; i++)
                {
                    MoveCommand command = mulitpleMoveCommands[i];
                    command.Undo();
                    _commandList.Remove(command);
                }
            }
            else
            {
                lastCommand.Undo();
                _commandList.Remove(lastCommand);
            }
        }

        // If last undo command was a pick command, undo also the move command previous to it, called by the card move
        if (lastCommand is PickCommand)
        {
            if (_commandList[_commandList.Count - 2] is MoveCommand)
            {
                _commandList[_commandList.Count - 2].Undo();
                _commandList.Remove(_commandList[_commandList.Count - 2]);
            }
        }
    }
}

