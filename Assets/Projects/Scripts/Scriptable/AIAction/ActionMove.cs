using MiniGame;
using MoreMountains.Tools;
using UnityEngine;

[CreateAssetMenu(fileName = "Action_Move", menuName = "Data/AI/Actions/ActionMove", order = 1)]
public class ActionMove : AIAction
{
    public MoveDirection Direction;

    protected override void OneTimeAction(AIBrain _brain)
    {
        base.OneTimeAction(_brain);
        switch (Direction)
        {
            case MoveDirection.Left:
                _brain.PlayerController.MoveLeft();
                break;
            case MoveDirection.Right:
                _brain.PlayerController.MoveRight();
                break;
        }
    }
}