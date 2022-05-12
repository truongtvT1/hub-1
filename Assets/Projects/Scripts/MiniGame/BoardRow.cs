using TMPro;
using UnityEngine;
namespace MiniGame
{
    public class BoardRow : MonoBehaviour
    {
        public TextMeshProUGUI nameText, scoreText;
        public GameObject finishTick;

        public void UpdateScore(RankIngame rankIngame, bool isBot = true)
        {
            nameText.text = rankIngame.name;
            scoreText.text = rankIngame.score + "";
            SetThisToPlayerRank(!isBot);
        }

        public void UpdateFinish(RankIngame rankIngame, bool isBot = true)
        {
            nameText.text = rankIngame.name;
            scoreText.gameObject.SetActive(false);
            finishTick.SetActive(rankIngame.isFinish);
            SetThisToPlayerRank(!isBot);
        }
        
        private void SetThisToPlayerRank(bool enable = true)
        {
            if (enable)
            {
                nameText.color = new Color(0,234,255);
                scoreText.color = new Color(0,234,255);
            }
            else
            {
                nameText.color = Color.white;
                scoreText.color = Color.white;
            }
        }
    }
}