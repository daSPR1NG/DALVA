﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(LineRenderer))]
//Enemies + players inherits from
public class CharacterController : MonoBehaviour
{
    [Header("CHARACTER RENDERER")]
    [SerializeField] private GameObject rendererGameObject;

    [Header("MOVEMENT PARAMETERS")]
    [Range(0,1)]
    [SerializeField] private float movementSpeed = 5f;
    Ray ray;
    RaycastHit cursorRaycastHit;

    [SerializeField] private LayerMask walkableLayer;
    [SerializeField] private GameObject movementFeedback;
    
    [SerializeField] private GameObject pathLandmark;
    private LineRenderer lineRenderer;

    private Animator animator;

    public float MovementSpeed { get => movementSpeed; set => movementSpeed = value; }
    public NavMeshAgent NavMeshAgent { get; set; }
    public GameObject PathLandmark { get => pathLandmark; }
    public GameObject RendererGameObject { get => rendererGameObject; }

    protected virtual void Start()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();

        animator = transform.GetChild(0).GetComponent<Animator>();

        lineRenderer = GetComponent<LineRenderer>();
    }

    protected virtual void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Process Moving character");
            SetNavMeshDestinationWithRayCast();
        }

        MoveWithMouseClick();
        DebugPathing(lineRenderer);
    }

    #region Handle Movement 
    private void SetNavMeshDestinationWithRayCast()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out cursorRaycastHit, 100f, walkableLayer))
        {
            MovementFeedbackInstantiation(movementFeedback, cursorRaycastHit.point);
            NavMeshAgent.SetDestination(cursorRaycastHit.point);
        }
    }

    private void MoveWithMouseClick()
    {
        if (NavMeshAgent.remainingDistance > NavMeshAgent.stoppingDistance)
        {
            //NavMeshAgent
            NavMeshAgent.Move(NavMeshAgent.desiredVelocity * MovementSpeed * Time.deltaTime);

            rendererGameObject.transform.LookAt(pathLandmark.transform);

            HandleAnimation("isMoving", true);
        }
        else
        {
            NavMeshAgent.Move(Vector3.zero);
            HandleAnimation("isMoving", false);
        }
    }

    public void MoveAgentToSpecificDestination(Transform destination)
    {
        if (NavMeshAgent.remainingDistance > NavMeshAgent.stoppingDistance)
        {
            //NavMeshAgent
            NavMeshAgent.destination = destination.position;


            HandleAnimation("isMoving", true);
        }
        else
        {
            NavMeshAgent.Move(Vector3.zero);
            HandleAnimation("isMoving", false);
        }
    }

    private void DebugPathing(LineRenderer line)
    {
        if (NavMeshAgent.hasPath)
        {
            line.positionCount = NavMeshAgent.path.corners.Length;
            line.SetPositions(NavMeshAgent.path.corners);
            line.enabled = true;
        }
        else
        {
            line.enabled = false;
        }
    }

    private void MovementFeedbackInstantiation(GameObject _movementFeedback, Vector3 pos)
    {
        if (_movementFeedback != null)
            Instantiate(_movementFeedback, pos, Quaternion.identity);
    }
    #endregion

    private void HandleAnimation(string boolName, bool value)
    {
        animator.SetBool(boolName, value);
    }
}