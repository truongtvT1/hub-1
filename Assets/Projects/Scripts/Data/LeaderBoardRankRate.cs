using System;
using System.Collections.Generic;
using UnityEngine;

namespace Projects.Scripts.Data
{
    [CreateAssetMenu(fileName = "LeaderBoardRankRate", menuName = "Truongtv/GameData/LeaderBoardRankRate", order = 0)]
    public class LeaderBoardRankRate : ScriptableObject
    {
        public List<Rank> ranks;
        public int minMatchesPlayed = 25, maxMatchesPlayed = 30;
    }

    [Serializable]
    public class Rank
    {
        public float rank1Rate, rank2Rate, rank3Rate, rank4Rate;
    }
}