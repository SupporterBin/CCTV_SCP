using UnityEngine;

public class StartParentSetter : MonoBehaviour
{
    [SerializeField]
    private Transform parent;

    void Start()
    {
        if (parent != null) transform.parent = parent;
    }
}
