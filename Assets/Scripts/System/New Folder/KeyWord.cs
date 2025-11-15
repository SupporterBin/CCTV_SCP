using UnityEngine;

public enum KeyOption
{
    None,
    Delete,
    Call
}

public class KeyWord : MonoBehaviour
{

    public bool isKeyType;

    public int keyValue;
    public KeyOption keyOption;

    public int KeyWordGet()
    {
        return keyValue;
    }

    public KeyOption KeyOptionGet()
    {
        return keyOption;
    }
}
