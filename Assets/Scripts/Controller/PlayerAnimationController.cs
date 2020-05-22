using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;

    public void Start()
    {
        animator = GetComponent<Animator>();
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