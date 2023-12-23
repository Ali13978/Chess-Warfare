using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
public class video_playing : MonoBehaviour {

    public VideoPlayer video;
    bool isactivel = true;
    AsyncOperation op;

    // Use this for initialization
    void Start () {

        
     
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (video.isPlaying && isactivel)
        {
            StartCoroutine(setscene());
            isactivel = false;

        }
        //if (!video.isPlaying && isactivel)
        //{
        //    Debug.Log("hello");
        //    SceneManager.LoadScene(1, LoadSceneMode.Single);
        //   // SceneManager.LoadSceneAsync("menu", LoadSceneMode.Single);
        //    isactivel = false;
        //    //
        //    // StartCoroutine(setscene());
        //}
        
	}

    IEnumerator setscene()
    {
        Debug.Log("start");
        yield return new WaitForSeconds(5f);
        DeepClass.inst.loginPanel.SetActive(true);
        gameObject.SetActive(false);
    }

}
