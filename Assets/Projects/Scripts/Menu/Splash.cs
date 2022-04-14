using System;
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

        SceneManager.LoadScene("Menu");
    }
}
