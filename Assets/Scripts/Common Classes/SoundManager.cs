using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource musicSource;

    public AudioSource MovePiece, KillPiece,Win,Lose,Tap;
    public static SoundManager instance=null;

    public GameObject Music;
    public GameObject muteMusic;
    void Awake()
    {
        //Check if there is already an instance of SoundManager
        if (instance == null)
            //if not, set it to this.
            instance = this;
        //If instance already exists:
        else if (instance != this)
            //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
            Destroy(gameObject);

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);

        if(PlayerPrefs.HasKey("music"))
        {
            if(PlayerPrefs.GetInt("music")==1)
            {
                musicSource.Play();
            }
        }
        else
        {
            PlayerPrefs.SetInt("music", 1);
            musicSource.Play();
        }
    }

    public void ToggleMusic() //turns on if off and vice versa
    {
        if(musicSource.isPlaying)
        {
            musicSource.Stop();
            PlayerPrefs.SetInt("music", 0);
            
        }
        else
        {
            musicSource.Play();
            PlayerPrefs.SetInt("music", 1);
            

        }
    }

    public void PlayMovePiece()
    {
        MovePiece.Play();
    }

    public void PlayKillPiece()
    {
        KillPiece.Play();
    }
    
    public void PlayWinner()
    {
        Win.Play();
    }

    public void PlayLosser()
    {
        Lose.Play();
    }

    public void PlayTap()
    {
        Tap.Play();
    }
}
