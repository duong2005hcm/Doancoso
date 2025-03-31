using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private bool gameFinished = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Entity"))
        {
            EntityType entity = collision.GetComponent<EntityType>();
            if (entity == null) return;

            switch (entity.entityType)
            {
                case EntityType.EntityTypes.Coin:
                    CoinManager.Instance.AddMoney(1);
                    Destroy(collision.gameObject);
                    break;

                case EntityType.EntityTypes.People:
                    if (!gameFinished)
                    {
                        gameFinished = true;
                        FinishGameManager.Instance.FinishGame();
                    }
                    break;
            }
        }
    }

}
