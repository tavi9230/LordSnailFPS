using UnityEngine;

public class PlayerCollisionChecker : MonoBehaviour
{
    private SpriteRenderer sprite;
    private PlayerController playerController;

    public void Start()
    {
        sprite = gameObject.GetComponentInParent<SpriteRenderer>();
        playerController = gameObject.GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("HideableObject"))
        {
            HidePlayer();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("HideableObject"))
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);
            playerController.RemoveCondition(ConditionEnum.Invisible);
            playerController.AddCondition(ConditionEnum.Visible);
        }
    }

    private void HidePlayer()
    {
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.5f);
        playerController.RemoveCondition(ConditionEnum.Visible);
        playerController.AddCondition(ConditionEnum.Invisible);
    }
}