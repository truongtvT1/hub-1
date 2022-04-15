using MoreMountains.Tools;
using UnityEngine;

[CreateAssetMenu(fileName = "Action_Jump", menuName = "Data/AI/Actions/ActionJump", order = 1)]
public class ActionJump : AIAction
{
    public int maxJumpCount = 1;
    public float maxJumpTime = 1f;
    private float jumpTime;
    private int jumpCount;
    public override void PerformAction(AIBrain _brain)
    {
        base.PerformAction(_brain);
        if (jumpTime < maxJumpTime)
        {
            jumpTime += Time.deltaTime;
        }
        else
        {
            Jump(_brain);
            jumpTime = 0;
        }
    }

    protected override void OneTimeAction(AIBrain _brain)
    {
        base.OneTimeAction(_brain);
        jumpCount = 0;
        Jump(_brain);
    }

    void Jump(AIBrain brain)
    {
        if (jumpCount < maxJumpCount)
        {
            brain.PlayerController.JumpStart();
            jumpCount++;
        }
    }
}
