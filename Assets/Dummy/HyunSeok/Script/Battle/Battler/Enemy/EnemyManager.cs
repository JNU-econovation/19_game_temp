﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;

namespace Battle
{
    public class EnemyManager : MonoBehaviour
    {
        public static EnemyManager _instance;
        public AnimalController enemy;
        public Transform enemyPos;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(gameObject);
            SpawnEnemyUseData();
        }

        public void SpawnEnemyUseData()
        {
            int enemyIndex = DataManager._instance.rigionIndex;
            GameObject enemyObj = Instantiate(Resources.Load("Battle/Enemy/Prefab_Enemy_" + enemyIndex) as GameObject);
            enemyObj.transform.position = enemyPos.transform.position;
            enemy = enemyObj.transform.GetChild(0).GetComponent<AnimalController>();
        }
    }
}