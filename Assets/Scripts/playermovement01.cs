using UnityEngine;

public class playermovement : MonoBehaviour
{
    private Camera mainCamera;
    private Rigidbody2D rb;


    [SerializeField] private float playerSpeed = 250;
    [SerializeField] private float Xmargin = 2;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        int dirX = 0;
        transform.rotation = Quaternion.Euler(0, 0, 0);

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
        else
        {
            if (Input.touches.Length > 0)
            {
                Vector3 touchPosition = Input.touches[0].position;
                touchPosition = mainCamera.ScreenToWorldPoint(touchPosition);

                if (touchPosition.x > 0)
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
        rb.linearVelocity = new Vector2(dirX * playerSpeed * Time.deltaTime, 0);

        float posX = transform.position.x;
        posX = Mathf.Clamp(posX, -Xmargin, Xmargin);
        transform.position = new Vector3(posX, transform.position.y, transform.position.z);
    }
}


