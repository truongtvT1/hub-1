using System;
using System.Collections;
using Com.LuisPedroFonseca.ProCamera2D;
using MiniGame;
using UnityEngine;

namespace Base
{
    public class ScreenShakeHelper : MonoBehaviour
    {
        [SerializeField] private ShakePreset shakePreset;
        [SerializeField] private ConstantShakePreset constantShakePreset;
        private bool _canShake = true;
        private Renderer _renderer;
        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
        }

        private void Start()
        {
            InvokeRepeating(nameof(CheckCanShake),0f,0.5f);
        }

        private void CheckCanShake()
        {
            _canShake = _renderer.isVisible;
            Debug.Log("check can shake " + _canShake);
        }

        public void Shake()
        {
            if(!_canShake) return;
            StartCoroutine(ShakeCoroutine());
        }
        public void ConstantShake()
        {
            if(!_canShake) return;
            StartCoroutine(ConstantShakeCoroutine());
        }

        private IEnumerator ConstantShakeCoroutine()
        {
            ProCamera2DShake.Instance.ConstantShake(constantShakePreset);
            yield return new WaitForSeconds(0.5f);
            ProCamera2DShake.Instance.StopConstantShaking(0f);
        }
        private IEnumerator ShakeCoroutine()
        {
            ProCamera2DShake.Instance.Shake(shakePreset);
            yield return new WaitForSeconds(0.5f);
            ProCamera2DShake.Instance.StopShaking();
        }

        private void OnBecameInvisible()
        {
            _canShake = false;
        }

        private void OnBecameVisible()
        {
            _canShake = true;
        }
    }
}