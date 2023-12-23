using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using Firebase.Auth;
using UnityEngine.UI;

public class FacebookManager : MonoBehaviour
{
    public static FacebookManager Instance;

    [SerializeField]
    private GameObject CountrySelectionPanel;

    [SerializeField]
    private Dropdown CountryDropdown;

    private bool infoGrabbed=false;
    private bool isDownloaded = false;

    private InfoPanel infoPanel;
    private LoadingPanel loadingPanel;

    private FirebaseAuth auth;
    private FirebaseUser firebaseUser;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        infoPanel = FindObjectOfType<InfoPanel>();
        loadingPanel = FindObjectOfType<LoadingPanel>();

        auth = FirebaseAuth.DefaultInstance;
    }

    private void Awake()
    {
        Instance = this;
        if (!FB.IsInitialized)
        {
            FB.Init(SetInit, onHidenUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    void SetInit()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
            infoPanel.SetText("failed to Initialize Facebook");
            infoPanel.ShowInfoPanel();
        }
    }

    void onHidenUnity(bool isGameShown)
    {
    }

    public void FBLogin()
    {
        if (IsInternetConnected())
        {
            loadingPanel.SetProgress(20);
            loadingPanel.Show();

            infoGrabbed = false;
            StartCoroutine(GetCountry());
        }
        else
        {
            infoPanel.SetText("Please Connect to internet first");
            infoPanel.ShowInfoPanel();
        }
    }

    private IEnumerator GetCountry()
    {
        CountryDropdown.ClearOptions();
        CountryDropdown.AddOptions(CountrySelection.Instance.GetCountryCodeList());

        CountrySelectionPanel.SetActive(true);

        CountrySelectionPanel.GetComponent<CountryNamePanel>().SetIsSelected(false);

        yield return new WaitUntil(() => CountrySelectionPanel.GetComponent<CountryNamePanel>().IsSelected());

        FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email" }, AuthCallBack);
    }

    void AuthCallBack(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            loadingPanel.SetProgress(40);
            Debug.Log("Facebook is Logged in!");
            StartCoroutine(DealWithFbMenus());
        }
        else
        {
            loadingPanel.Hide();
            Debug.Log("Facebook is not Logged in!");
            infoPanel.SetText("Facebook login was cancelled");
            infoPanel.ShowInfoPanel();
        }

    }

    IEnumerator DealWithFbMenus()
    {
        Debug.Log("Deal with FB Menus");

        foreach (string perm in AccessToken.CurrentAccessToken.Permissions)
        {
            // log each granted permission
            Debug.Log(perm);
        }

        List<string> myPerms = new List<string>();
        myPerms.Add("email");
        myPerms.Add("name");
        myPerms.Add("first_name");
        myPerms.Add("last_name");

        string myPermsStr = string.Join(",", myPerms.ToArray());
        Debug.Log(myPermsStr);
        FB.API("/me?fields=" + myPermsStr, HttpMethod.GET, OnUserInfoGrabbed);

        loadingPanel.SetProgress(60);
        yield return new WaitUntil(() => isDownloaded);

        if (infoGrabbed)
        {
            //Now connect this user to firebase

            int authresult = 0;
            Credential credential =
            FacebookAuthProvider.GetCredential(AccessToken.CurrentAccessToken.TokenString);
            yield return new YieldTask(auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    authresult = 1;
                    Debug.LogError("SignInWithCredentialAsync was canceled.");
                }
                else if (task.IsFaulted)
                {
                    authresult = 2;
                    Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                }
                else
                {
                    authresult = 3;
                    firebaseUser = task.Result;
                    Debug.LogFormat("User signed in successfully: {0} ({1})",
                        firebaseUser.DisplayName, firebaseUser.UserId);
                }

            }));

            if(authresult==3)
            {
                Debug.Log("firebase user email : " + firebaseUser.Email);

                loadingPanel.SetProgress(80);

                DatabaseController.Instance.ResetAsyncResult();

                //check profile already exist?
                DatabaseController.Instance.DownloadProfileTask(firebaseUser.UserId);

                yield return new WaitWhile(() => DatabaseController.Instance.GetAsyncResult() == 0);

                if (DatabaseController.Instance.GetAsyncResult() == 3)
                {
                    if (DatabaseController.Instance.GetJson() == null)
                    {
                        Debug.Log("json null");
                        //make new profile 

                        PlayerProfile playerProfile = new PlayerProfile();
                        playerProfile.UID = firebaseUser.UserId;
                        playerProfile.nm = firebaseUser.DisplayName;
                        playerProfile.pD.AT = "FB";
                        playerProfile.pD.em = firebaseUser.Email;
                        playerProfile.C = CountrySelection.Instance.GetCountryCode(CountryDropdown.options[CountryDropdown.value].text);


                        DatabaseController.Instance.ResetAsyncResult();
                        //save to DB
                        DatabaseController.Instance.MakeNewProfileTask(playerProfile);

                        yield return new WaitUntil(() => DatabaseController.Instance.GetAsyncResult() != 0);
                        if (DatabaseController.Instance.GetAsyncResult() == 3) //uploaded to DB
                        {
                            //save Local Profile
                            ProfileSaver profileSaver = new ProfileSaver();

                            //Add mini games to DB
                            DatabaseController.Instance.ResetAsyncResult();

                            MiniGame miniGame = new MiniGame();
                            System.DateTime dateA = System.DateTime.Now;
                            System.DateTime dateB = System.DateTime.Now;

                            //dateA = dateA.AddDays(-1);
                            dateB = dateA.AddDays(-1);

                            miniGame.a = dateA.ToString();
                            miniGame.b = dateB.ToString();
                            profileSaver.SaveMiniGames(miniGame);

                            DatabaseController.Instance.AddNewMiniGame(playerProfile.UID, miniGame);

                            yield return new WaitUntil(() => DatabaseController.Instance.GetAsyncResult() != 0);

                            loadingPanel.SetProgress(90);

                            int allAdded = 0;

                            if (DatabaseController.Instance.GetAsyncResult() == 3)
                            {
                                allAdded++;
                            }

                            //add avatars to DB
                            DatabaseController.Instance.ResetAsyncResult();

                            MyAvatars myAvatars = new MyAvatars();

                            A a = new A();
                            myAvatars.avatars.Add(a);

                            DatabaseController.Instance.AddMyAvatars(playerProfile.UID, myAvatars);

                            yield return new WaitUntil(() => DatabaseController.Instance.GetAsyncResult() != 0);

                            if(DatabaseController.Instance.GetAsyncResult()==3)
                            {
                                allAdded++;
                            }

                            //add boards to DB
                            DatabaseController.Instance.ResetAsyncResult();

                            MyBoards myBoards = new MyBoards();
                            B b = new B();
                            myBoards.Boards.Add(b);
                            DatabaseController.Instance.AddMyBoards(playerProfile.UID, myBoards);

                            yield return new WaitUntil(() => DatabaseController.Instance.GetAsyncResult() != 0);

                            if (DatabaseController.Instance.GetAsyncResult() == 3)
                            {
                                allAdded++;
                            }

                            //add language pack to DB
                            DatabaseController.Instance.ResetAsyncResult();

                            MyPacks myPacks = new MyPacks();
                            P p = new P();
                            myPacks.packs.Add(p);
                            DatabaseController.Instance.AddMyPacks(playerProfile.UID, myPacks);

                            yield return new WaitUntil(() => DatabaseController.Instance.GetAsyncResult() != 0);

                            if (DatabaseController.Instance.GetAsyncResult() == 3)
                            {
                                allAdded++;
                                profileSaver.SaveMyPacks(myPacks);
                            }

                            if(allAdded==4)
                            {
                                profileSaver.SaveProfile(playerProfile);

                                PlayerPrefs.SetInt("currntBoard", 1);
                                PlayerPrefs.SetInt("Login", 2); //1 for chess, 2 for facebook 3 for play games 4 for guest

                                //initialize and load main menu
                                ProfileController.Instance.InitializeProfile();
                            }
                            else
                            {
                                Debug.Log("Something went wrong");
                                infoPanel.SetText("Something went wrong please try again");
                                infoPanel.ShowInfoPanel();
                                loadingPanel.Hide();
                            }
                        }
                        else
                        {
                            infoPanel.SetText("Something went wrong please try again");
                            infoPanel.ShowInfoPanel();
                            loadingPanel.Hide();
                        }
                    }
                    else
                    {
                        Debug.Log("json not null");
                        //Save the downloaded profile
                        PlayerProfile playerProfile = new PlayerProfile();
                        playerProfile = JsonUtility.FromJson<PlayerProfile>(DatabaseController.Instance.GetJson());
                        ProfileSaver profileSaver = new ProfileSaver();
                        DatabaseController.Instance.ResetAsyncResult();

                        DatabaseController.Instance.DownloadMiniGames(playerProfile.UID);

                        yield return new WaitUntil(() => DatabaseController.Instance.GetAsyncResult() != 0);

                        loadingPanel.SetProgress(90);

                        if (DatabaseController.Instance.GetAsyncResult() == 3)
                        {
                            if (DatabaseController.Instance.GetJson() == null)
                            {
                                MiniGame miniGame = new MiniGame();
                                System.DateTime dateA = System.DateTime.Now;
                                System.DateTime dateB = System.DateTime.Now;

                                //dateA = dateA.AddDays(-1);
                                dateB = dateA.AddDays(-1);

                                miniGame.a = dateA.ToString();
                                miniGame.b = dateB.ToString();
                                profileSaver.SaveMiniGames(miniGame);

                                DatabaseController.Instance.AddNewMiniGame(playerProfile.UID, miniGame);
                            }
                            else
                            {
                                MiniGame miniGame = JsonUtility.FromJson<MiniGame>(DatabaseController.Instance.GetJson());
                                profileSaver.SaveMiniGames(miniGame);
                            }

                            //save Local Profile
                            profileSaver.SaveProfile(playerProfile);
                            PlayerPrefs.SetInt("currntBoard", 1);
                            PlayerPrefs.SetInt("Login", 2); //1 for chess, 2 for facebook 3 for play games 4 for guest

                            //initialize and load main menu
                            ProfileController.Instance.InitializeProfile();
                        }
                        else
                        {
                            Debug.Log("Something went wrong");
                            infoPanel.SetText("Something went wrong please try again");
                            infoPanel.ShowInfoPanel();
                            loadingPanel.Hide();
                        }
                    }
                }
                else
                {
                    Debug.Log("Something went wrong");
                    infoPanel.SetText("Something went wrong please try again");
                    infoPanel.ShowInfoPanel();
                    loadingPanel.Hide();
                }
            }
            else
            {
                infoPanel.SetText("Somethign went wrong please try again");
                infoPanel.ShowInfoPanel();
                loadingPanel.Hide();
            }
        }
        else
        {
            loadingPanel.Hide();
            infoPanel.SetText("Please Grant email permissions to login to facebook");
            infoPanel.ShowInfoPanel();
        }
    }

    private void OnUserInfoGrabbed(IResult _FbUserResp)
    {
        if (_FbUserResp.Error == null)
        {
            Debug.Log(_FbUserResp.RawResult);
            _FbUserResp.ResultDictionary["name"].ToString();
            _FbUserResp.ResultDictionary["first_name"].ToString();
            _FbUserResp.ResultDictionary["last_name"].ToString();
            int total = 0;
            foreach (string perm in AccessToken.CurrentAccessToken.Permissions)
            {
                total++;
                // log each granted permission
                Debug.Log(perm);
            }

            if(total>1)
            {
                infoGrabbed = true;
            }
            else
            {
                infoGrabbed = false;
            }
            
        }
        else
        {
            Debug.Log(_FbUserResp.Error);
        }
        isDownloaded = true;
    }

    void GetFBUserName(IResult result)
    {
        if (result.Error == null)
        {
            string name = "" + result.ResultDictionary["first_name"];
            //playerProfile.Name = name;
            infoGrabbed = true;
            Debug.Log("user Name" + name);
        }
        else
        {
            Debug.Log(result.Error);
            infoPanel.InfoText.text = "Error Occurred while Logging you in please try again";
            infoPanel.ShowInfoPanel();
        }
        isDownloaded = true;
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
