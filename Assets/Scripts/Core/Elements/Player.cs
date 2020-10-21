using System;
using Cinemachine;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Token virtualCameraToken;

    void Start()
    {
        var virtualCamera = Repository.GetFirst<CinemachineVirtualCamera>(virtualCameraToken);
        virtualCamera.m_Follow = transform;
    }

    public void Place(Vector3 position)
    {
        var delta = position - transform.position;
        transform.position = position;
        
        var virtualCamera = Repository.GetFirst<CinemachineVirtualCamera>(virtualCameraToken);
        virtualCamera.OnTargetObjectWarped(transform, delta);
    }
}