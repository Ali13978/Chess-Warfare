using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpinAndWin : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D Spinner;

    [SerializeField]
    private Button SpinButton;

    [SerializeField]
    private Text SpinsRemainingText;

    [SerializeField]
    private GameObject Pointer,Arrow;

    [SerializeField]
    private GameObject[] OutComes;

    [SerializeField]
    private GameObject SpinAndWinPanel;

    private bool isSpinning=false;
    private int TotalSpins = 0;

    System.DateTime lastSpinTime = new System.DateTime();
    //System.DateTime lastSpinTime;

    private void OnEnable()
    {
        if(IsInternetConnected())
        {
            ProfileSaver profileSaver = new ProfileSaver();
            MiniGame miniGame = profileSaver.LoadMiniGames();
            TotalSpins = miniGame.spins;
            miniGame.TS = 0;


            if (TotalSpins > 2)
            {
                TotalSpins = 2;
            }
            else if (TotalSpins <= 0)
            {
                TotalSpins = 0;
                Arrow.SetActive(false);
            }

            lastSpinTime = System.DateTime.Parse(miniGame.a);

            System.DateTime dateTime2 = new System.DateTime();
            dateTime2 = System.DateTime.Now;
            double hours = (dateTime2 - lastSpinTime).TotalHours;
            if (hours >= 24) //add new spin
            {
                if (TotalSpins < 2)
                {
                    TotalSpins++;
                }
                miniGame.spins = TotalSpins;
                miniGame.a = System.DateTime.Now.ToString();
                miniGame.TS = 1;
                profileSaver.SaveMiniGames(miniGame);

                PlayerProfile playerProfile = profileSaver.LoadProfile();
                DatabaseController.Instance.UpdateMiniGame(playerProfile.UID, "spins", miniGame.spins.ToString());
                DatabaseController.Instance.UpdateMiniGame(playerProfile.UID, "a", miniGame.a);
                DatabaseController.Instance.UpdateMiniGame(playerProfile.UID, "TS", miniGame.TS.ToString());
                Arrow.SetActive(true);
            }

            if(hours<24 && miniGame.TS>=2)
            {
                InfoPanel.Instance.SetText("You can only spin two times per day come back after "+(24-hours).ToString("F0")+" hours");
                InfoPanel.Instance.ShowInfoPanel();
                ExitSpinAndWin();
                return;
            }
            if (miniGame.spins==0)
            {
                InfoPanel.Instance.SetText("You can only spin two times per day come back after " + (24 - hours).ToString("F0") + " hours");
                InfoPanel.Instance.ShowInfoPanel();
                ExitSpinAndWin();
                return;
            }

            if (TotalSpins <= 0)
            {
                SpinButton.interactable = false;
            }
            else
            {
                SpinButton.interactable = true;
            }
            SpinsRemainingText.text = "Spins Remaining : " + TotalSpins;
        }
        else
        {
            InfoPanel.Instance.SetText("Please connect to internet first");
            InfoPanel.Instance.ShowInfoPanel();
            ExitSpinAndWin();
        }
        

        
    }

    public void GetFreeSpin()
    {
        ProfileSaver profileSaver = new ProfileSaver();
        MiniGame miniGame = profileSaver.LoadMiniGames();

        System.DateTime dateTime2 = new System.DateTime();
        dateTime2 = System.DateTime.Now;
        double hours = (dateTime2 - lastSpinTime).TotalHours;

        if(hours<24)
        {
            if (miniGame.TS >= 2)
            {
                InfoPanel.Instance.SetText("You can only get max 2 spins per day");
                InfoPanel.Instance.ShowInfoPanel();
                return;
            }
            if(miniGame.spins==1 && miniGame.TS==1)
            {
                InfoPanel.Instance.SetText("You can only get max 2 spins per day");
                InfoPanel.Instance.ShowInfoPanel();
                return;
            }
        }
        else if(hours>=24)
        {
            miniGame.TS = 0;
        }

        //show Ad
        StartCoroutine(EarnReward());
    }

    IEnumerator EarnReward()
    {
        if(TotalSpins<2)
        {

            if(GoogleAdsManager.Instance.IsLoading())
            {
                InfoPanel.Instance.SetText("Loading Ads this might take some moment.. try again after some time");
                InfoPanel.Instance.ShowInfoPanel();
                yield break;
            }

            GoogleAdsManager.Instance.SetProcess(2);
            GoogleAdsManager.Instance.SetEarnedReward(false);
            GoogleAdsManager.Instance.SetProcessCompleted(false);
            GoogleAdsManager.Instance.PlayRewarded();

            yield return new WaitUntil(() => GoogleAdsManager.Instance.isProcessCompleted());

            GoogleAdsManager.Instance.SetProcess(-1);

            if (GoogleAdsManager.Instance.GetEarnedReward() == true)
            {
                InfoPanel.Instance.SetText("Congratulations You earned a free spin");
                InfoPanel.Instance.ShowInfoPanel();

                TotalSpins++;
                SpinsRemainingText.text = "Remaining Spins : " + TotalSpins;

                ProfileSaver profileSaver = new ProfileSaver();
                PlayerProfile playerProfile = profileSaver.LoadProfile();
                MiniGame miniGame = profileSaver.LoadMiniGames();
                miniGame.spins = TotalSpins;
                profileSaver.SaveMiniGames(miniGame);

                DatabaseController.Instance.ResetAsyncResult();
                DatabaseController.Instance.UpdateMiniGame(playerProfile.UID, "spins", miniGame.spins.ToString());
                yield return new WaitUntil(() => DatabaseController.Instance.GetAsyncResult() != 0);

                if (!SpinButton.IsInteractable())
                {
                    SpinButton.interactable = true;
                    Arrow.SetActive(true);
                }
            }
            else
            {
                InfoPanel.Instance.SetText("Please watch complete video to get free spin");
                InfoPanel.Instance.ShowInfoPanel();
            }
        }
        else
        {
            InfoPanel.Instance.SetText("You already have Max Spins");
            InfoPanel.Instance.ShowInfoPanel();
        }
    }

    public void Spin()
    {
        SpinButton.interactable = false;
        isSpinning = true;
        for (int i = 0; i < OutComes.Length; i++)
        {
            OutComes[i].GetComponent<BoxCollider2D>().enabled = false;
        }

        if(TotalSpins>0) //spin
        {
            TotalSpins--;

            Arrow.SetActive(false);
            Spinner.AddTorque(Random.Range(-700,-300));
            StartCoroutine(SpinAndWait());
            ProfileSaver profileSaver = new ProfileSaver();
            PlayerProfile playerProfile = profileSaver.LoadProfile();
            MiniGame miniGame = profileSaver.LoadMiniGames();
            miniGame.spins = TotalSpins;
            DatabaseController.Instance.UpdateMiniGame(playerProfile.UID, "spins", miniGame.spins.ToString());

            lastSpinTime = System.DateTime.Now;
            PlayerPrefs.SetString("time", lastSpinTime.ToString());
            miniGame.a = lastSpinTime.ToString();
            miniGame.TS++;
            profileSaver.SaveMiniGames(miniGame);

            DatabaseController.Instance.UpdateMiniGame(playerProfile.UID, "a", miniGame.a);
            DatabaseController.Instance.UpdateMiniGame(playerProfile.UID, "TS", miniGame.TS.ToString());

        }
        else
        {
            SpinButton.interactable = false;
        }

        SpinsRemainingText.text = "Spins Remaining : "+TotalSpins;
    }

    public void ExitSpinAndWin()
    {
        if(!isSpinning)
        {
            SpinAndWinPanel.SetActive(false);
            this.gameObject.SetActive(false);
        }
    }

    IEnumerator SpinAndWait()
    {
        yield return new WaitForSeconds(1f);
        yield return new WaitWhile(() => Spinner.angularVelocity<0f);

        Pointer.GetComponent<Pointer>().ResetPrize();

        for (int i = 0; i < OutComes.Length; i++)
        {
            OutComes[i].GetComponent<BoxCollider2D>().enabled = true;
        }

        yield return new WaitForSeconds(1f);
        ProfileSaver profileSaver = new ProfileSaver();
        PlayerProfile playerProfile = profileSaver.LoadProfile();

        if (Pointer.GetComponent<Pointer>().GetPrize()==0)
        {
            InfoPanel.Instance.SetText("Better Luck Next time");
            InfoPanel.Instance.ShowInfoPanel();
        }
        else
        {
            InfoPanel.Instance.SetText("Congratulations! You earned " + Pointer.GetComponent<Pointer>().GetPrize()+" Coins");
            InfoPanel.Instance.ShowInfoPanel();


            playerProfile.pD.Gld += Pointer.GetComponent<Pointer>().GetPrize();
            profileSaver.SaveProfile(playerProfile);

            ProfileController profileController = FindObjectOfType<ProfileController>();
            profileController.SetCoins(playerProfile.pD.Gld);

            DatabaseController.Instance.UpdateCoins(playerProfile.UID, playerProfile.pD.Gld);
        }

        Pointer.GetComponent<Pointer>().ResetPrize();
        
        if (TotalSpins>0)
        {
            Arrow.SetActive(true);
            SpinButton.interactable = true;
        }
        Debug.Log("Spinner Stopped");
        isSpinning = false;
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