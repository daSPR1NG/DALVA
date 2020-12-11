﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryBox : MonoBehaviour
{
    [SerializeField] private int transactionID;
    private Inventory Inventory => GetComponentInParent<Inventory>();
    public Item StoredItem { get; set; }
    private Image ItemIcon => transform.GetChild(0).GetComponent<Image>();
    private CanvasGroup CanvasGrp => transform.GetChild(0).GetComponent<CanvasGroup>();
    public int TransactionID { get => transactionID; set => transactionID = value; }
    public Inventory PlayerInventory { get => Inventory; }

    private void Start()
    {
        CanvasGrp.alpha = 0;
    }

    public void UpdateInventoryBoxItem(Item newItem, Sprite newItemIcon)
    {
        StoredItem = newItem;
        StoredItem.InventoryBox = this;

        CanvasGrp.alpha = 1;
        ItemIcon.sprite = newItemIcon;
    }

    public void ResetInventoryBoxItem(InventoryBox inventoryBoxToReset)
    {
        inventoryBoxToReset.StoredItem.InventoryBox = null;
        inventoryBoxToReset.StoredItem = null;

        inventoryBoxToReset.CanvasGrp.alpha = 0;
        inventoryBoxToReset.ItemIcon.sprite = null;
    }
}