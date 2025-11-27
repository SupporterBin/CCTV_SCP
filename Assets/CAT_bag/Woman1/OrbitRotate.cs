using UnityEngine;

public class OrbitRotate : MonoBehaviour
{
    // 회전 중심
    public Transform center;

    // 초당 회전 속도 (도/초)
    public float angularSpeed = -90f;

    // y 높이를 고정하고 싶으면 사용
    public bool lockY = true;
    public float fixedY = 0f;

    void Start()
    {
        if (lockY)
        {
            fixedY = transform.position.y;
        }
    }

    void Update()
    {
        if (center == null) return;

        // 중심을 기준으로 Y축(위쪽)을 따라 회전
        transform.RotateAround(center.position, Vector3.up, angularSpeed * Time.deltaTime);

        // y 높이 고정 (캐릭터가 위아래로 안 튀게)
        if (lockY)
        {
            Vector3 p = transform.position;
            p.y = fixedY;
            transform.position = p;
        }

        // 항상 중심을 바라보도록 회전
        Vector3 lookPos = center.position;
        lookPos.y = transform.position.y; // 고개만 돌리게 y 맞춤
        transform.LookAt(lookPos);
    }
}
