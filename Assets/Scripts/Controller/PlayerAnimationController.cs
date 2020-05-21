using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;

    public void Start()
    {
        animator = GetComponent<Animator>();
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

    public void SetAttacking(bool isAttacking)
    {
        animator.SetBool("isAttacking", isAttacking);
    }
}