using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicToggle : MonoBehaviour
{

    public void Toggle()
    {
        SoundManager.instance.ToggleMusic();
    }
}
