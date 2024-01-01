using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using mmocircle;

public class DeepClass : MonoBehaviour
{
    public static DeepClass inst;

    private string redirectUri = "chesswarfare://com.chesswarfare.auth";
    private string clientId = "chess-warfare-android";

    //public Text ggp1;
    //public Text ggp2;
    //public Text ggp3;
    //public Text ggp4;
    public string Str;
    public string server_Str;

    [SerializeField] Text username;
    [SerializeField] Text gems;

    public GameObject loginPanel;
    public GameObject profile;
    public String profileimgurl;

    public Image profileimage;
    public Image preloader;
    RectTransform rectLoader;
    internal bool isLoaded = false;

    string name;
    string Profile_url = "https://wgcapi.mmocircles.com/Profile/basicinfo";
    public char[] trim_char_arry = new char[] { '"' };


    public float ggp;

    public float gemsValue;
    //----------------------------------------------------------------------------------------------------------------

    public GameObject guest;
    public UniWebView uniWebView;
    public GameObject webviewPrefab; // to save UniWebView GameObject 
    int checkerror = 0;
    public string code = "";
    public Text state;
    public Button openUrlBtn;
    public Button playwithout;
    public GameObject loadingAnimation;
    public GameObject loadingPanel;
    // public GameObject loginPanel;
    public GameObject settingsUI;
    public static int playWithoutLoiginVar = 0;
    public GameObject SystemPanel;
    //----------------------history----------------------------------
    public List<GameObject> historyObjects;
    public GameObject historyCheck;
    public GameObject logoutpanels;

    [System.Serializable]
    public class SGCHistoryData
    {
        public string date;
        public string type;
        public string amount;
        public string game;
        public string productName;
        public string parti;
        public string starttime;
        public string endtime;
    }
    [System.Serializable]
    public class Participant
    {
        public int user_id;
        public string user_name;
        public string display_name;
        public string avatar_url;
        public object banner_url;
        public DateTime created;
    }
    [System.Serializable]
    public class PoolDetails
    {
        public int id;
        public string description;
        public DateTime completed;
        public DateTime start_time;
        public DateTime end_time;
    }
    [System.Serializable]
    public class HistoryClass
    {
        public DateTime issue_date;
        public string type;
        public double amount;
        public string game;
        public object product_name;
        public List<Participant> participants;
        public PoolDetails pool_details;
        public string details;
    }


    [Header("UI")]
    Transform SGCHistoryParent;
    GameObject SGCHistoryPrefab;
    public List<HistoryClass> HistoryClassObj;
    // public ParentHistory ParentHistoryObj;

    [Header("Data")]
    public List<SGCHistoryData> SGCHistoryUserData = new List<SGCHistoryData>();

    public List<GameObject> SGCHistoryGameObjs = new List<GameObject>();

    int range = 10;


    //------------------------------------------------------------- battile log -----------------------------------------
    public UIController uiManager;

    [SerializeField] Text username__1;
    public GameObject ReloadGame;
    public static String profileID;
    public Image default_image;
    public Image noimage;
    public GameObject battileLogNotification;
    public static int notification_value = 0;
    public GameObject ErrorTxt;
    public GameObject SubmitBtn;
    public GameObject ErrorRestart;
    public int imageCount = 0;
    public int imageCountCheck = 0;
    public static int Challenge_Var = 0;
    public static string ChallengeID = "";
    public Image profileimage__1;
    public static Image profileimage_demo;
    public Button inprogressBtn;
    public Button CompleteBttn;
    public Color blueColour;
    public Color AshColour;
    public static String profileimage_URL;
    public GameObject OponentPage;
    public GameObject noSGC;
    //-----------------------------submit score---------------------
    public static int HighScoreValue = 0;
    public GameObject ExitBtn;
    public GameObject prograsss;
    public GameObject scoreobject;
    public GameObject profile__1;
    public GameObject profile__2;
    public GameObject firstObject;
    public GameObject secondObject;
    public GameObject battileMsg;
    public Text DateForscore_txt;
    public Image DefenderPic;
    public Text Defender_name;
    public Text Defender_score;
    public Image Challenger_pic;
    public Text Challenger_name;
    public Text Challenger_score;


    //------------------------------------------------
    public List<GameObject> battileObjects;
    public List<GameObject> battileObjects1;
    public GameObject battilCheck;
    public GameObject itemsForproress;
    public GameObject itemsForcompleted;

    public Sprite Pink_clr;
    public Sprite green_clr;
    public Sprite blue_clr;
    public Sprite red_clr;

    public GameObject PlayPVP_page;
    public int takeChallenge; // add
    public GameObject LoadingAvatar;
    public static float gemValue_Store;
    public static String Username_Store;

    //--------------------------Check





    [System.Serializable]
    public class BattileData
    {
        public string _date;
        public string user_id1;
        public string username1;
        public string score1;
        public string avatar_url_1;
        public string user_id2;
        public string username2;
        public Image profileimage1;
        public string score2;
        public string avatar_url_2;
        public Image profileimage2;
    }
    [System.Serializable]
    public class challenger
    {
        public double score;
        public string user_id;
        public string user_name;
        public string display_name;
        public string avatar_url;
        public object banner_url;
        public DateTime created;
    }
    [System.Serializable]
    public class defender
    {
        public double score;
        public string user_id;
        public string user_name;
        public string display_name;
        public string avatar_url;
        public object banner_url;
        public DateTime created;
    }
    public class BattileClass
    {
        public string id;
        public string pool_id;
        public string created;
        public string wager;
        public challenger _challenger;
        public defender _defender;
    }



    [Header("UI")]
    Transform BattileParent;
    GameObject BattilePrefab;
    public List<BattileClass> BattileClassObj;

    [Header("Data")]
    public List<BattileData> BattileUserData = new List<BattileData>();
    public List<BattileData> BattileUserData1 = new List<BattileData>();

    public List<GameObject> BattileGameObjs = new List<GameObject>();


    //--------------------------------------


    //-------------------

    public GameObject neversionAvailable;
    public GameObject completedProfile;
    public Text Completed_username;
    public Text Completed_WinorLose;
    public Text Opponent_date;
    public Image Completed_UserPic;


    //----------------------------------------------

    public GameObject loadingPanelforoppo;
    public GameObject yourTurn;
    public GameObject Onprograss;
    public Image yourTurn_tImage;
    public static Image yourTurn_tImage_demo;
    public Text yourTurn_username;










    //----------------------------------------------------------------------------------------------------------------------
















    // Use this for initialization
    public void Awake()
    {
        inst = this;
        //rectLoader = preloader.GetComponent<RectTransform>();
    }

    void Start()
    {
        ShowPreLoader(true);
        //StartCoroutine(CheckVersion());
        Openlink.Inst.gamejsondata();
        ShowPreLoader(true);
        //loadingAnimation.SetActive(false);
        Debug.Log("start" + PlayerPrefs.GetString("r_token").ToString());
        loginPanel.SetActive(true);
        //if (PlayerPrefs.GetString("r_token").ToString() == "")
        //{
        //    if (playWithoutLoiginVar == 1)
        //    {
        //        guest.SetActive(true);
        //        profile.SetActive(false);
        //        loginPanel.SetActive(false);
        //    }
        //    else
        //    {
        //        //guest.SetActive(true);
        //        //profile.SetActive(false);
        //        loginPanel.SetActive(true);
        //    }
        //}
        //else
        //{
        //    playWithoutLoiginVar = 0;
        //    guest.SetActive(false);
        //    loginPanel.SetActive(false);


        //    //if (UIController.tieValue == 0)
        //    //{

        //    //    if (UIController.PlayAgain == 1)
        //    //    {
        //    //        loadingPanel.SetActive(true);

        //    //    }

        //    //    if (UIController.OpenSettingsUI == 0)
        //    //    {
        //    //        loadingPanel.SetActive(true);
        //    //        StartCoroutine(refreshtoken());
        //    //    }
        //    //    else
        //    //    {
        //    //        //StartCoroutine(refreshtoken());
        //    //        profile.SetActive(true);
        //    //        guest.SetActive(false);
        //    //        profileimage.sprite = profileimage_demo.sprite;
        //    //        profileimage__1.sprite = profileimage_demo.sprite;
        //    //        username__1.text = "" + Username_Store;
        //    //        username.text = "" + Username_Store;

        //    //        double Round_gemValue_Store = Math.Round(gemValue_Store, 2);
        //    //        gems.text = "" + Round_gemValue_Store;


        //    //        gemsValue = gemValue_Store;

        //    //    }

        //    //}


        //}


        DateTime leaveDateTime = DateTime.Now;    // for battileLog
        string newdate = leaveDateTime.ToString();
        PlayerPrefs.SetString("CheckforReload", newdate);


    }








    // Update is called once per frame
    void Update()
    {
        //if (!preloader.enabled)
            return;
        rectLoader.Rotate(rectLoader.forward, -2);

        if (!IsInternetConnection())
        {
            profile.SetActive(false);
            loginPanel.SetActive(true);
        }



        if (notification_value == 1)
        {
            battileLogNotification.SetActive(true);

        }
        else
        {

            battileLogNotification.SetActive(false);
        }



        DateTime leaveDateTime1 = Convert.ToDateTime(PlayerPrefs.GetString("CheckforReload").ToString());
        TimeSpan diff = DateTime.Now - leaveDateTime1;
        if (diff.TotalMinutes >= 25)  // for battileLog
        {
            ReloadGame.SetActive(true);
        }
        else
        {
            ReloadGame.SetActive(false);

        }

    }









    //------------------for smalll games//




    public void reloadGame()
    {

        SceneManager.LoadScene("Main");
    }



    internal IEnumerator CheckVersion()
    {


        //  string CurrentVersion = "\"V2\"";
        //  string CurrentVersion = "V2"; // for suponic
        float CurrentVersion = 1.2f;

        string BASE_URL = "https://wgcapi.mmocircles.com/apps/" + clientId + "/version";
        UnityWebRequest www = UnityWebRequest.Get(BASE_URL);
        www.SetRequestHeader("accept", "text/plain");

        yield return www.SendWebRequest();


        if (www.isNetworkError)
        {

        }
        else
        {
            string full = www.downloadHandler.text;

            Debug.Log("CurrentVersion--->" + CurrentVersion);
            Debug.Log("full--->" + full);

            float VersionbyAPI = float.Parse(full);
            if (VersionbyAPI > CurrentVersion)
            {

                Debug.Log("CheckVersion----------->" + full);


                neversionAvailable.SetActive(true);

            }
            else
            {

                neversionAvailable.SetActive(false);
            }


        }
    }



























    public static bool IsInternetConnection()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }
    void LaunchFromUrl(string arg)
    {
        Debug.Log("respones=" + arg);

        try
        {
            Str = arg;
            if (arg != null)
            {
                string[] splitArray = arg.Split(char.Parse("="));
                name = splitArray[1];
                name = name.Replace("\r", "").Replace("\n", "");
                Str = name;
                Debug.Log("name>>>>>>>11111>" + name);
            }
            StartCoroutine(Servercall());


        }
        catch (Exception e)
        {
            Str = e.ToString();
        }


        if (PlayerPrefs.GetString("a_token").ToString() == "")
        {

            profile.SetActive(false);
            loginPanel.SetActive(true);
        }
        else
        {

            profile.SetActive(true);
            loginPanel.SetActive(false);

        }


    }

    IEnumerator Servercall()
    {
        uniWebView.Hide();
        Debug.Log("servercall");
        guest.SetActive(false);
        playWithoutLoiginVar = 0;
        loadingPanel.SetActive(true);
        battileLogNotification.SetActive(false);

        string url = "https://wgcapi.mmocircles.com/auth/token";
        WWWForm form = new WWWForm();

        form.AddField("grant_type", "authorization_code");
        form.AddField("code", code.ToString());
        form.AddField("client_id", clientId);
        form.AddField("redirect_uri", redirectUri);

        UnityWebRequest uwr = UnityWebRequest.Post(url, form);
        uwr.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        server_Str = "hello";



        yield return uwr.Send();
        server_Str = "hello2";

        if (uwr.error != null)
        {
            server_Str = uwr.error.ToString();
            Debug.Log("Error from Server Call: " + uwr.error.ToString());
        }
        else
        {
            Debug.Log(" uwr.downloadHandler.text>>>>" + uwr.downloadHandler.text);
            server_Str = uwr.downloadHandler.text;
            string full = server_Str;

            JSONObject obj = new JSONObject(full);

            string refresh_token = obj.GetField("refresh_token").ToString();

            refresh_token = refresh_token.ToString().Trim((trim_char_arry));
            PlayerPrefs.SetString("r_token", refresh_token);
            Debug.Log("refresh_token: " + PlayerPrefs.GetString("r_token").ToString());

            string accrss_token = obj.GetField("access_token").ToString();
            accrss_token = accrss_token.ToString().Trim((trim_char_arry));
            PlayerPrefs.SetString("a_token", accrss_token);
            Debug.Log("accrss_token" + PlayerPrefs.GetString("a_token").ToString());



            //string accrss_token = obj[2].ToString();
            //accrss_token = accrss_token.ToString().Trim((trim_char_arry));
            //Debug.Log("before==" + PlayerPrefs.GetString("a_token").ToString());
            //PlayerPrefs.SetString("a_token", accrss_token);
            //Debug.Log("after==" + PlayerPrefs.GetString("a_token").ToString());

            //Openlink.Inst.gamejsondata();



            StartCoroutine(getprofile());



        }

    }
    //new function...........
    IEnumerator refreshtoken()
    {
        Debug.Log("refresh_token");

        string url = "https://wgcapi.mmocircles.com/auth/token";
        WWWForm form = new WWWForm();
        Debug.Log("hello1");
        form.AddField("grant_type", "refresh_token");
        form.AddField("client_id", clientId);
        form.AddField("redirect_uri", redirectUri);
        form.AddField("refresh_token", PlayerPrefs.GetString("r_token").ToString());
        UnityWebRequest rft = UnityWebRequest.Post(url, form);
        rft.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        Debug.Log("hello2");


        yield return rft.Send();

        Debug.Log("hello3");
        if (rft.error != null)
        {
            server_Str = rft.error.ToString();
            Debug.Log(" rft.downloadHandler.text>>>>" + rft.error.ToString());
            loadingPanel.SetActive(false);
            logoutpanels.SetActive(true);
        }
        else
        {
            Debug.Log(" rft.downloadHandler.text>>>>" + rft.downloadHandler.text);
            server_Str = rft.downloadHandler.text;
            string full = server_Str;

            JSONObject obj = new JSONObject(full);
            string refresh_token = obj.GetField("refresh_token").ToString();
            refresh_token = refresh_token.ToString().Trim((trim_char_arry));
            PlayerPrefs.SetString("r_token", refresh_token);
            Debug.Log("refresh_token" + PlayerPrefs.GetString("r_token").ToString());

            string accrss_token = obj.GetField("access_token").ToString();
            accrss_token = accrss_token.ToString().Trim((trim_char_arry));
            PlayerPrefs.SetString("a_token", accrss_token);
            Debug.Log("accrss_token" + PlayerPrefs.GetString("a_token").ToString());


            StartCoroutine(getprofile());

        }

    }
    public JSONObject pro;

    IEnumerator getprofile()
    {
        Debug.Log("profile call");
        Openlink.Inst.gamejsondata();

        StartCoroutine(getggpprice());
        Debug.Log(IsInternetConnection());
        if (IsInternetConnection())
        {

            UnityWebRequest upr = UnityWebRequest.Get(Profile_url);
            upr.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("a_token").ToString());
            Debug.Log("profilr_call0000000>>>>>>" + "Bearer " + PlayerPrefs.GetString("a_token").ToString());
            yield return upr.Send();
            if (upr.error != null)
            {
                Debug.Log("Profile upr error: " + upr.error.ToString());
                profile.SetActive(false);
                loginPanel.SetActive(false);
            }
            else
            {
                profile.SetActive(true);
                loginPanel.SetActive(false);
            }
            Debug.Log("profilr_call1111111>>>>>>" + upr.downloadHandler.text);
            string profile_upr = upr.downloadHandler.text;
            pro = new JSONObject(profile_upr);

            username.text = pro[1].ToString().Trim((trim_char_arry)).ToString();
            username__1.text = pro[1].ToString().Trim((trim_char_arry)).ToString();
            //  gems.text = pro[2].ToString().Trim((trim_char_arry)).ToString();
            ggp = float.Parse(pro[2].ToString().Trim((trim_char_arry)).ToString());
            //      profileimgurl = pro[3].ToString().Trim((trim_char_arry)).ToString();
            ///     Debug.Log("profile url" + profileimgurl);
            // StartCoroutine(Load(profileimgurl, IsInternetConnection()));



            gemsValue = float.Parse(pro[2].ToString().Trim((trim_char_arry)).ToString());

            double Round_gemsValue = Math.Round(gemsValue, 2);
            gems.text = "" + Round_gemsValue;

            Username_Store = pro[1].ToString().Trim((trim_char_arry)).ToString();
            gemValue_Store = gemsValue;
            string full = profile_upr;
            JSONObject obj = new JSONObject(full);
            string profileimgurl = obj.GetField("avatarurl").ToString();
            profileimgurl = profileimgurl.ToString().Trim((trim_char_arry));
            Debug.Log("id_string---->" + profileimgurl);




            string UerdID = obj.GetField("id").ToString();
            UerdID = UerdID.ToString().Trim((trim_char_arry));
            profileID = UerdID;




            if (profileimage_URL == profileimgurl)
            {

                isLoaded = true;
                ShowPreLoader(false);
                profileimage.sprite = profileimage_demo.sprite;
                profileimage__1.sprite = profileimage_demo.sprite;
                loadingPanel.SetActive(false);
                //  profileimage_demo = profileimage;

                //if (UIController.PlayAgain == 1)
                //{
                //    loadingPanel.SetActive(false);
                //    EnablePVP();
                //}

            }
            else
            {
                Debug.Log("profile url" + profileimgurl);
                StartCoroutine(Load(profileimgurl, IsInternetConnection()));
            }




        }
        else
        {
            Debug.Log("else");
            profile.SetActive(false);
            loginPanel.SetActive(true);
        }



    }
    public void cutggp()
    {
        //StartCoroutine(getprofile());
        StartCoroutine(cout_add_ggp());
    }
    IEnumerator cout_add_ggp()
    {
        Openlink.Inst.gamejsondata();
        UnityWebRequest ca = UnityWebRequest.Get(Profile_url);
        ca.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("a_token").ToString());

        yield return ca.Send();
        string caurl = ca.downloadHandler.text;
        JSONObject cal = new JSONObject(caurl);



        gemsValue = float.Parse(cal[2].ToString().Trim((trim_char_arry)).ToString());

        double Round_gemsValue = Math.Round(gemsValue, 2);
        gems.text = "" + Round_gemsValue;




        //   gems.text = cal[2].ToString().Trim((trim_char_arry)).ToString();
        ggp = float.Parse(pro[2].ToString().Trim((trim_char_arry)).ToString());
    }
    void ShowPreLoader(bool show)
    {
        //preloader.enabled = true;
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }
    IEnumerator Load(string url, bool offline)
    {
        Debug.Log("call load" + "url=" + url + "   " + offline);
        // ShowPreLoader(true);
        ShowPreLoader(true);
        WWW www = new WWW(url);
        yield return www;
        if (www.texture != null)
        {
            Debug.Log("www.texture=" + www.texture);
            ShowPreLoader(false);

            isLoaded = true;
            Texture2D texture = www.texture;
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            profileimage.sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));


            profileimage__1.sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));


            loadingPanel.SetActive(false);

            profileimage_URL = url;



            profileimage_demo = profileimage;



            //if (UIController.PlayAgain == 1)
            //{
            //    loadingPanel.SetActive(false);
            //    EnablePVP();
            //}


        }
        else
        {
            if (offline)
            {
                Debug.Log("your offline");

            }

        }

    }



    public JSONObject obj;
    public JSONObject obj1;
    internal IEnumerator getggpprice()
    {
        Debug.Log("<color=blue>SEND:</color>");
        string BASE_URL = "https://wgcapi.mmocircles.com/market/rates";

        UnityWebRequest www = UnityWebRequest.Get(BASE_URL);
        www.chunkedTransfer = false;

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("this is get ggp  error" + www.error);
        }
        else
        {
            string demo = www.downloadHandler.text.ToString();
            Debug.Log("<color=yellow> Receive: </color>" + demo);
            JSONObject obj = new JSONObject(demo);
            JSONObject dataarray = new JSONObject();
            for (int i = 0; i < obj.Count; i++)
            {
                dataarray.Add(obj[i]);

            }
            float price = float.Parse(dataarray[0].GetField("price").ToString().Trim(trim_char_arry));

            //ggp1.text = (1.99 / price).ToString();
            //ggp2.text = (4.99 / price).ToString();
            //ggp3.text = (9.99 / price).ToString();
            //ggp4.text = (19.99 / price).ToString();

            //Debug.Log("<color=yellow> Receive: </color>" + CarromGS.Inst.OldDate);

        }
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------





    //--------------------------------------------------------------------------------------



    public void LoginFun()
    {

        if (DeepClass.IsInternetConnection())
        {
            //profileimage.sprite = noimage.sprite;
            profileimage_URL = "";
            CreatehWebview();
            uniWebView.urlOnStart = "https://wgcapi.mmocircles.com/auth/authorize?client_id=" + clientId + "&response_type=code&redirect_uri=" + redirectUri+"&scope=offline_access";
            uniWebView.gameObject.SetActive(true);
            uniWebView.Show();

            PlayerPrefs.SetString("Defender__display_name", "");
            PlayerPrefs.SetString("Defender__URL", "");
        }
        else
        {
            ////"No internet connection available!...";
            //uiManager.msgTxt_noLogin.SetActive(false);
            //uiManager.msgTxt_noInternet.SetActive(true);
            //uiManager.MiddileOK.SetActive(true);
            //uiManager.LoginOK.SetActive(false);
            //uiManager.Login.SetActive(false);
            //uiManager.MsgPopUp.SetActive(true);
        }

    }


    public void CreatehWebview()
    {
        Debug.Log(" Create Start");
        if (uniWebView != null)
        {
            if (uniWebView.gameObject.transform.parent != null)
            {
                Destroy(uniWebView.gameObject.transform.parent.gameObject);
            }
            else
            {
                Destroy(uniWebView.gameObject);
            }
        }
        uniWebView = Instantiate(webviewPrefab).GetComponent<UniWebView>();

        uniWebView.OnPageFinished += (UniWebView webView, int statusCode, string url) =>
        {
            loadingAnimation.SetActive(true);
            playwithout.interactable = false;
            if (url.Contains(redirectUri + "?code="))
            {
                checkerror = 1;
                OnLoginCallBackUrl(url);
            }
            if (url.Contains("access_denied"))
            {
                uniWebView.Hide();
                uniWebView.gameObject.SetActive(false);
            }

        };

        uniWebView.OnPageStarted += (UniWebView webView, string url) =>
        {
        };

        uniWebView.OnPageErrorReceived += (UniWebView webView, int errorCode, string errorMessage) =>
        {
            if (checkerror == 1)
            {
                checkerror = 0;
            }
        };
        callForOpenURL();
    }


    void callForOpenURL()
    {
        Load("https://wgcapi.mmocircles.com/auth/authorize?client_id=" + clientId + "&response_type=code&redirect_uri=" + redirectUri+ "&scope=offline_access");

    }


    public void Load(string url)
    {
        gameObject.SetActive(true);
        uniWebView.CleanCache();
        uniWebView.Load(url);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }




    private void OnLoginCallBackUrl(string str)
    {
        uniWebView.Hide();
        uniWebView.gameObject.SetActive(false);

        if (str.Contains("access_denied"))
        {
            Debug.LogError("HERE PROBLEM");
        }
        else
        {
            string SplitHere= "&";
            code = "";
            for (int i = (redirectUri + "?code=").Length; i < str.Length; i++)
            {
            if(str[i]==SplitHere[0]) break;
                code += str[i];
            }
            StartCoroutine(Servercall());
        }
    }



    //-------------------------------------
    public void PlayWithoutLogin()
    {
        playWithoutLoiginVar = 1;
        loginPanel.SetActive(false);
        guest.SetActive(true);
        loadingPanel.SetActive(false);
        profile.SetActive(false);



    }


    //------------------------------------------------------------------------------------------------------------


    public void openSystemPanel()
    {
        Debug.Log("open");
        SystemPanel.SetActive(true);
    }

    public void CloseSystemPanel()
    {
        SystemPanel.SetActive(false);
    }


    //------------------------------------------------------------------------------------------------------------------------
    public void Logout()
    {

        openUrlBtn.interactable = true;
        playwithout.interactable = true;
        state.text = "";
        loadingAnimation.SetActive(false);
        logoutpanels.SetActive(false);

        if (DeepClass.IsInternetConnection())
        {
            UniWebView.ClearCookies();
            PlayerPrefs.SetString("a_token", "");
            Debug.Log("accrss_token" + PlayerPrefs.GetString("a_token").ToString());
            PlayerPrefs.SetString("r_token", "");
            Debug.Log("refresh_token" + PlayerPrefs.GetString("r_token").ToString());



            settingsUI.SetActive(false);
            SystemPanel.SetActive(false);
            profile.SetActive(false);
            guest.SetActive(true);
            loginPanel.SetActive(true);
            //uiManager.MsgPopUp.SetActive(false);
        }
        else
        {


            //"No internet connection available!...";
            //uiManager.msgTxt_noLogin.SetActive(false);
            //uiManager.msgTxt_noInternet.SetActive(true);
            //uiManager.MiddileOK.SetActive(true);
            //uiManager.LoginOK.SetActive(false);
            //uiManager.Login.SetActive(false);
            //uiManager.MsgPopUp.SetActive(true);
        }




    }



    //-----------------------------------------History-----------------------------------------------------------

    public void showHistory()
    {
        if (DeepClass.IsInternetConnection())
        {
            if (DeepClass.playWithoutLoiginVar == 0)
            {
                loadingPanel.SetActive(true);
                StartCoroutine(gethistory());
            }
            else
            {


                // msgTxt.text = "Please login with your GGP account";
                //uiManager.msgTxt_noLogin.SetActive(true);
                //uiManager.msgTxt_noInternet.SetActive(false);
                //uiManager.MiddileOK.SetActive(false);
                //uiManager.LoginOK.SetActive(true);
                //uiManager.Login.SetActive(true);
                //uiManager.MsgPopUp.SetActive(true);

            }
        }
        else
        {



            //"No internet connection available!...";
            //uiManager.msgTxt_noLogin.SetActive(false);
            //uiManager.msgTxt_noInternet.SetActive(true);
            //uiManager.MiddileOK.SetActive(true);
            //uiManager.LoginOK.SetActive(false);
            //uiManager.Login.SetActive(false);
            //uiManager.MsgPopUp.SetActive(true);
        }

    }









    public void hide_History()
    {

        historyCheck.SetActive(false);
    }


    internal IEnumerator gethistory()
    {
        Debug.Log("<color=blue>SEND:</color>");

        string HISTORY_URL = "https://wgcapi.mmocircles.com/profile/transaction-history?pageSize=10";


        UnityWebRequest www = UnityWebRequest.Get(HISTORY_URL);

        www.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("a_token").ToString());
        www.SetRequestHeader("Content-Type", "application/json");




        www.chunkedTransfer = false;

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("this is get ggp  error" + www.error);
            loadingPanel.SetActive(false);
        }
        else
        {

            loadingPanel.SetActive(false);
            string full = www.downloadHandler.text;
            print(full);
            JSONObject obj_1 = new JSONObject(full);

            string adon = @"{""HistoryClass"":";

            string HardCode1 = full.Insert(0, adon);
            string HardCode2 = String.Concat(HardCode1, "}");

            Debug.Log("Hard coded" + HardCode2);
            // ParentHistoryObj =  JsonConvert.DeserializeObject<ParentHistory>(HardCode2);

            JSONObject obj = new JSONObject(HardCode2);



            Debug.Log("FetchSGCHistoryData " + www.downloadHandler.text);
            SGCHistoryUserData.Clear();

            historyCheck.SetActive(true); // should enable


            //----------------------------------------------------------------------------------------------------


            for (int i = 0; i < obj.GetField("HistoryClass").Count; i++)
            {
                JSONObject obj1 = obj.GetField("HistoryClass")[i];

                SGCHistoryData user = new SGCHistoryData();
                //user.date = obj1.GetField("issue_date").str.ToString();

                string datevalue = obj1.GetField("issue_date").str.ToString();
                DateTime dateTime12 = Convert.ToDateTime(datevalue);
                string newdate = dateTime12.ToString("dd-MM-yyyy");
                user.date = newdate;

                if (!string.IsNullOrEmpty(obj1.GetField("type").str))
                {
                    user.type = obj1.GetField("type").str.ToString();

                }

                user.amount = obj1.GetField("amount").ToString();

                if (!string.IsNullOrEmpty(obj1.GetField("game").str))
                    user.game = obj1.GetField("game").str.ToString();

                if (!string.IsNullOrEmpty(obj1.GetField("type").str))
                {
                    if (user.type.Equals("Entry") || user.type.Equals("Prize"))
                    {
                        if (obj1.GetField("game") != null)
                        {
                            user.game = obj1.GetField("game").str.ToString();
                            if (!string.IsNullOrEmpty(obj1.GetField("product_name").str))
                            {
                                user.productName = obj1.GetField("product_name").str.ToString();
                            }


                            JSONObject obj2 = obj1.GetField("participants")[0];

                            /* if (obj2.GetField("user_id").ToString().
                                   Equals(ProfileManager.Instance.profileData.id.ToString()) &&
                                   obj1.GetField("participants").Count > 1)*/


                            if (obj1.GetField("participants").Count > 1)
                            {
                                user.parti = obj1.GetField("participants")[1].GetField("display_name").str;
                            }
                            else
                            {
                                user.parti = obj1.GetField("participants")[0].GetField("display_name").str;
                            }
                            //  user.starttime = obj1.GetField("pool_details").GetField("start_time").str.ToString();

                            String DateStringof = obj1.GetField("pool_details").GetField("start_time").str.ToString();
                            DateTime dateTime12of = Convert.ToDateTime(DateStringof);
                            user.starttime = dateTime12of.ToString(("dd-MM-yyyy hh:mm tt"));

                            if (!string.IsNullOrEmpty(obj1.GetField("pool_details").GetField("end_time").str))
                            {
                                // user.endtime = obj1.GetField("pool_details").GetField("end_time").str.ToString();

                                String DateStringof_1 = obj1.GetField("pool_details").GetField("end_time").str.ToString();
                                DateTime dateTime12of_1 = Convert.ToDateTime(DateStringof_1);
                                user.endtime = dateTime12of_1.ToString(("dd-MM-yyyy hh:mm tt"));

                            }
                            else
                            {
                                user.endtime = user.starttime;

                            }
                        }
                    }
                    else if (user.type.Equals("Transfer"))
                    {
                        JSONObject obj2 = obj1.GetField("participants")[0];
                        user.parti = obj1.GetField("participants")[0].GetField("display_name").str;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(obj1.GetField("product_name").str))
                        {
                            user.type = obj1.GetField("product_name").str.ToString();
                        }

                    }

                    SGCHistoryUserData.Add(user);
                }

            }
            ListSGCHistoryItems();

        }


        void ListSGCHistoryItems()
        {


            for (int i = 0; i < SGCHistoryUserData.Count; i++)
            {

                Text Date_txt = historyObjects[i].transform.GetChild(0).GetChild(0).GetComponent<Text>();
                Text Type_txt = historyObjects[i].transform.GetChild(0).GetChild(1).GetComponent<Text>();
                Text Amount_txt = historyObjects[i].transform.GetChild(0).GetChild(2).GetComponent<Text>();
                Text Game_txt = historyObjects[i].transform.GetChild(0).GetChild(3).GetComponent<Text>();
                Text Part_txt = historyObjects[i].transform.GetChild(0).GetChild(4).GetComponent<Text>();
                Text Start_txt = historyObjects[i].transform.GetChild(0).GetChild(5).GetComponent<Text>();
                Text End_time = historyObjects[i].transform.GetChild(0).GetChild(6).GetComponent<Text>();

                Date_txt.text = SGCHistoryUserData[i].date;
                Type_txt.text = SGCHistoryUserData[i].type;
                string AmountValueSring = SGCHistoryUserData[i].amount;
                float AmountValue = float.Parse(AmountValueSring);
                if (AmountValue > 0)
                {
                    Amount_txt.color = Color.green;
                    Amount_txt.text = SGCHistoryUserData[i].amount;

                }
                else
                {
                    Amount_txt.color = Color.red;
                    Amount_txt.text = SGCHistoryUserData[i].amount;

                }

                Game_txt.text = SGCHistoryUserData[i].game;
                Part_txt.text = SGCHistoryUserData[i].parti;
                Start_txt.text = SGCHistoryUserData[i].starttime;
                End_time.text = SGCHistoryUserData[i].endtime;


            }
        }

    }





    //----------------------------------pvp-----------------------------------------------------------








    public void PlayfromponentPage()
    {

    }
    public void hideponentPage()
    {

        OponentPage.SetActive(false);

    }



    public void playPVP_page_func()
    {
        if (DeepClass.IsInternetConnection())
        {
            if (DeepClass.playWithoutLoiginVar == 0)
            {

                string gem_String = gems.text;
                float gem_float = float.Parse(gem_String);
                if (gem_float < 1)
                {
                    noSGC.SetActive(true);
                }
                else
                {
                    PlayPVP_page.SetActive(true);


                }
                //
            }
            else
            {
                // msgTxt.text = "Please login with your GGP account";
                //uiManager.msgTxt_noLogin.SetActive(true);
                //uiManager.msgTxt_noInternet.SetActive(false);
                //uiManager.MiddileOK.SetActive(false);
                //uiManager.LoginOK.SetActive(true);
                //uiManager.Login.SetActive(true);
                //uiManager.MsgPopUp.SetActive(true);
            }
        }
        else
        {
            //"No internet connection available!...";
            //uiManager.msgTxt_noLogin.SetActive(false);
            //uiManager.msgTxt_noInternet.SetActive(true);
            //uiManager.MiddileOK.SetActive(true);
            //uiManager.LoginOK.SetActive(false);
            //uiManager.Login.SetActive(false);
            //uiManager.MsgPopUp.SetActive(true);

        }

    }






















    public void hidePlay_PVP_fun()
    {

        PlayPVP_page.SetActive(false);

    }


    public void EnablePVP()
    {

        if (DeepClass.IsInternetConnection())
        {
            if (DeepClass.playWithoutLoiginVar == 0)
            {

                string gem_String = gems.text;
                float gem_float = float.Parse(gem_String);
                if (gem_float < 1)
                {
                    noSGC.SetActive(true);
                }
                else
                {
                    PlayPVP_page.SetActive(false);
                    takeChallenge = 0;
                    StartCoroutine(AcceptChallenge());
                    // loadingPanel.SetActive(true);
                    OponentPage.SetActive(true); //should remove
                    loadingPanelforoppo.SetActive(true);
                    Onprograss.SetActive(false);
                    yourTurn.SetActive(false);



                }
                //
            }
            else
            {
                // msgTxt.text = "Please login with your GGP account";
                //uiManager.msgTxt_noLogin.SetActive(true);
                //uiManager.msgTxt_noInternet.SetActive(false);
                //uiManager.MiddileOK.SetActive(false);
                //uiManager.LoginOK.SetActive(true);
                //uiManager.Login.SetActive(true);
                //uiManager.MsgPopUp.SetActive(true);
            }
        }
        else
        {
            ////"No internet connection available!...";
            //uiManager.msgTxt_noLogin.SetActive(false);
            //uiManager.msgTxt_noInternet.SetActive(true);
            //uiManager.MiddileOK.SetActive(true);
            //uiManager.LoginOK.SetActive(false);
            //uiManager.Login.SetActive(false);
            //uiManager.MsgPopUp.SetActive(true);

        }





    }












    internal IEnumerator AcceptChallenge()
    {
        String acc = "CfDJ8A-zzwCOzPJAjbSP19jEGivYa84YwkMkKc-8083XU5OGx3uHygkg6OHp4mjmo9cVEgUN5u8Nb52xeT_vVCp5D-n0Nw3gzyml9Ce6fXIlJ6E9zJBXzY5ZfTBzGF87na4ni-GBxgKDbeYHQOeCRUxg5AmfYHpIzrf_q5m_dm6a7ysBjfEvwrCBf2_UUl8-DoYo_nLWsKTJWqdpc_DI1UFwpVhJFUJWxw8pza9kSD3dkdW4kke9Fk3p50GpsQlWx4VzbfWrqYVr7y3OUSTZuW3020FbAMnAznrod3PR7bOfhlrLU9h0W1iqZTyagIj59T9pLGZ4C6EpckMvHbAjzE8CqUmzF0VTPXbaBVSE5As_7Xg0-3Sq90L_unPSN7FLrM7KpapBiNZL_lo5mI9tLuK1_kG_7KW-mlmAa7-_UrdpOH9tJ9fvJOzs-rtROSCeEbcNrJUfeirsv0_q7pBc9mO6zEV7Be2hrVFG2gbQoZLRY9pOkKnhX0rIHZX7pJcdJ33eAwB08Uh4_R9KJrdUd-u5TZmueU_JOuwkefRPQ9K6vqIKyY5vddB6AAqIiqrXjG7xQqynJCD-fBV-e9nZrrgS2Jw7MlFtYw8IA-YO3vyn8fwvNmGsUr7FT5tQehADnPhAUwouSBLW8hAVlz11hyIf5Fupf8EzJ0iy_j2vXUBkT7yY1o83VlNSHY8Cr_jBC0sdn4-2kOG3U09yWGDokSSEKrz2Q5CXYAnfh-alHsFn6qRIh4bR-DHkLeCQzOxhaJR9FbmVtVBh8PXid4WJqt51lp2rgUJPLGvOQYJTv3EULW_r4Lev1brFFxUKutjifxvjjZDg1eL3YFig3Kvc5fukJ9HVIrSw0TLQeeL_dISoWJyJF4DGZxRu6VQbjv3ZI71YPVdkM0RoAzj4rf5hKUKIK-Go9ut5D83ak68yIveh1MDJOJARsfGfcgJBKumvpC_M9VOIGB5ciFW9jOOspymJVQWo8aL9_q8QHGTBxljjyve5Gge45VYtCzVOGCEr5FV-e7l2aWAlFKMTIotOMCvf7oq6XqeFQmhmscJUhA0i7xaZvU8SP5bqFQdiEW47MS6Z4Q";
        string url = "https://wgcapi.mmocircles.com/pool/challenge/accept";
        JSONObject data = new JSONObject();
        data.AddField("wager", EntryFeeController.currentEntryFee);

        Debug.Log("form" + data.ToString());

        UnityWebRequest pro = UnityWebRequest.Put(url, data.ToString());
        pro.method = UnityWebRequest.kHttpVerbPOST;
        pro.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("a_token").ToString());
        //  pro.SetRequestHeader("Authorization", "Bearer " + acc);
        pro.SetRequestHeader("Content-Type", "application/json");
        yield return pro.SendWebRequest();

        if (pro.error != null)
        {
            string demo = pro.downloadHandler.text;

            Debug.Log("F---->" + demo);

            string full = demo;
            JSONObject obj = new JSONObject(full);
            string Error_string = obj.GetField("Message").ToString();
            Error_string = Error_string.ToString().Trim((trim_char_arry));
            Debug.Log("error-->>>>>>>>>>>>>>>>>>>>>>>>>>>-->" + Error_string);
            if (Error_string == "No valid outstanding challenges found.")
            {
                Debug.Log("No Challenges Found, Moving to Issue Challenge");
                StartCoroutine(issueChallenge());
            }
            else
            {
                //"No internet connection available!...";
                //uiManager.msgTxt_noLogin.SetActive(false);
                //uiManager.msgTxt_noInternet.SetActive(true);
                //uiManager.MiddileOK.SetActive(true);
                //uiManager.LoginOK.SetActive(false);
                //uiManager.Login.SetActive(false);
                //uiManager.MsgPopUp.SetActive(true);


                loadingPanel.SetActive(false);
                Debug.Log("Error");
            }

        }
        else
        {

            Debug.Log("No Error");
            string demo = pro.downloadHandler.text;
            string full = demo;
            JSONObject obj1 = new JSONObject(full);
            string id_string = obj1.GetField("id").ToString();
            id_string = id_string.ToString().Trim((trim_char_arry));
            Debug.Log("id_string---->" + id_string);
            DeepClass.ChallengeID = id_string;
            DeepClass.Challenge_Var = 2;




            string datevalue = obj1.GetField("created").str.ToString();
            DateTime dateTime12 = Convert.ToDateTime(datevalue);
            string newdate = dateTime12.ToString("dd-MM-yyyy");
            Opponent_date.text = "" + newdate.ToString();


            //----------------------------------------------------------------------------------------


            imageCount = 0;

            /*  if (PlayerPrefs.GetString("Defender__display_name").ToString() != "")
              {

                  imageCountCheck = 1;

                  completedProfile.SetActive(true);
                  Completed_username.text = "" + PlayerPrefs.GetString("Defender__display_name").ToString();
                  Completed_WinorLose.text = "" + PlayerPrefs.GetString("WinorLose").ToString();
              //    StartCoroutine(PicProfileforoponentfromAccept(PlayerPrefs.GetString("Defender__URL").ToString(), Completed_UserPic));
              }
              else
              {
                  imageCountCheck = 1;
                  completedProfile.SetActive(false);
              }
            */



            if ((obj1.GetField("challenger").Count) > 1)
            {

                if (!string.IsNullOrEmpty(obj1.GetField("defender").GetField("display_name").str))
                {
                    yourTurn_username.text = "" + obj1.GetField("defender").GetField("display_name").str.ToString();
                }
                if (!string.IsNullOrEmpty(obj1.GetField("defender").GetField("avatar_url").str))
                {
                    string challengerURL = obj1.GetField("defender").GetField("avatar_url").str.ToString(); //Challenger_pic

                    imageCountCheck = 1;
                    takeChallenge = 0;
                    StartCoroutine(PicProfileforoponentfromAccept(challengerURL, yourTurn_tImage));





                }
                else
                {
                    yourTurn_tImage_demo = default_image;
                    yourTurn_tImage.sprite = default_image.sprite;
                    yourTurn.SetActive(true);
                    Onprograss.SetActive(false);
                    loadingPanelforoppo.SetActive(false);
                    StartCoroutine(WaitForthree());

                }

            }


            //------------------------------------------------------------------------------------------completed..................................





        }


        Debug.Log(" uwr.downloadHandler.text>>>>" + pro.downloadHandler.text);
        Debug.Log("!------------------------DeepClass.ChallengeID------------------>" + DeepClass.ChallengeID);


    }



    IEnumerator PicProfileforoponentfromAccept(string url, Image pic)
    {

        takeChallenge = 1;
        StartCoroutine(WaitFor_45_secFromAccept(pic));

        WWW www = new WWW(url);
        yield return www;
        imageCount++;
        if (imageCount == imageCountCheck)
        {
            if (takeChallenge == 1)
            {
                yourTurn.SetActive(true);
                Onprograss.SetActive(false);
                loadingPanelforoppo.SetActive(false);
                takeChallenge = 0;
                LoadingAvatar.SetActive(false);
                StartCoroutine(WaitForthree());
            }
        }
        if (www.texture != null)
        {

            Texture2D texture = www.texture;
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            if (pic != null)
            {
                pic.sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
            }

            if (pic == yourTurn_tImage)
            {

                yourTurn_tImage_demo = yourTurn_tImage;
            }




        }
        else
        {
            Debug.Log("not reach");
        }


    }












    internal IEnumerator issueChallenge()
    {
        String acc = "CfDJ8A-zzwCOzPJAjbSP19jEGivYa84YwkMkKc-8083XU5OGx3uHygkg6OHp4mjmo9cVEgUN5u8Nb52xeT_vVCp5D-n0Nw3gzyml9Ce6fXIlJ6E9zJBXzY5ZfTBzGF87na4ni-GBxgKDbeYHQOeCRUxg5AmfYHpIzrf_q5m_dm6a7ysBjfEvwrCBf2_UUl8-DoYo_nLWsKTJWqdpc_DI1UFwpVhJFUJWxw8pza9kSD3dkdW4kke9Fk3p50GpsQlWx4VzbfWrqYVr7y3OUSTZuW3020FbAMnAznrod3PR7bOfhlrLU9h0W1iqZTyagIj59T9pLGZ4C6EpckMvHbAjzE8CqUmzF0VTPXbaBVSE5As_7Xg0-3Sq90L_unPSN7FLrM7KpapBiNZL_lo5mI9tLuK1_kG_7KW-mlmAa7-_UrdpOH9tJ9fvJOzs-rtROSCeEbcNrJUfeirsv0_q7pBc9mO6zEV7Be2hrVFG2gbQoZLRY9pOkKnhX0rIHZX7pJcdJ33eAwB08Uh4_R9KJrdUd-u5TZmueU_JOuwkefRPQ9K6vqIKyY5vddB6AAqIiqrXjG7xQqynJCD-fBV-e9nZrrgS2Jw7MlFtYw8IA-YO3vyn8fwvNmGsUr7FT5tQehADnPhAUwouSBLW8hAVlz11hyIf5Fupf8EzJ0iy_j2vXUBkT7yY1o83VlNSHY8Cr_jBC0sdn4-2kOG3U09yWGDokSSEKrz2Q5CXYAnfh-alHsFn6qRIh4bR-DHkLeCQzOxhaJR9FbmVtVBh8PXid4WJqt51lp2rgUJPLGvOQYJTv3EULW_r4Lev1brFFxUKutjifxvjjZDg1eL3YFig3Kvc5fukJ9HVIrSw0TLQeeL_dISoWJyJF4DGZxRu6VQbjv3ZI71YPVdkM0RoAzj4rf5hKUKIK-Go9ut5D83ak68yIveh1MDJOJARsfGfcgJBKumvpC_M9VOIGB5ciFW9jOOspymJVQWo8aL9_q8QHGTBxljjyve5Gge45VYtCzVOGCEr5FV-e7l2aWAlFKMTIotOMCvf7oq6XqeFQmhmscJUhA0i7xaZvU8SP5bqFQdiEW47MS6Z4Q";
        string url = "https://wgcapi.mmocircles.com/pool/challenge/issue";
        JSONObject data = new JSONObject();
        data.AddField("wager", EntryFeeController.currentEntryFee);
        data.AddField("score", 0);

        Debug.Log("form" + data.ToString());

        UnityWebRequest pro = UnityWebRequest.Put(url, data.ToString());
        pro.method = UnityWebRequest.kHttpVerbPOST;
        pro.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("a_token").ToString());
        // pro.SetRequestHeader("Authorization", "Bearer " + acc);
        pro.SetRequestHeader("Content-Type", "application/json");
        yield return pro.SendWebRequest();

        if (pro.error != null)
        {
            //"No internet connection available!...";
            //uiManager.msgTxt_noLogin.SetActive(false);
            //uiManager.msgTxt_noInternet.SetActive(true);
            //uiManager.MiddileOK.SetActive(true);
            //uiManager.LoginOK.SetActive(false);
            //uiManager.Login.SetActive(false);
            //uiManager.MsgPopUp.SetActive(true);



            loadingPanel.SetActive(false);
            Debug.Log("Error");
        }
        else
        {
            imageCount = 0;
            Debug.Log("No Error");
            string demo = pro.downloadHandler.text;
            string full = demo;
            JSONObject obj = new JSONObject(full);
            string id_string = obj.GetField("id").ToString();
            id_string = id_string.ToString().Trim((trim_char_arry));
            Debug.Log("id_string---->" + id_string);
            DeepClass.ChallengeID = id_string;
            DeepClass.Challenge_Var = 1;

            string datevalue = obj.GetField("created").str.ToString();



            DateTime dateTime12 = Convert.ToDateTime(datevalue);
            string newdate = dateTime12.ToString("dd-MM-yyyy");
            Opponent_date.text = "" + newdate.ToString();



            //----------------------------------------------------------------------------------------















            //------------------------------------------------------------------------------------------completed..................................

            completedProfile.SetActive(false);
            yourTurn.SetActive(false);
            Onprograss.SetActive(true);
            loadingPanelforoppo.SetActive(false);
            StartCoroutine(WaitForthree());

            /*  if (PlayerPrefs.GetString("Defender__display_name").ToString() != "")
              {

                      imageCountCheck = 1;

                      completedProfile.SetActive(true);
                      Completed_username.text = ""+ PlayerPrefs.GetString("Defender__display_name").ToString();
                      Completed_WinorLose.text = ""+ PlayerPrefs.GetString("WinorLose").ToString();
                  //    StartCoroutine(PicProfileforoponent(PlayerPrefs.GetString("Defender__URL").ToString(), Completed_UserPic));
              }
              else
              {
                  completedProfile.SetActive(false);
                  yourTurn.SetActive(false);
                  Onprograss.SetActive(true);
                  loadingPanelforoppo.SetActive(false);
                  StartCoroutine(WaitForthree());
              }
            */

        }





        Debug.Log(" uwr.downloadHandler.text>>>>" + pro.downloadHandler.text);


    }


    IEnumerator PicProfileforoponent(string url, Image pic)
    {

        WWW www = new WWW(url);
        yield return www;
        imageCount++;
        if (imageCount == imageCountCheck)
        {
            yourTurn.SetActive(false);
            Onprograss.SetActive(true);
            loadingPanelforoppo.SetActive(false);
            StartCoroutine(WaitForthree());
        }
        if (www.texture != null)
        {

            Texture2D texture = www.texture;
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            if (pic != null)
            {
                pic.sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
            }




        }
        else
        {
            Debug.Log("not reach");
        }


    }

    //----------------------------------------






    IEnumerator WaitForthree()
    {
        yield return new WaitForSeconds(3);
        PlayfromponentPage();
    }

    //----------------------------------------------------

    IEnumerator WaitFor_45_secFromAccept(Image pic)
    {
        yield return new WaitForSeconds(45);
        if (takeChallenge == 1)
        {
            // pic.sprite = LoadingAvatar.sprite;
            LoadingAvatar.SetActive(true);
            yourTurn.SetActive(true);
            Onprograss.SetActive(false);
            loadingPanelforoppo.SetActive(false);
            takeChallenge = 0;
            StartCoroutine(WaitForthree());


        }
        //  PlayfromponentPage();
    }








    //---------------------------------------------------------------------------------------


    public void CompleteForChallenge()
    {

        Debug.Log("!------------------------11111");
        StartCoroutine(completeChallenge());
    }
    internal IEnumerator completeChallenge()
    {



        Debug.Log("!------------------------22222------------------>" + DeepClass.HighScoreValue);
        Debug.Log("!------------------------DeepClass.ChallengeID------------------>" + DeepClass.ChallengeID);
        String acc = "CfDJ8A-zzwCOzPJAjbSP19jEGivYa84YwkMkKc-8083XU5OGx3uHygkg6OHp4mjmo9cVEgUN5u8Nb52xeT_vVCp5D-n0Nw3gzyml9Ce6fXIlJ6E9zJBXzY5ZfTBzGF87na4ni-GBxgKDbeYHQOeCRUxg5AmfYHpIzrf_q5m_dm6a7ysBjfEvwrCBf2_UUl8-DoYo_nLWsKTJWqdpc_DI1UFwpVhJFUJWxw8pza9kSD3dkdW4kke9Fk3p50GpsQlWx4VzbfWrqYVr7y3OUSTZuW3020FbAMnAznrod3PR7bOfhlrLU9h0W1iqZTyagIj59T9pLGZ4C6EpckMvHbAjzE8CqUmzF0VTPXbaBVSE5As_7Xg0-3Sq90L_unPSN7FLrM7KpapBiNZL_lo5mI9tLuK1_kG_7KW-mlmAa7-_UrdpOH9tJ9fvJOzs-rtROSCeEbcNrJUfeirsv0_q7pBc9mO6zEV7Be2hrVFG2gbQoZLRY9pOkKnhX0rIHZX7pJcdJ33eAwB08Uh4_R9KJrdUd-u5TZmueU_JOuwkefRPQ9K6vqIKyY5vddB6AAqIiqrXjG7xQqynJCD-fBV-e9nZrrgS2Jw7MlFtYw8IA-YO3vyn8fwvNmGsUr7FT5tQehADnPhAUwouSBLW8hAVlz11hyIf5Fupf8EzJ0iy_j2vXUBkT7yY1o83VlNSHY8Cr_jBC0sdn4-2kOG3U09yWGDokSSEKrz2Q5CXYAnfh-alHsFn6qRIh4bR-DHkLeCQzOxhaJR9FbmVtVBh8PXid4WJqt51lp2rgUJPLGvOQYJTv3EULW_r4Lev1brFFxUKutjifxvjjZDg1eL3YFig3Kvc5fukJ9HVIrSw0TLQeeL_dISoWJyJF4DGZxRu6VQbjv3ZI71YPVdkM0RoAzj4rf5hKUKIK-Go9ut5D83ak68yIveh1MDJOJARsfGfcgJBKumvpC_M9VOIGB5ciFW9jOOspymJVQWo8aL9_q8QHGTBxljjyve5Gge45VYtCzVOGCEr5FV-e7l2aWAlFKMTIotOMCvf7oq6XqeFQmhmscJUhA0i7xaZvU8SP5bqFQdiEW47MS6Z4Q";

        string url = "https://wgcapi.mmocircles.com/pool/challenge/" + DeepClass.ChallengeID + "/complete";
        JSONObject data = new JSONObject();


        data.AddField("score", DeepClass.HighScoreValue);

        Debug.Log("form" + data.ToString());

        UnityWebRequest pro = UnityWebRequest.Put(url, data.ToString());
        pro.method = UnityWebRequest.kHttpVerbPOST;
        pro.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("a_token").ToString());
        //    pro.SetRequestHeader("Authorization", "Bearer " + acc);
        pro.SetRequestHeader("Content-Type", "application/json");
        yield return pro.SendWebRequest();

        if (pro.error != null)
        {




        }
        else
        {
            int compareScore1 = 0;
            int compareScore2 = 0;
            imageCount = 0;
            //  uiManager.shareUI.SetActive(false);
            ExitBtn.SetActive(true);
            SubmitBtn.SetActive(false);
            loadingPanel.SetActive(false);
            Debug.Log("No Error");



            string demo = pro.downloadHandler.text;
            string full = demo;

            Debug.Log("full");

            if (full != "")
            {
                JSONObject obj1 = new JSONObject(full);
                string id_string = obj1.GetField("id").ToString();
                id_string = id_string.ToString().Trim((trim_char_arry));
                Debug.Log("id_string---->" + id_string);
                string datevalue = obj1.GetField("created").str.ToString();

                DateTime dateTime12 = Convert.ToDateTime(datevalue);
                string newdate = dateTime12.ToString("dd-MM-yyyy");
                DateForscore_txt.text = "" + newdate.ToString();

                string Defendername = obj1.GetField("defender").GetField("display_name").str.ToString();
                PlayerPrefs.SetString("Defender__display_name", Defendername);
                Defender_name.text = "" + Defendername;
                string DefenderScore = obj1.GetField("defender").GetField("score").ToString();
                Defender_score.text = "" + DefenderScore;
                compareScore1 = int.Parse(DefenderScore);

                //  string DefenderURL = obj1.GetField("defender").GetField("avatar_url").str.ToString(); //DefenderPic
                //  PlayerPrefs.SetString("Defender__URL", DefenderURL);
                //  Debug.Log("URL--->" + DefenderURL);


                /* DefenderPic.sprite = yourTurn_tImage.sprite;
                 Challenger_pic.sprite = profileimage__1.sprite; */

                /*  if (!string.IsNullOrEmpty(obj1.GetField("defender").GetField("avatar_url").str.ToString()))
                  {
                      DefenderPic.sprite = yourTurn_tImage_demo.sprite;
                  }
                  else
                  {
                      DefenderPic.sprite = default_image.sprite;

                  } */

                DefenderPic.sprite = yourTurn_tImage_demo.sprite;



                int checkchallengerExist = 0;
                if ((obj1.GetField("challenger").Count) > 1)
                {
                    imageCountCheck = 2;
                    //    StartCoroutine(PicProfilePicFor(DefenderURL, DefenderPic));

                    if (!string.IsNullOrEmpty(obj1.GetField("challenger").GetField("display_name").str))
                    {
                        Challenger_name.text = "" + obj1.GetField("challenger").GetField("display_name").str.ToString();
                        checkchallengerExist++;

                    }
                    else
                    {
                        //"waiting for challenger";
                    }
                    if (!string.IsNullOrEmpty(obj1.GetField("challenger").GetField("score").ToString()))
                    {
                        string scoreOfchallenger = obj1.GetField("challenger").GetField("score").ToString();
                        Challenger_score.text = "" + scoreOfchallenger;

                        compareScore2 = int.Parse(scoreOfchallenger);
                        checkchallengerExist++;
                    }

                    Challenger_pic.sprite = profileimage_demo.sprite;
                    checkchallengerExist++;

                    /*  if (!string.IsNullOrEmpty(obj1.GetField("challenger").GetField("avatar_url").str))
                      {
                          string challengerURL = obj1.GetField("challenger").GetField("avatar_url").str.ToString(); //Challenger_pic
                                                                                                                    //   StartCoroutine(PicProfilePicFor(challengerURL, Challenger_pic));
                          Challenger_pic.sprite = profileimage_demo.sprite;
                          checkchallengerExist++;
                      }
                      else
                      {

                          Challenger_pic.sprite = default_image.sprite;
                          checkchallengerExist++;
                      }*/


                }
                else
                {
                    imageCountCheck = 1;
                    //   StartCoroutine(PicProfilePicFor(DefenderURL, DefenderPic));
                    //    DefenderPic.sprite = profileimage__1.sprite;
                }
                if (checkchallengerExist == 3)
                {
                    prograsss.SetActive(false);
                    scoreobject.SetActive(true);
                    profile__1.SetActive(true);
                    profile__2.SetActive(true);
                    notification_value = 1;





                    if (compareScore1 < compareScore2)
                    {


                        firstObject.SetActive(true);
                        secondObject.SetActive(false);
                        PlayerPrefs.SetString("WinorLose", "VICTORY");
                    }
                    else
                    {
                        firstObject.SetActive(false);
                        secondObject.SetActive(true);
                        PlayerPrefs.SetString("WinorLose", "DEFEAT");
                    }

                }
                else
                {
                    prograsss.SetActive(true);
                    scoreobject.SetActive(false);
                    profile__1.SetActive(true);
                    profile__2.SetActive(false);

                }



            }
            else
            {

                loadingPanel.SetActive(false);
                // ErrorTxt.text = "TIE , Play again and Submit Score ";

                ErrorTxt.SetActive(true);
                SubmitBtn.SetActive(false);
                ErrorRestart.SetActive(true);
                ExitBtn.SetActive(false);


            }






        }


        Debug.Log(" uwr.downloadHandler.text>>>>" + pro.downloadHandler.text);


    }


    public void UpdateForchallenge()
    {

        StartCoroutine(updateChallenge());
        Debug.Log("!------------------------3333");
    }


    internal IEnumerator updateChallenge()
    {
        Debug.Log("!------------------------444");
        Debug.Log("H-------------------------->" + DeepClass.ChallengeID);
        Debug.Log("H-------------------hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh------->" + DeepClass.HighScoreValue);

        String acc = "CfDJ8A-zzwCOzPJAjbSP19jEGivYa84YwkMkKc-8083XU5OGx3uHygkg6OHp4mjmo9cVEgUN5u8Nb52xeT_vVCp5D-n0Nw3gzyml9Ce6fXIlJ6E9zJBXzY5ZfTBzGF87na4ni-GBxgKDbeYHQOeCRUxg5AmfYHpIzrf_q5m_dm6a7ysBjfEvwrCBf2_UUl8-DoYo_nLWsKTJWqdpc_DI1UFwpVhJFUJWxw8pza9kSD3dkdW4kke9Fk3p50GpsQlWx4VzbfWrqYVr7y3OUSTZuW3020FbAMnAznrod3PR7bOfhlrLU9h0W1iqZTyagIj59T9pLGZ4C6EpckMvHbAjzE8CqUmzF0VTPXbaBVSE5As_7Xg0-3Sq90L_unPSN7FLrM7KpapBiNZL_lo5mI9tLuK1_kG_7KW-mlmAa7-_UrdpOH9tJ9fvJOzs-rtROSCeEbcNrJUfeirsv0_q7pBc9mO6zEV7Be2hrVFG2gbQoZLRY9pOkKnhX0rIHZX7pJcdJ33eAwB08Uh4_R9KJrdUd-u5TZmueU_JOuwkefRPQ9K6vqIKyY5vddB6AAqIiqrXjG7xQqynJCD-fBV-e9nZrrgS2Jw7MlFtYw8IA-YO3vyn8fwvNmGsUr7FT5tQehADnPhAUwouSBLW8hAVlz11hyIf5Fupf8EzJ0iy_j2vXUBkT7yY1o83VlNSHY8Cr_jBC0sdn4-2kOG3U09yWGDokSSEKrz2Q5CXYAnfh-alHsFn6qRIh4bR-DHkLeCQzOxhaJR9FbmVtVBh8PXid4WJqt51lp2rgUJPLGvOQYJTv3EULW_r4Lev1brFFxUKutjifxvjjZDg1eL3YFig3Kvc5fukJ9HVIrSw0TLQeeL_dISoWJyJF4DGZxRu6VQbjv3ZI71YPVdkM0RoAzj4rf5hKUKIK-Go9ut5D83ak68yIveh1MDJOJARsfGfcgJBKumvpC_M9VOIGB5ciFW9jOOspymJVQWo8aL9_q8QHGTBxljjyve5Gge45VYtCzVOGCEr5FV-e7l2aWAlFKMTIotOMCvf7oq6XqeFQmhmscJUhA0i7xaZvU8SP5bqFQdiEW47MS6Z4Q";
        string url = "https://wgcapi.mmocircles.com/pool/challenge/" + DeepClass.ChallengeID + "/update";
        JSONObject data = new JSONObject();
        data.AddField("score", DeepClass.HighScoreValue);
        Debug.Log("form" + data.ToString());

        UnityWebRequest pro = UnityWebRequest.Put(url, data.ToString());
        pro.method = UnityWebRequest.kHttpVerbPOST;
        pro.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("a_token").ToString());
        //   pro.SetRequestHeader("Authorization", "Bearer " + acc);
        pro.SetRequestHeader("Content-Type", "application/json");
        yield return pro.SendWebRequest();

        if (pro.error != null)
        {


            Debug.Log("Error");
        }
        else
        {
            int compareScore1 = 0;
            int compareScore2 = 0;
            imageCount = 0;

            //  uiManager.shareUI.SetActive(false);
            //   uiManager.submitscore.SetActive(true);
            ExitBtn.SetActive(true);
            loadingPanel.SetActive(false);
            SubmitBtn.SetActive(false);
            Debug.Log("No Error");



            string demo = pro.downloadHandler.text;
            string full = demo;
            JSONObject obj1 = new JSONObject(full);
            string id_string = obj1.GetField("id").ToString();
            id_string = id_string.ToString().Trim((trim_char_arry));
            Debug.Log("id_string---->" + id_string);
            string datevalue = obj1.GetField("created").str.ToString();

            DateTime dateTime12 = Convert.ToDateTime(datevalue);
            string newdate = dateTime12.ToString("dd-MM-yyyy");
            DateForscore_txt.text = "" + newdate.ToString();
            string Defendername = obj1.GetField("defender").GetField("display_name").str.ToString();
            Defender_name.text = "" + Defendername;
            string DefenderScore = obj1.GetField("defender").GetField("score").ToString();
            Defender_score.text = "" + DefenderScore;
            compareScore1 = int.Parse(DefenderScore);




            //   if (!string.IsNullOrEmpty(obj1.GetField("defender").GetField("avatar_url").str.ToString()))
            //  {
            //     string DefenderURL = obj1.GetField("defender").GetField("avatar_url").str.ToString(); //DefenderPic
            // }

            /*  if (!string.IsNullOrEmpty(DefenderURL))
              {
                  DefenderPic.sprite = profileimage__1.sprite;
              }
              else
              {
                  DefenderPic.sprite = default_image.sprite;

              }*/
            DefenderPic.sprite = profileimage__1.sprite;
            prograsss.SetActive(true);
            scoreobject.SetActive(false);
            profile__1.SetActive(true);
            profile__2.SetActive(false);

            //Debug.Log("URL--->"+ DefenderURL);



            /*     int checkchallengerExist = 0;
                 if ((obj1.GetField("challenger").Count) > 1)
                 {
                     imageCountCheck = 2;
                     //   StartCoroutine(PicProfilePicFor(DefenderURL, DefenderPic));
                  //   DefenderPic.sprite = profileimage__1.sprite;
                     if (!string.IsNullOrEmpty(obj1.GetField("challenger").GetField("display_name").str))
                     {
                         Challenger_name.text = "" + obj1.GetField("challenger").GetField("display_name").str.ToString();
                         checkchallengerExist++;
                     }
                     else
                     {
                         //"waiting for challenger";
                     }
                     if (!string.IsNullOrEmpty(obj1.GetField("challenger").GetField("score").ToString()))
                     {
                         string scoreOfchallenger = obj1.GetField("challenger").GetField("score").ToString();
                         Challenger_score.text = "" + scoreOfchallenger;

                         compareScore2 = int.Parse(scoreOfchallenger);
                         checkchallengerExist++;
                     }

                     if (!string.IsNullOrEmpty(obj1.GetField("challenger").GetField("avatar_url").str))
                     {
                         string challengerURL = obj1.GetField("challenger").GetField("avatar_url").str.ToString(); //Challenger_pic
                   //      StartCoroutine(PicProfilePicFor(challengerURL, Challenger_pic));
                         checkchallengerExist++;
                     }else
                     {
                         checkchallengerExist++;

                     }
                 }
                 else
                 {
                     imageCountCheck = 1;
                     //     StartCoroutine(PicProfilePicFor(DefenderURL, DefenderPic));
                //     DefenderPic.sprite = profileimage__1.sprite;
                 }
                 if(checkchallengerExist==3)
                  {
                      prograsss.SetActive(false);
                      scoreobject.SetActive(true);
                      profile__1.SetActive(true);
                      profile__2.SetActive(true);
                     if (compareScore1 < compareScore2)
                     {


                         firstObject.SetActive(true);
                         secondObject.SetActive(false);
                         PlayerPrefs.SetString("WinorLose", "VICTORY");
                     }
                     else
                     {
                         firstObject.SetActive(true);
                         secondObject.SetActive(true);
                         PlayerPrefs.SetString("WinorLose", "DEFEAT");
                     }

                 }
                  else
                  {
                      prograsss.SetActive(true);
                      scoreobject.SetActive(false);
                      profile__1.SetActive(true);
                      profile__2.SetActive(false);

                  }*/



        }


        Debug.Log(" uwr.downloadHandler.text>>>>" + pro.downloadHandler.text);


    }



    IEnumerator PicProfilePicFor(string url, Image pic)
    {

        WWW www = new WWW(url);
        yield return www;
        imageCount++;
        /*   if (imageCount == imageCountCheck)
           {
               uiManager.shareUI.SetActive(false);
               uiManager.submitscore.SetActive(true);
               loadingPanel.SetActive(false);
           } */
        if (www.texture != null)
        {

            Texture2D texture = www.texture;
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            if (pic != null)
            {
                pic.sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
            }



        }
        else
        {
            Debug.Log("not reach");
        }


    }




    //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&//


    public void getDetails()
    {







        if (DeepClass.IsInternetConnection())
        {
            if (DeepClass.playWithoutLoiginVar == 0)
            {

                BattileUserData.Clear();
                BattileUserData1.Clear();
                loadingPanel.SetActive(true);
                StartCoroutine(getprofiledetails());

            }
            else
            {
                // msgTxt.text = "Please login with your GGP account";
                //uiManager.msgTxt_noLogin.SetActive(true);
                //uiManager.msgTxt_noInternet.SetActive(false);
                //uiManager.MiddileOK.SetActive(false);
                //uiManager.LoginOK.SetActive(true);
                //uiManager.Login.SetActive(true);
                //uiManager.MsgPopUp.SetActive(true);
            }
        }
        else
        {
            //"No internet connection available!...";
            //uiManager.msgTxt_noLogin.SetActive(false);
            //uiManager.msgTxt_noInternet.SetActive(true);
            //uiManager.MiddileOK.SetActive(true);
            //uiManager.LoginOK.SetActive(false);
            //uiManager.Login.SetActive(false);
            //uiManager.MsgPopUp.SetActive(true);
        }



        //   loadingPanel.SetActive(true);
        //  StartCoroutine(getprofiledetails());





        /*  String DateString = "2020-01-02T05:18:55.5872048";
          DateTime dateTime12 = Convert.ToDateTime(DateString);
          //  user._date = dateTime12.ToString("hh:mm:ss tt");
          Debug.Log("Date--------------------->" + dateTime12.ToString("hh:mm tt")); */


    }


    internal IEnumerator getprofiledetails()
    {
        String acc = "CfDJ8A-zzwCOzPJAjbSP19jEGithr-4aTC-QqI19ZGcntNaSVxkgJ1Ki_Ut2dkxZWhqmrPa0TrTDlRKdJCkw5oVGPJ5LBc2Jqitjsgz-os9CObYyenl6EsySNg2SqfuQuOe5lHQBJaGpU1blHJ7hKFhhJn9er20JLsEErBy4YD2I0HWqdCYnynuPzcjQq2MTEXqXOMMJQ4dSzy0dOEg6HlyKu7-RTdQXWSOq4sBaZOh6JrDvKKfkAAzoIpwMXx_UgAnmj9uUJejIs2uzmC_EfBJT3_VLMCHtAqKfQj6b4pEA3UrAg-Rxo-eAiEoXbPiTk9SOncZPQzUIW7Cmxl8wNtEFHudefELOniGAmIdLL3Gg2ivQyj7XirDJrc3UdUmZfwHXEu758cEQYTnFXeF2POK5ZKFTwBlMgTiDG70sgHQ29HjOav5FiNrd_FaVyN3CyYwnYwVs1XEitXbN3f7Q8WA96gZdRX6FIQVayDsH7aGZJGFo5AzbkPq9eAIwy9zbPwaSxpdmywMkqLM3RQCR-W2gABdfman0HcXQSuF73RFv_M839qfFvBSA1lIDjZIhTydNhH0JsHe8UNcWj_UP7pkHZS0a07u1tXC6vWvqPqUgWDzVUKfNtS8MP84ZHAGe7aT-oPjmPCHSpFAiUDfcUdY5XxLda1Fh9DBXMJGxCC1K65d51D4eXo55w9wEtTaU6F_ZeCi5t6KBrChESOUleGv_OXR_6vHHnAc5PdUeWa3GSUnlLPyqSa2k6WVPH95_0cgx8vA_zvQwIoP9rHU-4ZEHoUGl5YSFTQCPxMIQ4qBZ5Ujw8f0LtNk54vfUjCpn3dcsfyyTVLHf4rQyMkpxvaNsH7GroZEOU5qIBbkeUfgq25LdPZ9K26UD8yoWgjpoWSla5xusfYMaQLkZ3RhV4DMpvcURGbUd765EfQo2SJCydNnWDXH-ijdAXvy75KfhwSlgwPilLgKWap3clmb2d2F4UaYsoPLhwxBwoJa3y3Ke6hRnJoXwJg5z80lKumoBtl8LAVv9QYeExAgi7mAnpa5kMAISXqKmKRj4-zh84k-_bZpo6ogxMEMOzooToUg5cG_jHv6_qEZd8lcg6Ca8GjKlDMo";
        string BASE_URL = "https://wgcapi.mmocircles.com/pool/challenges?range=50";

        UnityWebRequest www = UnityWebRequest.Get(BASE_URL);
        //  www.chunkedTransfer = false;
        www.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("a_token").ToString());
        //  www.SetRequestHeader("Authorization", "Bearer " + acc);
        www.SetRequestHeader("accept", "application/json");

        yield return www.SendWebRequest();


        if (www.isNetworkError)
        {
            Debug.Log("this is get ggp  error" + www.error);
            battileMsg.SetActive(true);
            loadingPanel.SetActive(false);
        }
        else
        {


            string full = www.downloadHandler.text;


            Debug.Log("full========================>" + full);


            if (full == "[]")
            {

                battileMsg.SetActive(true);
                loadingPanel.SetActive(false);
            }
            else
            {
                addPVPDatas(full);
            }

            //----------------------------------------------------------------------------------------------------


            imageCount = 0;
            imageCountCheck = (BattileUserData1.Count * 2) + (BattileUserData.Count * 2);
            notification_value = 0;


            ListBattileItems();
            ListBattileItemsforwaitingChallenger();

            if (BattileUserData1.Count > 0)
            {
                inprogress_btn();
            }
            else
            {
                completed_btn();

            }

        }


    }

    void addPVPDatas(string full)
    {
        JSONObject obj_1 = new JSONObject(full);
        string adon = @"{""BattileClass"":";
        string HardCode1 = full.Insert(0, adon);
        string HardCode2 = String.Concat(HardCode1, "}");
        JSONObject obj = new JSONObject(HardCode2);
        BattileUserData.Clear();


        for (int i = 0; i < obj.GetField("BattileClass").Count; i++)
        {
            JSONObject obj1 = obj.GetField("BattileClass")[i];

            BattileData user = new BattileData();
            if (!string.IsNullOrEmpty(obj1.GetField("created").str))
            {
                String DateString = obj1.GetField("created").str.ToString();
                DateTime dateTime12 = Convert.ToDateTime(DateString);
                user._date = dateTime12.ToString(("dd-MM-yyyy hh:mm tt"));

            }
            //----

            Debug.Log("V----->" + (obj1.GetField("challenger").Count));
            if ((obj1.GetField("challenger").Count) > 1)
            {
                if (!string.IsNullOrEmpty(obj1.GetField("challenger").GetField("display_name").str))
                {
                    user.username1 = obj1.GetField("challenger").GetField("display_name").str.ToString();
                }
                else
                {
                    user.username1 = "waiting for challenger";
                }
                if (!string.IsNullOrEmpty(obj1.GetField("challenger").GetField("score").ToString()))
                {
                    user.score1 = obj1.GetField("challenger").GetField("score").ToString();
                }

                if (!string.IsNullOrEmpty(obj1.GetField("challenger").GetField("user_id").ToString()))
                {
                    user.user_id1 = obj1.GetField("challenger").GetField("user_id").ToString();
                }

                if (!string.IsNullOrEmpty(obj1.GetField("challenger").GetField("avatar_url").str))
                {
                    user.avatar_url_1 = obj1.GetField("challenger").GetField("avatar_url").str.ToString();
                }
            }
            else
            {
                user.username1 = "waiting  for  challenger";
            }

            //------
            if (!string.IsNullOrEmpty(obj1.GetField("defender").GetField("display_name").str))
                user.username2 = obj1.GetField("defender").GetField("display_name").str.ToString();

            if (!string.IsNullOrEmpty(obj1.GetField("defender").GetField("user_id").ToString()))
            {
                user.user_id2 = obj1.GetField("defender").GetField("user_id").ToString();
            }

            if (!string.IsNullOrEmpty(obj1.GetField("defender").GetField("score").ToString()))
            {
                user.score2 = obj1.GetField("defender").GetField("score").ToString();
            }
            if (!string.IsNullOrEmpty(obj1.GetField("defender").GetField("avatar_url").str))
            {
                user.avatar_url_2 = obj1.GetField("defender").GetField("avatar_url").str.ToString();
            }


            //   Debug.Log("user.username1-------->" + user.username1);




            if (user.username1 == "waiting  for  challenger")
            {
                BattileUserData1.Add(user);
            }
            else
            {
                BattileUserData.Add(user);
            }
        }


    }


     void ListBattileItems()
    {
        battilCheck.SetActive(true);
        loadingPanel.SetActive(false);
        for (int i = 0; i < 50; i++)
        {
            battileObjects[i].transform.GetChild(0).transform.gameObject.SetActive(false);
            battileObjects[i].transform.GetChild(1).transform.gameObject.SetActive(false);
            battileObjects[i].transform.GetChild(2).transform.gameObject.SetActive(false);
            battileObjects[i].transform.GetChild(3).transform.gameObject.SetActive(false);
            battileObjects[i].transform.GetChild(4).transform.gameObject.SetActive(false);
            battileObjects[i].transform.GetChild(5).transform.gameObject.SetActive(false);
            battileObjects[i].transform.GetChild(6).transform.gameObject.SetActive(false);
            battileObjects[i].transform.GetChild(7).transform.gameObject.SetActive(false);
            battileObjects[i].transform.GetChild(8).transform.gameObject.SetActive(false);
            battileObjects[i].transform.GetChild(9).GetComponent<Text>().text = "";
            battileObjects[i].transform.GetChild(10).GetComponent<Text>().text = "";
            //  battileObjects[i].SetActive(false);

        }



        for (int i = 0; i < BattileUserData.Count; i++)
        {
            battileObjects[i].transform.GetChild(0).transform.gameObject.SetActive(true);
            battileObjects[i].transform.GetChild(1).transform.gameObject.SetActive(true);
            battileObjects[i].transform.GetChild(2).transform.gameObject.SetActive(true);
            battileObjects[i].transform.GetChild(3).transform.gameObject.SetActive(true);
            battileObjects[i].transform.GetChild(4).transform.gameObject.SetActive(true);
            battileObjects[i].transform.GetChild(5).transform.gameObject.SetActive(true);
            battileObjects[i].transform.GetChild(6).transform.gameObject.SetActive(true);
            battileObjects[i].transform.GetChild(7).transform.gameObject.SetActive(true);
            battileObjects[i].transform.GetChild(8).transform.gameObject.SetActive(true);         
            battileObjects[i].transform.GetChild(9).GetComponent<Text>().text = "";
            battileObjects[i].transform.GetChild(10).GetComponent<Text>().text = "";

            // battileObjects[i].SetActive(false);


            Text Date_txt = battileObjects[i].transform.GetChild(3).GetChild(0).GetComponent<Text>();
            Text Username1_txt = battileObjects[i].transform.GetChild(3).GetChild(1).GetComponent<Text>();
            Text Score1_txt = battileObjects[i].transform.GetChild(3).GetChild(2).GetComponent<Text>();
            Text Username2_txt = battileObjects[i].transform.GetChild(3).GetChild(3).GetComponent<Text>();
            Text Score2_txt = battileObjects[i].transform.GetChild(3).GetChild(4).GetComponent<Text>();

            // Text OnCheck_txt = battileObjects[i].transform.GetChild(3).GetChild(5).GetComponent<Text>();
            GameObject OnCheck_txt_progress = battileObjects[i].transform.GetChild(3).GetChild(5).transform.gameObject;
            GameObject OnCheck_txt_defeat = battileObjects[i].transform.GetChild(3).GetChild(6).transform.gameObject;
            GameObject OnCheck_txt_victory = battileObjects[i].transform.GetChild(3).GetChild(7).transform.gameObject;
            GameObject OnCheck_txt_waiting = battileObjects[i].transform.GetChild(3).GetChild(8).transform.gameObject;
            GameObject WaitingChallenger = battileObjects[i].transform.GetChild(3).GetChild(9).transform.gameObject;


            
            Text Entry_txt = battileObjects[i].transform.GetChild(9).GetComponent<Text>();
            Text Rp_txt = battileObjects[i].transform.GetChild(10).GetComponent<Text>();


            OnCheck_txt_progress.SetActive(false);
            OnCheck_txt_defeat.SetActive(false);
            OnCheck_txt_victory.SetActive(false);
            OnCheck_txt_waiting.SetActive(false);
            WaitingChallenger.SetActive(false);




            // int changei = (((BattileUserData.Count) - i)-1);
            int changei = i;


            Date_txt.text = "" + BattileUserData[changei]._date;
            Username1_txt.text = BattileUserData[changei].username1;
            Score1_txt.text = "" + BattileUserData[changei].score1;
            if (BattileUserData[changei].score1 == "null")
                Score1_txt.text = "--";
            Username2_txt.text = BattileUserData[changei].username2;
            Score2_txt.text = "" + BattileUserData[changei].score2;

            Debug.Log("I-------->" + BattileUserData[changei].score1);
            Debug.Log("J-------->" + BattileUserData[changei].score2);

             Entry_txt.text = "" + EntryFeeController.currentRewardPrice;
             Rp_txt.text = "" + EntryFeeController.Rpvalue_;


            if (BattileUserData[changei].score1 == "null")
            {
                //   OnCheck_txt.text = "WAITING";
                Image profile3 = battileObjects[i].transform.GetChild(1).GetComponent<Image>();
                profile3.sprite = Pink_clr;
                OnCheck_txt_waiting.SetActive(true);

            }
            else
            {
                float s1 = float.Parse(BattileUserData[changei].score1);
                float s2 = float.Parse(BattileUserData[changei].score2);
                float our_value;
                float their_value;

                if (BattileUserData[changei].user_id1 == profileID)
                {
                    our_value = s1;
                    their_value = s2;

                }
                else
                {

                    our_value = s2;
                    their_value = s1;

                }


                if (our_value < their_value)
                {
                    Image profile3 = battileObjects[i].transform.GetChild(1).GetComponent<Image>();
                    profile3.sprite = red_clr;
                    //    OnCheck_txt.text = "DEFEAT";
                    OnCheck_txt_defeat.SetActive(true);

                }
                else
                {
                    //   OnCheck_txt.text = "VICTORY";
                    Image profile3 = battileObjects[i].transform.GetChild(1).GetComponent<Image>();
                    profile3.sprite = green_clr;
                    OnCheck_txt_victory.SetActive(true);

                }

            }


            //battileObjects[i].transform.GetChild(1).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).GetChild(0);


            Image profile1 = battileObjects[changei].transform.GetChild(4).GetChild(0).GetComponent<Image>();
            Image profile2 = battileObjects[changei].transform.GetChild(5).GetChild(0).GetComponent<Image>();

            GameObject P1 = battileObjects[changei].transform.GetChild(6).transform.gameObject;
            GameObject P2 = battileObjects[changei].transform.GetChild(7).transform.gameObject;


            if (!string.IsNullOrEmpty(BattileUserData[changei].avatar_url_1))
            {

                if (profileimage_URL == BattileUserData[changei].avatar_url_1)
                {
                    P1.SetActive(false);
                    profile1.sprite = profileimage.sprite;
                }
                else
                {
                    StartCoroutine(PicProfilePic(BattileUserData[changei].avatar_url_1, profile1, P1));

                }
            }
            else
            {

                P1.SetActive(false);
                profile1.sprite = default_image.sprite;

            }



            if (!string.IsNullOrEmpty(BattileUserData[changei].avatar_url_2))
            {


                if (profileimage_URL == BattileUserData[changei].avatar_url_2)
                {
                    P2.SetActive(false);
                    profile2.sprite = profileimage.sprite;
                }
                else
                {
                    StartCoroutine(PicProfilePic(BattileUserData[changei].avatar_url_2, profile2, P2));

                }
            }
            else
            {

                P2.SetActive(false);
                profile2.sprite = default_image.sprite;


            }


        }
    }




    void ListBattileItemsforwaitingChallenger()
    {
        battilCheck.SetActive(true);
        loadingPanel.SetActive(false);

        for (int i = 0; i < 50; i++)
        {
            battileObjects1[i].transform.GetChild(0).transform.gameObject.SetActive(false);
            battileObjects1[i].transform.GetChild(1).transform.gameObject.SetActive(false);
            battileObjects1[i].transform.GetChild(2).transform.gameObject.SetActive(false);
            battileObjects1[i].transform.GetChild(3).transform.gameObject.SetActive(false);
            battileObjects1[i].transform.GetChild(4).transform.gameObject.SetActive(false);
            battileObjects1[i].transform.GetChild(5).transform.gameObject.SetActive(false);
            battileObjects1[i].transform.GetChild(6).transform.gameObject.SetActive(false);
            battileObjects1[i].transform.GetChild(7).transform.gameObject.SetActive(false);
            battileObjects1[i].transform.GetChild(8).transform.gameObject.SetActive(false);
            battileObjects1[i].transform.GetChild(9).GetComponent<Text>().text = "";
            battileObjects1[i].transform.GetChild(10).GetComponent<Text>().text = "";
            //  battileObjects1[i].SetActive(false);

        }



        for (int i = 0; i < BattileUserData1.Count; i++)
        {
            battileObjects1[i].transform.GetChild(0).transform.gameObject.SetActive(true);
            battileObjects1[i].transform.GetChild(1).transform.gameObject.SetActive(true);
            battileObjects1[i].transform.GetChild(2).transform.gameObject.SetActive(true);
            battileObjects1[i].transform.GetChild(3).transform.gameObject.SetActive(true);
            battileObjects1[i].transform.GetChild(4).transform.gameObject.SetActive(true);
            battileObjects1[i].transform.GetChild(5).transform.gameObject.SetActive(true);
            battileObjects1[i].transform.GetChild(6).transform.gameObject.SetActive(true);
            battileObjects1[i].transform.GetChild(7).transform.gameObject.SetActive(true);
            battileObjects1[i].transform.GetChild(8).transform.gameObject.SetActive(true);
            battileObjects1[i].transform.GetChild(9).GetComponent<Text>().text = "";
            battileObjects1[i].transform.GetChild(10).GetComponent<Text>().text = "";
            // battileObjects1[i].SetActive(true);


            Text Date_txt = battileObjects1[i].transform.GetChild(3).GetChild(0).GetComponent<Text>();
            Text Username1_txt = battileObjects1[i].transform.GetChild(3).GetChild(1).GetComponent<Text>();
            Text Score1_txt = battileObjects1[i].transform.GetChild(3).GetChild(2).GetComponent<Text>();
            Text Username2_txt = battileObjects1[i].transform.GetChild(3).GetChild(3).GetComponent<Text>();
            Text Score2_txt = battileObjects1[i].transform.GetChild(3).GetChild(4).GetComponent<Text>();




            GameObject OnCheck_txt_progress = battileObjects1[i].transform.GetChild(3).GetChild(5).transform.gameObject;
            GameObject OnCheck_txt_defeat = battileObjects1[i].transform.GetChild(3).GetChild(6).transform.gameObject;
            GameObject OnCheck_txt_victory = battileObjects1[i].transform.GetChild(3).GetChild(7).transform.gameObject;
            GameObject OnCheck_txt_waiting = battileObjects1[i].transform.GetChild(3).GetChild(8).transform.gameObject;
            GameObject WaitingChallenger = battileObjects1[i].transform.GetChild(3).GetChild(9).transform.gameObject;
            Text Entry_txt = battileObjects1[i].transform.GetChild(9).GetComponent<Text>();
            Text Rp_txt = battileObjects1[i].transform.GetChild(10).GetComponent<Text>();

            OnCheck_txt_progress.SetActive(false);
            OnCheck_txt_defeat.SetActive(false);
            OnCheck_txt_victory.SetActive(false);
            OnCheck_txt_waiting.SetActive(false);
            WaitingChallenger.SetActive(true);
            Entry_txt.text = "" + EntryFeeController.currentRewardPrice;
            Rp_txt.text = "" + EntryFeeController.Rpvalue_;


            Debug.Log("F------------->" + OnCheck_txt_progress);



            Image profile3 = battileObjects1[i].transform.GetChild(1).GetComponent<Image>();
            profile3.sprite = blue_clr;


            // OnCheck_txt.text = "IN  PROGRESS";
            OnCheck_txt_progress.SetActive(true);


            // int changei = (((BattileUserData.Count) - i)-1);
            int changei = i;
            Debug.Log("I-------->" + changei);

            Date_txt.text = "" + BattileUserData1[changei]._date;
            //   Username1_txt.text = BattileUserData1[changei].username1;
            Username1_txt.text = "";
            Score1_txt.text = "" + BattileUserData1[changei].score1;
            if (BattileUserData1[changei].score1 == "null")
                Score1_txt.text = "--";
            Username2_txt.text = BattileUserData1[changei].username2;
            Score2_txt.text = "" + BattileUserData1[changei].score2;
            if (BattileUserData1[changei].score2 == "null")
                Score2_txt.text = "--";

            //battileObjects[i].transform.GetChild(1).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).GetChild(0);


            Image profile1 = battileObjects1[changei].transform.GetChild(4).GetChild(0).GetComponent<Image>();
            Image profile2 = battileObjects1[changei].transform.GetChild(5).GetChild(0).GetComponent<Image>();


            GameObject P1 = battileObjects1[changei].transform.GetChild(6).transform.gameObject;
            GameObject P2 = battileObjects1[changei].transform.GetChild(7).transform.gameObject;

            Score1_txt.text = "--";

            P1.SetActive(false);
            P2.SetActive(false);
            profile2.sprite = profileimage.sprite;
            // StartCoroutine(PicProfilePic(BattileUserData1[changei].avatar_url_1, profile1));
            //StartCoroutine(PicProfilePic(BattileUserData1[changei].avatar_url_2, profile2, P2));





        }
    }










    IEnumerator PicProfilePic(string url, Image pic, GameObject P)
    {



        WWW www = new WWW(url);
        yield return www;
        imageCount++;
        if (www.texture != null)
        {

            Texture2D texture = www.texture;
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            if (pic != null)
            {
                pic.sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
            }
            if (P != null)
            {
                P.SetActive(false);
            }



        }
        else
        {
            Debug.Log("not reach");
        }


    }



    public void completed_btn()
    {

        itemsForproress.SetActive(false);
        itemsForcompleted.SetActive(true);

        Debug.Log("Enable Blue");

        CompleteBttn.interactable = false;
        ColorBlock cb = CompleteBttn.colors;
        cb.normalColor = AshColour;
        cb.highlightedColor = AshColour;
        //  cb.pressedColor = AshColour;
        CompleteBttn.colors = cb;

        inprogressBtn.interactable = true;
        ColorBlock cb1 = inprogressBtn.colors;
        cb1.normalColor = blueColour;
        cb1.highlightedColor = blueColour;
        cb1.pressedColor = blueColour;
        inprogressBtn.colors = cb1;




    }

    public void inprogress_btn()
    {




        Debug.Log("Enable 1  Blue2222");

        itemsForproress.SetActive(true);
        itemsForcompleted.SetActive(false);

        inprogressBtn.interactable = false;
        ColorBlock cb = inprogressBtn.colors;
        cb.normalColor = AshColour;
        cb.highlightedColor = AshColour;
        // cb.pressedColor = AshColour;
        inprogressBtn.colors = cb;

        CompleteBttn.interactable = true;
        ColorBlock cb1 = CompleteBttn.colors;
        cb1.normalColor = blueColour;
        cb1.highlightedColor = blueColour;
        cb1.pressedColor = blueColour;
        CompleteBttn.colors = cb1;



    }
















    public void NoSGCOk()
    {

        noSGC.SetActive(false);

    }


    public void closeBattileLog()
    {

        battilCheck.SetActive(false);

    }


    public void hidebattilemessage()
    {
        battileMsg.SetActive(false);

    }

}




