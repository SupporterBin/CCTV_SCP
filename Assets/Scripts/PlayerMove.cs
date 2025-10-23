using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("캐릭터 화면 이동 속도")]
    public float sensitivity = 20f;
    float rotX = 0f;

    [Header("캐릭터 스피드")]
    public float speed = 3;

    Vector2 lookDelta;
    Vector2 moveInput;

    public GameObject headObject;
    private Rigidbody rb;

    void Start()
    {
        // ▼▼▼ 마우스 고정 및 숨기기 ▼▼▼
        //Cursor.lockState = CursorLockMode.Locked; // 커서를 화면 중앙에 고정
        //Cursor.visible = false;

        // Rigidbody 컴포넌트를 시작할 때 가져옵니다.
        rb = GetComponent<Rigidbody>();
    }

    // Update는 카메라 회전처럼 물리와 관련 없는 로직만 처리합니다.
    void Update()
    {
        //카메라 회전
        float mouseX = lookDelta.x * sensitivity * Time.deltaTime;
        float mouseY = lookDelta.y * sensitivity * Time.deltaTime;

        rotX -= mouseY;
        rotX = Mathf.Clamp(rotX, -80f, 80f);
        headObject.transform.localRotation = Quaternion.Euler(rotX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX); // headObject.transform.parent 대신 transform 사용
    }

    // 물리 관련 코드는 FixedUpdate에서 처리합니다.
    void FixedUpdate()
    {
        //움직임
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        Vector3 newPosition = rb.position + move * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition); // MovePosition 사용
    }

    void OnLook(InputValue value) => lookDelta = value.Get<Vector2>();

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
}