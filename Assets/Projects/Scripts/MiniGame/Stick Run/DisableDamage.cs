using UnityEngine;

namespace MiniGame.StickRun
{
    public class DisableDamage : MonoBehaviour
    {
        public GameObject[] target;

        public void Enable()
        {
            for (int i = 0; i < target.Length; i++)
            {
                target[i].GetComponent<Collider2D>().enabled = true;
            }
        }

        public void Disable()
        {
            for (int i = 0; i < target.Length; i++)
            {
                target[i].GetComponent<Collider2D>().enabled = false;
            }
        }
    }
}