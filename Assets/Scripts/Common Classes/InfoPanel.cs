using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    public static InfoPanel Instance;

    public GameObject InfoPan;
    public Text InfoText;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowInfoPanel()
    {
        InfoPan.SetActive(true);
    }

    public void HideInfoPanel()
    {
        InfoPan.SetActive(false);
    }

    public void SetText(string text)
    {
        InfoText.text = text;
    }
}
