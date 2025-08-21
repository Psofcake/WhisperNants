using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class RangedEnemy : Enemy
{
    [SerializeField] private float range = 8;
    [SerializeField] private float rangedAnimLength = 2.2f;
    [SerializeField] private bool isSpitting = false;
    protected readonly int _rangedHash = Animator.StringToHash("Attack_Ranged");
    
    public GameObject prefab;  // spittle 프리팹 바인딩하기
    public Transform spawnPoint;    //  spittle 프리팹 스폰 위치
    
    protected override void Update()
    {
        _animator.SetFloat(_speedHash,_navAgent.moveSpeed);
        
        if (_characterHp.enemyDied && state!=EnemyState.DEAD)
        {
            Dead();
        }
        
        // isDecreasing이 변경될 때만 상태를 갱신
        if (_characterHp._isDecreasing && !getHit)
        {
            getHit = true;  // getHit를 true로 설정
        }
        else if (!_characterHp._isDecreasing && getHit)
        {
            getHit = false;  // isDecreasing이 false로 변경되면 false로 설정
        }

        if (isAttacking)
        {
            if (Time.time >= nextAttack)
            {
                _animator.SetTrigger(_attackHash);
                nextAttack = Time.time + attackAnimLength+Random.Range(0.0f,0.3f);
                //animLength와 0.0f에서 0.3f 사이의 랜덤 값을 더하여 공격 간격에 약간의 변화를 줍니다.
            }
            //공격중일때는 항상 타겟 방향을 바라보도록 함.
            Quaternion rot = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
        }
        if (isSpitting)
        {
            if (Time.time >= nextAttack)
            {
                StartCoroutine(LookTarget());
                _animator.SetTrigger(_rangedHash);//원거리공격 애니메이션 재생
                //Spawn();
                
                nextAttack = Time.time + rangedAnimLength+Random.Range(0.0f,0.3f);
                //animLength와 0.0f에서 0.3f 사이의 랜덤 값을 더하여 공격 간격에 약간의 변화를 줍니다.
            }
        }
    }
    
    public void Spawn()
    {
        if (prefab != null)
        {
            // 스폰포인트 위치에서 오브젝트 생성
            GameObject spittle = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
            spittle.transform.SetParent(this.transform, true);  // 월드 좌표를 유지
        }
    }

    private IEnumerator LookTarget()
    {
        float time = Time.time;
        while (Time.time < time + 0.3f)
        {
            //원거리 공격 시작 시에만 타겟 방향을 바라보도록 함.
            Quaternion rot = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
            yield return null;
        }
    }
    
    protected override IEnumerator CheckStateCoroutine()
    {
        while (!_characterHp.enemyDied)
        {
            if(state == EnemyState.DEAD) yield break;
            
            //거리 계산 (몬스터의 위치와 타겟위치간의 거리)
            float distance = Vector3.Distance(transform.position, target.position);
            
            if (distance <= attackDistance) //attackD보다 가까우면 공격시작
            {
                state = EnemyState.ATTACK;
            } 
            else if (distance <= chaseDistance) //chaseD보다 가까우면 추격시작
            {
                if(state==EnemyState.ATTACK || state==EnemyState.ATTACK_Ranged) yield return new WaitForSeconds(attackAnimLength-0.2f);
                state = EnemyState.CHASE;
            }
            else if (distance <= range)
            {
                state = EnemyState.ATTACK_Ranged;
            }
            else if (distance <= range + 2)
            {
                if(state==EnemyState.PATROL || state==EnemyState.IDLE) PlayMonsterSound(MonsterClipType.Scream);
                state = EnemyState.CHASE;
            }
            else    //해당 없으면 순찰돌기
            {
                if (state == EnemyState.CHASE)
                {
                    state = EnemyState.IDLE;
                    yield return new WaitForSeconds(1f);
                }
                if (_navAgent.waypoints.Count <= 1)
                    state = EnemyState.IDLE;
                else state = EnemyState.PATROL;
            }
            
            yield return new WaitForSeconds(0.3f);
        }
    }

    protected override IEnumerator ActionCoroutine()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(0.3f);
            switch (state)
            {
                case EnemyState.PATROL:
                    isAttacking = false;
                    isSpitting = false;
                    _navAgent.patrollSpeed = walkSpeed;
                    _navAgent.isPatrolling = true;
                    _animator.SetBool(_moveHash, true);
                    break;
                
                case EnemyState.CHASE:
                    isAttacking = false;
                    isSpitting = false;
                    _navAgent.chaseSpeed = runSpeed;
                    _navAgent.targetPosition = target.position;
                    _animator.SetBool(_moveHash, true);
                    break;
                
                case EnemyState.ATTACK:
                    if(isAttacking==false) isAttacking = true;
                    isSpitting = false;
                    _navAgent.Stop();
                    _animator.SetBool(_moveHash, false);
                    break;
                
                case EnemyState.ATTACK_Ranged:
                    isAttacking = false;
                    if(isSpitting==false) isSpitting = true;
                    _navAgent.Stop();
                    _animator.SetBool(_moveHash, false);
                    break;
                
                case EnemyState.DEAD:
                    isAttacking = false;
                    isSpitting = false;
                    isDead = true;
                    _navAgent.Stop();
                    _animator.SetBool(_moveHash, false);
                    break;
                case EnemyState.IDLE:
                default:
                    isAttacking = false;
                    isSpitting = false;
                    _navAgent.Stop();
                    _animator.SetBool(_moveHash, false);
                    
                    break;
            }
        }
    }
}
