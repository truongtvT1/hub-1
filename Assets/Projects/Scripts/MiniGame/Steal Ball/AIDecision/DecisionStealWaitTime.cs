using MoreMountains.Tools;
using Truongtv.Utilities;
using UnityEngine;

[CreateAssetMenu(fileName = "Decision_WaitTime", menuName = "Data/AI/StealBall/Decision/WaitTime", order = 1)]
public class DecisionStealWaitTime : AIDecision
{
    private float timeToWait;


    public override bool Decide(AIBrain _brain)
    {
        var min = _brain.PlayerStealBall.minTimeToThink;
        var max = _brain.PlayerStealBall.maxTimeToThink;
        timeToWait = Extended.RandomFloat(min, max);
        return _brain.TimeInThisState >= timeToWait;
    }
}