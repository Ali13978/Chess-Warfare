using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PD
{
    public int XP = 0;  //Current Level experience
    public int Gld = 1000; //gold
    public string AT = "GU";  //Account Type FB,GU,GO,EM (facebook, guest, google, email)
    public string Ads = "0"; //Flag for ads 0 show ads 1 hide ads
    public string em = ""; //email (for facebook and sign in with email)
}
