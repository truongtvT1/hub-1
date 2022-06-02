using MoreMountains.Tools;
using UnityEngine;


[CreateAssetMenu(fileName = "Decision_ReachTarget", menuName = "Data/AI/StealBall/Decision/CheckReachTarget", order = 1)]
public class DecisionStealCheckReachTarget : AIDecision
{
    public override bool Decide(AIBrain _brain)
    {
        return _brain.PlayerStealBall.CheckReachTarget();
    }
}