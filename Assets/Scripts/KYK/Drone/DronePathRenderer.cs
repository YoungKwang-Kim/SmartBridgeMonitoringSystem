using UnityEngine;
using System.Collections;

public class DronePathRenderer : MonoBehaviour
{
    /// <summary>
    /// 경로 렌더링 관련 설정을 관리하는 클래스
    /// </summary>
    [System.Serializable]
    public class PathSettings
    {
        public Color startColor = Color.white;         // 경로 시작 색상
        public Color endColor = new Color(1, 1, 1, 0); // 페이드 아웃 시 최종 색상
        public float fadeDuration = 1.0f;             // 페이드 아웃 지속 시간
        public float lineWidth = 0.1f;                // 라인 두께
        public bool autoFade = true;                  // 자동 페이드 활성화 여부
        public bool loopPath = false;                 // 순환 경로 여부
    }

    [Header("Path Components")]
    [SerializeField] private LineRenderer lineRenderer;  // 경로 렌더러
    [SerializeField] private Transform[] waypoints;      // 경로 웨이포인트 배열

    [Header("Path Settings")]
    [SerializeField] private PathSettings settings = new PathSettings();

    private float startTime;
    private bool isFading = false;

    /// <summary>
    /// 에디터에서 값이 변경될 때 호출되는 메서드
    /// </summary>
    private void OnValidate()
    {
        InitializeLineRenderer();
    }

    /// <summary>
    /// 초기화 및 경로 그리기를 시작하는 메서드
    /// </summary>
    private void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        InitializeLineRenderer();
        DrawPath();

        if (settings.autoFade)
        {
            StartCoroutine(FadePath());
        }
    }

    /// <summary>
    /// LineRenderer 컴포넌트 초기화
    /// </summary>
    private void InitializeLineRenderer()
    {
        if (lineRenderer == null || waypoints == null) return;

        lineRenderer.startWidth = settings.lineWidth;
        lineRenderer.endWidth = settings.lineWidth;
        lineRenderer.startColor = settings.startColor;
        lineRenderer.endColor = settings.startColor;
        lineRenderer.positionCount = settings.loopPath ? waypoints.Length + 1 : waypoints.Length;
        lineRenderer.useWorldSpace = true;
    }

    /// <summary>
    /// 웨이포인트를 따라 경로를 그리는 메서드
    /// </summary>
    public void DrawPath()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning("No waypoints assigned to DronePathRenderer!");
            return;
        }

        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] != null)
            {
                lineRenderer.SetPosition(i, waypoints[i].position);
            }
            else
            {
                Debug.LogError($"Waypoint at index {i} is null!");
            }
        }

        // 순환 경로인 경우 마지막 점을 처음 점과 연결
        if (settings.loopPath && waypoints.Length > 0)
        {
            lineRenderer.SetPosition(waypoints.Length, waypoints[0].position);
        }
    }

    /// <summary>
    /// 경로를 점차 사라지게 하는 코루틴
    /// </summary>
    private IEnumerator FadePath()
    {
        if (isFading) yield break;

        isFading = true;
        float elapsedTime = 0f;
        Color startColor = settings.startColor;
        Color endColor = settings.endColor;

        while (elapsedTime < settings.fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / settings.fadeDuration;

            Color currentColor = Color.Lerp(startColor, endColor, normalizedTime);
            lineRenderer.startColor = currentColor;
            lineRenderer.endColor = currentColor;

            yield return null;
        }

        isFading = false;
    }

    /// <summary>
    /// 경로를 보이게 하는 메서드
    /// </summary>
    public void ShowPath()
    {
        StopAllCoroutines();
        lineRenderer.enabled = true;
        SetPathColor(settings.startColor);
    }

    /// <summary>
    /// 경로를 숨기는 메서드
    /// </summary>
    public void HidePath()
    {
        lineRenderer.enabled = false;
    }

    /// <summary>
    /// 페이드 아웃 효과를 시작하는 메서드
    /// </summary>
    public void StartFading()
    {
        StartCoroutine(FadePath());
    }

    /// <summary>
    /// 경로 색상을 설정하는 메서드
    /// </summary>
    private void SetPathColor(Color color)
    {
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    /// <summary>
    /// 웨이포인트를 업데이트하는 메서드
    /// </summary>
    /// <param name="newWaypoints">새로운 웨이포인트 배열</param>
    public void UpdateWaypoints(Transform[] newWaypoints)
    {
        waypoints = newWaypoints;
        InitializeLineRenderer();
        DrawPath();
    }

#if UNITY_EDITOR
    /// <summary>
    /// 에디터에서 경로를 시각화하는 메서드
    /// </summary>
    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        // 에디터에서 경로 미리보기
        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }

        // 순환 경로인 경우 마지막 점과 첫 점을 연결
        if (settings.loopPath && waypoints.Length > 1)
        {
            Gizmos.DrawLine(waypoints[waypoints.Length - 1].position, waypoints[0].position);
        }
    }
#endif
}