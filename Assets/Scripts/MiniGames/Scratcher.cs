using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scratcher : MonoBehaviour
{
    bool isTriggered = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        isTriggered = true;
    }

    public bool IsTriggered()
    {
        return isTriggered;
    }
}
