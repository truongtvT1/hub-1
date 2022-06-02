
using MoreMountains.Tools;
using UnityEngine;

[CreateAssetMenu(fileName = "Action_Idle", menuName = "Data/AI/Memory/Actions/Idle", order = 1)]
public class ActionIdle : AIAction
{
    
    
    protected override void OneTimeAction(AIBrain _brain)
    {
        base.OneTimeAction(_brain);
        _brain.PlayerController.Idle();
    }
}
