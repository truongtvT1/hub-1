using TMPro;
using UnityEngine;

namespace Projects.Scripts.Hub.Component
{
    public class ModeGameLauncher : MonoBehaviour
    {
        [SerializeField] private string gameId;
        [SerializeField] private TextMeshProUGUI  winText, loseText;
        [SerializeField] private GameObject mostPlayObj, favoriteObj,lastPlayObj;
    }
}