using UnityEngine;

namespace MiniGame.Effect
{
    public class ShinyEffect : MonoBehaviour
    {
        [SerializeField] private Material _materialReflex;
        [SerializeField] private float speed;
        [SerializeField] private float interval = 5f;
        [SerializeField] private float minValue, maxValue;
        private float counter;
        private bool isActiveTween;
        private float valueReflex;
        private void OnEnable()
        {
            _materialReflex.SetFloat("_ShinyFX_Pos_1", maxValue);
            counter = interval;
            isActiveTween = false;
        }

        private void Update()
        {
            if (isActiveTween)
            {
                valueReflex += Time.deltaTime * speed;
                _materialReflex.SetFloat("_ShinyFX_Pos_1", valueReflex);
                if (valueReflex >= maxValue)
                {
                    isActiveTween = false;
                    counter = interval;
                }
            }
            if(counter<0)
                return;
            counter -= Time.deltaTime;
            if (counter <= 0)
            {
                isActiveTween = true;
                valueReflex = minValue;
            }
        }
    }
}