using Firebase.Database;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;

public class LeaderBoard : MonoBehaviour
{
    [SerializeField]
    GameObject PlayerRank,Grid,LeaderBoardPanel,NoLeaderBoardText;

    [SerializeField]
    Button WorldButton;

    public void ResetGrid()
    {
        foreach (Transform child in Grid.transform)
        {
            Destroy(child.gameObject);
        }
    }
    public void Show()
    {
        if(IsInternetConnected())
        {
            LeaderBoardWorld();
            WorldButton.Select();
            LeaderBoardPanel.SetActive(true);
        }
        else
        {
            InfoPanel.Instance.SetText("Please connect to internet first");
            InfoPanel.Instance.ShowInfoPanel();
            LeaderBoardPanel.SetActive(false);
        }
    }

    public void LeaderBoardCountry()
    {
        if(IsInternetConnected())
        {
            ProfileSaver profileSaver = new ProfileSaver();

            NoLeaderBoardText.SetActive(false);
            Debug.Log("leaderboard country is called");
            ResetGrid();
            FirebaseDatabase.DefaultInstance
            .GetReference("country").Child(profileSaver.LoadProfile().C).OrderByChild("TW").LimitToLast(20) //country/pk/uid/tw
            .ValueChanged += LeaderBoardCountryValueChanged;
        }
        else
        {
            InfoPanel.Instance.SetText("Please connect to internet first");
            InfoPanel.Instance.ShowInfoPanel();
        }
    }

    public void LeaderBoardFriends()
    {
        if(IsInternetConnected())
        {
            ProfileSaver profileSaver = new ProfileSaver();
            PlayerProfile playerProfile = profileSaver.LoadProfile();
            NoLeaderBoardText.SetActive(false);
            Debug.Log("leaderboard friends is called");
            ResetGrid();
            FirebaseDatabase.DefaultInstance
                .GetReference("Friends").Child(playerProfile.UID).OrderByKey()
                .ValueChanged += LeaderBoardFriendsValueChanged;
        }
        else
        {
            InfoPanel.Instance.SetText("Please connect to internet first");
            InfoPanel.Instance.ShowInfoPanel();
        }
    }

    public void LeaderBoardMyLeague()
    {
        //ResetGrid();
        //NoLeaderBoardText.SetActive(true);
    }

    public void LeaderBoardEmpire()
    {
        //ResetGrid();
        //NoLeaderBoardText.SetActive(true);
    }

    public void LeaderBoardWorld()
    {
        if(IsInternetConnected())
        {
            NoLeaderBoardText.SetActive(false);
            Debug.Log("leaderboard world is called");
            ResetGrid();
            FirebaseDatabase.DefaultInstance
            .GetReference("us").OrderByChild("TW").LimitToLast(20)
            .ValueChanged += LeaderBoardWorldValueChanged;
        }
        else
        {
            InfoPanel.Instance.SetText("Please connect to internet first");
            InfoPanel.Instance.ShowInfoPanel();
            LeaderBoardPanel.SetActive(false);
        }
        
    }

    void LeaderBoardWorldValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        Stack<GameObject> stack = new Stack<GameObject>();
        Stack<string> userIds = new Stack<string>();

        foreach (var childSnapshot in args.Snapshot.Children)
        {
            Debug.Log("Leaders entry : " +
                    childSnapshot.Child("nm").Value.ToString() + " - " +
                    childSnapshot.Child("Lvl").Value.ToString() + " - " +
                    childSnapshot.Child("TW").Value.ToString());

            GameObject gameObject = Instantiate(PlayerRank, new Vector3(0, 0, 0), Quaternion.identity);
            
            gameObject.transform.Find("Text Area").Find("Player Name").GetComponent<Text>().text = "PLAYER NAME : "+childSnapshot.Child("nm").Value.ToString();

            string TW = childSnapshot.Child("TW").Value.ToString();
            string TL = childSnapshot.Child("TL").Value.ToString();

            int total = 0;
            int.TryParse(TW, out total);
            int totalLosses = 0;
            int.TryParse(TL, out totalLosses);
            total += totalLosses;

            gameObject.transform.Find("Text Area").Find("Games Played").GetComponent<Text>().text = "GAMES PLAYED : "+total.ToString();
            gameObject.transform.Find("Text Area").Find("Country").GetComponent<Text>().text = "COUNTRY : "+childSnapshot.Child("C").Value.ToString();
            gameObject.transform.Find("Star").Find("Text").GetComponent<Text>().text = childSnapshot.Child("TW").Value.ToString();
            gameObject.transform.Find("Player Photo").Find("star2").Find("Text").GetComponent<Text>().text = childSnapshot.Child("Lvl").Value.ToString();

            string avatarId = childSnapshot.Child("av").Value.ToString();
            bool found = false;
            foreach (Sprite sprite in Avatars.Instance.GetBasicAvatars())
            {
                if (sprite.name.Equals(avatarId))
                {
                    found = true;
                    gameObject.transform.Find("Player Photo").GetComponent<Image>().sprite = sprite;
                }
            }
            if (!found)
            {
                foreach (Sprite sprite in Avatars.Instance.GetPremiumAvatars())
                {
                    if (sprite.name.Equals(avatarId))
                    {
                        found = true;
                        gameObject.transform.Find("Player Photo").GetComponent<Image>().sprite = sprite;
                    }
                }
            }

            gameObject.transform.Find("Coins Area").Find("Text").GetComponent<Text>().text = childSnapshot.Child("pD").Child("Gld").Value.ToString();

            stack.Push(gameObject);
            userIds.Push(childSnapshot.Child("UID").ToString());
        }

        while (stack.Count > 0)
        {
            GameObject gameObject = stack.Pop();
            gameObject.transform.SetParent(Grid.transform);
            gameObject.transform.localScale = new Vector3(1, 1, 1);
        }

            //StartCoroutine(DownloadProfilePhotos(userIds,stack));
    }

    void LeaderBoardCountryValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        Stack<string> stack = new Stack<string>();

        foreach (var childSnapshot in args.Snapshot.Children)
        {
            Debug.Log("Leaders entry : " +
                    childSnapshot.Child("uid").Value.ToString());
            stack.Push(childSnapshot.Child("uid").Value.ToString());
        }

        StartCoroutine(DownloadProfiles(stack,0));
    }

    void LeaderBoardFriendsValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        Stack<string> stack = new Stack<string>();

        foreach (var childSnapshot in args.Snapshot.Children)
        {
            Debug.Log("Leaders entry : " +
                    childSnapshot.Child("uid").Value.ToString());
            stack.Push(childSnapshot.Key.ToString());
        }

        StartCoroutine(DownloadProfiles(stack,1));
    }

    //download and set data to leaderboard
    IEnumerator DownloadProfiles(Stack<string> stack,int option)
    {
        DatabaseController databaseController = FindObjectOfType<DatabaseController>();

        if(stack.Count==0)
        {
            NoLeaderBoardText.SetActive(true);
            yield break;
        }

        List<GameObject> friends = new List<GameObject>();

        while (stack.Count > 0)
        {
            // get user details of the user

            databaseController.ResetAsyncResult();

            //check profile already exist?
            databaseController.DownloadProfileTask(stack.Pop());

            yield return new WaitWhile(() => databaseController.GetAsyncResult() == 0);

            if (databaseController.GetAsyncResult() == 3)
            {
                if (databaseController.GetJson() != null)
                {
                    Debug.Log("json not null");
                    //Save the downloaded profile
                    PlayerProfile playerProfile = new PlayerProfile();
                    playerProfile = JsonUtility.FromJson<PlayerProfile>(databaseController.GetJson());

                    //add this to grid
                    GameObject gameObject = Instantiate(PlayerRank, new Vector3(0, 0, 0), Quaternion.identity);

                    gameObject.transform.Find("Text Area").Find("Player Name").GetComponent<Text>().text = "PLAYER NAME : " + playerProfile.nm;
                    gameObject.name = playerProfile.TW.ToString();
                    
                    string TW = playerProfile.TW.ToString();
                    string TL = playerProfile.TL.ToString();

                    int total = 0;
                    int.TryParse(TW, out total);
                    int totalLosses = 0;
                    int.TryParse(TL, out totalLosses);
                    total += totalLosses;

                    gameObject.transform.Find("Text Area").Find("Games Played").GetComponent<Text>().text = "GAMES PLAYED : " + total.ToString();
                    gameObject.transform.Find("Text Area").Find("Country").GetComponent<Text>().text = "COUNTRY : " + playerProfile.C.ToString();
                    gameObject.transform.Find("Star").Find("Text").GetComponent<Text>().text = playerProfile.TW.ToString();
                    gameObject.transform.Find("Player Photo").Find("star2").Find("Text").GetComponent<Text>().text = playerProfile.Lvl.ToString();

                    string avatarId = playerProfile.av;
                    bool found = false;
                    foreach (Sprite sprite in Avatars.Instance.GetBasicAvatars())
                    {
                        if (sprite.name.Equals(avatarId))
                        {
                            found = true;
                            gameObject.transform.Find("Player Photo").GetComponent<Image>().sprite = sprite;
                        }
                    }
                    if (!found)
                    {
                        foreach (Sprite sprite in Avatars.Instance.GetPremiumAvatars())
                        {
                            if (sprite.name.Equals(avatarId))
                            {
                                found = true;
                                gameObject.transform.Find("Player Photo").GetComponent<Image>().sprite = sprite;
                            }
                        }
                    }

                    gameObject.transform.Find("Coins Area").Find("Text").GetComponent<Text>().text = playerProfile.pD.Gld.ToString();
                    if(option==1)
                    {
                        friends.Add(gameObject);
                    }
                    else
                    {
                        gameObject.transform.SetParent(Grid.transform);
                        gameObject.transform.localScale = new Vector3(1, 1, 1);
                    }
                }
            }
        }

        if(option==1)
        {
            //add my id to friends
            ProfileSaver profileSaver = new ProfileSaver();
            PlayerProfile playerProfile = profileSaver.LoadProfile();

            //add this to grid
            GameObject me = Instantiate(PlayerRank, new Vector3(0, 0, 0), Quaternion.identity);

            me.transform.Find("Text Area").Find("Player Name").GetComponent<Text>().text = "PLAYER NAME : "+playerProfile.nm+" (ME)";
            me.name = playerProfile.TW.ToString();

            string TW = playerProfile.TW.ToString();
            string TL = playerProfile.TL.ToString();

            int total = 0;
            int.TryParse(TW, out total);
            int totalLosses = 0;
            int.TryParse(TL, out totalLosses);
            total += totalLosses;

            me.transform.Find("Text Area").Find("Games Played").GetComponent<Text>().text = "GAMES PLAYED : " + total.ToString();
            me.transform.Find("Text Area").Find("Country").GetComponent<Text>().text = "COUNTRY : " + playerProfile.C.ToString();
            me.transform.Find("Star").Find("Text").GetComponent<Text>().text = playerProfile.TW.ToString();
            me.transform.Find("Player Photo").Find("star2").Find("Text").GetComponent<Text>().text = playerProfile.Lvl.ToString();

            string avatarId = playerProfile.av;
            bool found = false;
            foreach (Sprite sprite in Avatars.Instance.GetBasicAvatars())
            {
                if (sprite.name.Equals(avatarId))
                {
                    found = true;
                    me.transform.Find("Player Photo").GetComponent<Image>().sprite = sprite;
                }
            }
            if (!found)
            {
                foreach (Sprite sprite in Avatars.Instance.GetPremiumAvatars())
                {
                    if (sprite.name.Equals(avatarId))
                    {
                        found = true;
                        me.transform.Find("Player Photo").GetComponent<Image>().sprite = sprite;
                    }
                }
            }

            me.transform.Find("Coins Area").Find("Text").GetComponent<Text>().text = playerProfile.pD.Gld.ToString();

            friends.Add(me);

            List<GameObject> sortedList = friends.OrderBy(go => go.name).ToList();
            foreach(GameObject aFriend in sortedList)
            {
                aFriend.transform.SetParent(Grid.transform);
                aFriend.transform.localScale = new Vector3(1, 1, 1);
            }
        }
        yield return null;
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
