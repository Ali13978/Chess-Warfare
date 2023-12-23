using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Chat : MonoBehaviour
{
    public static Chat Instance;

    [SerializeField]
    private GameObject ChatTextPrefab,Grid,PopUpMessage,ChatPanel,ChatButton,ChatPackPan,MyPopUpMessage;

    private IEnumerator PopUpCoroutine=null;
    private IEnumerator myPopUpCoroutine = null;
    private PhotonView photonView;

    private void Start()
    {
        photonView = gameObject.GetComponent<PhotonView>();
    }
    private void Awake()
    {
        Instance = this;
    }

    public void AddNormalChat()
    {
        AddChatButton("Hi");
        AddChatButton("Welcome");
        AddChatButton("Good Luck");
        AddChatButton("You play well");
        AddChatButton("Rematch?");
    }

    public void AddLanguagePack()
    {
        AddChatButton("Bonjour");
        AddChatButton("Hola");
        AddChatButton("Guten Tag");
        AddChatButton("Ciao");
        AddChatButton("Namaste");
        AddChatButton("Salam");
        AddChatButton("Shalom");
        AddChatButton("Konichiwa");
        AddChatButton("Ni Hao");
        AddChatButton("Au Revoir");
        AddChatButton("Auf Wiedersehen");
        AddChatButton("Sayonara");
        AddChatButton("Zai Jian");
    }

    public void AddSupportPack()
    {
        AddChatButton("Good Move");
        AddChatButton("You're pretty good");
        AddChatButton("you Play well");
        AddChatButton("You're a master");
    }

    private void AddChatButton(string message)
    {
        GameObject ChatBtn = Instantiate(ChatTextPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        ChatBtn.transform.Find("Text").GetComponent<Text>().text = message;
        ChatBtn.transform.SetParent(Grid.transform);
        ChatBtn.transform.localScale = new Vector3(1, 1, 1);
        string msg = message;
        ChatBtn.GetComponent<Button>().onClick.AddListener(() => { SendMessag(msg); });
    }

    public void SendMessag(string Message)
    {
        photonView.RPC("ReceiveMessage", RpcTarget.Others,Message);
        ChatPanel.SetActive(false);
        ChatButton.SetActive(true);

        MyPopUpMessage.transform.Find("Text").GetComponent<Text>().text = Message;
        MyPopUpMessage.SetActive(true);
        StartCoroutine(MyPopUpTimer());
       
    }

    [PunRPC]
    private void ReceiveMessage(string message)
    {
        PopUpMessage.transform.Find("Text").GetComponent<Text>().text = message;
        PopUpMessage.SetActive(true);
        StartCoroutine(PopUpTimer());
    }

    private IEnumerator MyPopUpTimer()
    {
        yield return new WaitForSeconds(2);
        MyPopUpMessage.SetActive(false);
    }

    private IEnumerator PopUpTimer()
    {
        yield return new WaitForSeconds(2);
        PopUpMessage.SetActive(false);
    }

    public void BuyChatPack()
    {
        StartCoroutine(ChatPackPanel());
    }

    IEnumerator ChatPackPanel()
    {
        PlayerPrefs.SetInt("chatpack", 1);
        yield return new WaitForSeconds(3);
        ChatPackPan.SetActive(false);
    }
}
