using MoreMountains.Tools;
using UnityEngine;

[CreateAssetMenu(fileName = "Action_Jump", menuName = "Data/AI/Memory/Actions/ActionJump", order = 1)]
public class ActionJump : AIAction
{
    public float maxJumpTime = 1f;
    private float jumpTime;
    public override void PerformAction(AIBrain _brain)
    {
        base.PerformAction(_brain);
        if (_brain.PlayerController.IsGrounded())
        {
            _brain.PlayerController.JumpEnd();
        }
        if (jumpTime < maxJumpTime)
        {
            jumpTime += Time.deltaTime;
        }
        else
        {
            _brain.PlayerController.JumpEnd();
            jumpTime = 0;
        }
    }

    protected override void OneTimeAction(AIBrain _brain)
    {
        base.OneTimeAction(_brain);
        Jump(_brain);
    }

    void Jump(AIBrain brain)
    {
        brain.PlayerController.JumpStart();
    }
}
