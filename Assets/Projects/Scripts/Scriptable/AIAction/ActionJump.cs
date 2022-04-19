using MoreMountains.Tools;
using UnityEngine;

[CreateAssetMenu(fileName = "Action_Jump", menuName = "Data/AI/Actions/ActionJump", order = 1)]
public class ActionJump : AIAction
{
    public float maxJumpTime = 1f;
    private float jumpTime;
    public override void PerformAction(AIBrain _brain)
    {
        base.PerformAction(_brain);
        if (jumpTime < maxJumpTime)
        {
            Jump(_brain);
            jumpTime += Time.deltaTime;
        }
        else
        {
            _brain.PlayerController.JumpEnd();
            jumpTime = 0;
        }
    }

    void Jump(AIBrain brain)
    {
        brain.PlayerController.JumpStart();
    }
}
