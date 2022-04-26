using UnityEngine;

namespace MiniGame
{
    public class DamageToPlayer : MonoBehaviour
    {
        [SerializeField] private DamageType damageType;
        [SerializeField] private bool lostLifeWhenDie = true;
        [SerializeField] private int damage;
        
        public virtual void TriggerEnter(Collider2D other)
        {
            if (other.gameObject.tag.Equals("Player"))
            {
                other.GetComponent<PlayerController>().Damage(damageType,damage,lostLifeWhenDie);
            }
        }

        public virtual void OnCollide(Collision2D other)
        {
            if (other.gameObject.tag.Equals("Player"))
            {
                other.collider.GetComponent<PlayerController>().Damage(damageType,damage,lostLifeWhenDie);
            }
        }

    }
}