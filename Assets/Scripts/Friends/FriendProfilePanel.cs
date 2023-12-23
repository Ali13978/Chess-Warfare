using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendProfilePanel : MonoBehaviour
{
    [SerializeField]
    private Text TotalWinningsText, TotalLossesText,LevelText,NameText;

    [SerializeField]
    private Image PlayerPhoto;

    [SerializeField]
    private GameObject FriendsPan;

    public void SetPanel(PlayerProfile playerProfile)
    {
        TotalWinningsText.text = playerProfile.TW.ToString();
        TotalLossesText.text = playerProfile.TL.ToString();
        LevelText.text = playerProfile.Lvl.ToString();
        NameText.text = playerProfile.nm;

        string avatarId = playerProfile.av;
        bool found = false;
        foreach (Sprite sprite in Avatars.Instance.GetBasicAvatars())
        {
            if (sprite.name.Equals(avatarId))
            {
                found = true;
                PlayerPhoto.sprite = sprite;
            }
        }
        if (!found)
        {
            foreach (Sprite sprite in Avatars.Instance.GetPremiumAvatars())
            {
                if (sprite.name.Equals(avatarId))
                {
                    found = true;
                    PlayerPhoto.sprite = sprite;
                }
            }
        }

        FriendsPan.SetActive(true);
    }
}
