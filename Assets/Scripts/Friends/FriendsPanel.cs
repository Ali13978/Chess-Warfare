using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendsPanel : MonoBehaviour
{
    [SerializeField]
    private Button FriendsButton, FriendRequestsButton,NewFriendButton;

    [SerializeField]
    private GameObject FriendsPan, FriendRequestsPan,MidPanel,LoadingText,MainPanel,FriendsButtonPrefab,NoFriendsText,NoFriendRequestText,NoUserFoundText,NewFriendsPan;

    [SerializeField]
    private GameObject FriendsGrid, RequestsGrid,FindUserGrid,LoadingPanel;

    [SerializeField]
    private InputField SearchFieldFriends, SearchFieldNewFriends;

    private int TextCount = 0;
    private int TextCount2 = 0;

    private PlayerProfile playerProfile;
    private List<PlayerProfile> Friends = new List<PlayerProfile>();

    private void OnEnable()
    {
        ProfileSaver profileSaver = new ProfileSaver();
        playerProfile = profileSaver.LoadProfile();

        if(playerProfile.pD.AT.Equals("GU"))
        {
            InfoPanel.Instance.SetText("Please login with chesswarfare or facebook account to connect with friends");
            InfoPanel.Instance.ShowInfoPanel();
            MainPanel.SetActive(false);
            return;
        }

        if (IsInternetConnected())
        {
            ResetGrid(1);
            LoadingText.SetActive(true);
            MidPanel.SetActive(true);
            NoFriendsText.SetActive(false);

            Debug.Log("my uid " + playerProfile.UID);
            //download my friends list from database
            FirebaseDatabase.DefaultInstance
            .GetReference("Friends").Child(playerProfile.UID).OrderByKey()
            .ValueChanged += FriendsValueChanged;
        }
        else
        {
            InfoPanel.Instance.SetText("Please Connect to Internet First");
            InfoPanel.Instance.ShowInfoPanel();
            MainPanel.SetActive(false);
        }
    }

    private void ResetGrid(int Option)
    {
        if(Option==1)
        {
            foreach (Transform child in FriendsGrid.transform)
            {
                Destroy(child.gameObject);
            }
        }
        else if(Option==2)
        {
            foreach (Transform child in RequestsGrid.transform)
            {
                Destroy(child.gameObject);
            }
        }
        else if(Option==3)
        {
            foreach (Transform child in FindUserGrid.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void SelectFriends()
    {
        Color color = new Color32(159, 114, 43, 255);
        
        FriendRequestsButton.GetComponent<Image>().color = color;
        NewFriendButton.GetComponent<Image>().color = color;
        FriendsButton.GetComponent<Image>().color = new Color(0, 0, 0);

        ResetGrid(1);

        FriendsPan.SetActive(true);
        FriendRequestsPan.SetActive(false);
        NewFriendsPan.SetActive(false);
    }

    public void SelectFriendRequests()
    {
        Color color = new Color32(159, 114, 43, 255);
        FriendsButton.GetComponent<Image>().color = color;
        NewFriendButton.GetComponent<Image>().color = color;
        FriendRequestsButton.GetComponent<Image>().color = new Color(0, 0, 0);

        ResetGrid(2);

        FriendsPan.SetActive(false);
        NewFriendsPan.SetActive(false);
        FriendRequestsPan.SetActive(true);

        //download friend requests from DB
        FirebaseDatabase.DefaultInstance
            .GetReference("Requests").Child(playerProfile.UID).OrderByKey()
            .ValueChanged += RequestsValueChanged;

    }

    public void SelectNewFriends()
    {
        Color color = new Color32(159, 114, 43, 255);

        FriendRequestsButton.GetComponent<Image>().color = color;
        FriendsButton.GetComponent<Image>().color = color;
        NewFriendButton.GetComponent<Image>().color = new Color(0, 0, 0);

        ResetGrid(3);

        FriendsPan.SetActive(false);
        FriendRequestsPan.SetActive(false);
        NewFriendsPan.SetActive(true);
    }

    void RequestsValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.Log(args.DatabaseError.Message);

            InfoPanel.Instance.SetText("Something went wrong please try again");
            InfoPanel.Instance.ShowInfoPanel();
            NoFriendRequestText.SetActive(true);
            return;
        }

        Stack<string> userIds = new Stack<string>();

        foreach (var childSnapshot in args.Snapshot.Children)
        {
            Debug.Log("Friends : " +
                    childSnapshot.Value.ToString());
            Debug.Log("Keys : " + childSnapshot.Key.ToString());

            userIds.Push(childSnapshot.Key.ToString());
        }

        //Debug.Log(userIds.Count);
        StartCoroutine(DownloadProfiles(userIds, 2));
    }

    private void OnGUI()
    {
        //for searching my friends
        if (SearchFieldFriends.isFocused && SearchFieldFriends.text.Length!=TextCount)
        {
            TextCount = SearchFieldFriends.text.Length;
            Debug.Log("pressed enter");
            foreach (Transform child in FriendsGrid.transform)
            {
                child.gameObject.SetActive(true);
            }
            
            if(SearchFieldFriends.text=="")
            {
                return;
            }

            foreach (Transform child in FriendsGrid.transform)
            {
                string ChildName = child.name.ToLower();
                string TextToSearch = SearchFieldFriends.text.ToLower();

                Debug.Log(ChildName);
                Debug.Log(TextToSearch);
                if (!ChildName.Contains(TextToSearch))
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        //for searching new friend
        if (SearchFieldNewFriends.isFocused && /*Input.GetKey(KeyCode.KeypadEnter)*/ SearchFieldNewFriends.text.Length != TextCount2)
        {
            TextCount2 = SearchFieldNewFriends.text.Length;
            string emailToSearch = SearchFieldNewFriends.text.ToLower();
            Debug.Log("email to search " + emailToSearch);
            if (emailToSearch.Equals(playerProfile.pD.em.ToLower())) //if searching this user as friend
            {
                LoadingText.SetActive(false);
                NoUserFoundText.SetActive(true);
                return;
            }

            foreach(PlayerProfile friend in Friends)
            {
                if(friend.pD.em.Equals(emailToSearch))
                {
                    LoadingText.SetActive(false);
                    NoUserFoundText.SetActive(true);
                    return;
                }
            }
            //search email id
            FirebaseDatabase.DefaultInstance
            .GetReference("MyEmail").OrderByChild("em").EqualTo(emailToSearch)
            .ValueChanged += HandleValueChanged;
        }
    }

    void FriendsValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.Log(args.DatabaseError.Message);
            InfoPanel.Instance.SetText("Something went wrong please try again");
            InfoPanel.Instance.ShowInfoPanel();
            MainPanel.SetActive(false);
            return;
        }

        Stack<string> userIds = new Stack<string>();

        foreach (var childSnapshot in args.Snapshot.Children)
        {
            Debug.Log("Friends : " +
                    childSnapshot.Value.ToString());

            userIds.Push(childSnapshot.Key.ToString());
        }

        Debug.Log(userIds.Count);
        StartCoroutine(DownloadProfiles(userIds,1));
    }

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        Debug.Log("snapshot " + args.Snapshot.ChildrenCount);

        Stack<string> userIds = new Stack<string>();
        foreach (var childSnapshot in args.Snapshot.Children)
        {
            Debug.Log("Leaders entry : " +
                    childSnapshot.Child("uid").Value.ToString());

            userIds.Push(childSnapshot.Child("uid").Value.ToString());
        }
        StartCoroutine(DownloadProfiles(userIds, 3));
    }

    //download and set data to leaderboard
    IEnumerator DownloadProfiles(Stack<string> stack,int option)
    {
        LoadingText.SetActive(false);
        if (stack.Count == 0)
        {
            if(option==1)
            {
                NoFriendsText.SetActive(true);
            }
            else if(option==2)
            {
                NoFriendRequestText.SetActive(true);
            }
            else if(option==3)
            {
                NoUserFoundText.SetActive(true);
            }
            Debug.Log("count 0");
            yield break;
        }

        if(option==1)
        {
            NoFriendsText.SetActive(false);
        }
        else if(option==2)
        {
            NoFriendRequestText.SetActive(false);
        }
        else if(option==3)
        {
            NoUserFoundText.SetActive(false);
        }

        Friends.Clear();

        while (stack.Count > 0)
        {
            // get user details of the user

            DatabaseController.Instance.ResetAsyncResult();

            //Download profile
            DatabaseController.Instance.DownloadProfileTask(stack.Pop());

            yield return new WaitWhile(() => DatabaseController.Instance.GetAsyncResult() == 0);

            if (DatabaseController.Instance.GetAsyncResult() == 3)
            {
                if (DatabaseController.Instance.GetJson() != null)
                {
                    Debug.Log("json not null");
                    //Save the downloaded profile
                    PlayerProfile playerProfile = new PlayerProfile();
                    playerProfile = JsonUtility.FromJson<PlayerProfile>(DatabaseController.Instance.GetJson());

                    //add this to grid
                    GameObject gameObject = Instantiate(FriendsButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity);

                    gameObject.name = playerProfile.nm;
                    gameObject.transform.Find("Heading").GetComponent<Text>().text = playerProfile.nm;

                    gameObject.transform.Find("Avatar").Find("level").Find("Text").GetComponent<Text>().text = playerProfile.Lvl.ToString();

                    if(option==1)
                    {
                        Friends.Add(playerProfile);
                        gameObject.transform.Find("Action").Find("Text").GetComponent<Text>().text = "View Profile";
                        string uid = playerProfile.UID;
                        gameObject.transform.Find("Action").GetComponent<Button>().onClick.AddListener(() => { ViewProfile(uid); });
                        gameObject.transform.SetParent(FriendsGrid.transform);
                    }
                    else if(option==2)
                    {
                        gameObject.transform.Find("Action").Find("Text").GetComponent<Text>().text = "Accept Friend Request";
                        string uid = playerProfile.UID;
                        gameObject.transform.Find("Action").GetComponent<Button>().onClick.AddListener(() => { AcceptFriendRequest(uid); });
                        gameObject.transform.SetParent(RequestsGrid.transform);
                    }
                    else if(option==3)
                    {
                        gameObject.transform.Find("Action").Find("Text").GetComponent<Text>().text = "Send Friend Request";
                        string uid = playerProfile.UID;
                        gameObject.transform.Find("Action").GetComponent<Button>().onClick.AddListener(() => { SendFriendRequest(uid); });
                        gameObject.transform.SetParent(FindUserGrid.transform);
                    }

                    string avatarId = playerProfile.av;
                    bool found = false;
                    foreach (Sprite sprite in Avatars.Instance.GetBasicAvatars())
                    {
                        if (sprite.name.Equals(avatarId))
                        {
                            found = true;
                            gameObject.transform.Find("Avatar").GetComponent<Image>().sprite = sprite;
                        }
                    }
                    if (!found)
                    {
                        foreach (Sprite sprite in Avatars.Instance.GetPremiumAvatars())
                        {
                            if (sprite.name.Equals(avatarId))
                            {
                                found = true;
                                gameObject.transform.Find("Avatar").GetComponent<Image>().sprite = sprite;
                            }
                        }
                    }

                    gameObject.transform.localScale = new Vector3(1, 1, 1);
                }
            }
        }

        yield return null;
    }

    public void ViewProfile(string otherUserId)
    {
        foreach(PlayerProfile AFriend in Friends)
        {
            if(AFriend.UID.Equals(otherUserId))
            {
                FriendProfilePanel friendProfilePanel = FindObjectOfType<FriendProfilePanel>();
                friendProfilePanel.SetPanel(AFriend);
            }
        }
    }

    public void AcceptFriendRequest(string otherUserId)
    {
        Debug.Log("Accepting request " + otherUserId);
        LoadingPanel.transform.Find("Panel").transform.Find("Text").GetComponent<Text>().text = "Accepting Friend Request please wait";
        LoadingPanel.SetActive(true);
        StartCoroutine(AcceptRequestCoroutine(otherUserId));
    }

    public void SendFriendRequest(string otherUserUID)
    {
        LoadingPanel.transform.Find("Panel").transform.Find("Text").GetComponent<Text>().text = "Sending Request Please Wait";
        LoadingPanel.SetActive(true);
        StartCoroutine(SendRequestCoroutine(otherUserUID));
    }

    IEnumerator AcceptRequestCoroutine(string otherUserUID)
    {
        int asyncResult = 0;
        yield return new YieldTask(
            FirebaseDatabase.DefaultInstance
        .GetReference("Friends").Child(otherUserUID).Child(playerProfile.UID).Child("uid").SetValueAsync(playerProfile.UID).ContinueWith(task => {
            if (task.IsCanceled)
            {
                asyncResult = 1;
                return;
            }
            else if (task.IsFaulted)
            {
                asyncResult = 2;
                return;
            }
            else if (task.IsCompleted)
            {
                asyncResult = 3;
                return;
            }
        })
        );

        if (asyncResult == 3)
        {
            asyncResult = 0;
            yield return new YieldTask(
            FirebaseDatabase.DefaultInstance
            .GetReference("Friends").Child(playerProfile.UID).Child(otherUserUID).Child("uid").SetValueAsync(otherUserUID).ContinueWith(task => {
            if (task.IsCanceled)
            {
                asyncResult = 1;
                return;
            }
            else if (task.IsFaulted)
            {
                asyncResult = 2;
                return;
            }
            else if (task.IsCompleted)
            {
                asyncResult = 3;
                return;
            }
            })
            );

            if(asyncResult==3)
            {
                yield return new YieldTask(
                FirebaseDatabase.DefaultInstance
                .GetReference("Requests").Child(playerProfile.UID).Child(otherUserUID).Child("uid").RemoveValueAsync().ContinueWith(task => {
                if (task.IsCanceled)
                {
                    asyncResult = 1;
                    return;
                }
                else if (task.IsFaulted)
                {
                    asyncResult = 2;
                    return;
                }
                else if (task.IsCompleted)
                {
                    asyncResult = 3;
                    return;
                }
                })
                );
                if(asyncResult==3)
                {
                    InfoPanel.Instance.SetText("Friend Request successfully Accepted");
                    InfoPanel.Instance.ShowInfoPanel();
                }
                else
                {
                    InfoPanel.Instance.SetText("Something went wrong please try again");
                    InfoPanel.Instance.ShowInfoPanel();
                }

            }
            else
            {
                InfoPanel.Instance.SetText("Something went wrong try again");
                InfoPanel.Instance.ShowInfoPanel();
            }
        }
        else
        {
            InfoPanel.Instance.SetText("Failed to accept request try again");
            InfoPanel.Instance.ShowInfoPanel();
        }
        SelectFriendRequests();
        LoadingPanel.SetActive(false);
    }

    IEnumerator SendRequestCoroutine(string otherUserUID)
    {
        int asyncResult = 0;
        yield return new YieldTask(
            FirebaseDatabase.DefaultInstance
        .GetReference("Requests").Child(otherUserUID).Child(playerProfile.UID).Child("uid").SetValueAsync(playerProfile.UID).ContinueWith(task => {
            if (task.IsCanceled)
            {
                asyncResult = 1;
                return;
            }
            else if (task.IsFaulted)
            {
                asyncResult = 2;
                return;
            }
            else if (task.IsCompleted)
            {
                asyncResult = 3;
                return;
            }
            })
        );

        if(asyncResult==3)
        {
            InfoPanel.Instance.SetText("Friend Request successfully send");
            InfoPanel.Instance.ShowInfoPanel();
        }
        else
        {
            InfoPanel.Instance.SetText("Failed to send request try again");
            InfoPanel.Instance.ShowInfoPanel();
        }
        LoadingPanel.SetActive(false);
    }

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
