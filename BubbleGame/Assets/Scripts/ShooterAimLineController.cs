using UnityEngine;

// ShooterAimLineController는 슈터 앞에 조준선을 그려주는 스크립트입니다.
// 조준 계산은 ShooterAimController가 담당하고, 이 스크립트는 그 방향에 맞춰 선만 그립니다.
public class ShooterAimLineController : MonoBehaviour
{
    [Header("조준선 표시 설정")]
    [Tooltip("체크되어 있으면 조준선을 보여줍니다.")]
    [SerializeField] private bool showAimLine = true;

    [Tooltip("조준선 방향 기준 Transform입니다. 보통 회전하는 ShooterVisual을 연결합니다. 비워두면 이 오브젝트를 사용합니다.")]
    [SerializeField] private Transform aimDirectionSource;

    // LineRenderer는 선을 그리는 Unity 컴포넌트입니다.
    // 사용자가 Inspector에서 직접 연결할 필요가 없어서 숨겨둡니다.
    private LineRenderer lineRenderer;

    [Header("조준선 모양 설정")]
    [Tooltip("슈터 중심에서 얼마나 앞쪽부터 선을 시작할지 정합니다.")]
    [SerializeField] private float lineStartOffset = 0.6f;

    [Tooltip("조준선 길이입니다.")]
    [SerializeField] private float lineLength = 5f;

    [Tooltip("벽에 부딪힌 뒤 최대 몇 번까지 반사해서 조준선을 그릴지 정합니다. 1이면 한 번만 꺾입니다.")]
    [SerializeField] private int maxReflections = 1;

    [Tooltip("조준선 Raycast가 먼저 감지할 벽 Layer입니다. 벽이 wall Layer라면 wall을 체크하세요. 잘못 설정해도 전체 Layer를 한 번 더 검사합니다.")]
    [SerializeField] private LayerMask wallLayerMask = 1;

    [Tooltip("반사 후 같은 벽을 다시 맞지 않도록 충돌 지점에서 아주 조금 떨어져 다시 검사하는 거리입니다.")]
    [SerializeField] private float raycastStartPadding = 0.02f;

    [Tooltip("조준선 두께입니다.")]
    [SerializeField] private float lineWidth = 0.05f;

    [Tooltip("조준선 색입니다.")]
    [SerializeField] private Color lineColor = new Color(1f, 1f, 1f, 0.75f);

    [Tooltip("포토샵에서 만든 점선 Sprite를 여기에 넣으면 그 이미지로 조준선을 표시합니다.")]
    [SerializeField] private Sprite aimLineSprite;

    [Tooltip("점선 이미지가 선을 따라 반복되는 정도입니다. 숫자가 클수록 점선이 더 촘촘하게 반복됩니다.")]
    [SerializeField] private float textureRepeatCount = 6f;

    [Tooltip("조준선이 배경보다 앞에 보이도록 하는 정렬 순서입니다.")]
    [SerializeField] private int sortingOrder = 20;

    [Header("방향 보정 설정")]
    [Tooltip("조준선이 기준 Transform의 어느 로컬 방향을 앞쪽으로 볼지 정합니다. 위쪽 기준이면 (0, 1), 오른쪽 기준이면 (1, 0)입니다.")]
    [SerializeField] private Vector2 aimLocalDirection = Vector2.up;

    // LineRenderer에 사용할 머티리얼입니다.
    // 머티리얼은 선에 색이나 이미지를 입히는 재료라고 생각하면 됩니다.
    private Material lineMaterial;

    // 조준선 꺾임 지점들을 담아두는 배열입니다.
    // 매 프레임 새 배열을 만들지 않기 위해 재사용합니다.
    private Vector3[] aimLinePoints;

    // Awake는 게임이 시작될 때 Start보다 먼저 한 번 호출됩니다.
    private void Awake()
    {
        // 조준선에 필요한 연결과 모양을 준비합니다.
        PrepareAimLine();
    }

    // Start는 게임이 시작될 때 한 번 호출됩니다.
    private void Start()
    {
        // 시작하자마자 조준선을 한 번 그립니다.
        UpdateAimLine();
    }

    // Update는 게임이 실행되는 동안 매 프레임 호출됩니다.
    private void Update()
    {
        // 슈터가 회전할 때마다 조준선도 따라가게 매 프레임 다시 그립니다.
        UpdateAimLine();
    }

    // 조준선에 필요한 컴포넌트와 기본 설정을 준비하는 함수입니다.
    private void PrepareAimLine()
    {
        // 방향 기준이 비어 있으면 이 스크립트가 붙은 오브젝트를 사용합니다.
        // 직접 조절하려면 Inspector의 Aim Direction Source에 ShooterVisual 등을 연결하세요.
        if (aimDirectionSource == null)
        {
            aimDirectionSource = transform;
        }

        // LineRenderer가 없으면 자동으로 찾거나 만듭니다.
        FindOrCreateLineRenderer();

        // LineRenderer의 색, 두께, 정렬을 적용합니다.
        ApplyLineRendererSettings();
    }

    // LineRenderer를 찾거나 만드는 함수입니다.
    private void FindOrCreateLineRenderer()
    {
        // 이미 연결되어 있으면 새로 찾을 필요가 없습니다.
        if (lineRenderer != null)
        {
            return;
        }

        // 이 오브젝트에 붙어 있는 LineRenderer를 찾아봅니다.
        lineRenderer = GetComponent<LineRenderer>();

        // 없으면 새로 붙입니다.
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
    }

    // LineRenderer의 모양 설정을 적용하는 함수입니다.
    private void ApplyLineRendererSettings()
    {
        if (lineRenderer == null)
        {
            return;
        }

        // 조준선은 시작점과 끝점 2개로 이루어진 직선입니다.
        lineRenderer.positionCount = 2;

        // 선이 오브젝트 로컬 좌표가 아니라 월드 좌표를 사용하게 합니다.
        lineRenderer.useWorldSpace = true;

        // 선 두께를 적용합니다.
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        // 조준선에 사용할 머티리얼을 준비합니다.
        PrepareLineMaterial();

        // 선 색을 적용합니다.
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;

        // 배경보다 앞에 보이게 정렬 순서를 설정합니다.
        lineRenderer.sortingOrder = sortingOrder;

        // 점선 이미지가 선 길이를 따라 반복되게 합니다.
        lineRenderer.textureMode = LineTextureMode.Tile;
    }

    // LineRenderer에 사용할 머티리얼과 점선 이미지를 준비하는 함수입니다.
    private void PrepareLineMaterial()
    {
        if (lineRenderer == null)
        {
            return;
        }

        // 머티리얼이 없으면 새로 만듭니다.
        if (lineMaterial == null)
        {
            lineMaterial = new Material(Shader.Find("Sprites/Default"));
        }

        // 포토샵 점선 Sprite가 연결되어 있으면 그 이미지를 선에 입힙니다.
        if (aimLineSprite != null)
        {
            Texture2D texture = aimLineSprite.texture;

            // Repeat은 이미지가 선을 따라 반복되게 하는 설정입니다.
            texture.wrapMode = TextureWrapMode.Repeat;

            lineMaterial.mainTexture = texture;
            lineMaterial.mainTextureScale = new Vector2(textureRepeatCount, 1f);
        }
        else
        {
            // Sprite가 없으면 기본 흰색 선으로 표시합니다.
            lineMaterial.mainTexture = null;
        }

        // 머티리얼을 LineRenderer에 적용합니다.
        lineRenderer.material = lineMaterial;
    }

    // 조준선 위치를 계산해서 그리는 함수입니다.
    private void UpdateAimLine()
    {
        // 조준선이 꺼져 있으면 LineRenderer도 꺼둡니다.
        if (!showAimLine)
        {
            if (lineRenderer != null)
            {
                lineRenderer.enabled = false;
            }

            return;
        }

        // 필요한 연결이 없으면 다시 준비합니다.
        PrepareAimLine();

        if (lineRenderer == null || aimDirectionSource == null)
        {
            return;
        }

        // 조준선을 켭니다.
        lineRenderer.enabled = true;

        // 매 프레임 Inspector 값 변경이 반영되게 모양 설정을 다시 적용합니다.
        ApplyLineRendererSettings();

        // 기준 Transform의 로컬 방향을 월드 방향으로 바꿉니다.
        Vector3 worldDirection = aimDirectionSource.TransformDirection(new Vector3(aimLocalDirection.x, aimLocalDirection.y, 0f));

        // 방향 길이를 1로 맞춥니다.
        worldDirection.Normalize();

        // 선 시작점은 슈터 위치에서 앞쪽으로 살짝 이동한 지점입니다.
        Vector3 startPoint = aimDirectionSource.position + worldDirection * lineStartOffset;

        // 벽에 닿으면 꺾이도록 조준선 경로를 계산해서 그립니다.
        DrawReflectedAimLine(startPoint, worldDirection);
    }

    // 벽에 닿으면 반사되는 조준선을 계산해서 LineRenderer에 넣는 함수입니다.
    private void DrawReflectedAimLine(Vector3 startPoint, Vector3 worldDirection)
    {
        // Inspector에서 이상한 값이 들어와도 안전하게 보정합니다.
        int safeMaxReflections = Mathf.Max(0, maxReflections);
        float safeLineLength = Mathf.Max(0.01f, lineLength);
        float safePadding = Mathf.Max(0.001f, raycastStartPadding);

        // 시작점 1개 + 반사 지점 개수 + 마지막 끝점 1개가 필요합니다.
        int neededPointCount = safeMaxReflections + 2;

        if (aimLinePoints == null || aimLinePoints.Length != neededPointCount)
        {
            aimLinePoints = new Vector3[neededPointCount];
        }

        Vector2 currentOrigin = startPoint;
        Vector2 currentDirection = new Vector2(worldDirection.x, worldDirection.y).normalized;
        float remainingLength = safeLineLength;
        int pointCount = 1;

        // 첫 번째 점은 항상 조준선 시작점입니다.
        aimLinePoints[0] = startPoint;

        for (int reflectionIndex = 0; reflectionIndex <= safeMaxReflections; reflectionIndex++)
        {
            // 남은 길이만큼 앞으로 Raycast를 쏩니다.
            // Raycast는 보이지 않는 선을 쏴서 Collider2D와 부딪혔는지 확인하는 기능입니다.
            RaycastHit2D hit = CastAimRay(currentOrigin, currentDirection, remainingLength);

            if (hit.collider == null)
            {
                // 벽에 닿지 않았다면 남은 길이만큼 직선으로 끝냅니다.
                aimLinePoints[pointCount] = currentOrigin + currentDirection * remainingLength;
                pointCount++;
                break;
            }

            // 벽에 닿았다면 충돌 지점을 조준선의 다음 점으로 추가합니다.
            aimLinePoints[pointCount] = hit.point;
            pointCount++;

            // 이미 최대 반사 횟수만큼 꺾었다면 여기서 조준선을 끝냅니다.
            if (reflectionIndex >= safeMaxReflections)
            {
                break;
            }

            remainingLength -= hit.distance;

            if (remainingLength <= 0f)
            {
                break;
            }

            // Reflect는 반사 방향을 계산해주는 함수입니다.
            // hit.normal은 벽 표면이 바라보는 방향입니다.
            currentDirection = Vector2.Reflect(currentDirection, hit.normal).normalized;

            // 방금 맞은 벽을 바로 다시 맞지 않게 충돌 지점에서 아주 조금 떨어져 다시 시작합니다.
            currentOrigin = hit.point + currentDirection * safePadding;
        }

        lineRenderer.positionCount = pointCount;

        for (int i = 0; i < pointCount; i++)
        {
            lineRenderer.SetPosition(i, aimLinePoints[i]);
        }
    }

    // 조준선 Raycast를 쏘는 함수입니다.
    // 먼저 Inspector의 Wall Layer Mask로 검사하고, 실패하면 전체 Layer를 한 번 더 검사합니다.
    private RaycastHit2D CastAimRay(Vector2 origin, Vector2 direction, float distance)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, wallLayerMask);

        if (hit.collider != null)
        {
            return hit;
        }

        // 초보자가 Layer Mask를 잘못 골라도 벽 반사 테스트가 되도록 안전장치를 둡니다.
        return Physics2D.Raycast(origin, direction, distance, Physics2D.DefaultRaycastLayers);
    }
}
