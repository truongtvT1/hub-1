using System;
using System.Collections;
using UnityEngine;
#if UNITY_ANDROID && USING_IN_APP_REVIEW
using Google.Play.Review;
#endif

namespace ThirdParties.Truongtv.Rating
{
    public class RatingHelper : MonoBehaviour
    {
        public void Rate()
        {
#if UNITY_ANDROID
            #if USING_IN_APP_REVIEW
            StartCoroutine(RequestReviewInfo());
            #else
            Application.OpenURL("market://details?id=" + Application.identifier);
            #endif
#elif UNITY_IOS
                var result = UnityEngine.iOS.Device.RequestStoreReview();
                if (!result)
                {
                    Application.OpenURL("https://apps.apple.com/app/id" + Application.identifier);
                }
#endif
        }
#if UNITY_ANDROID && USING_IN_APP_REVIEW
        private static ReviewManager _reviewManager;
        private static PlayReviewInfo _playReviewInfo;

        private IEnumerator RequestReviewInfo() // call this before show in app review
        {
            _reviewManager = new ReviewManager();

            var requestFlowOperation = _reviewManager.RequestReviewFlow();
            yield return requestFlowOperation;
            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                yield break;
            }

            _playReviewInfo = requestFlowOperation.GetResult();
            var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
            yield return launchFlowOperation;
            _playReviewInfo = null;
            if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            {
                yield break;
            }
        }

#endif
    }
}