using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class ZombieDamageField : MonoBehaviour
{
    private bool canDamage = false;
    private Collider col;   //현재 오브젝트의 콜라이더
    //private Animator bloodAnim;
    private Rigidbody rb;

    public Enemy enemy; //인스펙터창에서 직접 바인딩해주세요
    public int damageAmount;
    
    private IHealthpower _playerHp; // playerHp를 클래스 필드로 선언
    
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider>();
        //bloodAnim = GameObject.Find("Blood").GetComponent<Animator>();
        
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; //물리연산에서 제외

        if(enemy!=null) damageAmount = enemy.atk;
    }

    // Update is called once per frame
    void Update()
    {
        if (col.enabled == false)   //콜라이더가 비활성화되면 
        {
            canDamage = true;   //다음 공격을 준비.
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (canDamage == true) //공격 가능 상태일때 트리거가 들어오면
            {
                canDamage = false; //트리거가 여러번 입력되지 않도록 바로 공격 불가능으로 바꿔준다.

                if (other.gameObject.TryGetComponent(out IHealthpower hp))
                {
                    hp.UnderAttack(damageAmount);
                    //Debug.Log(other.gameObject.name + " Damaged -" + damageAmount + " , 현재체력 : " + hp.CurrentHealthPower);
                }
                //bloodAnim.SetTrigger("Blood");
                enemy.PlayMonsterSound(MonsterClipType.Attack);
            }
        }
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     Debug.Log(other.name);
    //     if (other.CompareTag("Player"))
    //     {
    //         if (canDamage == true) //공격 가능 상태일때 트리거가 들어오면
    //         {
    //             canDamage = false; //트리거가 여러번 입력되지 않도록 바로 공격 불가능으로 바꿔준다.
    //
    //             if (other.TryGetComponent(out IHealthpower hp))
    //             {
    //                 hp.UnderAttack(damageAmount);
    //             }
    //
    //             Debug.Log(other.name + " Damaged -" + damageAmount + " , 현재체력 : " + hp.CurrentHealthPower);
    //
    //             //bloodAnim.SetTrigger("Blood");
    //
    //         }
    //     }
    // }
}
