using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum EnemyState
{
    IDLE,   //대기 -제자리
    PATROL, //순찰 -걸어다님
    CHASE,  //추격 -달리기
    ATTACK, //공격
    ATTACK_Ranged, //원거리 공격
    DEAD    //사망
}

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavAgent))]
[RequireComponent(typeof(Damageable))]
[RequireComponent(typeof(AudioSource))]
public class Enemy : MonoBehaviour, IMessageReceiver
{
    //몬스터 정보
    [SerializeField] protected string _enemyName;// 몬스터의 이름
    public string enemyName => _enemyName;  
    
    [SerializeField] protected int _maxHP;   //몬스터의 최대체력
    public int maxHP => _maxHP;

    public int atk;  // 몬스터의 공격력
    [SerializeField] protected float walkSpeed = 0.7f;   // 걷기 스피드.
    [SerializeField] protected float runSpeed = 3.5f;    //달리기 스피드.

    //상태 관련 변수
    public EnemyState state = EnemyState.PATROL;  //몬스터 상태
    public float attackDistance = 1.0f; //이 거리 안에 있으면 플레이어 공격
    public float chaseDistance = 5.0f; //이 거리 안에 있으면 플레이어 추격
    public bool isAttacking = false;
    public bool isDead = false;
    protected Damageable damageable;
    protected bool respawning;

    protected NavAgent _navAgent;
    
    // 필요한 컴포넌트
    protected Animator _animator;
    protected Rigidbody _rigidbody;
    protected CharacterHp _characterHp;
    protected AudioSource _audioSource;
    
    // 애니메이터컨트롤러의 Parameters 이름들을 해시값으로 가져오기.
    protected readonly int _moveHash = Animator.StringToHash("IsMove");
    protected readonly int _speedHash = Animator.StringToHash("Speed");
    protected readonly int _attackHash = Animator.StringToHash("Attack");
    protected readonly int _getHitHash = Animator.StringToHash("GetHit");
    protected readonly int _deadHash = Animator.StringToHash("Dead");
    
    protected Transform target; //타겟 (플레이어위치)
    protected float nextAttack = 0.0f; //다음 공격이 가능한 시간
    [SerializeField] protected float attackAnimLength = 1f; //공격애니메이션시간길이

    //오디오클립 바인딩
    [SerializeField] private AudioClip walkStepClip;
    [SerializeField] private AudioClip runStepClip;
    [SerializeField] private AudioClip screamClip;
    [SerializeField] private AudioClip swingClip;
    [SerializeField] private AudioClip swingFastClip;
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip getHitClip;
    [SerializeField] private AudioClip deathClip;
    
    private bool _getHit = false;
    protected bool getHit
    {
        get{return _getHit;}
        set
        {
            if (_getHit != value && !_characterHp.enemyDied)
            {
                _getHit = value;
                if (_getHit && _characterHp._isDecreasing)
                {
                    _animator.SetTrigger(_getHitHash);
                    PlayMonsterSound(MonsterClipType.GetHit);
                    
                }
            }
        }
    }

    protected void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _navAgent = GetComponent<NavAgent>();
        _characterHp = GetComponent<CharacterHp>();
        _audioSource = GetComponent<AudioSource>();
        damageable = GetComponent<Damageable>();
        damageable.onDamageMessageReceivers.Add(this);
        
        var player = GameObject.FindGameObjectWithTag("Player");
        if(player != null) target = player.GetComponent<Transform>();
    }

    
    protected void OnEnable()
    {
        StartCoroutine(CheckStateCoroutine());
        StartCoroutine(ActionCoroutine());
    }

    protected void Start()
    {
        _characterHp.SetHealth(_maxHP);
    }

    protected virtual IEnumerator CheckStateCoroutine()
    {
        while (!isDead)
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
                if(state==EnemyState.ATTACK) yield return new WaitForSeconds(attackAnimLength-0.2f);
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

    protected virtual IEnumerator ActionCoroutine()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(0.3f);
            switch (state)
            {
                case EnemyState.PATROL:
                    isAttacking = false;
                    _navAgent.patrollSpeed = walkSpeed;
                    _navAgent.isPatrolling = true;
                    _animator.SetBool(_moveHash, true);
                    break;
                
                case EnemyState.CHASE:
                    isAttacking = false;
                    _navAgent.chaseSpeed = runSpeed;
                    _navAgent.targetPosition = target.position;
                    _animator.SetBool(_moveHash, true);
                    break;
                
                case EnemyState.ATTACK:
                    if(isAttacking==false) isAttacking = true;
                    _navAgent.Stop();
                    _animator.SetBool(_moveHash, false);
                    break;
                
                case EnemyState.DEAD:
                    isAttacking = false;
                    isDead = true;
                    _navAgent.Stop();
                    _animator.SetBool(_moveHash, false);
                    break;
                case EnemyState.IDLE:
                default:
                    isAttacking = false;
                    _navAgent.Stop();
                    _animator.SetBool(_moveHash, false);
                    break;
            }
        }
    }
    
    // Update is called once per frame
    protected virtual void Update()
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
    }
    protected void Dead()
    {
        _animator.SetTrigger(_deadHash);
        walkSpeed = 0f;
        runSpeed = 0f;

        state = EnemyState.DEAD;
        PlayMonsterSound(MonsterClipType.Dead);

        damageable.isInvulnerable = true; //재사용시 onEnable에서 isInvulnerable=false; //respawning = true;
        
        //아이템 루팅 되고나면 사라지게끔 로직 추가작성 필요
        
        gameObject.layer = LayerMask.NameToLayer("DeadEnemies"); // ProjectSettings>physics>레이어마스크 'DeadEnemies'와 'player'의 충돌제외 필요
        Destroy(gameObject, 3f);    //3초 뒤 파괴
    }
    
    public void OnReceiveMessage(MsgType type, object sender, object data)
    {
        switch (type)
        {
            case MsgType.DAMAGED:
            {
                Damageable.DamageMessage damageData = (Damageable.DamageMessage)data;
                Damaged(damageData);
            }
                break;
            case MsgType.DEAD:
            {
                Damageable.DamageMessage damageData = (Damageable.DamageMessage)data;
                Die(damageData);
            }
                break;
        }
    }

    public void Damaged(Damageable.DamageMessage damageMessage)
    {
        _animator.SetTrigger(_getHitHash);
        PlayMonsterSound(MonsterClipType.GetHit);
    }
    
    public void Die(Damageable.DamageMessage damageMessage)
    {
        _animator.SetTrigger(_deadHash);
        walkSpeed = 0f;
        runSpeed = 0f;
        
        state = EnemyState.DEAD;
        PlayMonsterSound(MonsterClipType.Dead);
        
        damageable.isInvulnerable = true; //재사용시 onEnable에서 isInvulnerable=false; //respawning = true;
        
        //아이템 루팅 되고나면 사라지게끔 로직 추가작성 필요
        
        gameObject.layer = LayerMask.NameToLayer("DeadEnemies"); // 유니티에서 레이어 추가 및 플레이어와 충돌제외 작업 필요
        Destroy(gameObject, 3f);    //3초 뒤 파괴
    }

    public void PlayMonsterSound(MonsterClipType mClipType)
    {
        switch (mClipType)
        {
            case MonsterClipType.Walk:
                _audioSource.PlayOneShot(walkStepClip); 
                break;
            case MonsterClipType.Run:
                _audioSource.PlayOneShot(runStepClip); 
                break;
            case MonsterClipType.Scream:
                _audioSource.PlayOneShot(screamClip); 
                break;
            case MonsterClipType.Swing:
                _audioSource.PlayOneShot(swingClip); 
                break;
            case MonsterClipType.SwingFast:
                _audioSource.PlayOneShot(swingFastClip); 
                break;
            case MonsterClipType.Attack:
                _audioSource.PlayOneShot(attackClip); 
                break;
            case MonsterClipType.GetHit:
                _audioSource.PlayOneShot(deathClip); 
                break;
            case MonsterClipType.Dead:
                _audioSource.PlayOneShot(deathClip); 
                break;
        }
               
    }
}

public enum MonsterClipType
{
    Walk,
    Run,
    Scream,
    Swing,
    SwingFast,
    Attack,
    GetHit,
    Dead
}