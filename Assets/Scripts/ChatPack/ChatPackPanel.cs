using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

public class ChatPackPanel : MonoBehaviour
{
    public static ChatPackPanel Instance;

    [SerializeField]
    private GameObject ChatPan, AvatarPan,SocialPanel, BuyingItemPanel, AvatarImage, ChatPackImage;

    [SerializeField]
    private GameObject[] PackSelection;

    [SerializeField]
    private Text BuyItemText;

    private ProfileSaver profileSaver = new ProfileSaver();
    private MyPacks myPacks;

    private void Awake()
    {
        Instance = this;
    }
    
    public void Show()
    {
        AvatarImage.SetActive(false);
        ChatPackImage.SetActive(true);
        if (IsInternetConnected())
        {
            StartCoroutine(InitializePanel());
            AvatarPan.SetActive(false);
            ChatPan.SetActive(true);
            SocialPanel.SetActive(true);
        }
        else
        {
            AvatarPan.SetActive(true);
            ChatPan.SetActive(false);
            SocialPanel.SetActive(false);
            InfoPanel.Instance.SetText("please connect to internet first");
            InfoPanel.Instance.ShowInfoPanel();
        }
    }

    private IEnumerator InitializePanel()
    {
        BuyingItemPanel.SetActive(true);
        BuyItemText.text = "loading\n Just a moment...";

        PlayerProfile playerProfile = profileSaver.LoadProfile();

        Debug.Log(playerProfile.UID);
        //download my packs
        DatabaseController.Instance.ResetAsyncResult();
        DatabaseController.Instance.DownloadMyPacks(playerProfile.UID);
        yield return new WaitWhile(() => DatabaseController.Instance.GetAsyncResult() == 0);
        if (DatabaseController.Instance.GetJson() != null)
        {
            Debug.Log("downloaded my packs");
            myPacks = JsonUtility.FromJson<MyPacks>(DatabaseController.Instance.GetJson());
            profileSaver.SaveMyPacks(myPacks);
        }
        else
        {
            AvatarPan.SetActive(true);
            ChatPan.SetActive(false);
            SocialPanel.SetActive(false);
            InfoPanel.Instance.SetText("Something went wrong please check your internet connection and try again");
            InfoPanel.Instance.ShowInfoPanel();
            yield break;
        }

        BuyingItemPanel.SetActive(false);
        List<int> boughtIndexes = new List<int>();

        foreach(P p in myPacks.packs)
        {
            PackSelection[p.id].transform.Find("UseButton").GetComponent<Button>().onClick.RemoveAllListeners();
            if (p.s == true)
            {
                PackSelection[p.id].transform.Find("UseButton").transform.Find("Text").GetComponent<Text>().text = "Selected";
                PackSelection[p.id].transform.Find("UseButton").GetComponent<Button>().onClick.AddListener(() => { SelectPack(p.id); });
            }
            else
            {
                PackSelection[p.id].transform.Find("UseButton").transform.Find("Text").GetComponent<Text>().text = "Use";
                PackSelection[p.id].transform.Find("UseButton").GetComponent<Button>().onClick.AddListener(() => { SelectPack(p.id); });
            }
            boughtIndexes.Add(p.id);
        }


        //set remaining that are not bought yet
        for (int i=0;i<PackSelection.Length;i++)
        {
            if(!boughtIndexes.Contains(i))
            {
                Debug.Log("not bought " + i);
                PackSelection[i].transform.Find("UseButton").transform.Find("Text").GetComponent<Text>().text = "Buy (1000 coins)";
                int j = i;
                PackSelection[i].transform.Find("UseButton").GetComponent<Button>().onClick.AddListener(() => { BuyPack(j); });
            }
        }
    }

    public void BuyPack(int id)
    {
        Debug.Log("buy pack " + id);
        GameObject btn = EventSystem.current.currentSelectedGameObject;
        PlayerProfile playerProfile = profileSaver.LoadProfile();

        if(IsInternetConnected())
        {
            if (playerProfile.pD.Gld >= 1000)
            {
                BuyingItemPanel.SetActive(true);
                StartCoroutine(BuyItem(id, playerProfile, btn));
            }
            else
            {
                InfoPanel infoPanel = FindObjectOfType<InfoPanel>();
                infoPanel.SetText("You must have atleast 1000 coins to buy this pack");
                infoPanel.ShowInfoPanel();
            }
        }
        else
        {
            InfoPanel.Instance.SetText("please connect to internet first");
            InfoPanel.Instance.ShowInfoPanel();
        }
    }

    IEnumerator BuyItem(int id,PlayerProfile playerProfile,GameObject btn)
    {
        P p = new P();
        p.id = id;
        p.s = true;

        MyPacks tempPack = myPacks;
        tempPack.packs.Add(p);

        DatabaseController.Instance.ResetAsyncResult();
        
        DatabaseController.Instance.AddMyPacks(playerProfile.UID, tempPack);

        yield return new WaitUntil(() => DatabaseController.Instance.GetAsyncResult() != 0);

        if(DatabaseController.Instance.GetAsyncResult()==3)
        {
            playerProfile.pD.Gld -= 1000;
            myPacks.packs.Add(p);

            DatabaseController.Instance.ResetAsyncResult();
            DatabaseController.Instance.UpdateCoins(playerProfile.UID, playerProfile.pD.Gld);

            yield return new WaitUntil(() => DatabaseController.Instance.GetAsyncResult() != 0);
            profileSaver.SaveProfile(playerProfile);
            profileSaver.SaveMyPacks(myPacks);

            ProfileController profileController = FindObjectOfType<ProfileController>();
            profileController.SetCoins(playerProfile.pD.Gld);

            btn.transform.Find("Text").GetComponent<Text>().text = "Selected";
            btn.GetComponent<Button>().onClick.RemoveAllListeners();
            btn.GetComponent<Button>().onClick.AddListener(() => { SelectPack(id); });

            BuyingItemPanel.SetActive(false);

            InfoPanel.Instance.SetText("Language Pack Successfully bought");
            InfoPanel.Instance.ShowInfoPanel();
        }
        else
        {
            InfoPanel.Instance.SetText("Sorry buying item was unsuccessfull try again");
            InfoPanel.Instance.ShowInfoPanel();
        }
    }

    public void SelectPack(int id)
    {
        Debug.Log("select pack " + id);
        GameObject btn = EventSystem.current.currentSelectedGameObject;
        PlayerProfile playerProfile = profileSaver.LoadProfile();

        int totalSelected = 0;
        bool isSelected = false;
        foreach (P p in myPacks.packs)
        {
            if(p.s==true)
            {
                if(p.id==id)
                {
                    isSelected = true;
                }
                totalSelected++;
            }
        }

        if(totalSelected==0)
        {
            foreach(P p in myPacks.packs)
            {
                if (p.id == id)
                {
                    if (p.s == true)
                    {
                        p.s = false;
                        btn.transform.Find("Text").GetComponent<Text>().text = "Use";
                    }
                    else
                    {
                        p.s = true;
                        btn.transform.Find("Text").GetComponent<Text>().text = "Selected";
                    }
                    DatabaseController.Instance.AddMyPacks(playerProfile.UID, myPacks);
                    profileSaver.SaveMyPacks(myPacks);
                    break;
                }
            }
        }
        else if(totalSelected==1 && isSelected)
        {
            return;
        }
        else if(totalSelected>=1)
        {
            foreach(P p in myPacks.packs)
            {
                if (p.id == id)
                {
                    if (p.s == true)
                    {
                        p.s = false;
                        btn.transform.Find("Text").GetComponent<Text>().text = "Use";
                    }
                    else
                    {
                        p.s = true;
                        btn.transform.Find("Text").GetComponent<Text>().text = "Selected";
                    }
                    DatabaseController.Instance.AddMyPacks(playerProfile.UID, myPacks);
                    profileSaver.SaveMyPacks(myPacks);
                    break;
                }
            }
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
