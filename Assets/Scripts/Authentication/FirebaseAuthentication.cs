using System.Collections;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;
using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;


public class FirebaseAuthentication : MonoBehaviour
{
    [SerializeField]
    private InputField NameField, EmailField, PasswordField, ConfirmPasswordField, siEmailField, siPasswordField,GuestNameField,fEmailField;

    [SerializeField]
    private Dropdown CountryDropdown,CountryDropdown2,countryDropdown3;

    [SerializeField]
    private GameObject ChessWarfareAuthenticationPanel,EnterNamePanel,CountrySelectionPanel;

    public static FirebaseAuthentication Instance;

    private int ASynResult = 0;
    private string ASyncTaskText;
    private bool PlayGamesIsCompleted = false;
    private string authCode = null;

    private FirebaseAuth auth;
    private FirebaseUser firebaseUser;

    private LoadingPanel loadingPanel;

    private void Start()
    {
#if UNITY_ANDROID
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        .RequestServerAuthCode(false /* Don't force refresh */)
        .RequestEmail()
        .RequestIdToken()
        .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
#endif
        auth = FirebaseAuth.DefaultInstance;
    }

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        loadingPanel = FindObjectOfType<LoadingPanel>();
    }

    public void ForgotPassword()
    {
        if(fEmailField.text.Equals(""))
        {
            InfoPanel.Instance.SetText("Please fill all fields");
            InfoPanel.Instance.ShowInfoPanel();
            return;
        }
        else if (!fEmailField.text.Contains("@"))
        {
            InfoPanel.Instance.SetText("Email is invalid");
            InfoPanel.Instance.ShowInfoPanel();
            return;
        }
        else
        {
            loadingPanel.SetProgress(20);
            loadingPanel.Show();
            StartCoroutine(ForgotPasswordCoroutine());
        }
    }

    IEnumerator ForgotPasswordCoroutine()
    {
        yield return new YieldTask(
            FirebaseAuth.DefaultInstance.SendPasswordResetEmailAsync(fEmailField.text)
            .ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    ASynResult = 1;
                }
                if (task.IsFaulted)
                {
                    foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                    {
                        string authErrorCode = "";
                        Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                        if (firebaseEx != null)
                        {
                            authErrorCode = String.Format("AuthError.{0}: ",
                              ((Firebase.Auth.AuthError)firebaseEx.ErrorCode).ToString());
                        }
                        Debug.Log("number- " + authErrorCode + "the exception is- " + exception.ToString());
                        ASyncTaskText = ((Firebase.Auth.AuthError)firebaseEx.ErrorCode).ToString();
                        ASynResult = 2;
                    }
                }
                if (task.IsCompleted)
                {
                    // Firebase user has been created.
                    ASynResult = 3;
                }
            })
            );
        loadingPanel.Hide();
        if(ASynResult==3)
        {
            InfoPanel.Instance.SetText("reset password email was send successfully");
            InfoPanel.Instance.ShowInfoPanel();
        }
        else if(ASynResult==2)
        {
            InfoPanel.Instance.SetText(ASyncTaskText);
            InfoPanel.Instance.ShowInfoPanel();
        }
        else
        {
            InfoPanel.Instance.SetText("sending email was cancelled");
            InfoPanel.Instance.ShowInfoPanel();
        }

    }

    public void PlayGamesSignIn() //behavior may not be expected on Editor (Play Games not avaiable for IOS)
    {
#if UNITY_ANDROID
        loadingPanel.SetProgress(20);
        loadingPanel.Show();

        CountryDropdown2.ClearOptions();
        CountryDropdown2.AddOptions(CountrySelection.Instance.GetCountryCodeList());

        CountrySelectionPanel.SetActive(true);
        CountrySelectionPanel.GetComponent<CountryNamePanel>().SetIsSelected(false);

        StartCoroutine(PlayGamesFirebaseAuth());
#endif

#if UNITY_IOS
        InfoPanel.Instance.SetText("Sorry Play Games not available on IOS");
        InfoPanel.Instance.ShowInfoPanel();
#endif
    }

    IEnumerator PlayGamesFirebaseAuth()
    {
        yield return new WaitUntil(()=>CountrySelectionPanel.GetComponent<CountryNamePanel>().IsSelected());

        if (Social.localUser.authenticated)
        {
#if UNITY_ANDROID
            ((PlayGamesPlatform)Social.Active).SignOut();
#endif
        }

        PlayGamesIsCompleted = false;
        authCode = null;

        Social.localUser.Authenticate((bool success) => {
            if (success)
            {
#if UNITY_ANDROID
                authCode = PlayGamesPlatform.Instance.GetServerAuthCode();
#endif
                loadingPanel.SetProgress(25);
                PlayGamesIsCompleted = true;
            }
            else
            {
                InfoPanel.Instance.SetText("Cannot sign you to play games please try again");
                InfoPanel.Instance.ShowInfoPanel();
                loadingPanel.Hide();
                PlayGamesIsCompleted = true;
            }
        });

        yield return new WaitUntil(() => PlayGamesIsCompleted);

        if(authCode==null)
        {
            yield break;
        }

        int authResult = 0;
        //sign in to firebase
        Credential credential = PlayGamesAuthProvider.GetCredential(authCode);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
            if (task.IsCanceled)
            {
                authResult = 1;
                Debug.LogError("SignInWithCredentialAsync was canceled.");
            }
            else if (task.IsFaulted)
            {
                authResult = 2;
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
            }
            else
            {
                loadingPanel.SetProgress(40);
                authResult = 3;
                firebaseUser = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                    firebaseUser.DisplayName, firebaseUser.UserId);
            }
        });

        yield return new WaitUntil(() => authResult != 0);

        if(authResult==3)
        {
            //success
            loadingPanel.SetProgress(50);

            DatabaseController.Instance.ResetAsyncResult();

            //check profile already exist?
            DatabaseController.Instance.DownloadProfileTask(firebaseUser.UserId);

            loadingPanel.SetProgress(60);
            yield return new WaitWhile(() => DatabaseController.Instance.GetAsyncResult() == 0);

            if (DatabaseController.Instance.GetAsyncResult() == 3)
            {
                loadingPanel.SetProgress(70);
                if (DatabaseController.Instance.GetJson() == null)
                {
                    Debug.Log("json null");
                    //make new profile 

                    PlayerProfile playerProfile = new PlayerProfile();
                    playerProfile.UID = firebaseUser.UserId;
                    playerProfile.nm = firebaseUser.DisplayName;
                    playerProfile.pD.AT = "GO";
                    playerProfile.C = CountrySelection.Instance.GetCountryCode(CountryDropdown2.options[CountryDropdown2.value].text);


#if UNITY_ANDROID
                    playerProfile.pD.em = ((PlayGamesLocalUser)Social.localUser).Email;
#endif
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

                        if (DatabaseController.Instance.GetAsyncResult() == 3)
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

                        if (allAdded == 4)
                        {
                            profileSaver.SaveProfile(playerProfile);

                            PlayerPrefs.SetInt("currntBoard", 1);
                            PlayerPrefs.SetInt("Login", 3); //1 for chess, 2 for facebook 3 for play games 4 for guest

                            //initialize and load main menu
                            ProfileController.Instance.InitializeProfile();
                        }
                        else
                        {
                            Debug.Log("Something went wrong");
                            InfoPanel.Instance.SetText("Something went wrong please try again");
                            InfoPanel.Instance.ShowInfoPanel();
                            loadingPanel.Hide();
                        }
                    }
                    else
                    {
                        InfoPanel.Instance.SetText("Something went wrong please try again");
                        InfoPanel.Instance.ShowInfoPanel();
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
                        PlayerPrefs.SetInt("Login", 3); //1 for chess, 4 for guest 2 for facebook 3 for play games

                        ChessWarfareAuthenticationPanel.SetActive(false);
                        siEmailField.text = "";
                        siPasswordField.text = "";

                        loadingPanel.Hide();

                        //initialize and load main menu
                        ProfileController.Instance.InitializeProfile();
                    }
                    else
                    {
                        Debug.Log("Something went wrong");
                        InfoPanel.Instance.SetText("Something went wrong please try again");
                        InfoPanel.Instance.ShowInfoPanel();
                        loadingPanel.Hide();
                    }
                }
            }
            else
            {
                Debug.Log("Something went wrong");
                InfoPanel.Instance.SetText("Something went wrong please try again");
                InfoPanel.Instance.ShowInfoPanel();
                loadingPanel.Hide();
            }


        }
        else if(authResult==2)
        {
            InfoPanel.Instance.SetText("Something went wrong please try again");
            InfoPanel.Instance.ShowInfoPanel();
            loadingPanel.Hide();
        }
        else 
        {
            InfoPanel.Instance.SetText("Play games sign in is cancelled");
            InfoPanel.Instance.ShowInfoPanel();
            loadingPanel.Hide();
        }
    }

    public void InitEmailDropdown()
    {
        countryDropdown3.ClearOptions();
        countryDropdown3.AddOptions(CountrySelection.Instance.GetCountryCodeList());
    }

    public void EmailRegistration() //email signUp
    {
        if (IsInternetConnected())
        {
            //validate fields First
            if (NameField.text.Equals("") || PasswordField.text.Equals("") || ConfirmPasswordField.text.Equals("") || EmailField.text.Equals(""))
            {
                InfoPanel.Instance.SetText("Please fill all fields");
                InfoPanel.Instance.ShowInfoPanel();
                return;
            }
            else if (!EmailField.text.Contains("@"))
            {
                InfoPanel.Instance.SetText("Email is invalid");
                InfoPanel.Instance.ShowInfoPanel();
                return;
            }
            else if (PasswordField.text.Length < 7 || PasswordField.text.Length > 20)
            {
                InfoPanel.Instance.SetText("Password can only be 7 to 20 characters");
                InfoPanel.Instance.ShowInfoPanel();
                return;
            }
            else if (!PasswordField.text.Equals(ConfirmPasswordField.text))
            {
                InfoPanel.Instance.SetText("Both passwords does not match");
                InfoPanel.Instance.ShowInfoPanel();
                return;
            }

            StartCoroutine(SignUpTaskValidation());
            loadingPanel.SetProgress(20);
            loadingPanel.Show();

            auth.CreateUserWithEmailAndPasswordAsync(EmailField.text, PasswordField.text).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    ASynResult = 1;
                }
                if (task.IsFaulted)
                {
                    foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                    {
                        string authErrorCode = "";
                        Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                        if (firebaseEx != null)
                        {
                            authErrorCode = String.Format("AuthError.{0}: ",
                              ((Firebase.Auth.AuthError)firebaseEx.ErrorCode).ToString());
                        }
                        Debug.Log("number- " + authErrorCode + "the exception is- " + exception.ToString());
                        ASyncTaskText = ((Firebase.Auth.AuthError)firebaseEx.ErrorCode).ToString();
                        ASynResult = 2;
                    }
                }
                if (task.IsCompleted)
                {
                    // Firebase user has been created.
                    firebaseUser = task.Result;
                    ASynResult = 3;
                }
            });
        }
        else
        {
            InfoPanel.Instance.SetText("Please connect to internet first");
            InfoPanel.Instance.ShowInfoPanel();
        }
    }

    IEnumerator SignUpTaskValidation() //email sign up validation
    {
        yield return new WaitUntil(() => ASynResult != 0);

        if (ASynResult == 3) //completed
        {
            ASynResult = 0;

            UserProfile userProfile = new UserProfile();
            userProfile.DisplayName = NameField.text;
            
            yield return new YieldTask(firebaseUser.UpdateUserProfileAsync(userProfile));

            //send verification Email
            yield return new YieldTask(firebaseUser.SendEmailVerificationAsync());

            NameField.text = "";
            EmailField.text = "";
            PasswordField.text = "";
            ConfirmPasswordField.text = "";

            InfoPanel.Instance.SetText("Account created Successfully. A verification email has been sent to your email");
            InfoPanel.Instance.ShowInfoPanel();
            loadingPanel.Hide();

        }
        else if (ASynResult == 2) //faulted
        {
            switch (ASyncTaskText)
            {
                case "AccountExistsWithDifferentCredentials":
                    InfoPanel.Instance.SetText("An Acccount already exist with same email");
                    break;
                case "MissingPassword":
                    InfoPanel.Instance.SetText("Password Missing");
                    break;
                case "WeakPassword":
                    InfoPanel.Instance.SetText("Password strength is weak");
                    break;
                case "WrongPassword":
                    InfoPanel.Instance.SetText("Wrong Password");
                    break;
                case "EmailAlreadyInUse":
                    InfoPanel.Instance.SetText("Email Already Registered");
                    break;
                case "InvalidEmail":
                    InfoPanel.Instance.SetText("Invalid Email Format");
                    break;
                case "MissingEmail":
                    InfoPanel.Instance.SetText("Please fill email field");
                    break;
                default:
                    InfoPanel.Instance.SetText("An Error Occured please try again");
                    break;
            }
            InfoPanel.Instance.ShowInfoPanel();
            loadingPanel.Hide();
            ASyncTaskText = "";
            ASynResult = 0;
        }
        else if (ASynResult == 1) //cancelled
        {
            InfoPanel.Instance.SetText("Registration was cancelled");
            InfoPanel.Instance.ShowInfoPanel();
            loadingPanel.Hide();
            ASyncTaskText = "";
            ASynResult = 0;
        }
    }

    public void EmailSignIn() //email sign In
    {
        if (IsInternetConnected())
        {
            //validate fields First
            if (!siEmailField.text.Contains("@"))
            {
                InfoPanel.Instance.SetText("Invalid Email Format");
                InfoPanel.Instance.ShowInfoPanel();
                return;
            }
            else if (siPasswordField.text.Equals("") || siEmailField.text.Equals(""))
            {
                InfoPanel.Instance.SetText("Please Fill all fields");
                InfoPanel.Instance.ShowInfoPanel();
                return;
            }

            loadingPanel.SetProgress(20);
            loadingPanel.Show();
            StartCoroutine(SignInTaskValidation());

            auth.SignInWithEmailAndPasswordAsync(siEmailField.text, siPasswordField.text).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    ASynResult = 1;
                    return;
                }
                if (task.IsFaulted)
                {
                    ASynResult = 2;
                    return;
                }
                if (task.IsCompleted)
                {
                    // Firebase user has been created.
                    firebaseUser = task.Result;
                    ASynResult = 3;
                    return;
                }
            });
        }
    }

    IEnumerator SignInTaskValidation() //email sign In validation
    {
        Debug.Log("Waiting For Sign In Task Completion");
        yield return new WaitUntil(() => ASynResult != 0);

        loadingPanel.SetProgress(50);

        if (ASynResult == 3) //completed
        {
            ASynResult = 0;

            //Check if email is verified
            if (firebaseUser.IsEmailVerified)
            {
                DatabaseController.Instance.ResetAsyncResult();

                //check profile already exist?
                DatabaseController.Instance.DownloadProfileTask(firebaseUser.UserId);

                loadingPanel.SetProgress(60);
                yield return new WaitWhile(() => DatabaseController.Instance.GetAsyncResult() == 0);

                if (DatabaseController.Instance.GetAsyncResult() == 3)
                {
                    loadingPanel.SetProgress(70);
                    if (DatabaseController.Instance.GetJson() == null)
                    {
                        Debug.Log("json null");
                        //make new profile 

                        PlayerProfile playerProfile = new PlayerProfile();
                        playerProfile.UID = firebaseUser.UserId;
                        playerProfile.nm = firebaseUser.DisplayName;
                        playerProfile.pD.AT = "EM";
                        playerProfile.pD.em = firebaseUser.Email;

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

                            if (DatabaseController.Instance.GetAsyncResult() == 3)
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

                            if (allAdded == 4)
                            {
                                profileSaver.SaveProfile(playerProfile);

                                PlayerPrefs.SetInt("currntBoard", 1);
                                PlayerPrefs.SetInt("Login", 1); //1 for chess, 2 for facebook 3 for play games 4 for guest

                                //initialize and load main menu
                                ProfileController.Instance.InitializeProfile();
                            }
                            else
                            {
                                Debug.Log("Something went wrong");
                                InfoPanel.Instance.SetText("Something went wrong please try again");
                                InfoPanel.Instance.ShowInfoPanel();
                                loadingPanel.Hide();
                            }
                        }
                        else
                        {
                            InfoPanel.Instance.SetText("Something went wrong please try again");
                            InfoPanel.Instance.ShowInfoPanel();
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
                            MiniGame miniGame=null;
                            if (DatabaseController.Instance.GetJson() == null)
                            {
                                miniGame = new MiniGame();
                                System.DateTime dateA = System.DateTime.Now;
                                System.DateTime dateB = System.DateTime.Now;

                                //dateA = dateA.AddDays(-1);
                                dateB = dateA.AddDays(-1);

                                miniGame.a = dateA.ToString();
                                miniGame.b = dateB.ToString();


                                DatabaseController.Instance.AddNewMiniGame(playerProfile.UID, miniGame);
                            }
                            else
                            {
                                miniGame = JsonUtility.FromJson<MiniGame>(DatabaseController.Instance.GetJson());
                            }

                            PlayerPrefs.DeleteAll();
                            //save Local Profile
                            
                            PlayerPrefs.SetInt("currntBoard", 1);
                            PlayerPrefs.SetInt("Login", 1); //1 for chess, 4 for guest 2 for facebook 3 for play games

                            ChessWarfareAuthenticationPanel.SetActive(false);
                            siEmailField.text = "";
                            siPasswordField.text = "";

                            loadingPanel.Hide();

                            profileSaver.SaveProfile(playerProfile);
                            profileSaver.SaveMiniGames(miniGame);
                            //initialize and load main menu
                            ProfileController.Instance.InitializeProfile();
                        }
                        else
                        {
                            Debug.Log("Something went wrong");
                            InfoPanel.Instance.SetText("Something went wrong please try again");
                            InfoPanel.Instance.ShowInfoPanel();
                            loadingPanel.Hide();
                        }
                    }
                }
                else
                {
                    Debug.Log("Something went wrong");
                    InfoPanel.Instance.SetText("Something went wrong please try again");
                    InfoPanel.Instance.ShowInfoPanel();
                    loadingPanel.Hide();
                }

            }
            else
            {
                InfoPanel.Instance.SetText("Please verify your email first");
                InfoPanel.Instance.ShowInfoPanel();
                loadingPanel.Hide();
            }
        }
        else if (ASynResult == 2) //faulted
        {
            InfoPanel.Instance.SetText("Something went wrong please try again");
            InfoPanel.Instance.ShowInfoPanel();
            loadingPanel.Hide();
            ASynResult = 0;
        }
        else if (ASynResult == 1) //cancelled
        {
            InfoPanel.Instance.SetText("Login was Cancelled");
            InfoPanel.Instance.ShowInfoPanel();
            loadingPanel.Hide();
            ASynResult = 0;
        }
    }

    public void ShowGuestPanel()
    {
        CountryDropdown.ClearOptions();
        CountryDropdown.AddOptions(CountrySelection.Instance.GetCountryCodeList());
        EnterNamePanel.SetActive(true);
    }

    public void LoginGuest()
    {
        Debug.Log("login guest");
        if (IsInternetConnected())
        {
            if(GuestNameField.text.Equals(""))
            {
                InfoPanel.Instance.SetText("Please Enter your name");
                InfoPanel.Instance.ShowInfoPanel();
                return;
            }

            Debug.Log("Connected to internet");
            loadingPanel = FindObjectOfType<LoadingPanel>();
            loadingPanel.Show();
            EnterNamePanel.SetActive(false);
            loadingPanel.SetProgress(30);

            StartCoroutine(GuestTaskValidation());
            auth.SignInAnonymouslyAsync().ContinueWith(task => {
                if (task.IsCanceled)
                {
                    ASynResult = 1;
                    return;
                }
                if (task.IsFaulted)
                {
                    ASynResult = 2;
                    return;
                }
                if (task.IsCompleted)
                {
                    //firebase user has been created
                    firebaseUser = task.Result;
                    ASynResult = 3;
                    return;
                }
            });
        }
        else
        {
            InfoPanel.Instance.SetText("Please connect to internet first");
            InfoPanel.Instance.ShowInfoPanel();
            InfoPanel.Instance.ShowInfoPanel();
        }
    }

    IEnumerator GuestTaskValidation()
    {
        Debug.Log("Waiting For Guest Task Completion");
        yield return new WaitUntil(() => ASynResult != 0);

        loadingPanel.SetProgress(50);

        if (ASynResult == 3) //completed
        {
            Debug.Log("json null");
            //make new profile 

            PlayerProfile playerProfile = new PlayerProfile();
            playerProfile.UID = firebaseUser.UserId;
            playerProfile.nm = GuestNameField.text;
            playerProfile.pD.AT = "GU";
            playerProfile.C = CountrySelection.Instance.GetCountryCode( CountryDropdown.options[CountryDropdown.value].text);

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

                if (DatabaseController.Instance.GetAsyncResult() == 3)
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

                if (allAdded == 4)
                {
                    profileSaver.SaveProfile(playerProfile);

                    PlayerPrefs.SetInt("currntBoard", 1);
                    PlayerPrefs.SetInt("Login", 4); //1 for chess, 2 for facebook 3 for play games 4 for guest

                    //initialize and load main menu
                    ProfileController.Instance.InitializeProfile();
                }
                else
                {
                    Debug.Log("Something went wrong");
                    InfoPanel.Instance.SetText("Something went wrong please try again");
                    InfoPanel.Instance.ShowInfoPanel();
                    loadingPanel.Hide();
                    EnterNamePanel.SetActive(true);
                }
            }
            else
            {
                InfoPanel.Instance.SetText("Something went wrong please try again");
                InfoPanel.Instance.ShowInfoPanel();
                loadingPanel.Hide();
                EnterNamePanel.SetActive(true);
            }

            ASynResult = 0;
        }
        else if (ASynResult == 2 || ASynResult == 1) //faulted
        {
            InfoPanel.Instance.SetText("Error Occured while creating Guest Account");
            InfoPanel.Instance.ShowInfoPanel();
            loadingPanel.Hide();
            EnterNamePanel.SetActive(true);
            ASynResult = 0;
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
