using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileInitializer : MonoBehaviour
{
    public static ProfileInitializer Instance;
    [SerializeField]
    private Image[] PlayerImages;

    [SerializeField]
    private Text[] PlayerNames;

    [SerializeField]
    private Text[] Playerlevels;

    [SerializeField]
    private Text Bet;

    [SerializeField]
    private GameObject[] LevelImages,PlayerProfiles;

    [SerializeField]
    private GameObject ChatButton;

    [SerializeField]
    private GameObject BetImage;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowLevel()
    {
        foreach(GameObject level in LevelImages)
        {
            level.SetActive(true);
        }
    }

    public void HideLevel()
    {
        foreach (GameObject level in LevelImages)
        {
            level.SetActive(false);
        }
    }

    public void ShowBet()
    {
        BetImage.SetActive(true);
    }

    public void HideBet()
    {
        BetImage.SetActive(false);
    }

    public void ShowChatButton()
    {
        ChatButton.SetActive(true);
    }

    public void HideChatButton()
    {
        ChatButton.SetActive(false);
    }

    public void SetPlayerName(string name,int index)
    {
        PlayerNames[index].text = name;
    }

    public void SetPlayerLevel(string level,int index)
    {
        Playerlevels[index].text = level;
    }

    public void SetPlayerImage(string avatarId,int index)
    {
        bool found = false;
        foreach (Sprite sprite in Avatars.Instance.GetBasicAvatars())
        {
            if (sprite.name.Equals(avatarId))
            {
                found = true;
                PlayerImages[index].sprite = sprite;
            }
        }
        if (!found)
        {
            foreach (Sprite sprite in Avatars.Instance.GetPremiumAvatars())
            {
                if (sprite.name.Equals(avatarId))
                {
                    found = true;
                    PlayerImages[index].sprite = sprite;
                }
            }
        }
    }

    public void SetBet(int bet)
    {
        Bet.text = bet.ToString("n0");
    }

    public void ShowProfile()
    {
        PlayerProfiles[0].SetActive(true);
        PlayerProfiles[1].SetActive(true);
    }

    public void HideProfile()
    {
        PlayerProfiles[0].SetActive(false);
        PlayerProfiles[1].SetActive(false);
    }
}
