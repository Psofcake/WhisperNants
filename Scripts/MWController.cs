using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public enum MeleeState  //근거리 무기 사용시 타이밍 조절을 위한 상태
{
    IDLE,   //대기 상태
    READY,  //애니메이션 시작 및 준비동작
    ATTACK, //딜이 들어가는 타이밍
    READY_COMBO,    //콤보동작 시작
    ATTACK_COMBO,   //공격 타이밍
    DELAY   //후딜
}
// 회피/점프 애니메이션 실행 시 meleeState=None으로 만들면 후딜 캔슬 가능할 것으로 예상됨.

public class MWController : MonoBehaviour
{
    [SerializeField] private List<MeleeWeapon> MeleeWeapons;
    public MeleeWeapon CurrentMWeapon;
    [SerializeField] private MeleeState _meleeState;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private GameObject hitDamageUI;
    
    private RaycastHit hitInfo;
    private int _slotNumber;
    
    private Vector3 previousPosition;
    private float speed;

    private void Start()
    {
        previousPosition = transform.position;  // 초기 위치 설정
        if (MeleeWeapons.Count == 0)
        {
            new List<MeleeWeapon>();
            // 현재 오브젝트의 직접 자식들만 검색
            foreach (Transform child in transform)
            {
                // 자식 오브젝트에서 MeleeWeapon 컴포넌트를 찾기
                MeleeWeapon weapon = child.GetComponent<MeleeWeapon>();
                if (weapon != null)
                {
                    MeleeWeapons.Add(weapon);
                }
            }
            // 현재 오브젝트의 직접 자식 오브젝트에 접근
            // for (int i = 0; i < transform.childCount; i++)
            // {
            //     Transform child = transform.GetChild(i);
            //     MeleeWeapons.Add(child.GetComponent<MeleeWeapon>());
            // }
        }
        /*// 리스트의 내용을 출력 (디버깅용)
        foreach (var weapon in MeleeWeapons)
        {
            weapon.gameObject.SetActive(false);
            Debug.Log(weapon.name);
        }*/
        
        /*
        if(!CurrentMWeapon) CurrentMWeapon = MeleeWeapons[0];
        CurrentMWeapon.gameObject.SetActive(true);*/
    }

    private void OnEnable()
    {
        Start();
        EquippedItemUi.Instance.UpdateEquippedItemUI();
        
        Item_SO item = EquippedItemUi.Instance.BeforeWeaponItem;
        if (item != null)
        {
            Debug.Log(item.data.ItemName);    
        }
        if (item is EquipItem_Weapon weapon)
        {
            if (weapon.data.ItemCode.Equals(((int)EnumItemCode.Knife).ToString()))
            {
                EquippedItemUi.Instance.EquipWeaponItem();
            }
            else if (weapon.data.ItemCode.Equals(((int)EnumItemCode.Axe).ToString()))
            {
                EquippedItemUi.Instance.EquipWeaponItem();
            }
            else if (weapon.data.ItemCode.Equals(((int)EnumItemCode.Bat).ToString()))
            {
                EquippedItemUi.Instance.EquipWeaponItem();
            }
            else
            {
                Debug.Log("Error. not MW");
            }
        }
    }

    public void WeaponSelect() //차후에 InputManager같은 별도 클래스에서 관리
    {
        int slotNumber = _slotNumber;
        if(Input.GetKeyDown(KeyCode.Alpha1))
            slotNumber = 0;
        if(Input.GetKeyDown(KeyCode.Alpha2))
            slotNumber = 1;
        if(Input.GetKeyDown(KeyCode.Alpha3))
            slotNumber = 2;
        if(Input.GetKeyDown(KeyCode.Alpha4))
            slotNumber = 3;

        if (_slotNumber != slotNumber)
        {
            _slotNumber = slotNumber;
            WeaponChange(MeleeWeapons[_slotNumber]);
        }
            
    }
    public void WeaponChange(MeleeWeapon _weapon)
    {
        if (_weapon == null)
        {
            Debug.LogWarning("Attempted to change to a null weapon.");
            return;
        }
        
        if(CurrentMWeapon!=null)
            CurrentMWeapon.gameObject.SetActive(false);

        CurrentMWeapon = _weapon;
        CurrentMWeapon.gameObject.SetActive(true);
    }
    
    
    
    // Update is called once per frame
    void Update()
    {
        if (EquippedItemUi.Instance.UseConsumable)
        {
            return;
        }
        if (EquippedItemUi.Instance.CurrentWeaponItemCode == (int)EnumItemCode.Knife ||
            EquippedItemUi.Instance.CurrentWeaponItemCode == (int)EnumItemCode.Axe ||
            EquippedItemUi.Instance.CurrentWeaponItemCode == (int)EnumItemCode.Bat)
        {
            TryAttack();    
        }

        /*WeaponSelect();*/
        
    }

    private void TryAttack()
    {
        if (!CurrentMWeapon)
        {
            Debug.Log("currentWeapon is null");
            return;
        }

        if (!FindFirstObjectByType<MeleeWeapon>().gameObject)
        {
            Debug.Log("MeleeWeapon gameObject is not found");
            return;
        }
        if (Input.GetButtonDown("Attack01")) //우클릭
        {
            if (UiManager.Instance.Player.GetComponent<CursorState>().IsMouseVisible())
            {
                return;
            }
            CurrentMWeapon.animator.SetFloat("AttackSpeed",CurrentMWeapon.attackSpeed);
            if (_meleeState == MeleeState.IDLE)
            {
                StartCoroutine(AttackCoroutine());
            }
        }
    }

    IEnumerator AttackCoroutine() //공격 코루틴
    {
        //준비
        _meleeState = MeleeState.READY;
        CurrentMWeapon.animator.Play("Attack(1)");  //애니메이션 실행
        yield return new WaitForSeconds(CurrentMWeapon.startOfAttack/CurrentMWeapon.attackSpeed);   //공격타이밍 대기
        
        //공격
        _meleeState = MeleeState.ATTACK;
        StartCoroutine(HitCoroutine()); //타격 코루틴 실행
        yield return new WaitForSeconds(CurrentMWeapon.endOfAttack/CurrentMWeapon.attackSpeed); //일정시간동안만 타격가능
        
        //후딜, 콤보가능시간
        if(_meleeState==MeleeState.ATTACK) _meleeState = MeleeState.READY_COMBO;
        StartCoroutine(TryComboCoroutine());   //콤보 코루틴 실행
        yield return new WaitForSeconds((CurrentMWeapon.attackDelay-CurrentMWeapon.startOfAttack-CurrentMWeapon.endOfAttack)/CurrentMWeapon.attackSpeed);
        
        //종료
        if(_meleeState==MeleeState.READY_COMBO) _meleeState = MeleeState.IDLE;
    }
    IEnumerator HitCoroutine()
    {   
        if(CurrentMWeapon.weaponName=="Knife")
            AudioManager.Instance.PlaySwingKnifeSound();
        else
            AudioManager.Instance.PlaySwingSound();
        while (_meleeState == MeleeState.ATTACK) //어택 상태일때 타격대상이 있는지 체크
        {
            if (CheckObject())  //대상이 있으면
            {
                _meleeState = MeleeState.READY_COMBO; //바로 상태를 바꾸어주어 같은 대상을 여러번 타격하지 않도록 방지해준다.(단일타격)
                //Debug.Log("Hit:"+hitInfo.transform.name);
                
                //추가 기능 구현해야 할 부분
                if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("DamageField"))  //만약 타격대상이 데미지필드일 경우
                {
                    Debug.Log("Hit a Box: " + hitInfo.collider.name + " with damage - " + CurrentMWeapon.attackDamage*2);
                    DamageToEnemy(CurrentMWeapon.attackDamage*2);
                    //대상에게 상태이상을 걸거나
                    StartCoroutine(ComboCoroutine()); //콤보공격코루틴을 실행하여 반격을 수행한다.
                }
                else if(hitInfo.collider.CompareTag("Enemy"))   //그냥 적을 맞춘 경우는 hp만 깎는다.
                {
                    CharacterHp character = hitInfo.collider.GetComponentInParent<CharacterHp>();
                    
                    int damageAmount = CurrentMWeapon.attackDamage;
                    Debug.Log("근거리 타격함");
                    //적의 전방방향과 나의 전방방향이 30도이상 차이나지 않는다면 내가 적의 후방에 있다고 볼 수 있다.
                    if (Vector3.Angle(hitInfo.collider.transform.forward, transform.forward) < 30) 
                    {
                        Debug.Log("후방 공격함");
                        damageAmount *= 2;
                    }
                    
                    character.SetDamage(damageAmount); // 체력 감소 메서드 호출
                    Instantiate(hitEffect, hitInfo.point, Quaternion.identity);
                    if (CurrentMWeapon.weaponName == "Fist") AudioManager.Instance.PlayPunchSound();
                    else if (CurrentMWeapon.weaponName == "Knife") AudioManager.Instance.PlayKnifeSound();
                    else AudioManager.Instance.PlayAxeSound();
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
        if (Physics.Raycast(origin, direction, out hitInfo, CurrentMWeapon.range))
        {
            Debug.DrawLine(origin, hitInfo.point, Color.magenta);
            return true;
        }

        //대상이 없을 경우 false
        Debug.DrawLine(origin, origin + direction * CurrentMWeapon.range, Color.green);
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

    IEnumerator TryComboCoroutine()
    {
        while (_meleeState == MeleeState.READY_COMBO) //콤보가능 상태에서
        {
            if (Input.GetButtonDown("Attack01"))   //공격버튼 입력시 작동
            {
                StartCoroutine(ComboCoroutine());
            }
            yield return null;
        }
    }

    IEnumerator ComboCoroutine()
    {
        _meleeState = MeleeState.ATTACK_COMBO;
        CurrentMWeapon.animator.SetTrigger("Attack"); //콤보 애니메이션 실행
        yield return new WaitForSeconds(CurrentMWeapon.startCombo / CurrentMWeapon.attackSpeed);

        StartCoroutine(ComboAttackCoroutine());
        yield return new WaitForSeconds(CurrentMWeapon.endOfAttack / CurrentMWeapon.attackSpeed);

        _meleeState = MeleeState.DELAY; //후딜
        yield return new WaitForSeconds(
            (CurrentMWeapon.attackDelay - CurrentMWeapon.startCombo - CurrentMWeapon.endOfAttack) /
            CurrentMWeapon.attackSpeed);

        _meleeState = MeleeState.IDLE;
    }


    IEnumerator ComboAttackCoroutine()
    {   
        if(CurrentMWeapon.weaponName=="Knife")
            AudioManager.Instance.PlaySwingKnifeSound();
        else
            AudioManager.Instance.PlaySwingSound();
        
        while (_meleeState == MeleeState.ATTACK_COMBO)
        {
            if (CheckObject())
            {
                _meleeState = MeleeState.DELAY;
                Debug.Log("Hit:"+hitInfo.transform.name);
                
                if(hitInfo.collider.CompareTag("Enemy"))
                {
                    Debug.Log("ComboHit an enemy: " + hitInfo.collider.name + " with damage - "+CurrentMWeapon.attackDamage*2);
                    //DamageToEnemy(CurrentMWeapon.attackDamage*2);
                    
                    CharacterHp character = hitInfo.collider.GetComponentInParent<CharacterHp>();
                    character.SetDamage(CurrentMWeapon.attackDamage*2); // 체력 감소 메서드 호출
                    Instantiate(hitEffect, hitInfo.point, Quaternion.identity);
                    AudioManager.Instance.PlayKnifeSound();
                }
            }
            yield return null;
        }
    }

    
}
