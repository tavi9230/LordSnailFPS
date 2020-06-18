using System.Collections;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private float crouchOffset = .8f;
    private float standingOffset = 1.3f;
    private Animator animator;

    public void Start()
    {
        animator = GetComponent<Animator>();
        standingOffset = Camera.main.transform.parent.position.y;
        crouchOffset = standingOffset - .5f;
    }

    private IEnumerator PanCamera(bool isCrouching)
    {
        float timeToWait = isCrouching ? .1f : 0f;
        yield return new WaitForSeconds(timeToWait);
        float yOffset = isCrouching ? crouchOffset : standingOffset;
        Camera.main.transform.parent.position = new Vector3(Camera.main.transform.parent.position.x, yOffset, Camera.main.transform.parent.position.z);
    }

    public void SetIsMoving(bool isMoving)
    {
        animator.SetBool("isMoving", isMoving);
    }

    public void SetMove(float moveX, float moveY, float moveZ)
    {
        animator.SetFloat("moveX", moveX);
        animator.SetFloat("moveY", moveY);
        animator.SetFloat("moveZ", moveZ);
    }

    public void SetCrouching(bool isCrouching)
    {
        StartCoroutine(PanCamera(isCrouching));
        animator.SetBool("isCrouching", isCrouching);
    }

    public void SetIsPreparingRightAttack(bool isAttacking)
    {
        animator.SetBool("isPreparingRightAttack", isAttacking);
    }

    public void SetAttackingRight(bool isAttacking)
    {
        animator.SetBool("isAttackingRight", isAttacking);
    }

    public void SetIsPreparingLeftAttack(bool isAttacking)
    {
        animator.SetBool("isPreparingLeftAttack", isAttacking);
    }

    public void SetAttackingLeft(bool isAttacking)
    {
        animator.SetBool("isAttackingLeft", isAttacking);
    }
}