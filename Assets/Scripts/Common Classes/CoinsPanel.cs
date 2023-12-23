using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject Panel;
    public void CheckProfile()
    {
        //if(PlayerPrefs.GetInt("Login") == 4)
        //{
        //    InfoPanel.Instance.SetText("Please login with chesswarfare or Facebook Account");
        //    InfoPanel.Instance.ShowInfoPanel();
        //}
        //else
        //{
            Panel.SetActive(true);
        //}
    }
}
