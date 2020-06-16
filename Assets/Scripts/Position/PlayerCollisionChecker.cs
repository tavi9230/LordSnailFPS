using UnityEngine;

public class PlayerCollisionChecker : MonoBehaviour
{
    private SpriteRenderer sprite;
    private PlayerController playerController;

    public void Start()
    {
        playerController = gameObject.GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HideableObject"))
        {
            HidePlayer();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("HideableObject"))
        {
            playerController.animationController.SetCrouching(false);
            playerController.RemoveCondition(ConditionEnum.Invisible);
            playerController.AddCondition(ConditionEnum.Visible);
        }
    }

    private void HidePlayer()
    {
        playerController.animationController.SetCrouching(true);
        playerController.RemoveCondition(ConditionEnum.Visible);
        playerController.AddCondition(ConditionEnum.Invisible);
    }
}