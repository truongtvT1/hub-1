using UnityEngine;

namespace MiniGame
{
    public class PoolMember : MonoBehaviour
    {
        public Pool pool;

        void OnDisable()
        {
            pool.nextThing = gameObject;
        }
    }
}