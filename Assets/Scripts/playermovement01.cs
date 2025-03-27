using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Camera mainCamera;
    private Rigidbody2D rb;

    [SerializeField] private float playerSpeed = 5f;
    [SerializeField] private float xMargin = 2f;
    [SerializeField] private float yMarginForInput = 2f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        int dirX = 0;
        transform.rotation = Quaternion.identity;

        if (Application.isEditor)
        {
            if (Input.GetKey(KeyCode.D))
            {
                dirX = 1;
                transform.rotation = Quaternion.Euler(0, 0, -30);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                dirX = -1;
                transform.rotation = Quaternion.Euler(0, 0, 30);
            }
        }
        else if (Input.touchCount > 0)
        {
            Vector3 touchPosition = Input.touches[0].position;
            if (touchPosition.y < Screen.height * yMarginForInput)
            {
                if (touchPosition.x > Screen.width / 2)
                {
                    dirX = 1;
                    transform.rotation = Quaternion.Euler(0, 0, -30);
                }
                else
                {
                    dirX = -1;
                    transform.rotation = Quaternion.Euler(0, 0, 30);
                }
            }
        }

        rb.linearVelocity = new Vector2(dirX * playerSpeed, rb.linearVelocity.y);
    }

    private void FixedUpdate()
    {
        float posX = Mathf.Clamp(transform.position.x, -xMargin, xMargin);
        transform.position = new Vector3(posX, transform.position.y, transform.position.z);
    }
}
