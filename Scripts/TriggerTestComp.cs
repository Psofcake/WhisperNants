using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTestComp : MonoBehaviour
{
    public MeleeWeapon _Weapon;

    private void OnTriggerEnter(Collider other)
    {
        //_Weapon.OnTriggerEnterOnChild(other);
    }
}
