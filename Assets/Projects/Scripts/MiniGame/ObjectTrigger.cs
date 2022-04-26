using UnityEngine;
using UnityEngine.Events;

namespace MiniGame
{
    public class ObjectTrigger : MonoBehaviour
    {
        [SerializeField] private UnityEvent<Collider2D> triggerEnter2DEvent,triggerStay2DEvent,triggerExit2DEvent;
        private void OnTriggerEnter2D(Collider2D other)
        {
            triggerEnter2DEvent.Invoke(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            triggerExit2DEvent.Invoke(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            triggerStay2DEvent.Invoke(other);
        }
    }
}