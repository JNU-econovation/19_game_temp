﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indigo : Animal
{
    public Sprite babyAnimalSprite;
    public Sprite growUpSprite;

    // Start is called before the first frame update
    void Start()
    {
        animalNumber = 4;
        spriteRenderer.sprite = babyAnimalSprite;
    }

    void growup()
    {
        spriteRenderer.sprite = growUpSprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
