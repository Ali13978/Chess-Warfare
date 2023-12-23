using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfilePanel : MonoBehaviour
{
    [SerializeField]
    private Text TL, TW, WP;

    private void OnEnable()
    {
        ProfileSaver profileSaver = new ProfileSaver();
        PlayerProfile playerProfile = profileSaver.LoadProfile();

        TL.text = playerProfile.TL.ToString();
        TW.text = playerProfile.TW.ToString();

        int percentage = playerProfile.TL + playerProfile.TW;

        if(percentage==0)
        {
            percentage = 100;
        }
        else
        {
            percentage = (playerProfile.TW * 100) / percentage;
        }

        WP.text = percentage + "%";
    }
}
