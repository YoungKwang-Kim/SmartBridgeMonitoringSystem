using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DroneController : MonoBehaviour
{
    /// <summary>
    /// ����� �⺻ ���� ��ҵ��� �����ϴ� Ŭ����
    /// </summary>
    [System.Serializable]
    public class DroneComponents
    {
        public GameObject dronePrefab;        // ��� ������
        public GameObject droneLight;         // ��� ����Ʈ
        public Animator propellerAnimator;    // �����緯 �ִϸ�����
    }

    /// <summary>
    /// ����� �̵� ��� ��������Ʈ ������ �����ϴ� Ŭ����
    /// </summary>
    [System.Serializable]
    public class WaypointSettings
    {
        public Transform[] leftWaypoints;     // ���� ���� ��������Ʈ �迭
        public Transform[] centerWaypoints;   // �߾� ���� ��������Ʈ �迭
        public Transform[] rightWaypoints;    // ������ ���� ��������Ʈ �迭
        public Transform basePoint;           // ��� ��ȯ ����
    }

    /// <summary>
    /// ����� ���� ���� ������ �����ϴ� Ŭ����
    /// </summary>
    [System.Serializable]
    public class FlightSettings
    {
        public float speed = 5f;              // ���� �ӵ�
        public float height = 10f;            // ���� ��
        public float readySpeed = 2.5f;       // ������ �ӵ�
        public float rotationDuration = 1.5f; // ȸ���� �ɸ��� �ð�
        public float rotationAngle = 90f;     // ȸ�� ����
        public float waypointThreshold = 0.1f;// ��������Ʈ ���� ���� �Ÿ�
    }

    /// <summary>
    /// ����� ���� ���¸� ��Ÿ���� ������
    /// </summary>
    public enum DroneState
    {
        TakeOff,        // �̷�
        LeftFlight,     // ���� ���� ����
        CenterFlight,   // �߾� ���� ����
        RightFlight,    // ������ ���� ����
        Return,         // ������ ��ȯ
        Landing         // ����
    }

    [SerializeField] private DroneComponents droneComponents;   // ��� �������
    [SerializeField] private WaypointSettings waypoints;       // ��������Ʈ ����
    [SerializeField] private FlightSettings flightSettings;    // ���� ����

    private DroneState currentState = DroneState.TakeOff;      // ���� ��� ����
    private int currentWaypointIndex;                         // ���� ��������Ʈ �ε���
    private bool isDroneActive;                               // ��� Ȱ��ȭ ����
    private bool isRotating;                                  // ȸ�� ����

    private void Start()
    {
        InitializeDrone();
    }

    private void Update()
    {
        if (isDroneActive)
        {
            UpdateDroneState();
        }
    }

    /// <summary>
    /// ��� �ʱ�ȭ �޼���
    /// </summary>
    private void InitializeDrone()
    {
        currentWaypointIndex = 0;
        droneComponents.droneLight.SetActive(false);
        ValidateWaypoints();
    }

    /// <summary>
    /// ��� �̼� ���� �޼���
    /// </summary>
    public void StartDroneMission()
    {
        isDroneActive = true;
    }

    /// <summary>
    /// ����� ���� ���¿� ���� �ൿ ������Ʈ
    /// </summary>
    private void UpdateDroneState()
    {
        switch (currentState)
        {
            case DroneState.TakeOff:
                HandleTakeOff();
                break;
            case DroneState.LeftFlight:
                HandleFlightPath(waypoints.leftWaypoints, DroneState.CenterFlight);
                break;
            case DroneState.CenterFlight:
                HandleRotationAndFlight(waypoints.centerWaypoints, DroneState.RightFlight);
                break;
            case DroneState.RightFlight:
                HandleRotationAndFlight(waypoints.rightWaypoints, DroneState.Return);
                break;
            case DroneState.Return:
                HandleReturn();
                break;
            case DroneState.Landing:
                HandleLanding();
                break;
        }
    }

    /// <summary>
    /// �̷� ó�� �޼���
    /// </summary>
    private void HandleTakeOff()
    {
        droneComponents.droneLight.SetActive(true);
        ControlPropellers(true);

        float newHeight = droneComponents.dronePrefab.transform.position.y +
                         (flightSettings.readySpeed * Time.deltaTime);

        droneComponents.dronePrefab.transform.Translate(Vector3.up * flightSettings.readySpeed * Time.deltaTime);

        if (newHeight > waypoints.leftWaypoints[0].position.y)
        {
            currentState = DroneState.LeftFlight;
        }
    }

    /// <summary>
    /// ������ ��������Ʈ�� ���� �����ϴ� �޼���
    /// </summary>
    /// <param name="currentWaypoints">���� ����� ��������Ʈ �迭</param>
    /// <param name="nextState">���� ����</param>
    private void HandleFlightPath(Transform[] currentWaypoints, DroneState nextState)
    {
        if (Vector3.Distance(currentWaypoints[currentWaypointIndex].position,
                           droneComponents.dronePrefab.transform.position) >
            flightSettings.waypointThreshold)
        {
            MoveTowardsTarget(currentWaypoints[currentWaypointIndex].position);
        }
        else
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= currentWaypoints.Length)
            {
                currentWaypointIndex = 0;
                isRotating = true;
                currentState = nextState;
            }
        }
    }

    /// <summary>
    /// ȸ���� ������ ó���ϴ� �޼���
    /// </summary>
    /// <param name="currentWaypoints">���� ����� ��������Ʈ �迭</param>
    /// <param name="nextState">���� ��� ����</param>
    private void HandleRotationAndFlight(Transform[] currentWaypoints, DroneState nextState)
    {
        if (isRotating)
        {
            StartCoroutine(RotateDrone());
            isRotating = false;
        }
        else
        {
            HandleFlightPath(currentWaypoints, nextState);
        }
    }

    /// <summary>
    /// ������ ��ȯ�ϴ� ������ ó���ϴ� �޼���
    /// </summary>
    private void HandleReturn()
    {
        droneComponents.droneLight.SetActive(false);
        Vector3 returnPoint = new Vector3(
            waypoints.basePoint.position.x,
            droneComponents.dronePrefab.transform.position.y,
            waypoints.basePoint.position.z
        );

        MoveTowardsTarget(returnPoint);

        if (Vector3.Distance(returnPoint, droneComponents.dronePrefab.transform.position) < flightSettings.waypointThreshold)
        {
            currentState = DroneState.Landing;
        }
    }

    /// <summary>
    /// ���� ������ ó���ϴ� �޼���
    /// </summary>
    private void HandleLanding()
    {
        droneComponents.dronePrefab.transform.Translate(Vector3.down * flightSettings.readySpeed * Time.deltaTime);

        if (droneComponents.dronePrefab.transform.position.y < waypoints.basePoint.position.y + flightSettings.waypointThreshold)
        {
            flightSettings.readySpeed = 0;
            ControlPropellers(false);
        }
    }

    /// <summary>
    /// ��ǥ �������� ����� �̵���Ű�� �޼���
    /// </summary>
    /// <param name="targetPoint">��ǥ ��ġ</param>
    private void MoveTowardsTarget(Vector3 targetPoint)
    {
        Vector3 direction = (targetPoint - droneComponents.dronePrefab.transform.position).normalized;
        droneComponents.dronePrefab.transform.Translate(
            direction * flightSettings.speed * Time.deltaTime,
            Space.World
        );
    }

    /// <summary>
    /// ����� ȸ����Ű�� �ڷ�ƾ
    /// </summary>
    /// <returns>���� ��⸦ ���� IEnumerator</returns>
    private IEnumerator RotateDrone()
    {
        float elapsedTime = 0f;
        Quaternion startRotation = droneComponents.dronePrefab.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(Vector3.up * flightSettings.rotationAngle) * startRotation;

        while (elapsedTime < flightSettings.rotationDuration)
        {
            float progress = Mathf.Clamp01(elapsedTime / flightSettings.rotationDuration);
            droneComponents.dronePrefab.transform.rotation =
                Quaternion.Lerp(startRotation, targetRotation, progress);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        droneComponents.dronePrefab.transform.rotation = targetRotation;
    }

    /// <summary>
    /// �����緯 ȸ���� �����ϴ� �޼���
    /// </summary>
    /// <param name="isStart">true: ȸ�� ����, false: ȸ�� ����</param>
    private void ControlPropellers(bool isStart)
    {
        if (droneComponents.propellerAnimator != null)
        {
            float weight = isStart ? 1f : 0f;
            for (int i = 0; i < 5; i++)
            {
                droneComponents.propellerAnimator.SetLayerWeight(i, weight);
            }
        }
    }

    /// <summary>
    /// ��������Ʈ ������ ��ȿ���� �˻��ϴ� �޼���
    /// </summary>
    private void ValidateWaypoints()
    {
        if (waypoints.leftWaypoints.Length == 0)
        {
            Debug.LogError("Left waypoints are not set!");
        }
    }
}