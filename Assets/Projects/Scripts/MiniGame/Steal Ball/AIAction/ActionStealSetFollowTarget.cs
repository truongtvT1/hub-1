using MoreMountains.Tools;
using UnityEngine;

[CreateAssetMenu(fileName = "Action_SetFollowTarget", menuName = "Data/AI/StealBall/Actions/ActionSetFollowTarget", order = 1)]
public class ActionStealSetFollowTarget : AIAction
{
    public bool targetBall;

    protected override void OneTimeAction(AIBrain _brain)
    {
        base.OneTimeAction(_brain);
        if (targetBall)
        {
            _brain.PlayerStealBall.SetDestination(_brain.PlayerStealBall.GetTargetBall().transform);
        }
        else
        {
            _brain.PlayerStealBall.SetDestination(_brain.PlayerStealBall.GetBallNest().transform);
        }
    }
}