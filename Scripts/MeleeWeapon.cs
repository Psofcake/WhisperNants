using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MeleeWeapon : MonoBehaviour
{
    public ItemSelector itemSelector;
    public string weaponName;

    public EquipItem_Weapon Weapon { get; private set; }
    
    //스크립터블 대체
    public int durability = 100; //내구도
    public float range = 3; //사거리
    public int attackDamage = 10; //공격력
    public float attackSpeed = 1; //공격속도
    public float damageReduction; // 입는 피해 감소량
    //스크립터블 대체
    
    
    //
    public int staminaConsumption; // 스태미나 소모량
    
    public float attackDelay =1;   //공격시작~후딜 종료시점
    public float startOfAttack = 0.4f; //공격 시작시점
    public float endOfAttack = 0.2f;   //공격 종료시점
    public float startCombo = 0.3f;
    
    public Animator animator;
    //public Collider collider;
    //public Collider otherCollider;
    

    private void Awake()
    {
        animator = GetComponent<Animator>();
        itemSelector = GetComponent<ItemSelector>();
        
    }
    
    public void SettingWeaponData(EquipItem_Weapon weaponData)
    {
        Weapon = weaponData;
        weaponName = Weapon.data.ItemName;
    }

    // public void OnTriggerEnterOnChild(Collider other)
    // {
    //     otherCollider = other;
    // }
}
