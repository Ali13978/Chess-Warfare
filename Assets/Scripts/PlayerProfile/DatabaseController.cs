using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Unity.Editor;
using Firebase;

public class DatabaseController : MonoBehaviour
{
    public static DatabaseController Instance;
    private int AsyncResult = 3;
    private string Json = null;

    private DatabaseReference databaseReference;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        
        // Set this before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://chess-warfare.firebaseio.com/");

        // Get the root reference location of the database.
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public int GetAsyncResult()
    {
        return AsyncResult;
    }

    public void ResetAsyncResult()
    {
        AsyncResult = 0;
        Json = null;
    }

    public string GetJson()
    {
        return Json;
    }

    public void MakeNewProfileTask(PlayerProfile playerProfile)
    {
        AsyncResult = 0;
        string json = JsonUtility.ToJson(playerProfile); //us = users
        databaseReference.Child("us").Child(playerProfile.UID).SetRawJsonValueAsync(json).ContinueWith(task => {
            if (task.IsCanceled)
            {
                AsyncResult = 1;
                return;
            }
            else if (task.IsFaulted)
            {
                // Handle the error...
                Debug.Log("Exception : " + task.Exception);
                AsyncResult = 2;
                return;
            }
            else if (task.IsCompleted)
            {
                AsyncResult = 3;
                return;
            }
        });

        if(!playerProfile.pD.em.Equals(""))
        {
            databaseReference.Child("MyEmail").Child(playerProfile.UID).Child("em").SetValueAsync(playerProfile.pD.em).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    AsyncResult = 1;
                    return;
                }
                else if (task.IsFaulted)
                {
                    // Handle the error...
                    Debug.Log("Exception : " + task.Exception);
                    AsyncResult = 2;
                    return;
                }
                else if (task.IsCompleted)
                {
                    AsyncResult = 3;
                    return;
                }
            });
            databaseReference.Child("MyEmail").Child(playerProfile.UID).Child("uid").SetValueAsync(playerProfile.UID).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    AsyncResult = 1;
                    return;
                }
                else if (task.IsFaulted)
                {
                    // Handle the error...
                    Debug.Log("Exception : " + task.Exception);
                    AsyncResult = 2;
                    return;
                }
                else if (task.IsCompleted)
                {
                    AsyncResult = 3;
                    return;
                }
            });
        }
    }

    public void DownloadProfileTask(string UserId)
    {
        Debug.Log("Downloading");
        databaseReference.Child("us").Child(UserId).GetValueAsync().ContinueWith(task =>
        {
            if(task.IsCanceled)
            {
                AsyncResult = 1;
                return;
            }
            if (task.IsFaulted)
            {
                AsyncResult = 2;
                return;
            }
            if (task.IsCompleted)
            {
                DataSnapshot dataSnapshot = task.Result;
                Json = dataSnapshot.GetRawJsonValue();
                AsyncResult = 3;
                Debug.Log("asyncresult" + dataSnapshot.GetRawJsonValue());
                return;
            }
        });
    }

    public void UpdateCoins(string userId,int coins)
    {
        databaseReference.Child("us").Child(userId).Child("pD").Child("Gld").SetValueAsync(coins).ContinueWith(task => {
            if (task.IsCanceled)
            {
                AsyncResult = 1;
                return;
            }
            else if (task.IsFaulted)
            {
                // Handle the error...
                Debug.Log("Exception : " + task.Exception);
                AsyncResult = 2;
                return;
            }
            else if (task.IsCompleted)
            {
                AsyncResult = 3;
                return;
            }
        });
    }

    public void UpdateLevel(string userId, int level)
    {
        databaseReference.Child("us").Child(userId).Child("Lvl").SetValueAsync(level).ContinueWith(task => {
            if (task.IsCanceled)
            {
                AsyncResult = 1;
                return;
            }
            else if (task.IsFaulted)
            {
                // Handle the error...
                Debug.Log("Exception : " + task.Exception);
                AsyncResult = 2;
                return;
            }
            else if (task.IsCompleted)
            {
                AsyncResult = 3;
                return;
            }
        });
    }

    public void UpdateXP(string userId, int xp)
    {
        databaseReference.Child("us").Child(userId).Child("pD").Child("XP").SetValueAsync(xp).ContinueWith(task => {
            if (task.IsCanceled)
            {
                AsyncResult = 1;
                return;
            }
            else if (task.IsFaulted)
            {
                // Handle the error...
                Debug.Log("Exception : " + task.Exception);
                AsyncResult = 2;
                return;
            }
            else if (task.IsCompleted)
            {
                AsyncResult = 3;
                return;
            }
        });
    }

    public void AddNewMiniGame(string userid,MiniGame miniGame)
    {
        AsyncResult = 0;
        string json = JsonUtility.ToJson(miniGame); //us = users
        databaseReference.Child("MiniGames").Child(userid).SetRawJsonValueAsync(json).ContinueWith(task => {
            if (task.IsCanceled)
            {
                AsyncResult = 1;
                return;
            }
            else if (task.IsFaulted)
            {
                // Handle the error...
                Debug.Log("Exception : " + task.Exception);
                AsyncResult = 2;
                return;
            }
            else if (task.IsCompleted)
            {
                AsyncResult = 3;
                return;
            }
        });
    }


    public void UpdateMiniGame(string userId,string key,string value)
    {
        databaseReference.Child("MiniGames").Child(userId).Child(key).SetValueAsync(value).ContinueWith(task => {
            if (task.IsCanceled)
            {
                AsyncResult = 1;
                return;
            }
            else if (task.IsFaulted)
            {
                // Handle the error...
                Debug.Log("Exception : " + task.Exception);
                AsyncResult = 2;
                return;
            }
            else if (task.IsCompleted)
            {
                AsyncResult = 3;
                return;
            }
        });
    }

    public void DownloadMiniGames(string UserId)
    {
        Debug.Log("Downloading");
        databaseReference.Child("MiniGames").Child(UserId).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                AsyncResult = 1;
                return;
            }
            if (task.IsFaulted)
            {
                AsyncResult = 2;
                return;
            }
            if (task.IsCompleted)
            {
                DataSnapshot dataSnapshot = task.Result;
                Json = dataSnapshot.GetRawJsonValue();
                AsyncResult = 3;
                Debug.Log("asyncresult" + dataSnapshot.GetRawJsonValue());
                return;
            }
        });
    }

    public void AddMyBoards(string userid, MyBoards myBoards)
    {
        AsyncResult = 0;
        string json = JsonUtility.ToJson(myBoards); //us = users
        databaseReference.Child("MyBoards").Child(userid).SetRawJsonValueAsync(json).ContinueWith(task => {
            if (task.IsCanceled)
            {
                AsyncResult = 1;
                return;
            }
            else if (task.IsFaulted)
            {
                // Handle the error...
                Debug.Log("Exception : " + task.Exception);
                AsyncResult = 2;
                return;
            }
            else if (task.IsCompleted)
            {
                AsyncResult = 3;
                return;
            }
        });
    }


    public void UpdateMyBoards(string userId, string key, string value)
    {
        AsyncResult = 0;
        databaseReference.Child("MyBoards").Child(userId).Child(key).SetValueAsync(value).ContinueWith(task => {
            if (task.IsCanceled)
            {
                AsyncResult = 1;
                return;
            }
            else if (task.IsFaulted)
            {
                // Handle the error...
                Debug.Log("Exception : " + task.Exception);
                AsyncResult = 2;
                return;
            }
            else if (task.IsCompleted)
            {
                AsyncResult = 3;
                return;
            }
        });
    }

    public void DownloadMyBoards(string UserId)
    {
        AsyncResult = 0;
        Debug.Log("Downloading");
        databaseReference.Child("MyBoards").Child(UserId).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                AsyncResult = 1;
                return;
            }
            if (task.IsFaulted)
            {
                AsyncResult = 2;
                return;
            }
            if (task.IsCompleted)
            {
                DataSnapshot dataSnapshot = task.Result;
                Json = dataSnapshot.GetRawJsonValue();
                AsyncResult = 3;
                Debug.Log("asyncresult" + dataSnapshot.GetRawJsonValue());
                return;
            }
        });
    }

    public void AddMyPacks(string userid, MyPacks mypacks)
    {
        AsyncResult = 0;
        string json = JsonUtility.ToJson(mypacks); //my packs
        databaseReference.Child("MyChatPacks").Child(userid).SetRawJsonValueAsync(json).ContinueWith(task => {
            if (task.IsCanceled)
            {
                AsyncResult = 1;
                return;
            }
            else if (task.IsFaulted)
            {
                // Handle the error...
                Debug.Log("Exception : " + task.Exception);
                AsyncResult = 2;
                return;
            }
            else if (task.IsCompleted)
            {
                AsyncResult = 3;
                return;
            }
        });
    }


    public void UpdateMyPacks(string userId, string key, string value)
    {
        AsyncResult = 0;
        databaseReference.Child("MyChatPacks").Child(userId).Child(key).SetValueAsync(value).ContinueWith(task => {
            if (task.IsCanceled)
            {
                AsyncResult = 1;
                return;
            }
            else if (task.IsFaulted)
            {
                // Handle the error...
                Debug.Log("Exception : " + task.Exception);
                AsyncResult = 2;
                return;
            }
            else if (task.IsCompleted)
            {
                AsyncResult = 3;
                return;
            }
        });
    }

    public void DownloadMyPacks(string UserId)
    {
        AsyncResult = 0;
        Debug.Log("Downloading");
        databaseReference.Child("MyChatPacks").Child(UserId).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                AsyncResult = 1;
                return;
            }
            if (task.IsFaulted)
            {
                AsyncResult = 2;
                return;
            }
            if (task.IsCompleted)
            {
                DataSnapshot dataSnapshot = task.Result;
                Json = dataSnapshot.GetRawJsonValue();
                AsyncResult = 3;
                Debug.Log("asyncresult" + dataSnapshot.GetRawJsonValue());
                return;
            }
        });
    }

    public void AddMyAvatars(string userid, MyAvatars myAvatars)
    {
        AsyncResult = 0;
        string json = JsonUtility.ToJson(myAvatars); //my packs
        databaseReference.Child("MyAvatars").Child(userid).SetRawJsonValueAsync(json).ContinueWith(task => {
            if (task.IsCanceled)
            {
                AsyncResult = 1;
                return;
            }
            else if (task.IsFaulted)
            {
                // Handle the error...
                Debug.Log("Exception : " + task.Exception);
                AsyncResult = 2;
                return;
            }
            else if (task.IsCompleted)
            {
                AsyncResult = 3;
                return;
            }
        });
    }


    public void UpdateMyAvatars(string userId, string key, string value)
    {
        AsyncResult = 0;
        databaseReference.Child("MyAvatars").Child(userId).Child(key).SetValueAsync(value).ContinueWith(task => {
            if (task.IsCanceled)
            {
                AsyncResult = 1;
                return;
            }
            else if (task.IsFaulted)
            {
                // Handle the error...
                Debug.Log("Exception : " + task.Exception);
                AsyncResult = 2;
                return;
            }
            else if (task.IsCompleted)
            {
                AsyncResult = 3;
                return;
            }
        });
    }

    public void DownloadMyAvatars(string UserId)
    {
        AsyncResult = 0;
        Debug.Log("Downloading");
        databaseReference.Child("MyAvatars").Child(UserId).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                AsyncResult = 1;
                return;
            }
            if (task.IsFaulted)
            {
                AsyncResult = 2;
                return;
            }
            if (task.IsCompleted)
            {
                DataSnapshot dataSnapshot = task.Result;
                Json = dataSnapshot.GetRawJsonValue();
                AsyncResult = 3;
                Debug.Log("asyncresult" + dataSnapshot.GetRawJsonValue());
                return;
            }
        });
    }

    public void UpdateProfileAvatar(string userId,string AvatarId)
    {
        databaseReference.Child("us").Child(userId).Child("av").SetValueAsync(AvatarId).ContinueWith(task => {
            if (task.IsCanceled)
            {
                AsyncResult = 1;
                return;
            }
            else if (task.IsFaulted)
            {
                // Handle the error...
                Debug.Log("Exception : " + task.Exception);
                AsyncResult = 2;
                return;
            }
            else if (task.IsCompleted)
            {
                AsyncResult = 3;
                return;
            }
        });
    }


    public void UpdateMatch(string userId, int value,string key,string country)
    {
        databaseReference.Child("us").Child(userId).Child(key).SetValueAsync(value).ContinueWith(task => {
            if (task.IsCanceled)
            {
                AsyncResult = 1;
                return;
            }
            else if (task.IsFaulted)
            {
                // Handle the error...
                Debug.Log("Exception : " + task.Exception);
                AsyncResult = 2;
                return;
            }
            else if (task.IsCompleted)
            {
                AsyncResult = 3;
                return;
            }
        });

        databaseReference.Child("country").Child(country).Child(userId).Child(key).SetValueAsync(value).ContinueWith(task => {
            if (task.IsCanceled)
            {
                AsyncResult = 1;
                return;
            }
            else if (task.IsFaulted)
            {
                // Handle the error...
                Debug.Log("Exception : " + task.Exception);
                AsyncResult = 2;
                return;
            }
            else if (task.IsCompleted)
            {
                AsyncResult = 3;
                return;
            }
        });

        databaseReference.Child("country").Child(country).Child(userId).Child("uid").SetValueAsync(userId).ContinueWith(task => {
            if (task.IsCanceled)
            {
                AsyncResult = 1;
                return;
            }
            else if (task.IsFaulted)
            {
                // Handle the error...
                Debug.Log("Exception : " + task.Exception);
                AsyncResult = 2;
                return;
            }
            else if (task.IsCompleted)
            {
                AsyncResult = 3;
                return;
            }
        });
    }
}
