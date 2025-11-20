using UnityEngine;

public class GrabComponent : MonoBehaviour
{
    public void SetChild(Transform child)
    {
        child.parent = transform;
    }
    private void Update()
    {
        Transform child = transform.GetChild(0);
        if (child != null)
            child.localPosition = new Vector3(0, 1.975f, 0);
            
    }
}
