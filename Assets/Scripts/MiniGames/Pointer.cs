using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    int prize = 0;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        int.TryParse(collision.gameObject.name, out prize);
        Debug.Log("triggered gameobject name : " + collision.gameObject.name);
    }

    public int GetPrize()
    {
        return prize;
    }

    public void ResetPrize()
    {
        prize = 0;
    }
}
