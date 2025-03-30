using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Entity"))
        {
            collision.GetComponent<Collider2D>().enabled = false;
            collision.GetComponent<Rigidbody2D>().simulated = false;

            Destroy(collision.gameObject);
        }    
    }
}
