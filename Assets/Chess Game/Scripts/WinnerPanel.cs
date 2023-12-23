using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class WinnerPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject WinnerPanelObject,Board,Plane,RematchButton,LeaderboardButton,responeTimerPanel,BackButton,Draw,Winner;

    [SerializeField]
    private Text []playerName,WinningAmount,Level;

    [SerializeField]
    private Text ResponseTimer;

    [SerializeField]
    private Image []PlayerImage;

    [SerializeField]
    private Sprite DefaultImage;

    public bool isActive()
    {
        return WinnerPanelObject.activeSelf;
    }

    public void Show()
    {
        ProfileInitializer.Instance.HideProfile();
        ProfileInitializer.Instance.HideBet();
        ProfileInitializer.Instance.HideChatButton();
        Plane.SetActive(false);
        Board.SetActive(false);
        WinnerPanelObject.SetActive(true);


    }

    public void ShowRematch()
    {
        RematchButton.SetActive(true);
    }
    
    public void HideRematch()
    {
        RematchButton.SetActive(false);
    }

    public void ShowBackButton()
    {
        BackButton.SetActive(true);
    }

    public void HideBackButton()
    {
        BackButton.SetActive(false);
    }

    public void ShowResponeTimer()
    {
        responeTimerPanel.SetActive(true);
    }

    public void HideResponseTimer()
    {
        responeTimerPanel.SetActive(false);
    }

    public void SetTimer(float timer)
    {
        ResponseTimer.text = timer.ToString();
    }

    public void ShowLeaderBoardButton()
    {
        LeaderboardButton.SetActive(true);
    }

    public void HideLeaderboardButton()
    {
        LeaderboardButton.SetActive(false);
    }

    public void Hide()
    {
        Plane.SetActive(true);
        Board.SetActive(true);
        WinnerPanelObject.SetActive(false);
    }

    public void SetPlayerName(int number,string name)
    {
        playerName[number].text = name;
    }

    public void SetWinningAmount(int number,int amount)
    {
        WinningAmount[number].text = amount.ToString();
    }

    public void SetPlayerLevel(int number,int level)
    {
        string lvl = level.ToString();
        Level[number].text = lvl;
    }

    public void SetPlayerImage(int number,string avatarId)
    {
        bool found = false;
        if(avatarId==null)
        {
            PlayerImage[number].sprite = DefaultImage;
        }
        else
        {
            foreach (Sprite sprite in Avatars.Instance.GetBasicAvatars())
            {
                if (sprite.name.Equals(avatarId))
                {
                    found = true;
                    PlayerImage[number].sprite = sprite;
                }
            }
            if (!found)
            {
                foreach (Sprite sprite in Avatars.Instance.GetPremiumAvatars())
                {
                    if (sprite.name.Equals(avatarId))
                    {
                        found = true;
                        PlayerImage[number].sprite = sprite;
                    }
                }
            }
        }
    }

    public void EnableDraw()
    {
        Draw.SetActive(true);
        Winner.SetActive(false);
    }

    public void DisableDraw()
    {
        Draw.SetActive(false);
        Winner.SetActive(true);
    }

}
