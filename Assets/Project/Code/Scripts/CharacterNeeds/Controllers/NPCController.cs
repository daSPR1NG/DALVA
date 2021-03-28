﻿using System.Collections.Generic;
using UnityEngine;

public class NPCController : CharacterController
{
    public int WaypointIndex { get; set; }
    public Transform waypointTarget = null; //public to debug
    public List<Transform> waypoints; //public to debug
    public float DistanceWithTarget { get; set; }

    public IState currentState; //set to private after tests
    public string CurrentStateName;

    public bool isACampNPC = false;
    public Transform startingPosition;

    #region Refs
    public NPCInteractions NPCInteractions => GetComponent<NPCInteractions>();
    public AggroRange AggroRange => GetComponentInChildren<AggroRange>();
    #endregion

    private void Start()
    {
        GetGlobalWaypoints();

        if (isACampNPC)
        {
            startingPosition.position = transform.position;
            ChangeState(new IdlingState());
        } 
        else ChangeState(new MovingState());
    }

    protected override void Update()
    {
        base.Update();
        currentState.OnUpdate();

        CurrentStateName = currentState.ToString();
    }

    public void ChangeState(IState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;
        currentState.Enter(this);
    }

    private void GetGlobalWaypoints()
    {
        foreach (Transform waypoints in MinionWaypointsManager.Instance.MinionsGlobalWaypoints)
        {
            this.waypoints.Add(waypoints);
        }
    }

    public virtual void CheckDistanceFromWaypoint(Transform waypoint)
    {
        if (Vector3.Distance(transform.position, waypoint.position) <= 1f)
        {
            if (WaypointIndex >= waypoints.Count) return;

            WaypointIndex++;
            waypointTarget = waypoints[WaypointIndex];
        }
    }
}