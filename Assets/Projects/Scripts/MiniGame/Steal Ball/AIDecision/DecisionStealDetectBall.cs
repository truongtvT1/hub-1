using MoreMountains.Tools;
using UnityEngine;

[CreateAssetMenu(fileName = "Decision_DetectBall", menuName = "Data/AI/StealBall/Decision/DetectBall", order = 1)]
public class DecisionStealDetectBall : AIDecision
{
    public override bool Decide(AIBrain _brain)
    {
        return _brain.PlayerStealBall.HasBallToTarget();
    }
}