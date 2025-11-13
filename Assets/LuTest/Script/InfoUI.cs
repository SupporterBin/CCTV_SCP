using UnityEngine;

public class InfoUI : MonoBehaviour
{
    [SerializeField]
    private Transform maincam;

    void Start()
    {
        if (maincam == null)
        {
            Debug.LogError("InfoUI mia");
        }
    }

    void Update()
    {
        if (maincam != null)
        {
            transform.LookAt(transform.position + maincam.rotation * Vector3.forward,
                             maincam.rotation * Vector3.up);
        }
    }
}
