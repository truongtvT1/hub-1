using System;
using UnityEngine;

namespace MiniGame
{
    public class FollowObjectOffset : MonoBehaviour
    {
        public Vector2 offset;
        public Transform targetTransform;

        private void Update()
        {
            transform.position = (Vector2) targetTransform.position + offset;
        }
    }
}