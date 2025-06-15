using UnityEngine;
using System.Collections;
using Cinemachine;

public class Drone : MonoBehaviour
{
    public GameObject dronePrefab;
    public Transform spawnPoint;
    public Transform startPoint; // 추가: startPoint를 사용
    public Transform droneDestination;
    public float moveSpeed = 5f;
    public float droneDescendSpeed = 2f;

    private bool isPlayerInRange = false;
    private GameObject spawnedDrone;
    private bool isDroneMoving = false;
    private bool isDroneDescending = false;
    private bool isDroneMovingToStartPoint = false; // 추가: startPoint로 이동 여부
    private GameObject player;
    public Vector3 droneScale = new Vector3(3f, 3f, 3f);

    public float ascendSpeed = 2f;
    public float rotationRadius = 150f;
    public float rotationSpeed = 2f;

    private bool isDroneAscending = false;
    private bool isDroneRotating = false;
    private bool isDroneReturning = false;
    private bool isDroneDescendingToStart = false;
    private float rotationAngle = 0f;
    private Vector3 rotationCenter;

    private Vector3 originalDronePosition;

    public Transform customRotationCenter;

    [Header("Camera Settings")]
    public CinemachineFreeLook normalCamera; // 일반 상태의 카메라
    public CinemachineFreeLook droneCamera;  // 드론 탑승 시 카메라

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPlayerInRange)
        {
            isPlayerInRange = true;
            player = other.gameObject;
            SpawnDrone();
        }
    }

    private void SpawnDrone()
    {
        spawnedDrone = Instantiate(dronePrefab, spawnPoint.position, Quaternion.identity);
        spawnedDrone.transform.localScale = droneScale;

        // Add collider 
        BoxCollider droneCollider = spawnedDrone.AddComponent<BoxCollider>();
        droneCollider.size = new Vector3(1f, 0.2f, 1f); // modify based on the drone size

        // Add rigidbody
        Rigidbody droneRb = spawnedDrone.AddComponent<Rigidbody>();
        droneRb.useGravity = false;
        droneRb.isKinematic = true;

        droneRb.constraints = RigidbodyConstraints.FreezeRotation;

        isDroneMoving = true;
    }

    private void Update()
    {
        if (isDroneMoving && spawnedDrone != null)
        {
            MoveDrone();
        }

        if (isDroneDescending && spawnedDrone != null)
        {
            DescendDrone();
        }

        if (isDroneAscending && spawnedDrone != null)
        {
            AscendDrone();
        }

        if (isDroneRotating && spawnedDrone != null)
        {
            RotateDrone();
        }

        if (isDroneReturning && spawnedDrone != null)
        {
            ReturnToOriginalPosition();
        }

        if (isDroneDescendingToStart && spawnedDrone != null)
        {
            ReturnToSpawnPoint();
        }

        if (isDroneMovingToStartPoint && spawnedDrone != null)
        {
            MoveToStartPoint();
        }
    }

    private void MoveDrone()
    {
        float step = moveSpeed * Time.deltaTime;
        spawnedDrone.transform.position = Vector3.MoveTowards(
            spawnedDrone.transform.position,
            droneDestination.position,
            step
        );

        if (Vector3.Distance(spawnedDrone.transform.position, droneDestination.position) < 0.1f)
        {
            isDroneMoving = false;
            isDroneDescending = true;
        }
    }

    private void AscendDrone()
    {
        Vector3 targetPosition = new Vector3(
            spawnedDrone.transform.position.x,
            droneDestination.position.y,
            spawnedDrone.transform.position.z
        );

        float step = ascendSpeed * Time.deltaTime;
        spawnedDrone.transform.position = Vector3.MoveTowards(
            spawnedDrone.transform.position,
            targetPosition,
            step
        );

        if (Vector3.Distance(spawnedDrone.transform.position, targetPosition) < 0.1f)
        {
            isDroneAscending = false;
            isDroneRotating = true;

            rotationCenter = customRotationCenter.position;

            // 카메라 전환
            normalCamera.Priority = 0;
            droneCamera.Priority = 60;
        }
    }

    private void DescendDrone()
    {
        Vector3 targetPosition = new Vector3(
            spawnedDrone.transform.position.x,
            player.transform.position.y + 4f * droneScale.y,
            spawnedDrone.transform.position.z
        );

        float step = droneDescendSpeed * Time.deltaTime;
        spawnedDrone.transform.position = Vector3.MoveTowards(
            spawnedDrone.transform.position,
            targetPosition,
            step
        );

        if (Vector3.Distance(spawnedDrone.transform.position, targetPosition) < 0.1f)
        {
            isDroneDescending = false;
            MovePlayerToDrone();
        }
    }

    private void MovePlayerToDrone()
    {
        Vector3 droneTopPosition = new Vector3(
            spawnedDrone.transform.position.x,
            spawnedDrone.transform.position.y + (2f * droneScale.y),
            spawnedDrone.transform.position.z - 0.1f
        );

        
        player.transform.SetParent(spawnedDrone.transform);
        player.transform.position = droneTopPosition;

        originalDronePosition = spawnedDrone.transform.position;

        StartCoroutine(StartDroneMission());
    }

    private IEnumerator StartDroneMission()
    {
        yield return new WaitForSeconds(1f);
        isDroneMovingToStartPoint = true;

    }

    private void MoveToStartPoint()
    {
        float step = moveSpeed * Time.deltaTime;
        spawnedDrone.transform.position = Vector3.MoveTowards(
            spawnedDrone.transform.position,
            startPoint.position,
            step
        );

        if (Vector3.Distance(spawnedDrone.transform.position, startPoint.position) < 0.1f)
        {
            isDroneMovingToStartPoint = false;
            isDroneRotating = true;
        }
    }

    private void RotateDrone()
    {
        droneCamera.Priority = 60;

        // Calculate the radius as the distance between startPoint and rotationCenter
        float radius = Vector3.Distance(startPoint.position, rotationCenter);

        // Increment the rotation angle
        rotationAngle += rotationSpeed * Time.deltaTime;

        // Calculate the new position of the drone along the circle
        float x = rotationCenter.x + radius * Mathf.Cos(rotationAngle);
        float z = rotationCenter.z + radius * Mathf.Sin(rotationAngle);

        // Preserve the current Y position of the drone
        float y = spawnedDrone.transform.position.y;

        Vector3 newPosition = new Vector3(x, y, z);
        spawnedDrone.transform.position = newPosition;

        // Optionally, make the drone face its next movement direction
        Vector3 lookDirection = newPosition - spawnedDrone.transform.position;
        if (lookDirection != Vector3.zero)
        {
            spawnedDrone.transform.rotation = Quaternion.LookRotation(lookDirection);
        }

        // Stop rotating after completing a full circle
        if (rotationAngle >= 2f * Mathf.PI) // 360 degrees in radians
        {
            isDroneRotating = false;
            isDroneReturning = true;
            rotationAngle = 0f; // Reset the angle for potential future rotations
        }
    }



    private void ReturnToOriginalPosition()
    {
        float step = moveSpeed * Time.deltaTime;
        spawnedDrone.transform.position = Vector3.MoveTowards(
            spawnedDrone.transform.position,
            originalDronePosition,
            step
        );

        if (Vector3.Distance(spawnedDrone.transform.position, originalDronePosition) < 0.1f)
        {
            isDroneReturning = false;
            DropOffPlayer();
        }
    }

    private void DropOffPlayer()
    {
        if (player != null)
        {
            player.transform.SetParent(null);
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.isKinematic = false;
            }

            normalCamera.Priority = 10;
            droneCamera.Priority = 0;
        }

        isDroneDescendingToStart = true;
    }

    private void ReturnToSpawnPoint()
    {
        float step = moveSpeed * Time.deltaTime;
        spawnedDrone.transform.position = Vector3.MoveTowards(
            spawnedDrone.transform.position,
            spawnPoint.position,
            step
        );

        if (Vector3.Distance(spawnedDrone.transform.position, spawnPoint.position) < 0.1f)
        {
            isDroneDescendingToStart = false;
            Destroy(spawnedDrone);
        }
    }
}
