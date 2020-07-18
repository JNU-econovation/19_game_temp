﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameData;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryButton;
    public GameObject shopButton;
    public GameObject mapButton;
    public GameObject inventory;
    public GameObject arrangeButton;
    public GameObject saleButton;
    public GameObject putButton;
    public GameObject cancelButton;
    public GameObject putInvenButton;

    public GameObject farmAnimal;

    public GameObject inventoryContents;
    public GameObject farmObjects;

    public GameObject arrangeImage;

    //public static int putFarmObNumber;

    public int inventorySlotNum;

    public List<Sprite> invenIcons;
    public List<GameObject> invenItems;

    public void Awake()
    {
        Debug.Log("깨어났다!");
        for(int i=0; i < 30; i++)
        {
            FarmObject invenSlotFarmOb = inventoryContents.transform.GetChild(i).gameObject.GetComponent<FarmObject>();
            invenSlotFarmOb.farmObjectNumber = -1;
            invenSlotFarmOb.farmObjectIndex = -1;
            inventorySlotNum++;
        }
        inventorySlotNum = 0;
        foreach (FarmObject farmObject in Spawner._instance.farmObjects)
        {
            if (farmObject.isField == false)
            {
                
                Image invenSlotImage = inventoryContents.transform.GetChild(inventorySlotNum).gameObject.GetComponent<Image>();
                invenSlotImage.sprite = invenIcons[farmObject.farmObjectNumber];
                FarmObject invenSlotFarmOb = inventoryContents.transform.GetChild(inventorySlotNum).gameObject.GetComponent<FarmObject>();
                invenSlotFarmOb.farmObjectNumber = farmObject.farmObjectNumber;
                invenSlotFarmOb.farmObjectIndex = farmObject.farmObjectIndex;
                inventorySlotNum++;
            }
        }

    }

    public void AddInventory()
    {
        Image invenSlotImage = inventoryContents.transform.GetChild(inventorySlotNum).gameObject.GetComponent<Image>();
        invenSlotImage.sprite = invenIcons[InputManager.farmObjectNumber];
        FarmObject invenSlotFarmOb = inventoryContents.transform.GetChild(inventorySlotNum).gameObject.GetComponent<FarmObject>();
        invenSlotFarmOb.farmObjectNumber = InputManager.farmObjectNumber;
        invenSlotFarmOb.farmObjectIndex = InputManager.farmObjectIndex;
        inventorySlotNum++;
        DataManager._instance.ParseFarmObjectData(Spawner._instance.farmObjects);
        for (int i = 0; i < farmAnimal.transform.childCount; i++)
        {
            AnimalController animalController = farmAnimal.transform.GetChild(i).GetComponent<AnimalController>();
            animalController.pathStart();
        }
    }


    public void OpenInventory()
    {
        inventoryButton.SetActive(false);
        shopButton.SetActive(false);
        mapButton.SetActive(false);
        inventory.SetActive(true);
        putInvenButton.SetActive(false);
    }



    public void CloseInventory() 
    {
        inventoryButton.SetActive(true);
        shopButton.SetActive(true);
        mapButton.SetActive(true);
        inventory.SetActive(false);
        saleButton.SetActive(false);
        arrangeButton.SetActive(false);
    }

    public void InvenFarmObTap()
    {
        if (InputManager.farmObjectNumber != -1)
        {
            saleButton.SetActive(true);
            arrangeButton.SetActive(true);
        }
    }

    public void FarmObjectSale()
    {
        Spawner._instance.farmObjects.RemoveAt(InputManager.farmObjectIndex);
        foreach(FarmObject farmObject in Spawner._instance.farmObjects)
        {
            if (farmObject.farmObjectIndex >= InputManager.farmObjectIndex)
                farmObject.farmObjectIndex--;
        }
        for(int i=InputManager.inventorySlotNumber; i < inventorySlotNum; i++)
        {
            
            Image invenSlotImage = inventoryContents.transform.GetChild(i).gameObject.GetComponent<Image>();
            Image nextInvenSlotImage = inventoryContents.transform.GetChild(i+1).gameObject.GetComponent<Image>();
            invenSlotImage.sprite = nextInvenSlotImage.sprite;
            FarmObject invenSlotFarmOb = inventoryContents.transform.GetChild(i).gameObject.GetComponent<FarmObject>();
            FarmObject nextInvenSlotFarmOb = inventoryContents.transform.GetChild(i+1).gameObject.GetComponent<FarmObject>();
            invenSlotFarmOb.farmObjectIndex = nextInvenSlotFarmOb.farmObjectIndex;
            invenSlotFarmOb.farmObjectNumber = nextInvenSlotFarmOb.farmObjectNumber;

        }
        inventorySlotNum--;
        DataManager._instance.ParseFarmObjectData(Spawner._instance.farmObjects);
        MoneyManager.MoneyUP(10);
        saleButton.SetActive(false);
        arrangeButton.SetActive(false);
    }

    public void FarmObjectArrange()
    {
        inventory.SetActive(false);
        saleButton.SetActive(false);
        arrangeButton.SetActive(false);
        putButton.SetActive(true);
        cancelButton.SetActive(true);
        Spawner._instance.farmObjects[InputManager.farmObjectIndex].isField = true;
        Drag arrangeImageDrag = arrangeImage.GetComponent<Drag>();
        arrangeImageDrag.PItransformMid();
        Image spriteRenderer = arrangeImage.GetComponent<Image>();
        spriteRenderer.sprite = invenIcons[InputManager.farmObjectNumber];
        
    } 

    public void ArrangeOk()
    {
        int putable=0;
        Debug.Log(farmAnimal.transform.childCount);
        Debug.Log(farmObjects.transform.childCount);
        for(int i=0; i < farmAnimal.transform.childCount; i++)
        {
            if (Mathf.Abs(farmAnimal.transform.GetChild(i).transform.position.x - arrangeImage.transform.position.x) > 1 && Mathf.Abs(farmAnimal.transform.GetChild(i).transform.position.y - arrangeImage.transform.position.y) > 1)
                putable++;
        }
        for(int i = 0; i < farmObjects.transform.childCount; i++)
        {
            if (Mathf.Abs(farmObjects.transform.GetChild(i).transform.position.x - arrangeImage.transform.position.x) > 1 && Mathf.Abs(farmObjects.transform.GetChild(i).transform.position.y - arrangeImage.transform.position.y) > 1)
                putable++;
        }
        if (putable==farmAnimal.transform.childCount+farmObjects.transform.childCount)
        {
            inventory.SetActive(true);
            putButton.SetActive(false);
            cancelButton.SetActive(false);
            Spawner._instance.farmObjects[InputManager.farmObjectIndex].harvestTime = System.DateTime.Now;
            GameObject arranged = Instantiate(invenItems[InputManager.farmObjectNumber], new Vector2(arrangeImage.transform.position.x, arrangeImage.transform.position.y), Quaternion.identity);
            FarmObjectController farmObjectController = arranged.GetComponent<FarmObjectController>();
            FarmObject farmObject = arranged.GetComponent<FarmObject>();
            farmObject.harvestTime = System.DateTime.Now;
            farmObjectController.state = FarmObjectController.State.producing;
            SpriteRenderer spriteRenderer = farmObjectController.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = farmObjectController.producingSprite;
            Debug.Log("되고있나");
            arranged.transform.parent = farmObjects.transform;
            Spawner._instance.farmObjects[InputManager.farmObjectIndex].posX = arrangeImage.transform.position.x;
            Spawner._instance.farmObjects[InputManager.farmObjectIndex].posY = arrangeImage.transform.position.y;
            Spawner._instance.farmObjects[InputManager.farmObjectIndex].isField = true;

            Drag arrangeImageDrag = arrangeImage.GetComponent<Drag>();
            arrangeImageDrag.PItransformBack();
            DataManager._instance.ParseFarmObjectData(Spawner._instance.farmObjects);
            inventorySlotNum--;
            for (int i = InputManager.inventorySlotNumber; i <= inventorySlotNum; i++)
            {

                Image invenSlotImage = inventoryContents.transform.GetChild(i).gameObject.GetComponent<Image>();
                Image nextInvenSlotImage = inventoryContents.transform.GetChild(i + 1).gameObject.GetComponent<Image>();
                invenSlotImage.sprite = nextInvenSlotImage.sprite;
                FarmObject invenSlotFarmOb = inventoryContents.transform.GetChild(i).gameObject.GetComponent<FarmObject>();
                FarmObject nextInvenSlotFarmOb = inventoryContents.transform.GetChild(i + 1).gameObject.GetComponent<FarmObject>();
                invenSlotFarmOb.farmObjectIndex = nextInvenSlotFarmOb.farmObjectIndex;
                invenSlotFarmOb.farmObjectNumber = nextInvenSlotFarmOb.farmObjectNumber;

            }
            for (int i = 0; i < farmAnimal.transform.childCount; i++)
            {
                AnimalController animalController = farmAnimal.transform.GetChild(i).GetComponent<AnimalController>();
                animalController.pathStart();
            }
        }
    }

    public void ArrangeCencel()
    {
        inventory.SetActive(true);
        putButton.SetActive(false);
        cancelButton.SetActive(false);
    }
}

