using MoreMountains.Tools;
using UnityEngine;

[CreateAssetMenu(fileName = "Decision_DetectTarget", menuName = "Data/AI/Memory/Decision/DetectTarget", order = 1)]
public class DecisionDetectTarget : AIDecision
{
    
    public override bool Decide(AIBrain _brain)
    {
        return _brain.PlayerController.GetTarget() != null;
    }
}
