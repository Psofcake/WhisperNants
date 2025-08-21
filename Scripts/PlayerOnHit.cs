using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnHit : MonoBehaviour
{
    [SerializeField] private int _hp = 100;

    public int hp
    {
        get { return _hp; }
        set
        {
            if(value >= 0)
                _hp = value;
        }
    }
    
    BoxCollider _boxCollider;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
