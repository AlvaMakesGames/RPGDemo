using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    [SerializeField] private Image btnImage;
    [SerializeField] private Button slotBtn;
    [SerializeField] private Text btnName, btnQuant;
    [SerializeField] private int slotIndex;

    public int SlotIndex { get => slotIndex; }

    public void SetButton(ItemData item)
    {
        if(item.Image != null)
        {
            slotBtn.interactable = true;
            btnImage.sprite = item.Image;
            btnImage.color = Color.white;
            btnName.text = item.Name;
            btnQuant.text = item.CurrentQuantity.ToString("000") + "/" + item.MaxQuantity.ToString("000");
        }
        else
        {
            slotBtn.interactable = false;
            btnImage.sprite = null;
            btnImage.color = Color.grey;
            btnName.text = "";
            btnQuant.text = "";
        }
    }

    public void ButtonSelected()
    {
        SingletonManager.Instance.SetSelectedItem(slotIndex);
    }
}
