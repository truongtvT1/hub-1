using System;
using UnityEngine;

namespace MiniGame.Steal_Ball
{
    public class Ball : MonoBehaviour
    {
        public State state = State.Available;
        public GameObject shadow;

        private void Awake()
        {
            shadow.SetActive(false);
        }

        
        
        [Serializable]
        public enum State
        {
            Unavailable,
            Available
        }

        public void Take()
        {
            state = State.Unavailable;
            GetComponent<UpdateZByY>().enabled = false;
            transform.localRotation = Quaternion.Euler(Vector3.zero);
            shadow.SetActive(false);
        }

        public void Release()
        {
            state = State.Available;
            GetComponent<UpdateZByY>().enabled = true;
            transform.localRotation = Quaternion.Euler(Vector3.zero);
            shadow.SetActive(true);
        }
    }
}