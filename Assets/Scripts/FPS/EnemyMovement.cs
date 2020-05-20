using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    private Transform destination;
    private NavMeshAgent navMeshAgent;

    public float speed = 12f;
    public float gravity = -19.62f;
    public float jumpHeight = 3f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.Log("No nav mesh agent for enemy");
        }
    }

    private void Patrol()
    {
        if (destination != null)
        {
            navMeshAgent.SetDestination(destination.transform.position);
        }
    }

    void Update()
    {
        Patrol();
    }
}
