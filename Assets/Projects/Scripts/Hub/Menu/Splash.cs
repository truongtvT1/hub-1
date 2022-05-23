using System;
using Projects.Scripts.Hub;
using ThirdParties.Truongtv;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour
{
    private void Start()
    {
        if (GameDataManager.Instance.IsUserCreate())
        {
            GameDataManager.Instance.LoadUserInfo();
        }
        else
        {
            GameDataManager.Instance.CreateUserInfo();
        }

        Loading.Instance.LoadMenu();
    }
}
