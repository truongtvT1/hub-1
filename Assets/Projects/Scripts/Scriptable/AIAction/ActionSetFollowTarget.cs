using MoreMountains.Tools;
using UnityEngine;

[CreateAssetMenu(fileName = "Action_SetFollowTarget", menuName = "Data/AI/Actions/ActionSetFollowTarget", order = 1)]
public class ActionSetFollowTarget : AIAction
{
    public bool enable = true;
    
    protected override void OneTimeAction(AIBrain _brain)
    {
        base.OneTimeAction(_brain);
        _brain.PlayerController.FollowTarget(enable);
    }
}