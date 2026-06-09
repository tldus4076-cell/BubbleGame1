using UnityEngine;

// BubbleProjectile은 발사된 버블을 target cell까지 이동시키는 스크립트입니다.
// Rigidbody2D 물리 충돌을 쓰지 않습니다.
public class BubbleProjectile : MonoBehaviour
{
    private BubbleGridManager gridManager;
    private BubbleSlot targetSlot;
    private float moveSpeed;
    private bool isMoving;
    private Vector3[] travelPoints;
    private int currentTravelPointIndex;

    public void LaunchToCell(BubbleGridManager gridManager, BubbleSlot targetSlot, float moveSpeed)
    {
        LaunchToCell(gridManager, targetSlot, moveSpeed, null);
    }

    public void LaunchToCell(BubbleGridManager gridManager, BubbleSlot targetSlot, float moveSpeed, Vector3[] aimLinePoints)
    {
        this.gridManager = gridManager;
        this.targetSlot = targetSlot;
        this.moveSpeed = Mathf.Max(0.1f, moveSpeed);
        travelPoints = CreateTravelPath(transform.position, targetSlot != null ? targetSlot.worldPosition : transform.position, aimLinePoints);
        currentTravelPointIndex = 0;
        isMoving = targetSlot != null;
    }

    private void Update()
    {
        if (!isMoving || targetSlot == null)
        {
            return;
        }

        Vector3 targetPosition = travelPoints != null && travelPoints.Length > 0
            ? travelPoints[currentTravelPointIndex]
            : targetSlot.worldPosition;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) <= 0.001f)
        {
            if (travelPoints != null && currentTravelPointIndex < travelPoints.Length - 1)
            {
                currentTravelPointIndex++;
            }
            else
            {
                SnapToTargetCell();
            }
        }
    }

    private Vector3[] CreateTravelPath(Vector3 startPosition, Vector3 targetPosition, Vector3[] aimLinePoints)
    {
        if (aimLinePoints == null || aimLinePoints.Length < 2)
        {
            return new[] { targetPosition };
        }

        int bestSegmentIndex = 0;
        float bestSegmentT = 0f;
        float bestDistance = float.MaxValue;

        for (int i = 0; i < aimLinePoints.Length - 1; i++)
        {
            Vector3 a = aimLinePoints[i];
            Vector3 b = aimLinePoints[i + 1];
            Vector3 ab = b - a;

            if (ab.sqrMagnitude < 0.001f)
            {
                continue;
            }

            float t = Vector3.Dot(targetPosition - a, ab) / ab.sqrMagnitude;
            t = Mathf.Clamp01(t);
            Vector3 closestPoint = a + ab * t;
            float distance = Vector3.Distance(targetPosition, closestPoint);

            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestSegmentIndex = i;
                bestSegmentT = t;
            }
        }

        int extraPointCount = bestSegmentIndex + 2;
        Vector3[] path = new Vector3[extraPointCount];

        int pathIndex = 0;
        for (int i = 1; i <= bestSegmentIndex; i++)
        {
            path[pathIndex] = aimLinePoints[i];
            pathIndex++;
        }

        Vector3 segmentStart = aimLinePoints[bestSegmentIndex];
        Vector3 segmentEnd = aimLinePoints[bestSegmentIndex + 1];
        Vector3 pointOnDottedLine = Vector3.Lerp(segmentStart, segmentEnd, bestSegmentT);
        path[pathIndex] = pointOnDottedLine;
        pathIndex++;

        path[pathIndex] = targetPosition;
        return path;
    }

    private void SnapToTargetCell()
    {
        isMoving = false;
        transform.position = targetSlot.worldPosition;

        if (gridManager != null)
        {
            gridManager.RegisterBubble(targetSlot, gameObject);
        }

        // 도착한 뒤에는 이동 스크립트가 더 필요 없습니다.
        Destroy(this);
    }
}
