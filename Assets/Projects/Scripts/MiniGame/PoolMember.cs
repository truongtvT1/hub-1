using System;
using Truongtv.Utilities;
using UnityEngine;

namespace MiniGame
{
    public class PoolMember : MonoBehaviour
    {
        public Pool pool;
        
        private void OnDestroy()
        {
            pool.RemoveChild(gameObject);
        }

        void OnDisable()
        {
            transform.localPosition = Vector3.zero;
            if (transform.childCount != 0) transform.GetChild(0).localPosition = Vector3.zero;
            pool.nextThing = gameObject;
        }
    }
}