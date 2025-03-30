using UnityEngine;
using UnityEngine.UI;

public class ImageLoader : MonoBehaviour
{
    public Image imageUI;

    public void LoadImage(string imageName)
    {
        Sprite loadedSprite = LoadSpriteFromResources(imageName);
        if (loadedSprite != null)
        {
            imageUI.sprite = loadedSprite;
        }
        else
        {
            Debug.LogError("Không tìm thấy ảnh: " + imageName);
        }
    }

    private Sprite LoadSpriteFromResources(string fileName)
    {
        Texture2D texture = Resources.Load<Texture2D>("Images/Iteams" + fileName);
        if (texture != null)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        return null;
    }
}
