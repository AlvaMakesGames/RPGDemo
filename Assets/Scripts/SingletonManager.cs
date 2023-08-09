using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SingletonManager : MonoBehaviour
{
    public static SingletonManager Instance { get; private set; }

    //Fields
    [SerializeField] private Transform spawnPosition;

    //UI
    [SerializeField] private Image healthBar;
    [SerializeField] private Text healthText;
    [SerializeField] private Image dangerImage;   

    private float timer = 120f;
    [SerializeField] private GameObject playerRef;
    
    private float alphaLerp;

    //Properties
    public bool GamePause { get; set; }
    public bool IsDead { get; set; }
    public Transform SpawnPosition { get => spawnPosition; set => spawnPosition = value; }
    public bool HasTravelled { get; set; }
    public int Score { get; set; }
    public string TimerString { get; private set; }
    public float Health { get; set; } = 1f;
    public bool InDanger { get; set; }
    public int Ammo { get; set; } = 100;

    public float MasterVolume { get; set; } = 1f;

    //Inventory
    public ItemData[] Inventory { get; set; } = new ItemData[8]
    {
        new ItemData(),
        new ItemData(),
        new ItemData(),
        new ItemData(),
        new ItemData(),
        new ItemData(),
        new ItemData(),
        new ItemData()
    };

    [SerializeField] private GameObject[] inventorySlots;
    [SerializeField] private GameObject inventoryMenu, selectedItemInfo;
    [SerializeField] private Text itemLabel, itemUseText;
    [SerializeField] private Image itemImage;

    public GameObject InventoryMenu { get => inventoryMenu; }
    public GameObject SelectedItemInfo { get => selectedItemInfo; }
    public int SelectedSlot { get; set; }

    //Methods
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

            Load();
        }        
    }

    private void Start()
    {
        inventoryMenu.SetActive(false);
        selectedItemInfo.SetActive(false);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        print("Loaded.");
    }

    void Update()
    {
        QuitGame();

        timer -= Time.deltaTime;

        TimerString = string.Format("{0:00}:{1:00}", Mathf.Floor(timer / 60), Mathf.Floor(timer % 60));

        //UI Update
        healthBar.fillAmount = Health;
        healthText.text = Mathf.Round(Health * 100f).ToString() + "%";

        if(InDanger)
        {
            if (alphaLerp < 1)
                alphaLerp += 2f * Time.deltaTime;

            Health -= 0.05f * Time.deltaTime;
        }
        else
        {
            if (alphaLerp > 0)
                alphaLerp -= 2f * Time.deltaTime;
        }

        dangerImage.color = LerpColor(alphaLerp);

        if(Input.GetKeyDown(KeyCode.Q))
        {
            Save();
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(@"C:\Users\s10018\Desktop\SaveData.dat");

        SaveData saveObj = new SaveData()
        {
            health = Health,

            posX = playerRef.transform.position.x,
            posY = playerRef.transform.position.y,
            posZ = playerRef.transform.position.z,
        };

        bf.Serialize(file, saveObj);
        file.Close();
    }

    public void Load()
    {

        if(File.Exists(@"C:\Users\s10018\Desktop\SaveData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(@"C:\Users\s10018\Desktop\SaveData.dat", FileMode.Open);

            SaveData loadObj = (SaveData)bf.Deserialize(file);

            Health = loadObj.health;

            playerRef.transform.position = new Vector3(loadObj.posX, loadObj.posY, loadObj.posZ);

            file.Close();
        }

    }

    void QuitGame()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            Save();
            UnityEditor.EditorApplication.isPlaying = false;
        }

    }

    public void ToggleInventory(bool toggle)
    {
        inventoryMenu.SetActive(toggle);

        if(inventoryMenu.activeSelf)
        {
            GamePause = true;
            Cursor.lockState = CursorLockMode.None;
            RefreshInventory();
        }
        else
        {
            GamePause = false;
            selectedItemInfo.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }    

    public void SetSelectedItem(int index)
    {
        SelectedSlot = index;
        
        selectedItemInfo.SetActive(true);

        itemLabel.text = Inventory[index].Name;
        itemImage.sprite = Inventory[index].Image;
        itemUseText.text = Inventory[index].Action;
    }

    public void UseItem()
    {
        switch (Inventory[SelectedSlot].Type)
        {
            #region Apple
            case ItemType.Apple:

                if (Health == 1f)
                    print("Health is at maximum.");
                else
                {
                    if (Health + 0.2f > 1f)
                        Health = 1f;
                    else
                        Health += 0.2f;

                    SetNullSlot(SelectedSlot);
                    RefreshInventory();
                }

                break;
            #endregion
            #region Banana
            case ItemType.Banana:

                if (Health == 1f)
                    print("Health is at maximum.");
                else
                {
                    if (Health + 0.05f > 1f)
                        Health = 1f;
                    else
                        Health += 0.05f;

                    if (Inventory[SelectedSlot].CurrentQuantity - 1 == 0)
                        SetNullSlot(SelectedSlot);
                    else
                        DecrementStackSlot(SelectedSlot, 1);

                    RefreshInventory();
                }

                break;
                #endregion
        }

    }

    public void RefreshInventory()
    {
        for (int i = 0; i < Inventory.Length; i++)
        {
            inventorySlots[i].GetComponent<SlotUI>().SetButton(Inventory[i]);
        }
    }

    public void SetNullSlot(int index)
    {
        Inventory[index] = new ItemData();
    }

    public void DecrementStackSlot(int index, int amtToUse)
    {
        Inventory[index].CurrentQuantity -= amtToUse;
    }    

    Color LerpColor(float alpha)
    {
        return Color.Lerp(new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), alpha);
    }


}

[Serializable]
public class SaveData
{
    public float health;
    public float posX, posY, posZ;
}





