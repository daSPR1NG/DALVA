﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RotationArroundAPivot : MonoBehaviourPun
{
    void Update()
    {
        if (GameObject.Find("GameNetworkManager") != null && photonView.IsMine == false && PhotonNetwork.IsConnected == true) { return; }

        if (Physics.Raycast(UtilityClass.RayFromMainCameraToMousePosition(), out RaycastHit hit, 100f))
        {
            RotateAroundAPivot(hit.point);
        }
    }

    public void RotateAroundAPivot(Vector3 pointToRotateTowards)
    {
        pointToRotateTowards = new Vector3(pointToRotateTowards.x, transform.position.y, pointToRotateTowards.z);
        transform.LookAt(pointToRotateTowards);
    }
}