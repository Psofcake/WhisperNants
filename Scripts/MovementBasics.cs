using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class MovementBasics : MonoBehaviour
{
    public Transform playerCameraTransform;
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float moveSpeed = 2.0f;
    public float jumpHeight = 1.5f;
    public float mouseSensitivity = 5f;
    
    //private PlayerInput _playerInput;
    private Camera _playerCamera;
    public float normalFOV = 75f; // 기본 FOV
    public float zoomedFOV = 100f;
    private CharacterController _controller;
    private Animator _animator;
    private Vector3 velocity;
    private Quaternion characterTargetRot;
    private Quaternion cameraTargetRot;
    private float _gravity = -9.81f;
    private bool _isWalking;
    private bool _isGrounded = true;
    private bool _isDodging = false;
    private bool _isFlashing = false;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _playerCamera = playerCameraTransform.GetComponent<Camera>();
        if (_playerCamera == null)
        {
            _playerCamera = GetComponent<Camera>(); // 카메라가 지정되지 않았을 경우 현재 오브젝트의 카메라를 가져옴
        }
        _playerCamera.fieldOfView = normalFOV; // 시작할 때 기본 FOV로 설정
        
        //_playerInput = GetComponent<PlayerInput>();
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        
        characterTargetRot = transform.localRotation;
        cameraTargetRot = playerCameraTransform.localRotation;
        Cursor.lockState = CursorLockMode.Locked; // 커서를 잠궈서 마우스 이동 시 화면 밖으로 나가지 않도록 함
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveVector3 = new Vector3(horizontal, 0, vertical).normalized;
        moveVector3 = transform.TransformDirection(moveVector3);
        _isWalking = !Input.GetKey(KeyCode.LeftShift);
        moveSpeed = _isWalking ? walkSpeed : runSpeed;
        
        if (!Input.GetKey(KeyCode.LeftControl))
            Look();
        Jump();
        Move(moveVector3);
        SlowDown();
        
        if(Input.GetMouseButton(1))
            ZoomIn();
        else
            ZoomOut();
    }
    void Look()
    {
        //카메라 시점 조정 (마우스 이동)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
     
        characterTargetRot *= Quaternion.Euler(0f, mouseX, 0f);
        cameraTargetRot *= Quaternion.Euler(-mouseY, 0f, 0f);
                 
        //수평 회전
        transform.localRotation = characterTargetRot;
        //수직 회전
        playerCameraTransform.localRotation = cameraTargetRot;
        //transform.Rotate(Vector3.up * mouseX);
        //playerCameraTransform.Rotate(Vector3.left * mouseY);
    }
    void ZoomIn()
    {
        if (_playerCamera.fieldOfView < zoomedFOV)
            _playerCamera.fieldOfView += 0.05f;
    }

    void ZoomOut()
    {
        if (_playerCamera.fieldOfView > normalFOV)
            _playerCamera.fieldOfView -= 1f;
    }
    void Jump()
    {
        // 지면에 닿아 있는지 확인
        _isGrounded = _controller.isGrounded;
        //지면에 있을 때 속도 초기화
        if (_isGrounded && velocity.y < 0)
        {
            velocity.y = 0f;
        }
        
        // 점프
        if (_isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y += Mathf.Sqrt(jumpHeight * -2f * _gravity);
        }

        // 중력 적용
        velocity.y += _gravity * Time.deltaTime;
        _controller.Move(velocity * Time.deltaTime);

    }
    void Move(Vector3 moveVector)
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (_isFlashing)
                return;
            StartCoroutine(Flash(moveVector));
        }

        _controller.Move(moveVector * Time.deltaTime * moveSpeed);

        if (Input.GetMouseButtonDown(0)&&!_isDodging)
        {
            StartCoroutine(Dodge());
        }
    }

    IEnumerator Dodge()
    {
        _isDodging = true;
        _animator.SetTrigger("Dodge");
        float dodgeDuration = 0.4f;
        Vector3 startPosition = transform.position;
        Vector3 dodgeDirection = -transform.forward;
        float dodgeDistance = 3.0f;
        
        float elapsedTime = 0f;
        while (elapsedTime < dodgeDuration)
        {
            Vector3 newPosition = Vector3.Lerp(startPosition, startPosition+dodgeDirection*dodgeDistance, (elapsedTime / dodgeDuration));
            transform.position = newPosition;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.56f-dodgeDuration);
        _isDodging = false;
    }

    IEnumerator Flash(Vector3 flashVector)
    {
        _isFlashing = true;
        float flashDuration = 0.1f;
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 flashDirection = transform.forward;
        float flashDistance = 1.5f;
        
        while (elapsedTime<flashDuration)
        {
            if(flashVector != Vector3.zero)
                flashDirection = flashVector;
            Vector3 newPosition = Vector3.Lerp(startPosition, startPosition + flashDirection * flashDistance,
                (elapsedTime / flashDuration));
            transform.position = newPosition;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _isFlashing = false;
    }

    
    void SlowDown()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Time.timeScale = 0.5f;
        }
        if (Input.GetMouseButtonUp(1))
        {
            Time.timeScale = 1;
        }
    }
}
