using UnityEngine;

public class RaycastVisualizer : MonoBehaviour
{
    public float rayDistance = 10f;
    
    void Update()
    {
        // 레이캐스트를 발사할 위치
        Vector3 origin = transform.position;
        
        // 레이캐스트의 방향
        Vector3 direction = transform.forward;

        // 레이캐스트 발사
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, rayDistance))
        {
            // 충돌이 발생했을 때, 충돌 지점까지 선을 그립니다.
            Debug.DrawLine(origin, hit.point, Color.red);
        }
        else
        {
            // 충돌이 발생하지 않았을 때, 최대 거리까지 선을 그립니다.
            Debug.DrawLine(origin, origin + direction * rayDistance, Color.green);
        }
    }
}