using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
public class DeepLinksManager : MonoBehaviour
{
    public static DeepLinksManager Instance { get; private set; }
    public string deeplinkURL;

    private string redirectUri = "chesswarfare://com.chesswarfare.auth";
    private string clientId = "chess-warfare-android";
    private string code;

    private char[] trim_char_arry = new char[] { '"' };
    public Action LoginSucc;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Application.deepLinkActivated += onDeepLinkActivated;
            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                onDeepLinkActivated(Application.absoluteURL);
            }
            else deeplinkURL = "[none]";
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SceneManager.LoadSceneAsync("MainMenu");
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadSceneAsync("SplashScreen");
        }
    }

    private void onDeepLinkActivated(string url)
    {
        // Update DeepLink Manager global variable, so URL can be accessed from anywhere.
        deeplinkURL = url;

        string paremeters = url.Split('?')[1];

        Debug.Log("Deeplink Paremeters: " + paremeters);

        if (url.Contains(redirectUri + "?code="))
        {
            code = GetCode(url);
            Debug.Log("url: " + url);
            Debug.Log("code: " + code);
            ServerCall();
            LoginSucc?.Invoke();
        }

        if (url.Contains("access_denied"))
        {
            Debug.Log("Remote Request failed");
        }
    }

    private string GetCode(string url)
    {
        string[] parts = url.Split('?');

        if (parts.Length > 1)
        {
            string query = parts[1]; // Extract the query string part

            string[] queryParams = query.Split('&');

            foreach (string param in queryParams)
            {
                string[] keyValue = param.Split('=');

                if (keyValue.Length == 2 && keyValue[0] == "code")
                {
                    string codeValue = keyValue[1];
                    return codeValue;
                }
            }
        }

        return "";
    }
    private async void ServerCall()
    {
        Debug.Log("Servercall Started");
        try
        {
            string url = "https://wgcapi.mmocircles.com/auth/token";

            // Your form data
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code.ToString()),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("redirect_uri", redirectUri)
            });

            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            using (HttpClient client = new HttpClient(httpClientHandler))
            {
                HttpResponseMessage response = await client.PostAsync(url, formContent);

                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    // Process the response here
                    Debug.Log("Response: " + responseString);

                    // Handle the JSON response, extract tokens, etc.
                    // Example parsing JSON:
                    JSONObject obj = new JSONObject(responseString);
                    string refreshToken = obj.GetField("refresh_token").str;
                    string accessToken = obj.GetField("access_token").str;

                    Debug.Log("refreshToken: " + refreshToken);
                    Debug.Log("accessToken: " + accessToken);
                }
                else
                {
                    Debug.Log("Server Error: " + response.StatusCode);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Exception: " + e.Message);
        }
    }

    public void LoginFun()
    {
        string url = "";
        url = "https://wgcapi.mmocircles.com/auth/authorize?client_id=" + clientId + "&response_type=code&redirect_uri=" + redirectUri;
        Application.OpenURL(url);
        PlayerPrefs.SetString("Defender__display_name", "");
        PlayerPrefs.SetString("Defender__URL", "");

    }
}
