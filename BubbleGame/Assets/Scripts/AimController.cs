using UnityEngine;

// AimController는 조준 방향과 조준선을 담당합니다.
// 발사 방향은 반드시 "firePoint에서 마우스 월드 위치로 가는 방향"으로 계산합니다.
public class AimController : MonoBehaviour
{
    [Header("조준 기준 설정")]
    [Tooltip("버블이 출발하는 위치입니다. 보통 슈터 앞쪽 빈 오브젝트를 연결합니다.")]
    [SerializeField] private Transform firePoint;

    [Tooltip("마우스 위치를 월드 좌표로 바꿀 카메라입니다. 비워두면 Main Camera를 사용합니다.")]
    [SerializeField] private Camera targetCamera;

    [Header("조준선 연결 설정")]
    [Tooltip("화면에 보이는 dotted line 조준선입니다. 연결하면 이 조준선 방향을 실제 발사 방향으로 우선 사용합니다.")]
    [SerializeField] private ShooterAimLineController visibleAimLineController;

    private Vector2 currentAimDirection = Vector2.up;

    public Vector2 CurrentAimDirection
    {
        get
        {
            if (visibleAimLineController == null)
            {
                visibleAimLineController = FindFirstObjectByType<ShooterAimLineController>();
            }

            if (visibleAimLineController != null)
            {
                Vector2 visibleDirection = visibleAimLineController.GetCurrentAimDirection();
                if (visibleDirection.sqrMagnitude > 0.001f)
                {
                    return KeepDirectionUpward(visibleDirection);
                }
            }

            return currentAimDirection;
        }
    }

    private void Awake()
    {
        PrepareReferences();
    }

    private void Update()
    {
        UpdateAimDirection();
    }

    private void PrepareReferences()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (firePoint == null)
        {
            firePoint = transform;
        }

    }

    private void UpdateAimDirection()
    {
        PrepareReferences();

        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = Mathf.Abs(targetCamera.transform.position.z - firePoint.position.z);
        Vector3 mouseWorldPosition = targetCamera.ScreenToWorldPoint(mouseScreenPosition);

        Vector2 direction = mouseWorldPosition - firePoint.position;
        if (direction.sqrMagnitude < 0.001f)
        {
            return;
        }

        direction.Normalize();

        currentAimDirection = KeepDirectionUpward(direction);
    }

    public Vector3[] GetCurrentAimLinePoints()
    {
        if (visibleAimLineController == null)
        {
            visibleAimLineController = FindFirstObjectByType<ShooterAimLineController>();
        }

        if (visibleAimLineController != null)
        {
            Vector3[] visiblePoints = visibleAimLineController.GetCurrentAimLinePoints();
            if (visiblePoints != null && visiblePoints.Length >= 2)
            {
                return visiblePoints;
            }
        }

        return null;
    }

    private Vector2 KeepDirectionUpward(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.001f)
        {
            return Vector2.up;
        }

        direction.Normalize();

        if (direction.y < 0f)
        {
            direction = -direction;
        }

        return direction;
    }
}
