using System.Collections;
using System.Collections.Generic;
using Projects.Scripts.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankText, userNameText, trophyText, winText, loseText;
    private Vector2 _startPos;
    private ScrollRect _scroll;
    private float _moveLength;
    private float move = 20f/240;
    private RectTransform rect;
    
    
    public void UpdateLayoutPosition(Vector2 normalize)
    {
        var distanceMove = (1 - normalize.y) * _moveLength;
        var pos = rect.localPosition;
        pos.x = _startPos.x + distanceMove * move;
        rect.localPosition = pos;
    }
    
    public void Init(UserRanking ranking,Vector2 pos,ScrollRect scroll)
    {
        rect = GetComponent<RectTransform>();
        rect.localPosition = pos;
        _startPos = pos;
        _scroll = scroll;
        _moveLength = _scroll.content.sizeDelta.y- _scroll.GetComponent<RectTransform>().sizeDelta.y;
        if (ranking != null)
        {
            rankText.text = ""+ranking.rank;
            userNameText.text = ranking.userName;
            trophyText.text = ""+ranking.trophy;
            winText.text = ""+ranking.win;
            loseText.text = ""+ranking.lose;
        }
        else
        {
            rankText.text = string.Empty;
            userNameText.text = string.Empty;
            trophyText.text = string.Empty;
            winText.text = string.Empty;
            loseText.text = string.Empty;
        }
    }
}
