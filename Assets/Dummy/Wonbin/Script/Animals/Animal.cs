﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;

//[System.Serializable]
public class Animal : MonoBehaviour
{
    public int animalNumber;

    public int animalIdx;

    protected int animalHP ;
    public int animalCost;

    public int level;
    public int exp;


    public Sprite animalSprite;



}
//유니티는 왠만하면 스크립트엔 클래스 하나만 하자
