using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public struct DamageMessage
    {
        public MonoBehaviour damager;   //공격자
        public int amount;  //데미지
        public Vector3 direction;   // 방향 : (피격자위치-공격자위치).normalized 이 방향으로 AddForce해서 밀려나게 만듬
        public Vector3 damageSource;    //공격자 position
        public bool throwing;   //투척물인지 타격물인지 체크

        public bool stopCamera; //카메라 흔들림
    }
    
    public int maxHP;   //최대체력
    public float invulnerabilityTime;   //무적지속시간
    public bool isInvulnerable { get; set; }    //무적상태인지?
    public int currentHP { get; private set; }  //현재체력
    
    public UnityEvent OnReset, OnDeath, OnReceiveDamage, OnHitWhileInvulnerable, OnBecomeVulnerable;
    
    [Tooltip("이 오브젝트가 피격될 시 아래 오브젝트에게 알려집니다.")]
    public List<MonoBehaviour> onDamageMessageReceivers;    
    //IMsgReceiver를 상속한 스크립트(ex:PlayerController.cs에서) damageable.onDamageMessageReceivers.Add(this);
    
    private float timeSinceLastDamage;  //데미지 입은지 몇초째인지 체크하기위한 변수.
    Action schedule;
    
    void Start()
    {
        ResetHP();  //HP 초기화
    }

    void ResetHP()
    {
        maxHP = GetComponent<Enemy>().maxHP;
        currentHP = maxHP;  //최대 HP로 설정
        isInvulnerable = false; //무적 false
        timeSinceLastDamage = 0;    //라스트데미지 0
        OnReset.Invoke(); //초기설정시 OnResetHP 이벤트 호출
    }

    private void Update()
    {
        if (isInvulnerable)
        {   //무적 true일때 라스트히트로부터 몇초 흘렀는지 업데이트.
            timeSinceLastDamage += Time.deltaTime;
            if (timeSinceLastDamage > invulnerabilityTime)  
            {   //무적시간 초과하면 타이머 0으로 리셋하고 bool값 false로 변경.
                timeSinceLastDamage = 0.0f;
                isInvulnerable = false;
                OnBecomeVulnerable.Invoke();    //무적해제시 실행될 이벤트 호출
            }
        }
    }

    public void ApplyDamage(DamageMessage data)
    {
        if (currentHP <= 0)
        { //이미 죽은 대상이면 return
            Debug.Log("Already Dead");
            return;
        }
        
        isInvulnerable = true;  //타격 입었기때문에 무적상태로 변경해줌.
        currentHP -= data.amount;
        
        if(currentHP<=0)
            schedule += OnDeath.Invoke; //한 객체가 사망하면서 동시에 다른 객체의 상태를 변경하려 할 때 발생하는 '경쟁 조건'을 피하고, 상태변경이 완료된 후 예약된 작업을 실행하여 게임 로직의 예기치 않은 충돌을 방지.
        else
            OnReceiveDamage.Invoke();   //피해 입었을때 이벤트 호출

        var messageType = currentHP <= 0 ? MsgType.DEAD : MsgType.DAMAGED;  //체력 0이하면 DEAD
        Debug.Log(messageType + ": 현재 체력 - " + currentHP);

        for (var i = 0; i < onDamageMessageReceivers.Count; ++i)
        {
            var receiver = onDamageMessageReceivers[i] as IMessageReceiver;
            receiver.OnReceiveMessage(messageType, this, data); //현재 상태, 죽은 객체, 데이터를 전송.
        }
    }
    
    void LateUpdate()
    {
        if (schedule != null)
        {
            schedule();
            schedule = null;
        }
    }
}

public enum MsgType
{
    DAMAGED,
    DEAD,
    RESPAWN,
    //필요 시 사용자 정의 메시지 유형 추가
}

public interface IMessageReceiver
{
    void OnReceiveMessage(MsgType type, object sender, object msg);
}