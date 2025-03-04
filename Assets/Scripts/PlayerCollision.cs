using UnityEngine;

public class PlayerCollision : MonoBehaviour
{   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Entity"))
        {
            switch (collision.GetComponent<EntityType>().entityType)
            {
                case EntityType.EntityTypes.Coin:
                    PlayerMoney.Instance.AddMoney(1);
                    Destroy(collision.gameObject);
                    break;

                case EntityType.EntityTypes.People:
                    FinishGameManager.Instance.FinishGame();
                    break;
            }
            
        }    
    }
}
