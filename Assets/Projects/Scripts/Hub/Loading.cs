using System.Collections;
using DG.Tweening;
using Projects.Scripts.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Projects.Scripts.Hub
{
    public class Loading : MonoBehaviour
    {
        [SerializeField] private GameObject container;
        [SerializeField] private Image background,progressBar;
        [SerializeField] private float loadingDuration;
        [SerializeField] private Ease loadingCurve;
        [SerializeField] private Sprite baseBackground;
        private static Loading _instance;
        public static Loading Instance => _instance;
        private void Awake()
        {
            if (_instance != null)
                Destroy(gameObject);
            _instance = this ;
        }
        public void LoadMenu()
        {
            background.sprite = baseBackground;
            background.SetNativeSize();
            progressBar.fillAmount = 0f;
            container.SetActive(true);
            var op = SceneManager.LoadSceneAsync("Menu");
            op.allowSceneActivation = false;
            var time =loadingDuration;
            progressBar.DOFillAmount(1, time).SetEase(loadingCurve)
                .OnComplete(() => { StartCoroutine(LoadFinish()); });
            IEnumerator LoadFinish()
            {
                op.allowSceneActivation = true;
                yield return new WaitForSeconds(0.1f);
                container.SetActive(false);
            }
        }

        public void LoadMiniGame(MiniGameInfo info,float duration =0f)
        {
            background.sprite = info.loadingBg;
            background.SetNativeSize();
            progressBar.fillAmount = 0f;
            container.SetActive(true);
            var op = SceneManager.LoadSceneAsync(info.gameScene);
            op.allowSceneActivation = false;
            var time = duration > 0f ? duration : loadingDuration;
            progressBar.DOFillAmount(1, time).SetEase(loadingCurve)
                .OnComplete(() => { StartCoroutine(LoadFinish()); });
            IEnumerator LoadFinish()
            {
                op.allowSceneActivation = true;
                yield return new WaitForSeconds(0.1f);
                container.SetActive(false);
            }
        }
    }
}
