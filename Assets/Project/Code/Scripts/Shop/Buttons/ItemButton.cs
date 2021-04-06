﻿using UnityEngine;
using UnityEngine.EventSystems;

public class ItemButton : ShopButton
{
    [SerializeField] private Item buttonItem;
    public Item ButtonItem { get => buttonItem; }

    void Start() => SetButton(ButtonItem.ItemIcon, ButtonItem.ItemCost);

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        PlayerShop.BuyItem(ButtonItem);
    }
}