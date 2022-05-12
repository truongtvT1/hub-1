using System;
using UnityEngine;

namespace MiniGame.MemoryMatter
{
    public class AutoDisableOnInvisible : MonoBehaviour
    {
        private void OnBecameInvisible()
        {
            transform.parent.gameObject.SetActive(false);
        }
    }
}