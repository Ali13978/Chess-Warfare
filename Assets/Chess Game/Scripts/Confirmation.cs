using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Confirmation : MonoBehaviour
{
    public static Confirmation Instance;

    [SerializeField]
    private GameObject Panel;

    [SerializeField]
    private Button Yes;

    [SerializeField]
    private Text DisplayText;

    private void Awake()
    {
        Instance = this;
    }

    public void SetYes(int id)
    {
        Yes.onClick.RemoveAllListeners();
        if(id==1) //exit match
        {
            Yes.onClick.AddListener(() => { GameManager.instance.ExitMatch(); });
        }
        else if(id==2) //rematch
        {
            Yes.onClick.AddListener(() => { GameManager.instance.Rematch(); });
        }
        else if(id==3) //undo vs player
        {
            Yes.onClick.AddListener(() => { GameManager.instance.UndoMovePlayer(); });
        }
    }

    public void SetText(string text)
    {
        DisplayText.text = text;
    }

    public void Show()
    {
        Panel.SetActive(true);
    }
}
