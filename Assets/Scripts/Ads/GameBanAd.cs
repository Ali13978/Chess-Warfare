using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBanAd : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bannerad();
    }
    void bannerad() {
        GoogleAdsManager.Instance.ShowBanner(3);
    }
}
