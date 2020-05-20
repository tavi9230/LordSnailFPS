using UnityEngine;

public class ObjectCollisionChecker : MonoBehaviour
{
    private SpriteRenderer sprite;
    private float defaultMass;
    private float defaultLinearDrag;
    private ObjectController objectController;

    public void Start()
    {
        sprite = gameObject.GetComponentInParent<SpriteRenderer>();
        defaultMass = gameObject.GetComponentInParent<Rigidbody2D>().mass;
        defaultLinearDrag = gameObject.GetComponentInParent<Rigidbody2D>().drag;
        objectController = gameObject.GetComponentInParent<ObjectController>();
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("PlayerPositionCollider"))
        {
            if (objectController.Size > WeightEnum.Medium)
            {
                var rigidbody = gameObject.GetComponentInParent<Rigidbody2D>();
                rigidbody.mass = 9999;
                rigidbody.drag = 9999;
            }
        }
    }

    public void OnCollisionStay2D(Collision2D other)
    {
    }

    public void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("PlayerPositionCollider"))
        {
            var rigidbody = gameObject.GetComponentInParent<Rigidbody2D>();
            rigidbody.mass = defaultMass;
            rigidbody.drag = defaultLinearDrag;
        }
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
    }

    private void CheckIfHidableObject(Collider2D other, bool isVisible)
    {
        if (other.gameObject.CompareTag("HideableObject"))
        {
            sprite.color = !isVisible
                ? new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.5f)
                : new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);
        }
    }
}