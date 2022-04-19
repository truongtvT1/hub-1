using MoreMountains.Tools;
using UnityEngine;

[CreateAssetMenu(fileName = "Action_SwitchRandomTarget", menuName = "Data/AI/Actions/ActionSwitchTarget", order = 1)]
public class ActionSwitchRandomTargetBot : AIAction
{
    protected override void OneTimeAction(AIBrain _brain)
    {
        base.OneTimeAction(_brain);
        _brain.PlayerController.SwitchRandomTarget();
    }
}