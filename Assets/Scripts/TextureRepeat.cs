using UnityEngine;

public class TextureRepeat : MonoBehaviour
{
    public float speed = 7;
    Vector2 offset;

    void Update()
    {
        offset = new Vector2(0, Time.time * speed);
        GetComponent<Renderer>().material.mainTextureOffset = offset;
    }
}