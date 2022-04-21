using UnityEngine;

namespace MiniGame
{
    public class Water : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D collider2D;
        [SerializeField] private Transform water;
        [SerializeField] private float deltaSurface, deltaCollider;
        [SerializeField] private BuoyancyEffector2D effect;

        private void Start()
        {
            var size = water.transform.localScale;
            size.y -= deltaCollider;
            collider2D.size = new Vector2(1, size.y / water.transform.localScale.y);
            collider2D.offset = new Vector2(0, -(1 - size.y / water.transform.localScale.y) / 2);
            size = water.transform.localScale;
            var delta = size.y / 2 - deltaSurface - deltaCollider;
            effect.surfaceLevel = delta/water.transform.localScale.y;
        }


        private void OnDrawGizmos()
        {
            if (Application.isPlaying) return;
            var size = water.transform.localScale;
            size.y -= deltaCollider;
            collider2D.size = new Vector2(1, size.y / water.transform.localScale.y);
            collider2D.offset = new Vector2(0, -(1 - size.y / water.transform.localScale.y) / 2);
            size = water.transform.localScale;
            var delta = size.y / 2 - deltaSurface - deltaCollider;
            effect.surfaceLevel = delta/water.transform.localScale.y;
        }
    }
}