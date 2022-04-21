using System;
using MiniGame;
using MoreMountains.Tools;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Action_Move", menuName = "Data/AI/Actions/ActionMove", order = 1)]
public class ActionMove : AIAction
{
    public MoveDirection Direction;
    public bool random = false;
    protected override void OneTimeAction(AIBrain _brain)
    {
        base.OneTimeAction(_brain);
        Random.InitState(DateTime.UtcNow.Millisecond);
        if (random)
        {
            Direction = (MoveDirection) Random.Range(-1, 2);
        }
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