using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Info : MonoBehaviour
{
    private int _health = 100;
    public int health
    {
        get { return _health; }
        set
        {
            _health = value; 
            if (_health > 100) _health = 100;
            if (_health < 0) _health = 0;   //사망
        }
    }
    
    private int _stamina = 100; 
    public int stamina
    {
        get { return _stamina; }
        set
        {
            _stamina = value;
            if (_stamina > 100) _stamina = 100;
            if (_stamina < 0) _stamina = 0; //달리기,슬라이딩 안됨. 벽타기는 가능.
        }
    }
    
    private int _infection = 0;
    public int infection
    {
        get { return _infection; }
        set
        {
            _infection = value; 
            if (_infection < 0) _infection = 0; //사망
            if (_infection > 100) _infection = 100;
        }
    }

    private int _strength;  //최대체력
    private int _attack;    //기본공격력:주먹(+무기=최종공격력)
    private int _defense;   //방어력(기본:0, 방어구 장착하여 올림)
    private int _recovery;   //초당 스태미너 회복력
    private int _speed;     //이동속도
    public int lemon { get;set; }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
