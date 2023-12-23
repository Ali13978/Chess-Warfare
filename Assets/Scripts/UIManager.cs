using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        Debug.Log("load main menu");
        SceneManager.LoadScene("MainMenu");
    }

    public void Leaderboard()
    {
        SceneManager.LoadScene("MainMenu");
        PlayerPrefs.SetInt("LB", 1);
    }

    public void VsComputer()
    {
        ProfileSaver profileSaver = new ProfileSaver();
        PlayerProfile playerProfile = profileSaver.LoadProfile();
        if(playerProfile.pD.Gld>=50)
        {
            playerProfile.pD.Gld -= 50;
            profileSaver.SaveProfile(playerProfile);
            DatabaseController.Instance.UpdateCoins(playerProfile.UID, playerProfile.pD.Gld);
            PlayerPrefs.SetInt("offline", 1);

            Game();
        }
        else
        {
            InfoPanel.Instance.SetText("You must have atleast 50 coins to play vs computer");
            InfoPanel.Instance.ShowInfoPanel();
        }
    }

    public void Game() //chess game
    {
        SceneManager.LoadScene("Game");
    }
}
