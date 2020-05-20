using System.Collections.Generic;
using UnityEngine;

public class AnimationController
{
    private Animator animator;

    public AnimationController(Animator animator)
    {
        this.animator = animator;
    }

    public void StopMove()
    {
        animator.SetBool("isMoving", false);
        animator.SetFloat("moveX", 0);
        animator.SetFloat("moveY", 0);
    }

    public void SetIsMoving(bool isMoving)
    {
        animator.SetBool("isMoving", isMoving);
    }

    public void SetMoveSpeed(float moveX, float moveY)
    {
        animator.SetFloat("moveX", moveX);
        animator.SetFloat("moveY", moveY);
    }

    public void SetLastMoveSpeed(float moveX, float moveY)
    {
        animator.SetFloat("lastMoveX", moveX);
        animator.SetFloat("lastMoveY", moveY);
    }

    public void SetIsAttacking(bool isAttacking)
    {
        animator.SetBool("isAttacking", isAttacking);
    }
}