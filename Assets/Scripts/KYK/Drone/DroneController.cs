using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DroneController : MonoBehaviour
{
    /// <summary>
    /// 드론의 기본 구성 요소들을 관리하는 클래스
    /// </summary>
    [System.Serializable]
    public class DroneComponents
    {
        public GameObject dronePrefab;        // 드론 프리팹
        public GameObject droneLight;         // 드론 라이트
        public Animator propellerAnimator;    // 프로펠러 애니메이터
    }

    /// <summary>
    /// 드론의 이동 경로 웨이포인트 설정을 관리하는 클래스
    /// </summary>
    [System.Serializable]
    public class WaypointSettings
    {
        public Transform[] leftWaypoints;     // 왼쪽 벽면 웨이포인트 배열
        public Transform[] centerWaypoints;   // 중앙 벽면 웨이포인트 배열
        public Transform[] rightWaypoints;    // 오른쪽 벽면 웨이포인트 배열
        public Transform basePoint;           // 드론 귀환 지점
    }

    /// <summary>
    /// 드론의 비행 관련 설정을 관리하는 클래스
    /// </summary>
    [System.Serializable]
    public class FlightSettings
    {
        public float speed = 5f;              // 비행 속도
        public float height = 10f;            // 비행 고도
        public float readySpeed = 2.5f;       // 이착륙 속도
        public float rotationDuration = 1.5f; // 회전에 걸리는 시간
        public float rotationAngle = 90f;     // 회전 각도
        public float waypointThreshold = 0.1f;// 웨이포인트 도달 판정 거리
    }

    /// <summary>
    /// 드론의 현재 상태를 나타내는 열거형
    /// </summary>
    public enum DroneState
    {
        TakeOff,        // 이륙
        LeftFlight,     // 왼쪽 벽면 비행
        CenterFlight,   // 중앙 벽면 비행
        RightFlight,    // 오른쪽 벽면 비행
        Return,         // 기지로 귀환
        Landing         // 착륙
    }

    [SerializeField] private DroneComponents droneComponents;   // 드론 구성요소
    [SerializeField] private WaypointSettings waypoints;       // 웨이포인트 설정
    [SerializeField] private FlightSettings flightSettings;    // 비행 설정

    private DroneState currentState = DroneState.TakeOff;      // 현재 드론 상태
    private int currentWaypointIndex;                         // 현재 웨이포인트 인덱스
    private bool isDroneActive;                               // 드론 활성화 상태
    private bool isRotating;                                  // 회전 상태

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
    /// 드론 초기화 메서드
    /// </summary>
    private void InitializeDrone()
    {
        currentWaypointIndex = 0;
        droneComponents.droneLight.SetActive(false);
        ValidateWaypoints();
    }

    /// <summary>
    /// 드론 미션 시작 메서드
    /// </summary>
    public void StartDroneMission()
    {
        isDroneActive = true;
    }

    /// <summary>
    /// 드론의 현재 상태에 따른 행동 업데이트
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
    /// 이륙 처리 메서드
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
    /// 지정된 웨이포인트를 따라 비행하는 메서드
    /// </summary>
    /// <param name="currentWaypoints">현재 경로의 웨이포인트 배열</param>
    /// <param name="nextState">다음 상태</param>
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
    /// 회전과 비행을 처리하는 메서드
    /// </summary>
    /// <param name="currentWaypoints">현재 경로의 웨이포인트 배열</param>
    /// <param name="nextState">다음 드론 상태</param>
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
    /// 기지로 귀환하는 과정을 처리하는 메서드
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
    /// 착륙 과정을 처리하는 메서드
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
    /// 목표 지점으로 드론을 이동시키는 메서드
    /// </summary>
    /// <param name="targetPoint">목표 위치</param>
    private void MoveTowardsTarget(Vector3 targetPoint)
    {
        Vector3 direction = (targetPoint - droneComponents.dronePrefab.transform.position).normalized;
        droneComponents.dronePrefab.transform.Translate(
            direction * flightSettings.speed * Time.deltaTime,
            Space.World
        );
    }

    /// <summary>
    /// 드론을 회전시키는 코루틴
    /// </summary>
    /// <returns>실행 대기를 위한 IEnumerator</returns>
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
    /// 프로펠러 회전을 제어하는 메서드
    /// </summary>
    /// <param name="isStart">true: 회전 시작, false: 회전 정지</param>
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
    /// 웨이포인트 설정의 유효성을 검사하는 메서드
    /// </summary>
    private void ValidateWaypoints()
    {
        if (waypoints.leftWaypoints.Length == 0)
        {
            Debug.LogError("Left waypoints are not set!");
        }
    }
}