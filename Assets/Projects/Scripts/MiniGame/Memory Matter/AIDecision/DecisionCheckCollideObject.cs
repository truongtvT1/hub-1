using MoreMountains.Tools;
using UnityEngine;

[CreateAssetMenu(fileName = "Decision_DetectCollideObject", menuName = "Data/AI/Memory/Decision/DetectCollideObject", order = 1)]
public class DecisionCheckCollideObject : AIDecision
{
    public override bool Decide(AIBrain _brain)
    {
        return _brain.PlayerController.IsCollidingObject();
    }
}