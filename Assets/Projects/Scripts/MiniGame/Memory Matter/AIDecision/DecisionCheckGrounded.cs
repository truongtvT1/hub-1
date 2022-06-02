using MoreMountains.Tools;
using UnityEngine;

[CreateAssetMenu(fileName = "Decision_DetectGrounded", menuName = "Data/AI/Memory/Decision/DetectGrounded", order = 1)]
public class DecisionCheckGrounded : AIDecision
{
    public override bool Decide(AIBrain _brain)
    {
        return _brain.PlayerController.IsGrounded();
    }
}