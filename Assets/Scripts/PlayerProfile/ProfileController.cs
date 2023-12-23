using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using System.IO;

public class ProfileController : MonoBehaviour
{
    [SerializeField]
    private GameObject MainPanel,ChessWarfareAuthenticationPanel;

    [SerializeField]
    private Text [] playerName,settingsText,settingsButtonText,coinsText,LevelText;

    [SerializeField]
    private Image[] playerImage;

    [SerializeField]
    private Image ProfileProgress;

    [SerializeField]
    private Button[] SettingsButtons;

    public static ProfileController Instance;

    private LoadingPanel loadingPanel;

    private PlayerProfile playerProfile;

    private LeaderBoard leaderBoard;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        loadingPanel = FindObjectOfType<LoadingPanel>();

        leaderBoard = FindObjectOfType<LeaderBoard>();

        if (PlayerPrefs.HasKey("LB"))
        {
            leaderBoard.ResetGrid();
            leaderBoard.Show();
            PlayerPrefs.DeleteKey("LB");
            PlayerPrefs.DeleteKey("chatpack");
        }
        else if (PlayerPrefs.HasKey("chatpack"))
        {
            //show chat pack panel
            ChatPackPanel.Instance.Show();
            PlayerPrefs.DeleteKey("chatpack");
        }

        playerProfile = new PlayerProfile();

        //check game version first
        if (PlayerPrefs.HasKey("Login"))
        {
            if (PlayerPrefs.GetInt("Login") == 0)
            {
                //login guest 
                FirebaseAuthentication.Instance.LoginGuest();
            }
            else
            {
                loadingPanel.SetProgress(20);
                loadingPanel.Show();
                InitializeProfile();
            }
        }
        else
        {
            //login as guest
            loadingPanel.Show();
            loadingPanel.SetProgress(20);
            FirebaseAuthentication.Instance.ShowGuestPanel();
        }
    }

    public void InitializeProfile()
    {
        ProfileSaver profileSaver = new ProfileSaver();
        playerProfile = profileSaver.LoadProfile();

        SetPlayerName();

        SetPlayerImage();

        SetCoins(-1);

        SetLevel();

        //set settings panel
        SetSettingsPanel();

        //show banner Ad
        GoogleAdsManager.Instance.ShowBanner(2);

        MainPanel.SetActive(true);
        loadingPanel.Hide();
    }

    private void SetSettingsPanel()
    {
        int type = PlayerPrefs.GetInt("Login");
        if(type==1)
        {
            settingsText[type - 1].text = ">> LOGOUT FROM CHESS WARFARE ACCOUNT";
            settingsButtonText[type-1].text= "LOGOUT";
            SettingsButtons[0].onClick.RemoveAllListeners();
            SettingsButtons[0].onClick.AddListener(Logout);

            settingsText[3].text = ">> LOGIN GUEST";
            settingsButtonText[3].text = "LOGIN";
            SettingsButtons[3].onClick.RemoveAllListeners();
            SettingsButtons[3].onClick.AddListener(FirebaseAuthentication.Instance.ShowGuestPanel);

            settingsText[2].text = ">> LOGIN TO PLAY GAMES";
            settingsButtonText[2].text = "LOGIN";
            SettingsButtons[2].onClick.AddListener(FirebaseAuthentication.Instance.PlayGamesSignIn);

            settingsText[1].text = ">> LOGIN WITH FACEBOOK";
            settingsButtonText[1].text = "LOGIN";
            SettingsButtons[1].onClick.RemoveAllListeners();
            SettingsButtons[1].onClick.AddListener(FacebookManager.Instance.FBLogin);
        }
        else if(type ==2)
        {
            settingsText[type - 1].text = ">> LOGOUT FROM FACEBOOK ACCOUNT";
            settingsButtonText[type - 1].text = "LOGOUT";
            SettingsButtons[1].onClick.RemoveAllListeners();
            SettingsButtons[1].onClick.AddListener(Logout);

            settingsText[2].text = ">> LOGIN TO PLAY GAMES";
            settingsButtonText[2].text = "LOGIN";
            SettingsButtons[2].onClick.AddListener(FirebaseAuthentication.Instance.PlayGamesSignIn);

            settingsText[0].text = ">> CREATE CHESS WARFARE ACCOUNT";
            settingsButtonText[0].text = "LOGIN";
            SettingsButtons[0].onClick.RemoveAllListeners();
            SettingsButtons[0].onClick.AddListener(ChessWarFareAccount);

            settingsText[3].text = ">> LOGIN GUEST";
            settingsButtonText[3].text = "LOGIN";
            SettingsButtons[3].onClick.RemoveAllListeners();
            SettingsButtons[3].onClick.AddListener(FirebaseAuthentication.Instance.ShowGuestPanel);

        }
        else if(type==3)
        {
            settingsText[type - 1].text = ">> LOGOUT PLAY GAMES";
            settingsButtonText[type - 1].text = "LOGOUT";
            SettingsButtons[2].onClick.RemoveAllListeners();
            SettingsButtons[2].onClick.AddListener(Logout);

            settingsText[0].text = ">> CREATE CHESS WARFARE ACCOUNT";
            settingsButtonText[0].text = "LOGIN";
            SettingsButtons[0].onClick.RemoveAllListeners();
            SettingsButtons[0].onClick.AddListener(ChessWarFareAccount);

            settingsText[1].text = ">> LOGIN WITH FACEBOOK";
            settingsButtonText[1].text = "LOGIN";
            SettingsButtons[1].onClick.RemoveAllListeners();
            SettingsButtons[1].onClick.AddListener(FacebookManager.Instance.FBLogin);

            settingsText[3].text = ">> LOGIN GUEST";
            settingsButtonText[3].text = "LOGIN";
            SettingsButtons[3].onClick.RemoveAllListeners();
            SettingsButtons[3].onClick.AddListener(FirebaseAuthentication.Instance.ShowGuestPanel);
        }
        else if(type==4)
        {
            settingsText[type - 1].text = ">> LOGOUT GUEST";
            settingsButtonText[type - 1].text = "LOGOUT";
            SettingsButtons[3].onClick.RemoveAllListeners();
            SettingsButtons[3].onClick.AddListener(Logout);

            settingsText[2].text = ">> LOGIN TO PLAY GAMES";
            settingsButtonText[2].text = "LOGIN";
            SettingsButtons[2].onClick.AddListener(FirebaseAuthentication.Instance.PlayGamesSignIn);

            settingsText[0].text = ">> CREATE CHESS WARFARE ACCOUNT";
            settingsButtonText[0].text = "LOGIN";
            SettingsButtons[0].onClick.RemoveAllListeners();
            SettingsButtons[0].onClick.AddListener(ChessWarFareAccount);

            settingsText[1].text = ">> LOGIN WITH FACEBOOK";
            settingsButtonText[1].text = "LOGIN";
            SettingsButtons[1].onClick.RemoveAllListeners();
            SettingsButtons[1].onClick.AddListener(FacebookManager.Instance.FBLogin);
        }
    }

    public void SetPlayerName()
    {
        for(int i=0;i<playerName.Length;i++)
        {
            playerName[i].text= playerProfile.nm;
        }
    }

    public void updateProfile(PlayerProfile playerProfile)
    {
        this.playerProfile = playerProfile;
    }

    public void SetPlayerImage()
    {
        string avatarId = playerProfile.av;
        bool found = false;
        foreach(Sprite sprite in Avatars.Instance.GetBasicAvatars())
        {
            if(sprite.name.Equals(avatarId))
            {
                found = true;
                for (int i = 0; i < playerImage.Length; i++)
                {
                    playerImage[i].sprite = sprite;
                }
            }
        }
        if(!found)
        {
            foreach (Sprite sprite in Avatars.Instance.GetPremiumAvatars())
            {
                if (sprite.name.Equals(avatarId))
                {
                    found = true;
                    for (int i = 0; i < playerImage.Length; i++)
                    {
                        playerImage[i].sprite = sprite;
                    }
                }
            }
        }
    }

    public void SetLevel()
    {
        for(int i=0;i<LevelText.Length;i++)
        {
            LevelText[i].text = playerProfile.Lvl.ToString();
        }

        //calculate xp
        if(playerProfile.pD.XP==0)
        {
            ProfileProgress.fillAmount = 0.05f;
        }
        else
        {
            int level = playerProfile.Lvl / 5;
            float xp = 0;
            if(level==0)
            {
                xp = (float)playerProfile.pD.XP / (float)100;
            }
            else
            {
                xp = (float)playerProfile.pD.XP /(float) (100 + (level * 100));
            }

            ProfileProgress.fillAmount = xp;
        }
    }

    public void SetCoins(int coins)
    {
        if(coins==-1)
        {
            for (int i = 0; i < coinsText.Length; i++)
            {
                coinsText[i].text = playerProfile.pD.Gld.ToString("n0");
            }
        }
        else
        {
            for (int i = 0; i < coinsText.Length; i++)
            {
                coinsText[i].text = coins.ToString("n0");
            }
        }
        
    }

    public void ChessWarFareAccount()
    {
        FirebaseAuthentication.Instance.InitEmailDropdown();
        ChessWarfareAuthenticationPanel.SetActive(true);
    }

    public void Logout()
    {
        Debug.Log("Logged out");
        PlayerPrefs.DeleteAll();
        Application.Quit();
    }

    public void PlayTap()
    {
        SoundManager.instance.PlayTap();
    }
}