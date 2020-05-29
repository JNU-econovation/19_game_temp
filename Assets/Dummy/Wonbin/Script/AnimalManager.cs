﻿using Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalManager : MonoBehaviour
{

    Animal animal;

    PathFinder pathfinder = new PathFinder();
    Spawner spawner = new Spawner();
    TabbyAnimator tabbyAnimator = new TabbyAnimator();

    Rigidbody2D animalrigidbody;
    private Animator animator;

    private float speed = 4f;
    [SerializeField]
    private float heartRateMin = 5f; //최소 생성주기
    [SerializeField]
    private float heartRateMax = 7f; //최대 생성주기
    private float heartRate;

    [SerializeField]
    private GameObject heartPrefabs;
    private float timeAfterHeart;

    [SerializeField]
    GameObject heart;

    [SerializeField]
    float distance = 10;


    SpriteRenderer spriteRenderer;


    public List<Animal> animals;

   
    private void Start()
    {
        animal = gameObject.GetComponent<Animal>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        animalrigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        pathfinder.PathFindingStart(animalrigidbody, 6);
        heartRate = UnityEngine.Random.Range(heartRateMin, heartRateMax);
    }
    private void Update()
    {
        //하트가 없어진 이후로 흐른 시간 체크
        if (timeAfterHeart <= heartRate)
            timeAfterHeart += Time.deltaTime;

        //랜덤시간 이상이 됐을때 하트 생성
        if (timeAfterHeart >= heartRate && transform.childCount == 0)
        {
            spawner.MakeChild(this.gameObject, heartPrefabs);
        }
        else


            //pathFinding이 끝난 길 Node 리스트를 따라 이동, 한칸 이동 후 i++
            pathfinder.FollwingPath(animalrigidbody, 4);


        if (Input.GetKeyDown(KeyCode.R))
            tabbyAnimator.ChangeSprite(spriteRenderer, animal.GetGrowUpSprite());
        //목적지에 도달했다면 다시 길찾기
        pathfinder.ReFinding(animalrigidbody, 6);


    }


    void OnMouseDrag()
    {
        if (timeAfterHeart <= heartRate && timeAfterHeart >= 1.3)//하트획득시 동물이 들리지 않도록 조절
            Drag.AnimalDrag(animalrigidbody);
    }



    //동물을 내려 놓았을 때
    private void OnMouseUp()
    {
        //다시 랜덤지정으로 길찾기 시작
        if (timeAfterHeart <= heartRate && timeAfterHeart >= 1.3)
        {
            pathfinder.ReFinding(animalrigidbody, 6);
            //애니메이션 돌아옴
            animator.SetBool("tapAnimal", false);
        }
    }

    //동물에 하트가 있다면 하트획득
    private void OnMouseDown()
    {
        if (transform.childCount != 0)
        {
            MoneyManager.heart += 1;
            PlayerPrefs.SetInt("Heart", MoneyManager.heart);
            Destroy(heart);
            heartRate = UnityEngine.Random.Range(heartRateMin, heartRateMax);
            timeAfterHeart = 0f;
        }
    }
}
