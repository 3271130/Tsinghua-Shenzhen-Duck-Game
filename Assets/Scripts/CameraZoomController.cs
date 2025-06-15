using UnityEngine;
using Cinemachine;

public class CameraZoomController : MonoBehaviour
{
    private CinemachineFreeLook freeLookCamera;

    [Header("Zoom Settings")]
    public float zoomSpeed = 2f;
    public float minDistance = 2f;
    public float maxDistance = 12f;

    private void Start()
    {
        freeLookCamera = GetComponent<CinemachineFreeLook>();
    }

    private void Update()
    {
        HandleZoom();
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            AdjustCameraDistance(scroll);
        }
    }

    private void AdjustCameraDistance(float scrollDelta)
    {
        // Top Rig
        float newTopDistance = freeLookCamera.m_Orbits[0].m_Radius - (scrollDelta * zoomSpeed);
        freeLookCamera.m_Orbits[0].m_Radius = Mathf.Clamp(newTopDistance, minDistance, maxDistance);

        // Middle Rig
        float newMiddleDistance = freeLookCamera.m_Orbits[1].m_Radius - (scrollDelta * zoomSpeed);
        freeLookCamera.m_Orbits[1].m_Radius = Mathf.Clamp(newMiddleDistance, minDistance, maxDistance);

        // Bottom Rig
        float newBottomDistance = freeLookCamera.m_Orbits[2].m_Radius - (scrollDelta * zoomSpeed);
        freeLookCamera.m_Orbits[2].m_Radius = Mathf.Clamp(newBottomDistance, minDistance, maxDistance);
    }
}