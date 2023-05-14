using System.Collections.Generic;
using UnityEngine;

public class MoveManager
{
    private Dictionary<string, Move> Moves = new Dictionary<string, Move>();

    private TargetController targetController;

    public MoveManager(TargetController targetController)
    {
        this.targetController = targetController;
    }

    public void addMove(Move move)
    {
        move.targetController = this.targetController;
        move.moveManager = this;
        move.humanIKController = targetController.humanIKController;
        Moves[move.name] = move;
    }

    public void removeMove(string moveName)
    {
        if (Moves.ContainsKey(moveName))
        {
            Moves.Remove(moveName);
        }
    }
    public Move getMove(string moveName)
    {
        if (Moves.ContainsKey(moveName))
        {
            return Moves[moveName];
        }
        else
        {
            Debug.LogWarning("Move with name " + moveName + " not found.");
            return null;
        }
    }
    public Move ChangeMove(string moveName)
    {
        Move mov = getMove(moveName);
        if (mov != null)
        {
            mov.init();
            if (targetController.advanceIKController != null) {
                targetController.advanceIKController.changeState(mov.getMoveType());
            }
            if (targetController.move != null) {
                targetController.move.finish();
            }
            targetController.move = mov;
        }
        return mov;
    }
}
