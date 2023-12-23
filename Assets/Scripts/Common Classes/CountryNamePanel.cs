using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountryNamePanel : MonoBehaviour
{
    private bool isSelected = false;

    public bool IsSelected()
    {
        return isSelected;
    }

    public void SetIsSelected(bool isselected)
    {
        isSelected = isselected;
    }
}
