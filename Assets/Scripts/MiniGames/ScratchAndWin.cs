using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ScratchAndWin : MonoBehaviour
{
    [SerializeField]
    private GameObject TempObjects,ScratchAndWinPanel,ScratchAndWinInfoPanel;

    [SerializeField]
    Text InfoTextCustom;

    [SerializeField]
    private GameObject[] Locations,ScratchParents;

    [SerializeField]
    private GameObject []Prizes,LuckyNumber;

    [SerializeField]
    private Sprite WinnerBackground;

    public GameObject Mask;

    private GameObject[] HiddenPrizes;

    private bool canScratch = false;
    private bool isPressed = false;

    private int prize = 0;
    int[] prizePositions = new int[3];

    private List<int> ScratchedIndexes = new List<int>();

    System.DateTime LastScratchTime = new System.DateTime();

    private InfoPanel infoPanel;
    private void Update()
    {
        var mousePos = Input.mousePosition;

        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        if (isPressed && canScratch)
        {
            GameObject maskSprite = Instantiate(Mask, mousePos, Quaternion.identity);

            maskSprite.transform.parent = TempObjects.transform;
            maskSprite.transform.localScale = new Vector3(200f, 200f, 200f);
            Vector3 pos = maskSprite.transform.localPosition;
            maskSprite.transform.localPosition = new Vector3(pos.x, pos.y, -1f);
        }

        if (Input.GetMouseButton(0))
        {
            isPressed = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isPressed = false;
        }
    }

    private void RandomPrize() //spawn random prize
    {
        HiddenPrizes = new GameObject[Locations.Length];
        
        prize= Random.Range(0, Prizes.Length-1);
        Debug.Log("Prize "+prize);

        prizePositions[0] = Random.Range(0, Locations.Length - 1);
        prizePositions[1] = Exclude(prizePositions[0]);
        prizePositions[2] = Exclude(prizePositions[0], prizePositions[1]);

        Debug.Log("Prize location " + prizePositions[0]);
        Debug.Log("Prize location " + prizePositions[1]);
        Debug.Log("Prize location " + prizePositions[2]);

        int []temp = new int[Prizes.Length];
        int[] exclusion = new int[Locations.Length];
        exclusion[0] = prize;
        int ex = 1;


        bool spawnPrize=false;

        for (int i = 0; i < Locations.Length; i++)
        {
            int next = Exclude(exclusion); //dont spawn any object 2 times except prizes

            if (prizePositions[0] == i)
            {
                next = prize;
                spawnPrize = true;
            }
            else if (prizePositions[1] == i)
            {
                next = prize;
                spawnPrize = true;
            }
            else if (prizePositions[2] == i)
            {
                next = prize;
                spawnPrize = true;
            }

            if (spawnPrize)
            {
                spawnPrize = false;
                HiddenPrizes[i] = new GameObject(Prizes[next].name);
                HiddenPrizes[i].AddComponent<SpriteRenderer>();
                HiddenPrizes[i].GetComponent<SpriteRenderer>().sprite = Prizes[next].GetComponent<SpriteRenderer>().sprite;
                HiddenPrizes[i].transform.parent = TempObjects.transform;
                HiddenPrizes[i].transform.localScale = new Vector2(50f, 50f);
                HiddenPrizes[i].transform.localPosition = new Vector3(Locations[i].transform.localPosition.x, Locations[i].transform.localPosition.y, -1f);
                HiddenPrizes[i].GetComponent<SpriteRenderer>().sortingOrder = 2;
            }
            else if (temp[next] < 2)
            {
                temp[next]++;
                HiddenPrizes[i] = new GameObject(Prizes[next].name);
                HiddenPrizes[i].AddComponent<SpriteRenderer>();
                HiddenPrizes[i].GetComponent<SpriteRenderer>().sprite = Prizes[next].GetComponent<SpriteRenderer>().sprite;
                HiddenPrizes[i].transform.parent = TempObjects.transform;
                HiddenPrizes[i].transform.localScale = new Vector2(50f, 50f);
                HiddenPrizes[i].transform.localPosition = new Vector3(Locations[i].transform.localPosition.x, Locations[i].transform.localPosition.y, -1f);
                HiddenPrizes[i].GetComponent<SpriteRenderer>().sortingOrder = 2;
                if(temp[next]==2)
                {
                    exclusion[ex] = next;
                    ex++;
                }
            }
        }
    }

    private void OnDisable() //exit
    {
        Debug.Log("on disable");
        foreach (Transform child in TempObjects.transform)
        {
            Destroy(child.gameObject);
        }

        for(int i=0;i<Locations.Length;i++)
        {
            Locations[i].SetActive(false);
        }
    }

    private void OnEnable() 
    {
        Debug.Log("onEnable scratch and win");
        isPressed = false;
        canScratch = false;
        infoPanel = FindObjectOfType<InfoPanel>();

        ProfileSaver profileSaver = new ProfileSaver();
        MiniGame miniGame = profileSaver.LoadMiniGames();

        LastScratchTime = System.DateTime.Parse(miniGame.b);

        Debug.Log("last scratch time " +LastScratchTime);

        System.DateTime dateTime2 = new System.DateTime();
        dateTime2 = System.DateTime.Now;
        double hours = (dateTime2 - LastScratchTime).TotalHours;
        if (hours >= 24)
        {
            canScratch = true;
        }

        
        if (IsInternetConnected())
        {
            isPressed = false;
            if (canScratch)
            {
                ScratchAndWinPanel.SetActive(true);
                RandomPrize();

                for (int i = 0; i < ScratchParents.Length; i++)
                {
                    StartCoroutine(WaitForScratch(i));
                }
            }
            else
            {
                StartCoroutine(CountDownForScratch());
                ScratchAndWinPanel.SetActive(false);
                //this.gameObject.SetActive(false);
            }
        }
        else
        {
            infoPanel.SetText("Please connect to internet first");
            infoPanel.ShowInfoPanel();
            this.gameObject.SetActive(false);
        }
        
    }

    IEnumerator CountDownForScratch()
    {
        ScratchAndWinInfoPanel.SetActive(true);
        float timeleft = 1;

        System.DateTime tempDate = LastScratchTime;
        tempDate = tempDate.AddDays(1);
        while (timeleft>0f)
        {
            //Get the current time
            System.DateTime now = System.DateTime.Now;

            System.TimeSpan span = tempDate - now;

            InfoTextCustom.text = "Time Until Next scratch \n" + span.Hours + ":" + span.Minutes + ":" + span.Seconds;

            timeleft = (span.Hours * 60 * 60) + (span.Minutes*60) + span.Seconds;
            Debug.Log("time left " + timeleft);
            yield return new WaitForSecondsRealtime(1);
        }

        ScratchAndWinInfoPanel.SetActive(false);

        isPressed = false;
        canScratch = true;
        ScratchAndWinPanel.SetActive(true);
        RandomPrize();

        for (int i = 0; i < ScratchParents.Length; i++)
        {
            StartCoroutine(WaitForScratch(i));
        }

        yield return null;
    }

    private int Exclude(int num1, int num2)
    {
        var exclude = new HashSet<int>() { num1, num2 };
        var range = Enumerable.Range(1, Locations.Length - 1).Where(i => !exclude.Contains(i));

        var rand = new System.Random();
        int index = rand.Next(0, (Locations.Length - 1) - exclude.Count);
        return range.ElementAt(index);
    }

    private int Exclude(int num1)
    {
        var exclude = new HashSet<int>() { num1};
        var range = Enumerable.Range(1, Locations.Length-1).Where(i => !exclude.Contains(i));

        var rand = new System.Random();
        int index = rand.Next(0, (Locations.Length - 1) - exclude.Count);
        return range.ElementAt(index);
    }

    private int Exclude(int []arr) //exclude all which are in the passed array
    {
        HashSet<int> hash = new HashSet<int>();
        for(int i=0;i<arr.Length;i++)
        {
            hash.Add(arr[i]);
        }
        var exclude = hash;
        var range = Enumerable.Range(1, Locations.Length - 1).Where(i => !exclude.Contains(i));

        var rand = new System.Random();
        int index = rand.Next(0, (Locations.Length - 1) - exclude.Count);
        return range.ElementAt(index);
    }

    IEnumerator WaitForScratch(int index)
    {
        yield return new WaitUntil(() => ScratchParents[index].GetComponent<ScratcherParent>().TotalTrigger() >= 3);
        ScratchParents[index].GetComponent<ScratcherParent>().enabled = false;
        ScratchedIndexes.Add(index);
        //Locations[index].SetActive(true);
        CalculateResult();
    }

    //showing result on one reveal
    void CalculateResult()
    {
        int count = 0;
        for (int i = 0; i < ScratchedIndexes.Count; i++)
        {
            if(ScratchedIndexes[i]==prizePositions[0] || ScratchedIndexes[i] == prizePositions[1] || ScratchedIndexes[i] == prizePositions[2]) //prize scratched
            {
                count++;
            }
        }

        if(count==3) //3 areas are scratched
        {
            for(int i=0;i<3;i++)
            {
                Locations[prizePositions[i]].SetActive(true);

                GameObject background = new GameObject();
                background.AddComponent<SpriteRenderer>();
                background.GetComponent<SpriteRenderer>().sprite = WinnerBackground;
                background.transform.parent = TempObjects.transform;
                background.transform.localScale = new Vector2(16f, 12f);
                background.transform.localPosition = new Vector3(Locations[prizePositions[i]].transform.localPosition.x, Locations[prizePositions[i]].transform.localPosition.y, -1f);
                background.GetComponent<SpriteRenderer>().sortingOrder = 1;
            }

            ProfileSaver profileSaver = new ProfileSaver();
            PlayerProfile playerProfile = profileSaver.LoadProfile();

            int result = 0;
            int.TryParse(Prizes[prize].name, out result);
            playerProfile.pD.Gld += result;

            //show lucky number

            for (int i = 5; i > -1; i--)
            {
                if (result != 0)
                {
                    int temp = result % 10;
                    LuckyNumber[i].transform.Find("Text").GetComponent<Text>().text = temp.ToString();
                    LuckyNumber[i].GetComponent<LuckyNumber>().PlayAnimation();
                    result /= 10;
                }
                else
                {
                    LuckyNumber[i].transform.Find("Text").GetComponent<Text>().text = " ";
                    LuckyNumber[i].GetComponent<LuckyNumber>().PlayAnimation();
                }
            }

            ProfileController profileController = FindObjectOfType<ProfileController>();
            profileController.SetCoins(playerProfile.pD.Gld);
            DatabaseController.Instance.UpdateCoins(playerProfile.UID, playerProfile.pD.Gld);

            canScratch = false;
            LastScratchTime = System.DateTime.Now;
            MiniGame miniGame = profileSaver.LoadMiniGames();
            miniGame.b = LastScratchTime.ToString();
            profileSaver.SaveMiniGames(miniGame);

            profileSaver.SaveProfile(playerProfile);
            DatabaseController.Instance.UpdateMiniGame(playerProfile.UID, "b", miniGame.b);

            StopAllCoroutines();
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