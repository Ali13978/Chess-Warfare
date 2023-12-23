using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class C_card : MonoBehaviour {
    public static C_card Inst;

    public UIController uiManager;

    public InputField cardNumberInputField;
    public InputField cvvInputField;
    public InputField yearInputField;
    public InputField monthInputField;
    public string card1;
    public string cv1;
    public int mon1;
    public int year1;

    public GameObject PopupMessage;
    public Text messagePopup_Message;
    public GameObject OkButtonPopupMessage;
    public GameObject LoginButtonPopupMessage;
    public GameObject CloseButtonPopupMessage;

    public GameObject PopupByGGP;
    public GameObject PopupCreditCardDetails;
    public GameObject PurchaseGGPBtn;
    public GameObject PopupMessageBuyGGP;

    public GameObject PurchaseSucessObj;

    public float price;
    public string gemsindex;
    public bool purcheshsuccess = false;
    public Image preloader;
    RectTransform rectLoader;
    public GameObject SubmitBtn;




    private void Awake()
    {
        Inst = this;
        rectLoader = preloader.GetComponent<RectTransform>();
    }

    void Start () 
    {
       
    }
    
    void Update () 
    {
        {
            if (!preloader.enabled)
                return;
            rectLoader.Rotate(rectLoader.forward, -2);
        }
    }

    public void onCleckSubmitBtnOncreditCardPopup()
    {
        card1 = cardNumberInputField.text ?? "";
        cv1 = cvvInputField.text ?? "";

        Debug.Log("Card_no= " + card1 + ",   CVV = " + cv1 + ",  Month = " + ",  Year = " + year1);
        if (card1 == "" || cv1 == "" || monthInputField.text == null || yearInputField.text == null)
        {
            PopupMessage.SetActive(true);
            messagePopup_Message.text = "Invalid card Detail...";
            LoginButtonPopupMessage.SetActive(false);
            OkButtonPopupMessage.SetActive(true);
            CloseButtonPopupMessage.SetActive(false);
            return;
        }
        mon1 = int.Parse(monthInputField.text);
        year1 = int.Parse(yearInputField.text);

        callstrip.Inst.payment(card1, mon1, year1, cv1);
        preloader.enabled = true;
        preloader.gameObject.SetActive(true);
        SubmitBtn.SetActive(false);
    }


    public void OnClickGGPButton()
    {
      /*  //PopupByGGP.SetActive(true);
        if (DeepClass.IsInternetConnection())
        {
            if (DeepClass.inst.loginPanel.activeSelf == true)
            {
                PopupMessage.SetActive(true);
                messagePopup_Message.text = "Please login with your SGC account...";
                LoginButtonPopupMessage.SetActive(true);
                OkButtonPopupMessage.SetActive(false);
                CloseButtonPopupMessage.SetActive(true);
            }
            else
            {

                PopupByGGP.SetActive(true);
            }
        }
        else
        {
            PopupMessage.SetActive(true);
            messagePopup_Message.text = "No internet connection available!...";
            LoginButtonPopupMessage.SetActive(false);
            OkButtonPopupMessage.SetActive(true);
            CloseButtonPopupMessage.SetActive(false);
        }


        */




        if (DeepClass.IsInternetConnection())
        {
            if (DeepClass.playWithoutLoiginVar == 0)
            {
                PopupByGGP.SetActive(true);
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

    public void OnClickPurchaseGGPButton(float i)
    {
        PopupByGGP.SetActive(false);

        PopupCreditCardDetails.SetActive(true);
        price = (i * 100);
        Debug.Log("Price = " + price);
      
    }

    public void char_buy(int i)
    {
        Debug.Log("char ID==" + i);
        purcheshsuccess = false;
        gemsindex = Openlink.Inst.obj1[i].GetField("id").ToString();
        Debug.Log(" gemsindex ID==" + gemsindex);

        gems();

    }
    public void gems()
    {

    
        string url = "https://wgcapi.mmocircles.com/Products/" + gemsindex + "/purchase";
        Debug.Log("URL= " + url);
        JSONObject data = new JSONObject();
        data.AddField("quantity", 1);

        Debug.Log("form" + data.ToString());

        UnityWebRequest pro = UnityWebRequest.Put(url, data.ToString());
        pro.method = UnityWebRequest.kHttpVerbPOST;
        pro.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("a_token").ToString());
        pro.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("in gems1 ");
        pro.SendWebRequest();
        Debug.Log("in gems 2");

        if (pro.error != null)
        {
            purcheshsuccess = false;

            /* PopupMessage.SetActive(true);
             messagePopup_Message.text = "Purchase Failed...";
             LoginButtonPopupMessage.SetActive(false);
             OkButtonPopupMessage.SetActive(true);
             CloseButtonPopupMessage.SetActive(false);*/

            //uiManager.msgTxt_noLogin.SetActive(false);
            //uiManager.msgTxt_noInternet.SetActive(true);
            //uiManager.MiddileOK.SetActive(true);
            //uiManager.LoginOK.SetActive(false);
            //uiManager.Login.SetActive(false);
            //uiManager.MsgPopUp.SetActive(true);
        }
        else
        {
            DeepClass.inst.cutggp();
            purcheshsuccess = true;

            PurchaseSucessObj.SetActive(true);

            /* PopupMessage.SetActive(true);
             messagePopup_Message.text = "Purchase Success...";
             LoginButtonPopupMessage.SetActive(false);
             OkButtonPopupMessage.SetActive(true);
             CloseButtonPopupMessage.SetActive(false);*/
        }
    }

    public void PopupBuyGGPClose()
    {
        PopupByGGP.SetActive(false);
    }
    public void  PopupCreditCardDetailsClose()
    {
        PopupCreditCardDetails.SetActive(false);
    }
    public void PopupOKBtn()
    {
        PopupMessage.SetActive(false);
    }
    public void PopupCloseBtn()
    {
        Debug.Log("Clicked....");
        PopupMessage.SetActive(false);
    }
    public void PopupLoginClose()
    {
        PopupMessage.SetActive(false);
    }

    public void PopupOKBtnBuyGGP()
    {
        PopupMessageBuyGGP.SetActive(false);
        C_card.Inst.preloader.enabled = false;
        C_card.Inst.SubmitBtn.SetActive(true);
        C_card.Inst.preloader.gameObject.SetActive(false);
        C_card.Inst.PopupCreditCardDetails.SetActive(false);
        C_card.Inst.cardNumberInputField.text = "";
        C_card.Inst.monthInputField.text = "";
        C_card.Inst.yearInputField.text = "";
        C_card.Inst.cvvInputField.text = "";

    }
    public void NumberFormat()
    {
        Debug.Log("Card number onvalue change.........");
        string scoreAsStringWithSpaces = cardNumberInputField.text.ToString().Aggregate(string.Empty, (c, i) => c + i + ' ');
    }






    public void HidePurchaseSucess()
    {
        PurchaseSucessObj.SetActive(false);

    }
}
