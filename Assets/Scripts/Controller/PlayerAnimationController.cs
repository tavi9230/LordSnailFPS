using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController
{
    private Animator animator;

    public PlayerAnimationController(Animator animator)
    {
        this.animator = animator;
    }

    public void SetMoveSpeed(float moveX, float moveY)
    {
        animator.SetFloat("moveX", moveX);
        animator.SetFloat("moveY", moveY);
    }

    public void SetLastMoveSpeed(float lastMoveX, float lastMoveY)
    {
        animator.SetFloat("lastMoveX", lastMoveX);
        animator.SetFloat("lastMoveY", lastMoveY);
    }

    public void SetIsAttacking(bool isAttacking)
    {
        animator.SetBool("isAttacking", isAttacking);
    }
}