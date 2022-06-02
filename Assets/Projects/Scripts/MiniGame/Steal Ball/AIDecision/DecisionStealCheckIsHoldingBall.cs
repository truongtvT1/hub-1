using MoreMountains.Tools;
using UnityEngine;

[CreateAssetMenu(fileName = "Decision_DetectIsHoldingBall", menuName = "Data/AI/StealBall/Decision/DetectIsHoldingBall", order = 1)]
public class DecisionStealCheckIsHoldingBall : AIDecision
{
    public override bool Decide(AIBrain _brain)
    {
        return _brain.PlayerStealBall.IsHoldingBall;
    }
}