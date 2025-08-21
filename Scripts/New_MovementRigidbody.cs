using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Rigidbody,CapsuleCollider 컴포넌트가 필수로 필요함
[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class New_MovementRigidbody : MonoBehaviour
{
    [Header("Player Setting")] [SerializeField] private GameObject playerObject; //★ 없애도 될듯..?
    [Header("Camera Setting")] [SerializeField] private Camera playerCamera; // 카메라 참조

    [Header("Movement Setting")]
    [SerializeField] private float playerSpeed = 3f; // 현재 스피드
    [SerializeField] private float walkSpeed = 3f; // 걷는 속도
    [SerializeField] private float runSpeed = 5f; // 달리는 속도
    [SerializeField] private float crouchSpeed = 1f; // 앉아 움직이기 속도
    [SerializeField] private float jumpHeight = 4f; // 점프 높이
    
    [Header("Height Setting")] 
    [SerializeField] private float crouchHeight = 1f; // 앉기시 높이
    [SerializeField] private float standingHeight; // 서있는 높이 (Awake초기화)
    [SerializeField] private float currentHeight; // 원활한 앉은키 높이 변경을 위한 변수

    [Header("Crouch Settings")] 
    [SerializeField] private float crouchTransitionSpeed = 5f; // 앉기의 속도 조절
    
    [Header("Dodge Settings")]
    [SerializeField] private float dodgeSpeed = 10f; // 회피 동작의 속도
    [SerializeField] private float dodgeDuration = 0.2f; // 회피 동작의 지속시간
    [SerializeField] private float checkDodgeTime = 0.2f;

    //플레이어 이동관련 변수
    private float _playerMoveHorizontal; // GetAxisRaw로 얻은 Horizontal 입력 값 저장 변수
    private float _playerMoveVertical; // GetAxisRaw로 얻은 Vertical 입력 값 저장 변수
    private Vector3 _playerMoveVector3; // 플레이어 이동 벡터값
    public bool IsPlayerMove { get; private set; } // 이동중 확인 bool변수

    //분기점 변수
    [SerializeField] private bool isGrounded = true; // 땅에 닿아 있는지 여부
    private bool _isCrouching; // 앉기중인지 여부

    private bool _isCrouchingInProgress; // 앉기 동작중 중복방지 bool 변수
    private bool _isDodging; // 회피 중인지 여부
    // private bool _isFlashing; // 플래시 중인지 여부

    //참조 변수
    private Camera _playerCamera; // 플레이어 카메라
    private Rigidbody _rigidbody; // Rigidbody 컴포넌트
    private CapsuleCollider _capsuleCollider; // CapsuleCollider 컴포넌트

    //키코드 입력 변수
    private bool _runKey; // 달리기
    private bool _jumpDodgeKey; // 점프
    private bool _crouchKey; // 앉기
    private bool _lockCameraKey; // 카메라고정
    private bool _slotQKey; // 점멸
    private bool _slotEKey; // 시간조작

    public MWController _mwController;
    
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked; // 커서를 잠금 상태로 설정
        InitializeComponents(); // 초기화 및 null체크
        standingHeight = _playerCamera.transform.localPosition.y; // 서 있는 높이 저장
        currentHeight = standingHeight; // 서있는 키 currnetHeight에 저장
    }
    private void InitializeComponents()
    {
        // 카메라 초기화
        _playerCamera = playerCamera.GetComponent<Camera>() ?? GetComponent<Camera>();
        if (_playerCamera == null) Debug.LogError("Camera component is null in MovementRigidbody.");
        // Rigidbody 초기화
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null) Debug.LogError("Rigidbody component is null in MovementRigidbody.");
        // CapsuleCollider 초기화
        _capsuleCollider = GetComponent<CapsuleCollider>();
        if (_capsuleCollider == null) Debug.LogError("CapsuleCollider component is null in MovementRigidbody.");
    } // null 체크
    private void Update()
    {
        KeySetting(); // 키매핑
        Move(); // 이동 처리
        InputLogic(); // 입력 처리
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (_isFlashing)
                return;
            StartCoroutine(Flash());
        }
    }
    private void KeySetting()
    {
        // 플레이어 움직임 AD,WS 키
        _playerMoveHorizontal = Input.GetAxisRaw("Horizontal");
        _playerMoveVertical = Input.GetAxisRaw("Vertical");
        // 달리기
        if (Input.GetButtonDown("Run")) _runKey = true;
        else if (Input.GetButtonUp("Run")) _runKey = false;
        // 점프&스탭
        if (Input.GetButtonDown("Jump_Step")) _jumpDodgeKey = true;
        else if (Input.GetButtonUp("Jump_Step")) _jumpDodgeKey = false;
        // 앉기
        if (Input.GetButtonDown("Crouch")) _crouchKey = true;
        else if (Input.GetButtonUp("Crouch")) _crouchKey = false;
        // 카메라잠금
        if (Input.GetButtonDown("LockCamera")) _lockCameraKey = true;
        else if (Input.GetButtonUp("LockCamera")) _lockCameraKey = false;
        // 슬롯 Q
        if (Input.GetButtonDown("Slot_Q")) _slotQKey = true;
        else if (Input.GetButtonUp("Slot_Q")) _slotQKey = false;
        // 슬롯 E
        if (Input.GetButtonDown("Slot_E")) _slotEKey = true;
        else if (Input.GetButtonUp("Slot_E")) _slotEKey = false;

    }
    private void Move()
    {
        // 카메라의 전방과 우측 방향 벡터를 가져옴
        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;
        forward.y = 0;
        right.y = 0;

       

        _playerMoveVector3 =
            (_playerMoveHorizontal * right + _playerMoveVertical * forward).normalized; // 이동 벡터 계산 및 정규화

        if (_playerMoveVector3 != Vector3.zero)
        {
            //playerObject.transform.position += playerSpeed * Time.deltaTime * _playerMoveVector3; // 캐릭터의 위치를 이동 벡터에 따라 업데이트
            _rigidbody.MovePosition(transform.position + _playerMoveVector3 * playerSpeed * Time.fixedDeltaTime); // ★위의 직관적인 이동도 좋지만 물리연산 고려한 rigidbody 이동이 좋아서 아래처럼 수정
        }

        IsPlayerMove = _playerMoveVector3.magnitude > 0; // 이동 상태 업데이트
        MoveAnimation();
    }
    
    private void InputLogic()
    {
        if (_jumpDodgeKey && isGrounded) // 점프키 + 땅판정
        {
            isGrounded = false;
            Jump();
        }
        Crouch();
        
        // // 슬롯 사용 Q = 플래시
        // if ( _slotQKey && !_isFlashing)
        //     StartCoroutine(Flash());
        //
        // // 슬롯 사용 E = 느리게 하기
        // if (_slotEKey)
        //     Time.timeScale = 0.5f; // 느리게
        // else
        //     Time.timeScale = 1f; // 정상 속도
    }
    private void Crouch() 
    {
        float difference = 0.01f; //★ 추가한부분
        if (isGrounded) // 땅에있는 상태
        {
            float targetHeight = _crouchKey ? crouchHeight : standingHeight; // 키에 따라 높이설정
            
            if(Mathf.Abs(currentHeight - targetHeight) > difference) //★추가- 차이가 difference이상일때만 (변경이유:업데이트-인풋로직-크라우치에서 계속 호출되기때문에 목표위치에 도달해도 불필요한 연산이 반복된다.)
                currentHeight = Mathf.MoveTowards(currentHeight, targetHeight, crouchTransitionSpeed * Time.deltaTime); // 현재 높이를 목표 높이로 천천히 이동 스피드는 crouchTransitionSpeed
            
            
            // 카메라의 현재 위치를 업데이트한다 - 목표높이로 천천히 업데이트 하기 때문에 자연스러운 전환 가능
            Vector3 cameraPosition = _playerCamera.transform.localPosition;
            _playerCamera.transform.localPosition = new Vector3(cameraPosition.x, currentHeight, cameraPosition.z);

            // 콜라이더의 높이를 현재 높이에 맞게 업데이트
            // 콜라이더는 캐릭터 중심에 만들어지기 때문에 위아래 크기를 생각해서 높이를 currentHeight 2배 만큼의 값을 넣어줘야한다
            // ★ height는 카메라(눈높이)위치라서 콜라이더 크기도 height랑 비슷하거나 약간 커야하지않을까요..?
            _capsuleCollider.height = currentHeight * 2;
            //_capsuleCollider.height = currentHeight+0.1f;
            // 콜라이더의 중심위치 또한 currentHeight와 같게 설정한다 // ★ 콜라이더의 중심 위치는 height보다 낮아야
            _capsuleCollider.center = new Vector3(0, currentHeight, 0);
            //_capsuleCollider.center = new Vector3(0, currentHeight/2, 0);

            // 현재 높이가 서있는 키 높이보다 약간 앉으면 앉고있다고 판단한다
            //_isCrouching = currentHeight < standingHeight - 0.05f; //작은 임계값 사용 
            
            // 앉은 상태에 따라 플레이어의 이동속도 업데이트
            //playerSpeed = _isCrouching ? crouchSpeed : (_runKey ? runSpeed : walkSpeed); // ★_isCrouching 대신에 _crouchKey를 써도 될것같아요!
            playerSpeed = _crouchKey ? crouchSpeed : (_runKey ? runSpeed : walkSpeed);
        }
        else // 땅이 아닐경우 (공중) //★ 앉아서 이동하다가 계단을 내려갈때도 공중으로 처리되는지..? 맵 에셋 받으면 그때 안일어서는지 플레이테스트 필요할것같네요
        {
            // 공중에 있는 경우에 서 있는 높이로 천천히 돌아감 crouchTransitionSpeed 스피드 만큼
            currentHeight = Mathf.MoveTowards(currentHeight, standingHeight, crouchTransitionSpeed * Time.deltaTime);
            
            // 카메라의 현재 위치 업데이트
            Vector3 cameraPosition = _playerCamera.transform.localPosition;
            _playerCamera.transform.localPosition = new Vector3(cameraPosition.x, currentHeight, cameraPosition.z);

            // 콜라이더 업데이트
            _capsuleCollider.height = standingHeight;
            _capsuleCollider.center = new Vector3(0, currentHeight, 0);
            
            // 공중판정시 isCrouching을 false로 설정
            _isCrouching = false;
        }
    }
    private void Jump() // ★ 요기는 수정중입니다
    {
        // // 일관성 유지를 위한 점프시 항상 서있는 상태 유지
        // currentHeight = standingHeight;
        // Vector3 cameraPosition = _playerCamera.transform.localPosition;
        // _playerCamera.transform.localPosition = new Vector3(cameraPosition.x, currentHeight, cameraPosition.z);
        // _capsuleCollider.height = currentHeight * 2;
        // _capsuleCollider.center = new Vector3(0, currentHeight, 0);

        _rigidbody.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse); // 점프 힘
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // 땅에 닿았는지 확인
        {
            isGrounded = true;
        }
    }
    
    // private IEnumerator Dodge()
    // {
    //     _isDodging = true; // 회피 상태 활성화
    //
    //     // 회피 방향 및 거리 설정
    //     Vector3 dodgeVector = -transform.forward * dodgeDistance; // 뒤쪽으로 회피
    //     Vector3 startPosition = transform.position; // 시작 위치 저장
    //     float dodgeDuration = 0.4f; // 회피 지속 시간
    //     float elapsedTime = 0f; // 경과 시간 초기화
    //
    //     // 회피 애니메이션 동안 위치 보간
    //     while (elapsedTime < dodgeDuration)
    //     {
    //         _rigidbody.MovePosition(Vector3.Lerp(startPosition, startPosition + dodgeVector,
    //             elapsedTime / dodgeDuration));
    //         elapsedTime += Time.deltaTime; // 경과 시간 증가
    //         yield return null; // 다음 프레임까지 대기
    //     }
    //
    //     // 회피 애니메이션이 끝난 후 잠시 대기
    //     yield return new WaitForSeconds(0.16f);
    //     _isDodging = false; // 회피 상태 비활성화
    // }
    //
    bool _isFlashing = false;
    private float flashDistance = 3f;
    
    IEnumerator Flash()
    {
        _isFlashing = true; // 플래시 상태 활성화
        Vector3 flashVector = transform.forward * flashDistance; // 앞으로 이동할 벡터
        Vector3 startPosition = transform.position; // 시작 위치 저장
        float flashDuration = 0.1f; // 플래시 지속 시간
        float elapsedTime = 0f; // 경과 시간 초기화
    
        // 플래시 애니메이션 동안 위치 보간
        while (elapsedTime < flashDuration)
        {
            if(!_isFlashing)
                yield break;
            _rigidbody.MovePosition(Vector3.Lerp(startPosition, startPosition + flashVector,
                elapsedTime / flashDuration));
            elapsedTime += Time.deltaTime; // 경과 시간 증가
            yield return null; // 다음 프레임까지 대기
        }
    
        _isFlashing = false; // 플래시 상태 비활성화
    }

    private void MoveAnimation()
    {
        if (IsPlayerMove)
        {
            _mwController.CurrentMWeapon.animator.SetBool("Walk", !_runKey);
            _mwController.CurrentMWeapon.animator.SetBool("Run", _runKey);
        }
        else
        {
            _mwController.CurrentMWeapon.animator.SetBool("Walk", false);
            _mwController.CurrentMWeapon.animator.SetBool("Run", false);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Debug.Log("Wall Collision");
            _isFlashing = false;
        }
    }

}




