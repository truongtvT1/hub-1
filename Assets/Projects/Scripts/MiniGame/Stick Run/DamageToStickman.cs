using UnityEngine;

namespace MiniGame.StickRun
{
    public class DamageToStickman : MonoBehaviour
    {
        [SerializeField] private DamageType damageType;
        [SerializeField] private Transform checkPoint;
        
        public virtual void TriggerEnter(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<StickmanPlayerController>();
                if (player)
                {
                    player.Die(damageType, checkPoint,player.transform.position);
                }
            }
        }

        public virtual void OnCollide(Collision2D other)
        {
            if (other.gameObject.tag.Equals("Player"))
            {
                var player = other.gameObject.GetComponent<StickmanPlayerController>();
                if (player)
                {
                    player.Die(damageType, checkPoint,player.transform.position);
                }
            }
        }

    }
}