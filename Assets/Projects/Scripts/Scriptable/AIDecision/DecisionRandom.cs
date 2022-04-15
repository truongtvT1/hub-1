using MoreMountains.Tools;
using Truongtv.Utilities;
using UnityEngine;

[CreateAssetMenu(fileName = "Decision_Random", menuName = "Data/AI/Decision/Random", order = 1)]
public class DecisionRandom : AIDecision
{
    public int TotalChance = 10;
    public int Odds = 4;

    public override bool Decide(AIBrain _brain)
    {
        return EvaluateOdds();
    }
    
    protected virtual bool EvaluateOdds()
    {
        int dice = Extended.RollADice(TotalChance);
        bool result = (dice <= Odds);
        return result;
    }
    
}

