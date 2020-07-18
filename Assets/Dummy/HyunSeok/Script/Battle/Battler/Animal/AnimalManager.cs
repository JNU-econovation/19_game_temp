﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;

namespace Battle
{
    public class AnimalManager : MonoBehaviour
    {
        public static AnimalManager _instance;
        public List<AnimalController> animals;
        public List<Transform> animalPos;

        public GameObject damagePrefab;

        private int endNumber;
        private int maxEndNumber = 3;
        public int EndNumber
        {
            get => endNumber;
            set
            {
                if (value == maxEndNumber && BattleManager._instance.battleState == BattleDefine.EBattleState.Playing)
                {
                    // 전투 끝내기
                    EndAnimals();
                    EnemyManager._instance.EndAnimals();
                    BattleManager._instance.isWin = false;
                    StartCoroutine(BattleManager._instance.BattleOverState());
                }
                else
                {
                    endNumber = value;
                }
            }
        }

        public List<int> tempRandomAnimalIndexes;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(gameObject);
            animals = new List<AnimalController>(3);
            SpawnAnimalUseData();
        }

        public void StartAnimals()
        {
            foreach(AnimalController animal in animals)
            {
                if (animal != null)
                {
                    animal.SetForceState(BattleDefine.EBattlerState.Idle);
                }
            }
        }

        public void EndAnimals()
        {
            foreach (AnimalController animal in animals)
            {
                if (animal != null)
                {
                    if (animal.state != BattleDefine.EBattlerState.Down)
                        animal.SetForceState(BattleDefine.EBattlerState.Ready);
                }
            }
        }


        // 
        public void SpawnAnimalUseData()
        {
            if (false)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (tempRandomAnimalIndexes[i] == -1)
                    {
                        animals.Add(null);
                        continue;
                    }
                    DataManager._instance.gogoAnimalIndexes[i] = tempRandomAnimalIndexes[i];
                    GameObject animal = Instantiate(Resources.Load("Battle/Animal/Prefab_Animal_" + tempRandomAnimalIndexes[i]) as GameObject);
                    if (animal == null)
                    {
                        animals.Add(null);
                        continue;
                    }
                    animals.Add(animal.transform.GetChild(0).GetComponent<AnimalController>());
                    animals[i].animalData.BattleIndex = i;
                    animal.transform.position = animalPos[i].transform.position;
                }
            }
            else
            { 
                for (int i = 0; i < 3; i++)
                {
                    int animalIndex = DataManager._instance.gogoAnimalIndexes[i];
                    // 만약 -1 일 경우 소환 안함
                    if (animalIndex == -1)
                    {
                        maxEndNumber--;
                        animals.Add(null);
                        continue;
                    }
                    Debug.Log("Animal index : " + animalIndex);
                    Debug.Log("What " + DataManager._instance.playerData.animalDatas[animalIndex]);
                    int realAnimalIndex = DataManager._instance.playerData.animalDatas[animalIndex].index;
                    Debug.Log("Real : " + realAnimalIndex);
                    GameObject animal = Instantiate(Resources.Load("Battle/Animal/Prefab_Animal_" + realAnimalIndex) as GameObject);
                    if (animal == null)
                    {
                        animals.Add(null);
                        continue;
                    }
                    animals.Add(animal.transform.GetChild(0).GetComponent<AnimalController>());
                    animals[i].animalData.BattleIndex = i;
                    animal.transform.position = animalPos[i].transform.position;
                    if (realAnimalIndex == 0)
                    {
                        animal.transform.position = animalPos[i].transform.position + new Vector3(0, 1f, 0);
                    }
                }
            }
        }
    }
}

