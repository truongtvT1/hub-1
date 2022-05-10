using System;
using System.Collections;
using UnityEngine;

namespace MiniGame.MemoryMatter
{
    public class AutoDisable : MonoBehaviour
    {
        public float duration = 1;

        private float timeCount;

        private void Update()
        {
            if (timeCount < duration)
            {
                timeCount += Time.deltaTime;
            }
            else
            {
                gameObject.SetActive(false);
                timeCount = 0;
            }
        }
    }
}