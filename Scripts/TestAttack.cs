using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAttack : MonoBehaviour
{
    private bool isAttacking = false;
    private int damage = 10;
    private RaycastHit hitInfo;
    private float range = 100f;

    private void Update()
    {
        TryAttack();
    }

    private void TryAttack()
    {
        if (Input.GetButtonDown("Attack01"))
        {
            if (!isAttacking)
            {
                StartCoroutine(AttackCoroutine());
            }
        }
    }

    IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        StartCoroutine(HitCoroutine());
        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

    IEnumerator HitCoroutine()
    {
        while (isAttacking)
        {
            if (CheckObject())
            {
                Debug.Log("Hit");
                isAttacking = false;
                
                //추가 기능 구현해야 할 부분
                if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("DamageField"))  //만약 타격대상이 데미지필드일 경우
                {
                    Debug.Log("Hit a Box: " + hitInfo.collider.name + " with damage: " + damage*2);
                    DamageToEnemy(damage*2);
                }
                else if(hitInfo.collider.CompareTag("Enemy"))   //그냥 적을 맞춘 경우는 hp만 깎는다.
                {
                    Debug.Log("Hit an enemy: " + hitInfo.collider.name + " with damage: " + damage);
                    DamageToEnemy(damage);
                }
            }
            yield return null;
        }
    }

    private bool CheckObject()
    {
        Vector3 origin = transform.parent.position;
        Vector3 direction = transform.forward;

        //레이캐스트가 충돌하면 true
        if (Physics.Raycast(origin, direction, out hitInfo, range))
        {
            Debug.DrawLine(origin, hitInfo.point, Color.magenta);
            return true;
        }
        Debug.Log("No Hit");
        //대상이 없을 경우 false
        Debug.DrawLine(origin, origin + direction * range, Color.green);
        return false;
    }
    
    void DamageToEnemy(int damageAmount)
    {
        Damageable.DamageMessage message = new Damageable.DamageMessage
        {
            amount = damageAmount,  //데미지양
            damageSource = transform.position,  //폭발 위치
            damager = this,
            stopCamera = false,
            throwing = false
        };

            Damageable d = hitInfo.collider.GetComponent<Damageable>();

            if (d != null)
                d.ApplyDamage(message);
        
    }
    
    
}
