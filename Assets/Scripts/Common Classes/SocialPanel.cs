using UnityEngine;
using UnityEngine.UI;

public class SocialPanel : MonoBehaviour
{
    [SerializeField]
    private Button ChatButton, AvatarButton;

    public void SelectChat()
    {
        Color color= new Color32(159, 114, 43, 255);
        AvatarButton.GetComponent<Image>().color = color;
        ChatButton.GetComponent<Image>().color = new Color(0, 0, 0);
    }

    public void SelectAvatar()
    {
        Color color = new Color32(159, 114, 43, 255);
        ChatButton.GetComponent<Image>().color = color;
        AvatarButton.GetComponent<Image>().color = new Color(0, 0, 0);
    }
}
