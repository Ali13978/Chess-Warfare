using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterNamePanel : MonoBehaviour
{
    [SerializeField]
    private GameObject settingsPanel,NamePanel,QuitGamePanel;
    
    public void Cross()
    {
        if(settingsPanel.activeSelf)
        {
            NamePanel.SetActive(false);
        }
        else
        {
            QuitGamePanel.SetActive(true);
        }
    }
}
