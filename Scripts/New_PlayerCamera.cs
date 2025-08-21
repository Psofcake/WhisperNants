using UnityEngine;

public class New_PlayerCamera : MonoBehaviour
{
    [Header("Camera Setting")] 
    [SerializeField] private float mouseSensitivity = 5f; // 마우스 감도 =100
    [SerializeField] private float cameraRotLimit = 70f; // 카메라 회전 제한

    private float _xRotation; // 상하 회전값
    private float _yRotation; // 좌우 회전값

    void Update()
    {
        // 마우스 입력을 처리
        HandleMouseInput();
    }

    private void LateUpdate()
    {
        // 카메라 회전 업데이트
        ApplyCameraRotation();
    }

    private void HandleMouseInput()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime; // 수평 마우스 이동
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime; // 수직 마우스 이동

        _xRotation -= mouseY; // 수직 회전값 업데이트
        _xRotation = Mathf.Clamp(_xRotation, -cameraRotLimit, cameraRotLimit); // 상하 회전 제한
        _yRotation += mouseX; // 좌우 회전값 업데이트
    }

    private void ApplyCameraRotation()
    {
        // 카메라 회전 적용
        transform.localRotation = Quaternion.Euler(_xRotation, _yRotation, 0f); // 카메라 회전
    }
}