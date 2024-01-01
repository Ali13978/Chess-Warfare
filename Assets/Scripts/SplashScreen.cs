using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] Button loginBtn;

    // Start is called before the first frame update
    void Start()
    {
        loginBtn.onClick.AddListener(DeepLinksManager.Instance.LoginFun);

        DeepLinksManager.Instance.LoginSucc = () => {
            SceneManager.LoadScene("MainMenu");
        };
    }
}
