﻿using BattleDummy;
using GameData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AnimalController : MonoBehaviour
{

    Animal animal;

    PathFinder pathfinder;
    Spawner spawner;
    TabbyAnimator tabbyAnimator;

    Rigidbody2D animalrigidbody;
    private Animator animator;

    private int totalEXP;

    
    private float speed = 1.5f;
    private float heartRateMin = 1f; //최소 생성주기
    private float heartRateMax = 2f; //최대 생성주기
    private float heartRate;

    [SerializeField]
    private GameObject heartPrefabs;
    private float timeAfterHeart;

    [SerializeField]
    float distance = 10;


    SpriteRenderer spriteRenderer;

    public GameObject farmObjectShop;




    private void Start()
    {
        pathfinder = new PathFinder();
        spawner = Spawner._instance;
        animal = gameObject.GetComponent<Animal>();
        totalEXP = animal.exp;
        Debug.Log("현재경험치 " + totalEXP);
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        animalrigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        pathfinder.PathFindingStart(animalrigidbody, 6);
        heartRate = UnityEngine.Random.Range(heartRateMin, heartRateMax);
        //Spawner._instance.animals[gameObject.transform.GetSiblingIndex()-1].animalIndex=gameObject.transform.GetSiblingIndex()-1;

        Debug.Log("진화경험치" + animal.evolExp);
        if (totalEXP >= animal.evolExp)
        {
            Spawner spawner = gameObject.transform.parent.GetComponent<Spawner>();
            spawner.Evolution(gameObject, spawner);
        }

    }

    public void Awake()
    {
        /*Debug.Log("진화경험치" + animal.evolExp);
        if (totalEXP >= animal.evolExp)
        {
            Spawner spawner = gameObject.transform.parent.GetComponent<Spawner>();
            spawner.Evolution(gameObject, spawner);
        }*/
    }
    private void Update()
    {
        //하트가 없어진 이후로 흐른 시간 체크
        if (timeAfterHeart <= heartRate)
            timeAfterHeart += Time.deltaTime;

        //랜덤시간 이상이 됐을때 하트 생성
        else if (timeAfterHeart >= heartRate && transform.childCount == 0)
        {
            spawner.MakeChild(this.gameObject, heartPrefabs);
            //하트 보기싫으니까 일단 없애둠
        }


        //pathFinding이 끝난 길 Node 리스트를 따라 이동, 한칸 이동 후 i++
        pathfinder.FollwingPath(animalrigidbody, speed);


        
        //목적지에 도달했다면 다시 길찾기
        pathfinder.ReFinding(animalrigidbody, 6);

        
    }

    void EXPUP(int argexp)
    {
        totalEXP += argexp;
        Spawner._instance.animals[gameObject.transform.GetSiblingIndex()].exp += argexp;
        DataManager._instance.ParseAnimalDate(Spawner._instance.animals);

    }


    public void pathStart()
    {
        pathfinder.PathFindingStart(animalrigidbody, 6);
    }//뜯어서 Animals에 붙이기





    void OnMouseDrag()
    {
        if (timeAfterHeart <= heartRate && timeAfterHeart >= 0.5)//하트획득시 동물이 들리지 않도록 조절
            Drag.AnimalDrag(animalrigidbody);
        
    }



    //동물을 내려 놓았을 때
    private void OnMouseUp()
    {
        
        //다시 랜덤지정으로 길찾기 시작
        if (timeAfterHeart <= heartRate && timeAfterHeart >= 1.3)
        {
            pathfinder.DropAnimal(animalrigidbody, 6);
            //pathfinder.ReFinding(animalrigidbody, 6);
            //애니메이션 돌아옴
            //animator.SetBool("tapAnimal", false);
        }
    }



    //동물에 하트가 있다면 하트획득
    private void OnMouseDown()
    {
        
        if (transform.childCount != 0)
        {
            MoneyManager.heart += 1;
            PlayerPrefs.SetInt("Heart", MoneyManager.heart);
            Destroy(gameObject.transform.GetChild(0).gameObject);
            heartRate = UnityEngine.Random.Range(heartRateMin, heartRateMax);
            timeAfterHeart = 0f;
        }
    }
}
