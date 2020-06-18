using UnityEngine;

public class PlayerCollisionChecker : MonoBehaviour
{
    private SpriteRenderer sprite;
    private PlayerController playerController;
    private bool isHidden;

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
            bool shouldUncrouch = true;
            Collider[] colliders = Physics.OverlapSphere(transform.position, .1f);
            foreach(var collider in colliders)
            {
                if (collider.CompareTag("HideableObject"))
                {
                    shouldUncrouch = false;
                }
            }
            if (shouldUncrouch)
            {
                Debug.Log("exit bush");
                if (!playerController.State.Exists(s => s == StateEnum.Crouching))
                {
                    playerController.animationController.SetCrouching(false);
                }
                playerController.RemoveCondition(ConditionEnum.Invisible);
                playerController.AddCondition(ConditionEnum.Visible);
            }
        }
    }

    private void HidePlayer()
    {
        playerController.animationController.SetCrouching(true);
        playerController.RemoveCondition(ConditionEnum.Visible);
        playerController.AddCondition(ConditionEnum.Invisible);
    }
}