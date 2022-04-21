using MoreMountains.Tools;
using UnityEngine;

[CreateAssetMenu(fileName = "Decision_DetectReachTarget", menuName = "Data/AI/Decision/DetectReachTarget", order = 1)]
public class DecisionCheckReachTarget : AIDecision
{
    public bool reachOrNot = true;
    
    public override bool Decide(AIBrain _brain)
    {
        return _brain.PlayerController.IsReachTarget() == reachOrNot;
    }
}
