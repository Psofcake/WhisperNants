using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

[RequireComponent(typeof(NavMeshAgent))]
public class NavAgent : MonoBehaviour
{
    //순찰지점 저장
    public List<Transform> waypoints;
    public int nextIndex;
    private NavMeshAgent agent; //using UnityEngine.AI; 네임스페이스 추가 필수

    private float _patrollSpeed;
    public float patrollSpeed { get { return _patrollSpeed; } set { _patrollSpeed = value; } }
    private float _chaseSpeed;
    public float chaseSpeed { get { return _chaseSpeed; } set { _chaseSpeed = value; } }

    public float moveSpeed
    {
        get {return agent.velocity.magnitude;}  //설정된 이동속도값이 아니라 현재의 속도값을 리턴해야함.
    }
    
    //~~~~순찰돌기~~~~//
    private bool _isPatrolling;
    public bool isPatrolling    //값이 변경될때의 agent 로직을 지정하기 위해 프로퍼티로 정의.
    {
        get { return _isPatrolling; }
        set //값이 변경되면 이하 실행
        {
            _isPatrolling = value;
            if (_isPatrolling)  //_isPatrolling이 true이면 agent의 이동속도가 변경된다.
            {
                agent.speed = _patrollSpeed;
                MoveWayPoint(); //웨이포인트를 돌며 이동
            }
        }
    }
    
    private void MoveWayPoint()
    {
        if (agent.isPathStale) return;
        //isPathStale=true일 경우 : 현재 경로가 유효하지 않음.
        //경로를 계산한 뒤에 환경이 변경된 경우, 또는 목표위치가 변경되는 경우 등.
        //새로운 경로 계산(업데이트:agent.SetDestination())이 필요하거나 아직 최단거리 경로계산이 끝나지 않았음을 의미.
        
        if (nextIndex >= 0 && nextIndex < waypoints.Count) { //다음 목적지를 지정
            agent.SetDestination(waypoints[nextIndex].position);
            agent.isStopped = false;    //내비게이션 기능 활성화
        } else {
            Debug.LogError("nextIndex 값이 범위를 벗어났습니다.");
            Debug.Log(transform.position);
        }
    }
    
    //~~~~추격하기~~~~//
    private Vector3 _targetPosition; //목표물 위치
    public Vector3 targetPosition
    {
        get { return _targetPosition; }
        set
        {
            _targetPosition = value;
            agent.speed = _chaseSpeed;
            ChaseTarget(_targetPosition); //목표물을 쫓아 이동
        }
    }

    void ChaseTarget(Vector3 target)
    {
        if (agent.isPathStale) return;
        agent.SetDestination(target);
        agent.isStopped = false;
    }

    public void Stop() //이동 정지
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        _isPatrolling = false;
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
	// Use this for initialization
	void Start () {
        //~~agent 세팅하기~~
        agent.autoBraking = true; 
        //- autoBraking=True:목적지에 가까워지면 속도가 느려짐, 다시 출발하면 서서히 속도가 빨라짐.
        //- autoBraking=False:균일한 속도로 이동
        agent.speed = _patrollSpeed; //이동속도 설정

        if (waypoints == null)
        {
            waypoints = new List<Transform>();
            waypoints.Add(transform);
            Debug.Log("Current object's transform added to waypoints list: " + transform.position);
            
        }

        if (waypoints.Count == 0)
        {
            waypoints.Add(transform);
            
            //~~순찰 경유지 설정하기~~
            // var group = GameObject.Find("WayPointGroup"); //이름이 WayPointGroup인 게임오브젝트를 탐색.
            // if (group != null)
            // {
            //     group.GetComponentsInChildren<Transform>(
            //         waypoints); //리스트(waypoints)에 자식오브젝트들의 transform을 담는다.
            //     waypoints.RemoveAt(
            //         0); //첫번째요소는 부모(WayPointGroup)의 transform이므로 삭제해준다.
            // }
        }

        //~~순찰 시작~~
        MoveWayPoint();
    }

    
	
    
	// Update is called once per frame
	void Update ()
    {
        // if (agent.isStopped == false)
        // {
        //     Quaternion rotation = Quaternion.LookRotation(agent.desiredVelocity);
        //     transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * patrollSpeed);
        // }
        
        if (!_isPatrolling) return;
        
        //agent.stoppingDistance = 0.5f;
        
        //<속도비교>sqrMagnitude는 벡터 크기의 제곱(x^2+y^2+z^2)을 구한다. magnitude는 벡터의 크기를 구하므로 sqrM~의 제곱근을 구한다.
        //따라서 단순히 크기 비교를 할때에는 제곱근 과정을 생략하는 sqrM~으로 계산하는 것이 더욱 빠르다.
        if (agent.velocity.sqrMagnitude >= 0.2f * 0.2f && agent.remainingDistance <= 0.5f) //현재 0.2속도 이상으로 움직이고 있는지+목적지에 도착했는지
        {
            //다음 인덱스로 nextIndex를 변경. 루프식으로 설정. 예)웨이포인트가 3개면 0,1,2 다음 다시 0번째로 돌아오도록 인덱스를 지정함.
            nextIndex = ++nextIndex % waypoints.Count;
            //변경된 nextIndex의 포인트로 목적지를 변경, 이동
            MoveWayPoint(); 
        }
        
        //Move();
        //ChangeDirection();
        //ElapseTime();
    }
    
}
