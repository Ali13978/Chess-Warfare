using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class callstrip : MonoBehaviour {
    public static callstrip Inst;
    public JSONObject pay;

    public GameObject ggppopupsuccess;
    public Text ggpsuccesstext;
    public GameObject buypopup;

   
    // Use this for initialization
    void Start () {
        Inst = this;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void payment(string card, int month, int your, string cvv)
    {
        //string key = "pk_live_KryR7hxCnmxzkzISu6rF6GBd00S4S3VTNo";

        string key = "pk_live_rbHS15hsQFzc16x0KTAIz3GJ";

        Debug.Log("call payment");
        Debug.Log("key = " + key);
        AndroidJavaClass ajc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

        var obj = ajc.GetStatic<AndroidJavaObject>("currentActivity");
        Debug.Log("call payment11111");
        //StripePaymentHelper(String scriptName, String methodName, String errorMethod, String publishableKey)
        AndroidJavaObject ajo = new AndroidJavaObject("com.artoon.stripemodule.StripePaymentHelper", "callstrip", "OnPaymentSuccess", "OnPaymentError", key);
        Debug.Log("call payment2222");
        ajo.Call("payment", obj, card, month, your, cvv);
        Debug.Log("call payment sucees call stripe......");
    }
    public void OnPaymentSuccess(string arg)
    {
        Debug.Log("payment success=" + arg);
        string full = arg;
        pay = new JSONObject(full);
        Debug.Log("token payment=>" + pay[0].ToString());
        Debug.Log("using id" + pay["id"].ToString());
       

        StartCoroutine(Stripe());


    }
    public void OnPaymentError(string error)
    {
        Debug.Log("payment error=" + error);
        ggpsuccesstext.text = error.ToString();
        ggppopupsuccess.SetActive(true);




    }

    public string s_token;
    public string s_email;
    public int price;
    public int s_id;
    public char[] trim_char_arry = new char[] { '"' };
    IEnumerator Stripe()
    {
        s_token = pay["id"].ToString();
        s_token = s_token.ToString().Trim((trim_char_arry));
        price = int.Parse(C_card.Inst.price.ToString());
   
        s_email = DeepClass.inst.pro.GetField("email").ToString().Trim((trim_char_arry)).ToString();
        s_id = int.Parse(DeepClass.inst.pro.GetField("id").ToString().Trim((trim_char_arry)).ToString());
        string stripe_S_url = "https://wgcapi.mmocircles.com/Webhook/stripecharge";
        JSONObject form = new JSONObject();

        form.AddField("stripeToken", s_token);
        form.AddField("amount", price);
        form.AddField("stripeEmail", s_email);
        form.AddField("userid", s_id);
        Debug.Log("S_form" + form.ToString());


        UnityWebRequest surl = UnityWebRequest.Put(stripe_S_url, form.ToString());
        surl.method = UnityWebRequest.kHttpVerbPOST;
        surl.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("a_token").ToString());
        surl.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("surl1 ");
        yield return surl.SendWebRequest();
        Debug.Log("surl 2");

        if (surl.error != null)
        {
            Debug.Log("surl error>>" + surl.error.ToString());
            DeepClass.inst.cutggp();
            ggppopupsuccess.SetActive(true);
            ggpsuccesstext.text = "Purchase Unsuccessful....";
            C_card.Inst.PopupCreditCardDetails.SetActive(false);
            C_card.Inst.preloader.enabled = false;
            C_card.Inst.preloader.gameObject.SetActive(true);
            C_card.Inst.SubmitBtn.SetActive(true);
           
            C_card.Inst.cardNumberInputField.text = "";
            C_card.Inst.monthInputField.text = "";
            C_card.Inst.yearInputField.text = "";
            C_card.Inst.cvvInputField.text = "";
        }
        else
        {
            Debug.Log(" surl.downloadHandler.text>>>>" + surl.downloadHandler.text);

            DeepClass.inst.cutggp();
            ggppopupsuccess.SetActive(true);
            ggpsuccesstext.text = "Purchase successful....";

            C_card.Inst.PopupCreditCardDetails.SetActive(false);
            C_card.Inst.preloader.enabled = false;
            C_card.Inst.SubmitBtn.SetActive(true);
            C_card.Inst.preloader.gameObject.SetActive(true);

            C_card.Inst.cardNumberInputField.text = "";
            C_card.Inst.monthInputField.text = "";
            C_card.Inst.yearInputField.text = "";
            C_card.Inst.cvvInputField.text = "";
        }
    }
    public void onclicksubmit()
    {
        ////////////////MenuEventListener.instance.ColseCrystalClick();
        buypopup.SetActive(false);
        ggppopupsuccess.SetActive(false);

    }
}
