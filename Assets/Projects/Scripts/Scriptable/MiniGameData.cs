using System.Collections.Generic;
using Projects.Scripts.Data;
using UnityEngine;

namespace Projects.Scripts.Scriptable
{
    [CreateAssetMenu(fileName = "MiniGameData", menuName = "Truongtv/GameData/MiniGameData", order = 0)]
    public class MiniGameData : ScriptableObject
    {
        public int maxGameCount;
        public List<MiniGameInfo> miniGameList;
    }
}