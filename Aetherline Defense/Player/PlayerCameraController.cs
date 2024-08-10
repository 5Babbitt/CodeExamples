using Cinemachine;
using UnityEngine;

public class PlayerCameraController : PlayerSystem
{
    [SerializeField] private CinemachineFreeLook freelookCam;

    protected override void SubscribeEvents()
    {
        player.Events.OnSpawn += (bool isOwner) => freelookCam.enabled = isOwner;
    }

    protected override void UnsubscribeEvents()
    {
        player.Events.OnSpawn -= (bool isOwner) => freelookCam.enabled = isOwner;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner && IsClient) 
            return;
        
        if (other.tag == "CameraZone")
        {
            CameraZone camZone = other.GetComponent<CameraZone>();
            camZone.ToggleCamera(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsOwner && IsClient)
            return;

        if (other.tag == "CameraZone")
        {
            CameraZone camZone = other.GetComponent<CameraZone>();
            camZone.ToggleCamera(false);
        }
    }
}
