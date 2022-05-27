using UnityEngine;

namespace MiniGame.StickRun
{
    public class Gate : MonoBehaviour
    {
        public virtual void TriggerEnter(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<StickmanPlayerController>();
                if (player)
                {
                    player.Finish();
                    if (!player.isBot)
                    {
                        StickRunGameController.Instance.Win();
                    }
                }
            }
        }
    }
}