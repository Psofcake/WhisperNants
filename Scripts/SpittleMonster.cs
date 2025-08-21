using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using UnityEngine;
using UnityEngine.Serialization;

public class SpittleMonster : MonoBehaviour
{
    public GameObject prefab;  // 독침 프리팹 바인딩하기
    public Transform spawnPoint;
    private Transform player; // 플레이어 transform
    private Vector3 _targetPosition;

    public float throwInterval = 10f;  // 10초마다 던지기
    private float timeSinceLastThrow;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        _targetPosition = player.position;
        timeSinceLastThrow = throwInterval;  // 던질 준비 완료
    }

    void Update()
    {
        // timeSinceLastThrow += Time.deltaTime;
        //
        // // 10초마다 큐브 던지기
        // if (timeSinceLastThrow >= throwInterval)
        // {
        //     timeSinceLastThrow = 0f;  // 타이머 리셋
        //     Debug.Log("Spit Poison");
        //     Spawn();
        // }
    }

    void Spawn()
    {
        if (prefab != null && player != null)
        {
            _targetPosition = player.position; //플레이어를 향하도록 타겟 위치를 갱신 

            // 스폰포인트 위치에서 오브젝트 생성
            GameObject spittle = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
            spittle.transform.SetParent(this.transform, false);  // 월드 좌표를 유지
            
            // 큐브에 힘을 가해 던지기 (예: Rigidbody 사용)
            // Rigidbody rb = spittle.GetComponent<Rigidbody>();
            // if (rb != null)
            // {
            //     rb.velocity = throwDirection * 10f;  // 10f는 던지는 속도
            // }
        }
    }
}

