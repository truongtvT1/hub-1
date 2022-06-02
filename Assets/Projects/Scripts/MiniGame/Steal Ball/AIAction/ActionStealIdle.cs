using MoreMountains.Tools;
using UnityEngine;

[CreateAssetMenu(fileName = "Action_Idle", menuName = "Data/AI/StealBall/Actions/ActionIdle", order = 1)]
public class ActionStealIdle : AIAction
{

    protected override void OneTimeAction(AIBrain _brain)
    {
        base.OneTimeAction(_brain);
        _brain.PlayerStealBall.Idle();
    }
}