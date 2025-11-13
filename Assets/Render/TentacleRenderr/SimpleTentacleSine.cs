// SimpleTentacleSine.cs
// �ܼ� ������ ��� ��ġ ������: ������Ʈ(���� ����) �� ������ ���� ������ ��ġ�� ����� positions[]�� ����
using UnityEngine;

    public enum Axis3D { X, Y, Z }
[ExecuteAlways]
public class SimpleTentacleSine : MonoBehaviour, IChainPos
{

    [Header("Shape")]
    [Min(2)] public int segments = 32;          // ���� �� (��Ʈ ����)
    [Min(0.001f)] public float totalLength = 2f;// ��ü ����
    public Axis3D alongAxis = Axis3D.Z;         // ���� ���� (������Ʈ ���� ����)
    public bool reverseAlong = false;           // ���� �������� ������

    [Header("Wave (sin)")]
    public Axis3D lateralAxis = Axis3D.X;       // �ĵ����� ��鸱 ��(����� ����)
    [Min(0f)] public float amplitude = 0.1f;    // ����(�ִ� ����)
    [Min(0.001f)] public float wavelength = 1f; // ����(���� ����, m/����Ŭ)
    public float speed = 1f;                    // ���� �ӵ�(����Ŭ/��)
    public float phaseOffset = 0f;              // ��ü ���� ������(����)
    [Range(0f, 1f)] public float tipTaper = 0.3f;// �� ������ ������ ���̴� ����(0=����)

    [Header("Output")]
    public bool outputWorldSpace = true;        // true�� ���� ��ǥ�� ����, false�� ���� ��ǥ�� ����
    [Tooltip("���� ���� ��ġ(��Ʈ���� ��)")]
    public Vector3[] positions;                 // ���� ��ġ ���

    // ���� ������Ƽ
    public Vector3[] Positions => positions;

    void OnEnable()
    {
        EnsureArray();
        RebuildPositions(0f); // �����Ϳ����� �ʱ� ��� ���̰�
    }

    void OnValidate()
    {
        // ����� Ⱦ���� ������ �ڵ� ����
        if (lateralAxis == alongAxis)
            lateralAxis = (alongAxis == Axis3D.Z) ? Axis3D.X : Axis3D.Z;

        EnsureArray();
        RebuildPositions(0f);
    }

    void Update()
    {
        float t = Application.isPlaying ? Time.time : 0f;
        RebuildPositions(t);
    }

    // === Core ===
    void RebuildPositions(float timeSeconds)
    {
        if (segments < 2) segments = 2;
        EnsureArray();

        float segLen = (segments > 1) ? (totalLength / (segments - 1)) : 0f;

        // ���� ���� ���� �� ����
        Vector3 a = AxisToVector(alongAxis) * (reverseAlong ? -1f : 1f);
        Vector3 l = AxisToVector(lateralAxis); // ����� ������ ������ �����Ǿ�� �ڿ�������

        // ���� �ĵ�: 2�� * (s/�� - f*t) + ��
        float k = Mathf.PI * 2f / Mathf.Max(0.0001f, wavelength); // �ļ�
        float w = Mathf.PI * 2f * speed;                           // �����ļ�
        float phi = phaseOffset;

        // ��Ʈ(���� ����)���� ����
        Vector3 rootLocal = Vector3.zero;

        for (int i = 0; i < segments; i++)
        {
            float s = segLen * i; // ��Ʈ������ �Ÿ�
            // �⺻(����) ��ġ: ��Ʈ + ���� * s
            Vector3 baseLocal = rootLocal + a * s;

            // ���� ������(������ ������ �پ��� �ɼ�)
            float tNorm = (segments <= 1) ? 0f : (i / (float)(segments - 1)); // 0~1
            float amp = Mathf.Lerp(amplitude, amplitude * (1f - tipTaper), tNorm);

            // ���� ����(Ⱦ�� ����)
            float theta = k * s - w * timeSeconds + phi;
            Vector3 offsetLocal = l * (amp * Mathf.Sin(theta));

            Vector3 pLocal = baseLocal + offsetLocal;

            positions[i] = outputWorldSpace
                ? transform.TransformPoint(pLocal)  // ����� ��ȯ ����
                : pLocal;                           // ���÷� �״�� ����
        }
    }

    void EnsureArray()
    {
        if (positions == null || positions.Length != Mathf.Max(2, segments))
            positions = new Vector3[Mathf.Max(2, segments)];
    }

    static Vector3 AxisToVector(Axis3D axis)
    {
        switch (axis)
        {
            case Axis3D.X: return Vector3.right;
            case Axis3D.Y: return Vector3.up;
            default: return Vector3.forward;
        }
    }
}
