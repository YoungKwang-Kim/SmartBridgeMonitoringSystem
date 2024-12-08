using UnityEngine;
using System.Collections;

public class DronePathRenderer : MonoBehaviour
{
    /// <summary>
    /// ��� ������ ���� ������ �����ϴ� Ŭ����
    /// </summary>
    [System.Serializable]
    public class PathSettings
    {
        public Color startColor = Color.white;         // ��� ���� ����
        public Color endColor = new Color(1, 1, 1, 0); // ���̵� �ƿ� �� ���� ����
        public float fadeDuration = 1.0f;             // ���̵� �ƿ� ���� �ð�
        public float lineWidth = 0.1f;                // ���� �β�
        public bool autoFade = true;                  // �ڵ� ���̵� Ȱ��ȭ ����
        public bool loopPath = false;                 // ��ȯ ��� ����
    }

    [Header("Path Components")]
    [SerializeField] private LineRenderer lineRenderer;  // ��� ������
    [SerializeField] private Transform[] waypoints;      // ��� ��������Ʈ �迭

    [Header("Path Settings")]
    [SerializeField] private PathSettings settings = new PathSettings();

    private float startTime;
    private bool isFading = false;

    /// <summary>
    /// �����Ϳ��� ���� ����� �� ȣ��Ǵ� �޼���
    /// </summary>
    private void OnValidate()
    {
        InitializeLineRenderer();
    }

    /// <summary>
    /// �ʱ�ȭ �� ��� �׸��⸦ �����ϴ� �޼���
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
    /// LineRenderer ������Ʈ �ʱ�ȭ
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
    /// ��������Ʈ�� ���� ��θ� �׸��� �޼���
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

        // ��ȯ ����� ��� ������ ���� ó�� ���� ����
        if (settings.loopPath && waypoints.Length > 0)
        {
            lineRenderer.SetPosition(waypoints.Length, waypoints[0].position);
        }
    }

    /// <summary>
    /// ��θ� ���� ������� �ϴ� �ڷ�ƾ
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
    /// ��θ� ���̰� �ϴ� �޼���
    /// </summary>
    public void ShowPath()
    {
        StopAllCoroutines();
        lineRenderer.enabled = true;
        SetPathColor(settings.startColor);
    }

    /// <summary>
    /// ��θ� ����� �޼���
    /// </summary>
    public void HidePath()
    {
        lineRenderer.enabled = false;
    }

    /// <summary>
    /// ���̵� �ƿ� ȿ���� �����ϴ� �޼���
    /// </summary>
    public void StartFading()
    {
        StartCoroutine(FadePath());
    }

    /// <summary>
    /// ��� ������ �����ϴ� �޼���
    /// </summary>
    private void SetPathColor(Color color)
    {
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    /// <summary>
    /// ��������Ʈ�� ������Ʈ�ϴ� �޼���
    /// </summary>
    /// <param name="newWaypoints">���ο� ��������Ʈ �迭</param>
    public void UpdateWaypoints(Transform[] newWaypoints)
    {
        waypoints = newWaypoints;
        InitializeLineRenderer();
        DrawPath();
    }

#if UNITY_EDITOR
    /// <summary>
    /// �����Ϳ��� ��θ� �ð�ȭ�ϴ� �޼���
    /// </summary>
    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        // �����Ϳ��� ��� �̸�����
        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }

        // ��ȯ ����� ��� ������ ���� ù ���� ����
        if (settings.loopPath && waypoints.Length > 1)
        {
            Gizmos.DrawLine(waypoints[waypoints.Length - 1].position, waypoints[0].position);
        }
    }
#endif
}