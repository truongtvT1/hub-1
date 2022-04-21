using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

[CreateAssetMenu(fileName = "BrainStateData", menuName = "Data/AI/BrainData/BrainStateData", order = 1)]
public class BrainStateData : ScriptableObject
{
    public List<AIState> States;
}
