using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avatars : MonoBehaviour
{
    public static Avatars Instance;

    [SerializeField]
    private Sprite[] BasicAvatars, PremiumAvatars;

    private void Awake()
    {
        Instance = this;
    }

    public Sprite[] GetBasicAvatars()
    {
        return BasicAvatars;
    }

    public Sprite[] GetPremiumAvatars()
    {
        return PremiumAvatars;
    }
}
