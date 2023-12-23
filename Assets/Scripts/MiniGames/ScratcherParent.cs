using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScratcherParent : MonoBehaviour
{
    private int TotalTriggers=0;

    IEnumerator Center;
    IEnumerator DiagonalLeftTop;
    IEnumerator DiagonalRightTop;
    IEnumerator DiagonalLeftBottom;
    IEnumerator DiagonalRightBottom;

    private void Start()
    {
        Center = GetScracherTrigger("center");
        DiagonalLeftBottom = GetScracherTrigger("DiagonalLeftBottom");
        DiagonalLeftTop = GetScracherTrigger("DiagonalLeftTop");
        DiagonalRightBottom = GetScracherTrigger("DiagonalRightBottom");
        DiagonalRightTop = GetScracherTrigger("DiagonalRightTop");

        StartCoroutine(Center);
        StartCoroutine(DiagonalLeftBottom);
        StartCoroutine(DiagonalRightBottom);
        StartCoroutine(DiagonalLeftTop);
        StartCoroutine(DiagonalRightTop);
    }

    IEnumerator GetScracherTrigger(string name)
    {
        yield return new WaitUntil(()=> this.gameObject.transform.Find(name).GetComponent<Scratcher>().IsTriggered());
        TotalTriggers++;
    }

    public int TotalTrigger()
    {
        return TotalTriggers;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
