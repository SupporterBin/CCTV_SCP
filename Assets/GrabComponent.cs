using UnityEngine;

public class GrabComponent : MonoBehaviour
{
    bool isGet = false;
    public void SetChild(Transform child)
    {
        child.parent = transform;
        isGet = true;
    }
    private void Update()
    {
        if (! isGet) return;
        Transform child = transform.GetChild(0);
        if (child != null)
            child.localPosition = new Vector3(0, 1.975f, 0);
            
    }
}
