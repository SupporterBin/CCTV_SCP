using System.Collections.Generic;
using UnityEngine;

public sealed class AnomalyContext
{
    public IReadOnlyList<GameObject> Targets; // 추가, 삭제 못하고 읽기만 가능 이벤트 대상 오브젝트.
    public Vector3 Origin; // 핵심 위치
    public int Seed; // 시드값 대충 랜덤 일관성을 위한 '일관 랜덤 고정값'.
}
