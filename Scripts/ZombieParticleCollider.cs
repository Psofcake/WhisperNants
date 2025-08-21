using System;
using System.Collections.Generic;
using UnityEngine;

public class ZombieParticleCollider : MonoBehaviour
{
    private ParticleSystem particleSystem;

    private void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    private void OnParticleCollision(GameObject other)
    {
        PlayerOnHit player = other.GetComponent<PlayerOnHit>();
        if (player != null)
         {
             player.hp -= 10;
             Debug.Log($"{other.name}'s HP: {player.hp}");
         }
    }

    private void OnParticleTrigger()
    {
        throw new NotImplementedException();
    }
}