using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class MovementRigidbody : MonoBehaviour
{
    public bool isWalking;
    public bool isRunning;
    //단위 int로 변경
    [SerializeField] private float walkSpeed = 3f; //걷는속도
    [SerializeField] private float runSpeed = 5f; //뛰는속도
    [SerializeField] private float crouchSpeed = 1f; //앉아서걷는속도
    [SerializeField] private float crouchHeight;    //
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float mouseSensitivity = 5f;
    [SerializeField] private float cameraRotLimit = 80f;
    [SerializeField] private float currentCameraRot;
    [SerializeField] private float flashDistance = 1.5f;
    [SerializeField] private float dodgeDistance = 3f;
    [SerializeField] private Transform camTransform;
    [SerializeField] private PostProcessVolume volume;
    
    private float _standingHeight;
    private float _crouchingHeight;
    private float _gravity = -9.81f;
    private float _horizontal;
    private float _vertical;
    private float _mouseX;
    private float _mouseY;
    private float _moveSpeed;

    private bool _isViewFixed;
    private bool _isGrounded = true;
    private bool _isDodging;
    private bool _isFlashing;
    private bool _isCrouching;
    
    private Vector3 velocity;
    private Camera _playerCamera;
    private Rigidbody _rigidbody;
    private Animator _animator;
    private ColorGrading _colorGrading;
    private CapsuleCollider _capsuleCollider;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        _playerCamera = camTransform.GetComponent<Camera>();
        if (_playerCamera == null)
        {
            _playerCamera = GetComponent<Camera>();
        }
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        //volume.profile.TryGetSettings(out _colorGrading);
        _standingHeight = _playerCamera.transform.localPosition.y;
        _crouchingHeight = crouchHeight;
    }

    void Update()
    {
        GetInput();
        
        Jump();
        if (!_isViewFixed)
            Look();
        
        if (Input.GetKeyDown(KeyCode.LeftControl))
            Crouch();
        
        Move();
        
        SlowDown();
    }

    void GetInput()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");
        _mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        _mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        isRunning = Input.GetKey(KeyCode.LeftShift); //Input.GetButton("Sprint")
        _isViewFixed = Input.GetKey(KeyCode.LeftAlt); //Input.GetButton(CursorLock")

        if (Input.GetKey(KeyCode.C))
        {
            Black();
            ZoomIn();
        }
        else
        {
            Black();
            ZoomOut();
        }
    }

    void Black()
    {
        if (_colorGrading != null)
        {
            //_colorGrading.saturation.value = (_colorGrading.saturation.value ==0)?100:0;
        }
    }
    void ZoomIn()
    {
        if (_playerCamera.fieldOfView < 100f)
            _playerCamera.fieldOfView += 0.05f;
    }

    void ZoomOut()
    {
        if (_playerCamera.fieldOfView > 70f)
            _playerCamera.fieldOfView -= 1f;
    }

    void Crouch()
    {
        _isCrouching = !_isCrouching;   //토글방식

        if (_isCrouching)
        {
            crouchHeight = _crouchingHeight;
        }else {
            crouchHeight = _standingHeight;
        }

        StartCoroutine(CrouchCoroutine());
    }

    IEnumerator CrouchCoroutine()
    {
        int i = 0;
        float posY = _playerCamera.transform.localPosition.y;
        while (posY!=crouchHeight)
        {
            //if(_playerCamera.transform.localPosition.y>=0.85)Debug.Log("count i:"+ ++i);
            i++;
            posY = Mathf.Lerp(posY, crouchHeight, 0.1f);
            _playerCamera.transform.localPosition = new Vector3(camTransform.localPosition.x,
                posY, camTransform.localPosition.z);
            if (i > 30) break;
            
            yield return null;
        }
        camTransform.localPosition = new Vector3(camTransform.localPosition.x,
            crouchHeight, camTransform.localPosition.z);
        _capsuleCollider.height = crouchHeight+0.1f;
        _capsuleCollider.center = new Vector3(0, _capsuleCollider.height/2+0.05f, 0);
    }
    
    void Jump()
    {
        _isGrounded = Physics.Raycast(transform.position+Vector3.up, Vector3.down, _capsuleCollider.bounds.extents.y + 0.1f);
        Debug.DrawLine(transform.position+new Vector3(0,0.5f,0), (transform.position+new Vector3(0,0.5f,0))+Vector3.down*(_capsuleCollider.bounds.extents.y+0.1f), Color.red);
        //_isGrounded = Physics.CheckSphere(transform.position, 0.1f, LayerMask.GetMask("Ground"));
        
        if (_isGrounded && Input.GetButtonDown("Jump_Step"))
        {
            if(_isCrouching) Crouch();
            // AddForce를 사용하여 점프
            _rigidbody.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * _gravity), ForceMode.Impulse);
        }
    }
    void Look()
    {
        //좌우 회전
        transform.Rotate(0f, _mouseX, 0f);
        //상하 회전
        //playerCameraTransform.Rotate(-_mouseY, 0f, 0f);
        
        //상하 회전 (카메라각 제한)
        currentCameraRot -= _mouseY;
        currentCameraRot = Mathf.Clamp(currentCameraRot, -80f, cameraRotLimit);
        camTransform.localEulerAngles = new Vector3(currentCameraRot, 0f, 0f);

    }
    void Move()
    {
        _moveSpeed = isRunning ? runSpeed : walkSpeed;
        _moveSpeed = _isCrouching ? crouchSpeed : _moveSpeed;
        
        Vector3 moveVector = new Vector3(_horizontal, 0, _vertical).normalized;
        moveVector = transform.TransformDirection(moveVector);
        moveVector *= _moveSpeed;
        
        if(moveVector == Vector3.zero) isWalking = false; else isWalking = !isRunning;
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (_isFlashing)
                return;
            StartCoroutine(Flash(moveVector));
        }

        _rigidbody.MovePosition(transform.position + moveVector * _moveSpeed * Time.fixedDeltaTime);
        
        if (Input.GetKeyDown(KeyCode.V) && !_isDodging && _isGrounded)
        {
            if(_isCrouching) Crouch();
            StartCoroutine(Dodge());
        }
    }

    IEnumerator Dodge()
    {   //AddForce로 효과 주는 게 더 자연스러워 보일듯 (테스트 후 변경 예정)
        _isDodging = true;
        _animator.SetTrigger("Dodge");
        float dodgeDuration = 0.4f;
        Vector3 dodgeVector = -transform.forward * dodgeDistance;

        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < dodgeDuration)
        {
            _rigidbody.MovePosition(startPosition + dodgeVector * (elapsedTime / dodgeDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.56f - dodgeDuration);
        _isDodging = false;
    }

    IEnumerator Flash(Vector3 flashDirection)
    {
        _isFlashing = true;
        float flashDuration = 0.1f;
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 flashVector = transform.forward * flashDistance;

        while (elapsedTime < flashDuration)
        {
            if (flashDirection != Vector3.zero)
                flashVector = flashDirection;

            _rigidbody.MovePosition(startPosition + flashVector * (elapsedTime / flashDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _isFlashing = false;
    }

    void SlowDown()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Time.timeScale = 0.5f;
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            Time.timeScale = 1;
        }
    }
    
    
    
    
}
