﻿using System.Collections;
using System.Collections.Generic;
using FSM;
using UnityEngine;

namespace Battle
{
    public abstract class Animal : MonoBehaviour
    {
        // Animal 능력치
        public AnimalBattleData stat;
        // FSM을 구동할 HeadMachine
        protected HeadMachine<Animal> stateControl;
        // 상태 보유 리스트
        protected IState[] states = new IState[(int) EAnimalState.END];
        // 현재 상태
        [SerializeField]
        protected EAnimalState animalState;
        public EAnimalState AnimalState { get => animalState; set => animalState = value; }
        // 타겟팅 대상
        protected Enemy target;
        /**
         *   Animal 스탯 초기화
         */
        public abstract void InitStat (AnimalStatData data);
        /**
         *   Animal FSM 초기화
         */
        protected abstract void InitFSM ();
    }

    public class AnimalBattleData
    {
        #region Stat
        [SerializeField]
        private float maxHP;
        public float MaxHP { get => maxHP; set => maxHP = value; }

        [SerializeField]
        private float hp;
        public float HP { get => hp; set => hp = value; }

        [SerializeField]
        private float atk;
        public float Atk { get => atk; set => hp = atk; }

        [SerializeField]
        private float atkSpd;
        public float AtkSpd { get => atkSpd; set => hp = atkSpd; }
        #endregion
        public AnimalBattleData (AnimalStatData animalStatData)
        {
            MaxHP = animalStatData.HP;
            HP = animalStatData.HP;
            Atk = animalStatData.Atk;
            AtkSpd = animalStatData.AtkSpd;
        }
    }
}
