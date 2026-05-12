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

        // 선 끝점은 시작점에서 앞쪽으로 lineLength만큼 더 간 지점입니다.
        Vector3 endPoint = startPoint + worldDirection * lineLength;

        // LineRenderer에 시작점과 끝점을 넣어 선을 그립니다.
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
    }
}
