using MoreMountains.Tools;
using UnityEngine;


[CreateAssetMenu(fileName = "Action_ReleaseBall", menuName = "Data/AI/StealBall/Actions/ActionReleaseBall", order = 1)]
public class ActionStealReleaseBall : AIAction
{

    protected override void OneTimeAction(AIBrain _brain)
    {
        base.OneTimeAction(_brain);
        _brain.PlayerStealBall.ReleaseBall();
    }
}