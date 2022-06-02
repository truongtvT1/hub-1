using MoreMountains.Tools;
using UnityEngine;


[CreateAssetMenu(fileName = "Action_TakeBall", menuName = "Data/AI/StealBall/Actions/ActionTakeBall", order = 1)]
public class ActionStealTakeBall : AIAction
{

    protected override void OneTimeAction(AIBrain _brain)
    {
        base.OneTimeAction(_brain);
        _brain.PlayerStealBall.TakeBall(_brain.PlayerStealBall.GetTargetBall());
    }
}