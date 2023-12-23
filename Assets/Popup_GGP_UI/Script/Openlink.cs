using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Openlink : MonoBehaviour {
    public static Openlink Inst;
    //public string url = "https://wgcapi.mmocircles.com/products?app_id=32&range=100";
    string jdata;
    public string RedirectURL;
    public string CliantID;
    private void Awake()
    {
        Inst = this;
    }
   public JSONObject obj;
    public JSONObject obj1;
    public string jurl;
    // Use this for initialization
    void Start ()
    {

      
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void gamejsondata()
    {
        StartCoroutine(jsondata());
    }
    IEnumerator jsondata()
    {
        jurl = "https://wgcapi.mmocircles.com/products?app_id=65&range=100";
        Debug.Log("acces token form json data" + PlayerPrefs.GetString("a_token").ToString());
         Debug.Log("jsondata" + jurl);
        UnityWebRequest jsa = UnityWebRequest.Get(jurl);
        jsa.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("a_token").ToString());

        yield return jsa.SendWebRequest();
        Debug.Log("jsondata1");
        if (jsa.error != null)
        {
            Debug.Log("jsondataerror" + jsa.error.ToString());

        }
        else
        {
            Debug.Log("jsondata====" + jsa.downloadHandler.text);
            jdata = jsa.downloadHandler.text;
           
        }
        jdata = jsa.downloadHandler.text;
      
         obj = new JSONObject(jdata);
  
         obj1 = obj.GetField("products");
       



        //  for (int j = 0; j < obj1.Count; j++)
        // {
        // Debug.Log("Json object " + j + " = " + obj1[j].ToString());
        //for (int i = 0; i < obj1[j].Count; i++)
        //{
        //Debug.Log("Field of filed " + j + " = " + obj1[i].ToString());
        // Debug.Log("id=" + obj1[j].GetField("price"));
        //}
        //}
/////////////        Menu.Inst.powertext();


    }

    public void onclickButton()
    {
        /////////////C_card.Ints.mainview.SetActive(true);
        /////////////C_card.Ints.login_popUp.SetActive(false);
        Debug.Log("on click button ");
        if (DeepClass.IsInternetConnection())
        {
            Application.OpenURL("https://wgcapi.mmocircles.com/auth/authorize?client_id=65&response_type=code&redirect_uri=epicroll://com.goodgameplatform.epicroll/mmoc/auth");
        }
        else
        {
            C_card.Inst.PopupMessage.SetActive(true);
            C_card.Inst.messagePopup_Message.text = "No internet connection available!...";
            C_card.Inst.LoginButtonPopupMessage.SetActive(false);
            C_card.Inst.OkButtonPopupMessage.SetActive(true);
            C_card.Inst.CloseButtonPopupMessage.SetActive(false);
        }
    }








    public void OpenStore()
    {

        Application.OpenURL("https://www.worldgamecommunity.com");
    }


}
