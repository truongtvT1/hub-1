using System;
using UnityEngine;

namespace ThirdParties.Truongtv.BundleService
{
    public class BundleDeliveryManager : MonoBehaviour
    {
        private IBundleDeliveryService _bundleService;
        private void Start()
        {
            StartCoroutine(_bundleService.GetDownloadSize("a"));
        }
    }
}