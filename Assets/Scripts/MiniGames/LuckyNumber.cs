using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LuckyNumber : MonoBehaviour
{
    public void PlayAnimation()
    {
        StartCoroutine(Animation());
    }

    IEnumerator Animation()
    {
        Debug.Log("Eular y " + this.gameObject.GetComponent<RectTransform>().eulerAngles.y);
        while (this.gameObject.GetComponent<RectTransform>().eulerAngles.y<=90)
        {
            this.gameObject.GetComponent<RectTransform>().Rotate(new Vector3(0, this.gameObject.GetComponent<RectTransform>().rotation.y + 1, 0));
            yield return new WaitForSeconds(0.01f);
        }

        if(this.gameObject.GetComponent<RectTransform>().eulerAngles.y>= 90)
        {
            this.gameObject.GetComponent<Image>().enabled = false;
            this.gameObject.transform.Find("Text").gameObject.SetActive(true);
        }
        transform.rotation = Quaternion.identity;
        yield break;
    }
}
