using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AvatarPanel : MonoBehaviour
{
    public static AvatarPanel Instance;

    [SerializeField]
    private GameObject ChatPan, AvatarPan, SocialPanel,AvatarGrid,BuyingItemPanel,AvatarImage,ChatPackImage;

    [SerializeField]
    private GameObject AvatarButtonPrefab;

    [SerializeField]
    private Text BuyItemText;

    private ProfileSaver profileSaver = new ProfileSaver();
    private MyAvatars myAvatars;
    private InfoPanel infoPanel;

    private void Awake()
    {
        Instance = this;
    }

    public void Show()
    {
        AvatarImage.SetActive(true);
        ChatPackImage.SetActive(false);
        if(IsInternetConnected())
        {
            ResetGrid();
            StartCoroutine(InitializePanel());
            AvatarPan.SetActive(true);
            ChatPan.SetActive(false);
            SocialPanel.SetActive(true);
        }
        else
        {
            infoPanel.SetText("please connect to internet and try again");
            infoPanel.ShowInfoPanel();
            AvatarPan.SetActive(true);
            ChatPan.SetActive(false);
            SocialPanel.SetActive(false);
        }
    }

    private IEnumerator InitializePanel()
    {
        BuyingItemPanel.SetActive(true);
        BuyItemText.text = "loading\n Just a moment...";

        PlayerProfile playerProfile = profileSaver.LoadProfile();
        DatabaseController.Instance.ResetAsyncResult();
        DatabaseController.Instance.DownloadMyAvatars(playerProfile.UID);
        yield return new WaitUntil(() => DatabaseController.Instance.GetAsyncResult() != 0);

        if (DatabaseController.Instance.GetJson() != null)
        {
            myAvatars = JsonUtility.FromJson<MyAvatars>(DatabaseController.Instance.GetJson());
        }
        else
        {
            infoPanel.SetText("Something went wrong please check your internet connection and try again");
            infoPanel.ShowInfoPanel();
            AvatarPan.SetActive(true);
            ChatPan.SetActive(false);
            SocialPanel.SetActive(false);
            yield break;
        }
        BuyingItemPanel.SetActive(false);

        List<int> BasicIndexes = new List<int>();
        List<int> PremiumIndexes = new List<int>();

        foreach(A a in myAvatars.avatars)
        {
            bool found = false;
            int index = 0;
            foreach(Sprite sprite in Avatars.Instance.GetBasicAvatars())
            {
                if(a.id.Equals(sprite.name))
                {
                    BasicIndexes.Add(index);
                    found = true;
                    GameObject anAvatar = Instantiate(AvatarButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                    anAvatar.transform.Find("name of avatar").GetComponent<Text>().text = a.nm;
                    anAvatar.transform.Find("use").transform.Find("Text").GetComponent<Text>().text = "Use";
                    anAvatar.transform.Find("avatar").GetComponent<Image>().sprite = sprite;
                    anAvatar.transform.SetParent(AvatarGrid.transform);
                    anAvatar.transform.localScale = new Vector3(1, 1, 1);
                    string id = sprite.name;
                    anAvatar.transform.Find("use").GetComponent<Button>().onClick.AddListener(() => { SelectAvatar(id); });
                }
                index++;
            }
            if(found==false)
            {
                index = 0;
                foreach(Sprite sprite in Avatars.Instance.GetPremiumAvatars())
                {
                    if (a.id.Equals(sprite.name))
                    {
                        PremiumIndexes.Add(index);
                        found = true;
                        GameObject anAvatar = Instantiate(AvatarButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                        anAvatar.transform.Find("name of avatar").GetComponent<Text>().text = a.nm;
                        anAvatar.transform.Find("use").transform.Find("Text").GetComponent<Text>().text = "Use";
                        anAvatar.transform.Find("avatar").GetComponent<Image>().sprite = sprite;
                        anAvatar.transform.SetParent(AvatarGrid.transform);
                        anAvatar.transform.localScale = new Vector3(1, 1, 1);
                        string id = sprite.name;
                        anAvatar.transform.Find("use").GetComponent<Button>().onClick.AddListener(() => { SelectAvatar(id); });
                    }
                    index++;
                }
            }
        }

        //set remaing that are not bought yet
        for (int i=0;i<Avatars.Instance.GetBasicAvatars().Length;i++)
        {
            if(!BasicIndexes.Contains(i))
            {
                GameObject anAvatar = Instantiate(AvatarButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                string avatarName = Avatars.Instance.GetBasicAvatars()[i].name;
                avatarName=avatarName.Replace('_', ' ');
                Debug.Log("new name"+ avatarName);

                anAvatar.transform.Find("name of avatar").GetComponent<Text>().text = avatarName;
                anAvatar.transform.Find("use").transform.Find("Text").GetComponent<Text>().text = "Buy (250 Coins)";
                anAvatar.transform.Find("avatar").GetComponent<Image>().sprite = Avatars.Instance.GetBasicAvatars()[i];
                anAvatar.transform.SetParent(AvatarGrid.transform);
                anAvatar.transform.localScale = new Vector3(1, 1, 1);
                string id = Avatars.Instance.GetBasicAvatars()[i].name;
                anAvatar.transform.Find("use").GetComponent<Button>().onClick.AddListener(() => { BuyAvatar(id,250); });
            }
        }

        for (int i = 0; i < Avatars.Instance.GetPremiumAvatars().Length; i++)
        {
            if (!PremiumIndexes.Contains(i))
            {
                GameObject anAvatar = Instantiate(AvatarButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                string avatarName = Avatars.Instance.GetPremiumAvatars()[i].name;
                avatarName=avatarName.Replace('_', ' ');
                Debug.Log("new name" + avatarName);

                anAvatar.transform.Find("name of avatar").GetComponent<Text>().text = avatarName;
                anAvatar.transform.Find("use").transform.Find("Text").GetComponent<Text>().text = "Buy (1000 Coins)";
                anAvatar.transform.Find("avatar").GetComponent<Image>().sprite = Avatars.Instance.GetPremiumAvatars()[i];
                anAvatar.transform.SetParent(AvatarGrid.transform);
                anAvatar.transform.localScale = new Vector3(1, 1, 1);
                string id = Avatars.Instance.GetPremiumAvatars()[i].name;
                anAvatar.transform.Find("use").GetComponent<Button>().onClick.AddListener(() => { BuyAvatar(id,1000); });
            }
        }
    }

    public void ResetGrid()
    {
        foreach(Transform child in AvatarGrid.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void BuyAvatar(string id,int cost)
    {
        infoPanel = FindObjectOfType<InfoPanel>();

        GameObject btn = EventSystem.current.currentSelectedGameObject;
        PlayerProfile playerProfile = profileSaver.LoadProfile();
        Debug.Log("buy " + id+" cost "+cost);
        if(IsInternetConnected())
        {
            if(playerProfile.pD.Gld>=cost)
            {
                BuyingItemPanel.SetActive(true);
                StartCoroutine(BuyItem(id, cost, playerProfile.UID,btn));
            }
            else
            {
                infoPanel.SetText("You don't have enough coins");
                infoPanel.ShowInfoPanel();
            }
        }
        else
        {
            infoPanel.SetText("Please Connect to Internet First");
            infoPanel.ShowInfoPanel();
        }
        
    }

    IEnumerator BuyItem(string id, int cost, string uid,GameObject Btn)
    {
        DatabaseController.Instance.ResetAsyncResult();

        MyAvatars TempAvatars = new MyAvatars();
        TempAvatars = myAvatars;
        A a = new A();
        a.id = id;
        a.nm = a.id.Replace('_', ' ');
        TempAvatars.avatars.Add(a);

        DatabaseController.Instance.AddMyAvatars(uid, TempAvatars);

        yield return new WaitUntil(() => DatabaseController.Instance.GetAsyncResult() != 0);

        if (DatabaseController.Instance.GetAsyncResult() == 3)
        {
            PlayerProfile playerProfile = profileSaver.LoadProfile();
            playerProfile.pD.Gld -= cost;
            profileSaver.SaveProfile(playerProfile);

            DatabaseController.Instance.ResetAsyncResult();
            DatabaseController.Instance.UpdateCoins(uid, playerProfile.pD.Gld);

            yield return new WaitUntil(() => DatabaseController.Instance.GetAsyncResult() != 0);
            ProfileController.Instance.SetCoins(playerProfile.pD.Gld);

            myAvatars = TempAvatars;

            Btn.GetComponent<Button>().onClick.RemoveAllListeners();
            Btn.GetComponent<Button>().onClick.AddListener(() => { SelectAvatar(id); });
            Btn.transform.Find("Text").GetComponent<Text>().text = "Use";
            BuyingItemPanel.SetActive(false);

            Show();

            infoPanel.SetText(a.nm + " successfully bought and added to your avatars list");
            infoPanel.ShowInfoPanel();

        }
        else
        {
            BuyingItemPanel.SetActive(false);
            infoPanel.SetText("Something went wrong avatar is not bought");
            infoPanel.ShowInfoPanel();
        }
    }

    public void SelectAvatar(string id)
    {
        infoPanel = FindObjectOfType<InfoPanel>();
        GameObject btn = EventSystem.current.currentSelectedGameObject;
        PlayerProfile playerProfile = profileSaver.LoadProfile();
        Debug.Log("select " + id);

        if(IsInternetConnected())
        {
            playerProfile.av = id;
            DatabaseController.Instance.UpdateProfileAvatar(playerProfile.UID, id);
            profileSaver.SaveProfile(playerProfile);
            btn.transform.Find("Text").GetComponent<Text>().text = "Selected";
            ProfileController.Instance.updateProfile(playerProfile);
            ProfileController.Instance.SetPlayerImage();
        }
        else
        {
            infoPanel.SetText("Please connect to Internet first");
            infoPanel.ShowInfoPanel();
            return;
        }
    }

    // to check internet connectivity
    private bool IsInternetConnected()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}