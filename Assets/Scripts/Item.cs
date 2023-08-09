using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public enum ItemType { None, Apple, Banana, Nuts };

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteInEditMode]
public class Item : MonoBehaviour
{
    [SerializeField] private ItemType type;
    [SerializeField] private int amtToAdd;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private Renderer meshRenderer;

    public int AmtToAdd { get => amtToAdd; }
    public ItemData Data { get; set; } = new ItemData();

    void Start()
    {
        InitialiseItem(type);
    }

    void Update()
    {
        if (!EditorApplication.isPlaying)
            LoadMesh(type);
    }

    void SetItemData(ItemType t, string n, string a, Sprite i, bool b, int m)
    {
        Data.Type = t;
        Data.Name = n;
        Data.Action = a;
        Data.Image = i;
        Data.CanStack = b;
        Data.MaxQuantity = m;
    }

    void LoadMesh(ItemType type)
    {
        if(type == ItemType.None)
        {
            meshFilter.mesh = null;
            meshRenderer.material = null;
        }
        else
        {
            meshFilter.mesh = Resources.Load<MeshFilter>("Inventory/" + type.ToString() + "_mesh").sharedMesh;
            meshRenderer.material = Resources.Load<Material>("Inventory/" + type.ToString() + "_mat");
        }
    }

    void InitialiseItem(ItemType type)
    {
        switch (type)
        {
            case ItemType.None:
                SetItemData(type, "", "", null, false, 0);
                break;
            case ItemType.Apple:
                SetItemData(type, "APPLE", "Eat Apple.", Resources.Load<Sprite>("Inventory/Apple_UI"), false, 1);
                break;
            case ItemType.Banana:
                SetItemData(type, "BANANA", "Eat Banana.", Resources.Load<Sprite>("Inventory/Banana_UI"), true, 5);
                break;
            case ItemType.Nuts:
                SetItemData(type, "PISTACHIOS", "Eat Pistachios.", Resources.Load<Sprite>("Inventory/Nuts_UI"), true, 100);
                break;
        }
    }
}

public class ItemData
{
    public ItemType Type { get; set; }
    public string Name { get; set; }
    public string Action { get; set; }
    public Sprite Image { get; set; }
    public bool CanStack { get; set; }
    public int CurrentQuantity { get; set; }
    public int MaxQuantity { get; set; }
}

