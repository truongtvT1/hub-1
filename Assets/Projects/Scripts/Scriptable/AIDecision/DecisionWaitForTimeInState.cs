using MoreMountains.Tools;
using Truongtv.Utilities;
using UnityEngine;

[CreateAssetMenu(fileName = "Decision_WaitTime", menuName = "Data/AI/Decision/WaitTime", order = 1)]
public class DecisionWaitForTimeInState : AIDecision
{
    public bool randomTime = false;
    public float timeToWait;


    public override bool Decide(AIBrain _brain)
    {
        if (randomTime)
        {
            var min = _brain.PlayerController.minTimeToThink;
            var max = _brain.PlayerController.maxTimeToThink;
            timeToWait = Extended.RandomFloat(min, max);
            return _brain.TimeInThisState >= timeToWait;
        }
        return _brain.TimeInThisState >= timeToWait;
    }
}
