using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject LoadingPan;

    [SerializeField]
    private Text LoadingText;

    [SerializeField]
    private Image FillImage;

    public void Show()
    {
        LoadingPan.SetActive(true);
    }

    public void Hide()
    {
        LoadingPan.SetActive(false);
    }

    public void SetProgress(float progress)
    {
        float amount = progress / 100;
        FillImage.fillAmount = amount;
        LoadingText.text = progress + "%";
    }

}
