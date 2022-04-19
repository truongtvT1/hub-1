using UnityEngine;
using UnityEngine.Events;

namespace MiniGame
{
    public class ObjectCollision : MonoBehaviour
    {
        [SerializeField] private UnityEvent<Collision2D> collisionEnter2DEvent,collisionStay2DEvent,collisionExit2DEvent;
        private void OnCollisionEnter2D(Collision2D other)
        {
            collisionEnter2DEvent?.Invoke(other);
        }
        private void OnCollisionExit2D(Collision2D other)
        {
            collisionExit2DEvent?.Invoke(other);
        }
        private void OnCollisionStay2D(Collision2D other)
        {
            collisionStay2DEvent?.Invoke(other);
        }
    }
}