using System.Collections;
using System.Collections.Generic;
using Projects.Scripts.Data;
using TMPro;
using UnityEngine;

public class LeaderBoardItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankText, userNameText, trophyText, winText, loseText;

    public void Init()
    {
        rankText.text = string.Empty;
        userNameText.text = string.Empty;
        trophyText.text = string.Empty;
        winText.text = string.Empty;
        loseText.text = string.Empty;
    }

    public void Init(UserRanking ranking)
    {
        rankText.text = ""+ranking.rank;
        userNameText.text = ranking.userName;
        trophyText.text = ""+ranking.trophy;
        winText.text = ""+ranking.win;
        loseText.text = ""+ranking.lose;
    }
}
