using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardsPanel : MonoBehaviour
{
    [SerializeField]
    private Sprite[] BoardSprites; //must be in sequence

    [SerializeField]
    private GameObject BoardButtonsPrefab,Grid,BuyingItemPanel,BoardsPan;

    [SerializeField]
    private Text BuyItemText;

    public int TotalBoards = 0; //must be equat to total number of sprites
    MyBoards myBoards=null;

    private void OnEnable()
    {
        if(IsInternetConnected())
        {
            StartCoroutine(InitializePanel());
        }
        else
        {
            InfoPanel.Instance.SetText("please connect to internet first");
            InfoPanel.Instance.ShowInfoPanel();
            BoardsPan.SetActive(false);
        }
    }

    public void ResetGrid()
    {
        foreach (Transform child in Grid.transform)
        {
            Destroy(child.gameObject);
        }
    }
    
    private IEnumerator InitializePanel()
    {
        Debug.Log("initializeing panel");
        BuyingItemPanel.SetActive(true);
        BuyItemText.text = "loading Please wait ...";
        ResetGrid();
        ProfileSaver profileSaver = new ProfileSaver();
        PlayerProfile playerProfile = profileSaver.LoadProfile();

        DatabaseController.Instance.ResetAsyncResult();
        DatabaseController.Instance.DownloadMyBoards(playerProfile.UID);
        yield return new WaitUntil(() => DatabaseController.Instance.GetAsyncResult() != 0);

        if (DatabaseController.Instance.GetJson() != null)
        {
            myBoards = JsonUtility.FromJson<MyBoards>(DatabaseController.Instance.GetJson());
        }
        else
        {
            InfoPanel.Instance.SetText("Something went wrong please try again");
            InfoPanel.Instance.ShowInfoPanel();
            BuyingItemPanel.SetActive(false);
            BoardsPan.SetActive(false);
            yield break;
        }

        BuyingItemPanel.SetActive(false);

        Debug.Log("total boards" + myBoards.Boards.Count);
        List<GameObject> TempButtons = new List<GameObject>();
        int current = 0;

        if(PlayerPrefs.HasKey("currntBoard"))
        {
            current = PlayerPrefs.GetInt("currntBoard");
        }
        else
        {
            PlayerPrefs.SetInt("currntBoard", 1);
        }

        if(myBoards.Boards[0].id==0) //for old version
        {
            myBoards.Boards[0].id = 1;
            DatabaseController.Instance.AddMyBoards(profileSaver.LoadProfile().UID, myBoards);
        }

        Debug.Log("current " + current);

        List<int> spriteIndexes = new List<int>();

        foreach(B board in myBoards.Boards)
        {
            int index = 0;
            foreach(Sprite sprite in BoardSprites)
            {
                Debug.Log("sprite name" + sprite.name);
                Debug.Log("board id " + board.id);
                if(sprite.name.Equals(board.id.ToString()))
                {
                    spriteIndexes.Add(index);
                    GameObject BoardButton = Instantiate(BoardButtonsPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                    if(sprite.name.Equals("11"))
                    {
                        BoardButton.transform.Find("name of Board").GetComponent<Text>().text = "Board 8";
                    }
                    else if(sprite.name.Equals("21"))
                    {
                        BoardButton.transform.Find("name of Board").GetComponent<Text>().text = "Board 9";
                    }
                    else if (sprite.name.Equals("22"))
                    {
                        BoardButton.transform.Find("name of Board").GetComponent<Text>().text = "Board 10";
                    }
                    else if (sprite.name.Equals("23"))
                    {
                        BoardButton.transform.Find("name of Board").GetComponent<Text>().text = "Board 11";
                    }
                    else if (sprite.name.Equals("24"))
                    {
                        BoardButton.transform.Find("name of Board").GetComponent<Text>().text = "Board 12";
                    }
                    else if(sprite.name.Equals("31"))
                    {
                        BoardButton.transform.Find("name of Board").GetComponent<Text>().text = "Board 13";
                    }
                    else
                    {
                        BoardButton.transform.Find("name of Board").GetComponent<Text>().text = "Board "+sprite.name;
                    }

                    BoardButton.transform.Find("Board Image").GetComponent<Image>().sprite = sprite;
                    if (current.ToString().Equals(sprite.name))
                    {
                        BoardButton.transform.Find("use").transform.Find("Text").GetComponent<Text>().text = "Selected";
                    }
                    else
                    {
                        BoardButton.transform.Find("use").transform.Find("Text").GetComponent<Text>().text = "Use";
                    }

                    BoardButton.transform.SetParent(Grid.transform);
                    BoardButton.transform.localScale = new Vector3(1, 1, 1);
                    int j = 0;
                    int.TryParse(sprite.name, out j);
                    BoardButton.transform.Find("use").gameObject.GetComponent<Button>().onClick.AddListener(() => { UseBoard(j); });
                    TempButtons.Add(BoardButton);
                    break;
                }
                index++;
            }
            
        }

        //add remaining that are not bought
        for(int i=0;i<BoardSprites.Length;i++)
        {
            bool isbought = false;
            foreach(int index in spriteIndexes)
            {
                if(index==i)
                {
                    isbought = true;
                    break;
                }
            }
            if(!isbought)
            {
                GameObject BoardButton = Instantiate(BoardButtonsPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                Sprite sprite = BoardSprites[i];
                if (sprite.name.Equals("11"))
                {
                    BoardButton.transform.Find("name of Board").GetComponent<Text>().text = "Board 8";
                }
                else if (sprite.name.Equals("21"))
                {
                    BoardButton.transform.Find("name of Board").GetComponent<Text>().text = "Board 9";
                }
                else if (sprite.name.Equals("22"))
                {
                    BoardButton.transform.Find("name of Board").GetComponent<Text>().text = "Board 10";
                }
                else if (sprite.name.Equals("23"))
                {
                    BoardButton.transform.Find("name of Board").GetComponent<Text>().text = "Board 11";
                }
                else if (sprite.name.Equals("24"))
                {
                    BoardButton.transform.Find("name of Board").GetComponent<Text>().text = "Board 12";
                }
                else
                {
                    BoardButton.transform.Find("name of Board").GetComponent<Text>().text = "Board " + sprite.name;
                }

                //BoardButton.transform.Find("name of Board").GetComponent<Text>().text = "Board " + BoardSprites[i].name;
                BoardButton.transform.Find("Board Image").GetComponent<Image>().sprite = BoardSprites[i];
                Debug.Log("i " + i);
                BoardButton.transform.Find("use").transform.Find("Text").GetComponent<Text>().text = "Buy (5000 Coins)";

                BoardButton.transform.SetParent(Grid.transform);
                BoardButton.transform.localScale = new Vector3(1, 1, 1);
                int j = 0;
                int.TryParse(BoardSprites[i].name, out j);
                BoardButton.transform.Find("use").gameObject.GetComponent<Button>().onClick.AddListener(() => { BuyBoard(j); });
                TempButtons.Add(BoardButton);
            }
        }
    }

    public void UseBoard(int id)
    {
        foreach(Transform btn in Grid.transform)
        {
            if(btn.transform.Find("use").transform.Find("Text").GetComponent<Text>().text.Equals("Selected"))
            {
                btn.transform.Find("use").transform.Find("Text").GetComponent<Text>().text = "Use";
                break;
            }
        }
        EventSystem.current.currentSelectedGameObject.transform.Find("Text").GetComponent<Text>().text = "Selected";
        Debug.Log("use board " + id);
        PlayerPrefs.SetInt("currntBoard", id);
    }

    public void BuyBoard(int id)
    {
        Debug.Log("buy board " + id);
        if (IsInternetConnected())
        {
            StartCoroutine(BuyItem(id));
        }
        else
        {
            InfoPanel.Instance.SetText("Please connect to internet first");
            InfoPanel.Instance.ShowInfoPanel();
        }
    }

    IEnumerator BuyItem(int id)
    {
        MyBoards tempboards = myBoards;
        B b = new B();
        b.id = id;
        tempboards.Boards.Add(b);

        ProfileSaver profileSaver = new ProfileSaver();
        PlayerProfile playerProfile = profileSaver.LoadProfile();

        if(playerProfile.pD.Gld>=5000)
        {
            BuyingItemPanel.SetActive(true);
            DatabaseController.Instance.ResetAsyncResult();

            DatabaseController.Instance.AddMyBoards(playerProfile.UID, tempboards);
            yield return new WaitUntil(() => DatabaseController.Instance.GetAsyncResult() != 0);

            if (DatabaseController.Instance.GetAsyncResult() == 3)
            {
                myBoards = tempboards;

                playerProfile.pD.Gld -= 5000;
                profileSaver.SaveProfile(playerProfile);

                DatabaseController.Instance.ResetAsyncResult();
                DatabaseController.Instance.UpdateCoins(playerProfile.UID, playerProfile.pD.Gld);
                yield return new WaitUntil(() => DatabaseController.Instance.GetAsyncResult() != 0);

                DatabaseController.Instance.ResetAsyncResult();
                ProfileController.Instance.SetCoins(playerProfile.pD.Gld);

                InfoPanel.Instance.SetText("Board successfully bought");
                InfoPanel.Instance.ShowInfoPanel();
                StartCoroutine(InitializePanel());
            }
            else
            {
                InfoPanel.Instance.SetText("Something went wrong while buying board please try again");
                InfoPanel.Instance.ShowInfoPanel();
            }
        }
        else
        {
            InfoPanel.Instance.SetText("You must have atlease 5000 Coins to buy this board");
            InfoPanel.Instance.ShowInfoPanel();
        }
        
        BuyingItemPanel.SetActive(false);
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
