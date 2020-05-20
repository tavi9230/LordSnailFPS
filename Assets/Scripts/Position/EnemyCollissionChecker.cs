using UnityEngine;

public class EnemyCollissionChecker : MonoBehaviour
{
    private SpriteRenderer enemySprite;
    private EnemyController enemyController;

    public void Start()
    {
        enemySprite = gameObject.GetComponentInParent<SpriteRenderer>();
        enemyController = gameObject.GetComponentInParent<EnemyController>();
    }

    public void Update()
    {
        var collider = GetComponent<Collider2D>();
        if (enemyController.State.Exists(s => s == StateEnum.Dead) && collider.isTrigger == false)
        {
            collider.isTrigger = true;
        }
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("PlayerPositionCollider") || other.gameObject.CompareTag("Player"))
        {
            enemyController.StartHunt();
        }
        //if (other.gameObject.CompareTag("Enemy"))
        //{
        //    gameObject.GetComponent<Collider2D>().isTrigger = true;
        //}
    }

    public void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("PlayerPositionCollider") || other.gameObject.CompareTag("Player"))
        {
            enemyController.StartHunt();
        }

        if (other.gameObject.CompareTag("PositionCollider"))
        {
            enemyController.RecalculatePath();
        }

        //if (other.gameObject.CompareTag("Enemy"))
        //{
        //    gameObject.GetComponent<Collider2D>().isTrigger = true;
        //}
    }

    public void OnCollisionExit2D(Collision2D other)
    {
        //if (other.gameObject.CompareTag("Enemy"))
        //{
        //    gameObject.GetComponent<Collider2D>().isTrigger = false;
        //}
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CheckIfHidableObject(other, false);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        CheckIfHidableObject(other, false);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        CheckIfHidableObject(other, true);
        //if (other.gameObject.CompareTag("Enemy"))
        //{
        //    gameObject.GetComponent<Collider2D>().isTrigger = false;
        //}
    }

    private void CheckIfHidableObject(Collider2D other, bool isVisible)
    {
        if (other.gameObject.CompareTag("HideableObject"))
        {
            enemySprite.color = new Color(enemySprite.color.r, enemySprite.color.g, enemySprite.color.b, 0.5f);
            if (isVisible)
            {
                enemyController.Conditions.Remove(ConditionEnum.Invisible);
                if (!enemyController.Conditions.Exists(c => c == ConditionEnum.Visible))
                {
                    enemyController.Conditions.Add(ConditionEnum.Visible);
                }
            }
            else
            {
                enemyController.Conditions.Remove(ConditionEnum.Visible);
                if (!enemyController.Conditions.Exists(c => c == ConditionEnum.Invisible))
                {
                    enemyController.Conditions.Add(ConditionEnum.Invisible);
                }
            }
        }
    }
}