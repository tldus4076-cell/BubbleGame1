using UnityEngine;

// ExecuteAlways는 "플레이 중이 아니어도 스크립트를 실행한다"는 뜻입니다.
// 이 스크립트는 플레이 전에도 Game 창에서 버블 배치를 미리 보기 위해 사용합니다.
[ExecuteAlways]
public class StageBubbleLayout : MonoBehaviour
{
    [Header("버블 배치 기본 설정")]
    [Tooltip("버블 줄 수입니다. Stage 1은 4줄을 사용합니다.")]
    public int rows = 4;

    [Tooltip("가장 윗줄의 버블 개수입니다. Stage 1은 6개를 사용합니다.")]
    public int cols = 6;

    [Tooltip("예비 간격값입니다. 실제 정렬은 왼쪽벽과 오른쪽벽 사이를 기준으로 자동 계산됩니다.")]
    public float bubbleSpacing = 0.8f;

    [Tooltip("버블이 한 칸 안에서 차지하는 비율입니다. 1보다 크면 버블끼리 조금 겹칩니다. 추천: 0.9~1.15")]
    public float bubbleVisualScale = 0.8f;

    [Tooltip("체크하면 버블슈터처럼 줄이 조금씩 밀린 모양으로 배치됩니다.")]
    public bool useStaggeredRows = true;

    [Tooltip("천장에서 아래로 얼마나 떨어진 곳에 첫 줄을 놓을지 정합니다.")]
    public float startYOffset = 1.0f;

    [Tooltip("왼쪽 벽과 오른쪽 벽에서 조금 띄울 간격입니다.")]
    public float horizontalPadding = 0.15f;

    [Tooltip("천장에서 조금 띄울 간격입니다.")]
    public float ceilingPadding = 0.15f;

    [Header("버블 이미지 설정")]
    [Tooltip("버블 이미지 목록입니다. 빨강, 파랑, 노랑 순서로 넣는 것을 추천합니다.")]
    public Sprite[] bubbleSprites;

    [Tooltip("체크하면 PNG 파일 대신 코드로 만든 깨끗한 원형 버블을 사용합니다. 정렬 테스트에는 체크를 추천합니다.")]
    public bool useGeneratedCircleSprites = true;

    // 코드로 만든 빨강, 파랑, 노랑 원형 버블 Sprite를 저장해둡니다.
    // 이렇게 해두면 매번 새로 만들지 않아도 됩니다.
    private Sprite[] generatedCircleSprites;

    // 이 스크립트가 만든 버블 이름 앞에는 항상 Bubble_을 붙입니다.
    // 나중에 지울 때, 이 이름으로 구분하기 위해 사용합니다.
    private const string GeneratedBubblePrefix = "Bubble_";

    // Stage 1 기획서에 있는 예시 배치를 숫자로 표현한 것입니다.
    // int[][]는 "숫자 표"라고 생각하면 됩니다.
    // 바깥쪽 줄은 세로 줄(row), 안쪽 숫자는 가로 칸(col)을 뜻합니다.
    // 0 = 첫 번째 이미지, 1 = 두 번째 이미지, 2 = 세 번째 이미지입니다.
    // Inspector의 Bubble Sprites에 빨강, 파랑, 노랑 순서로 넣으면
    // 0은 빨강, 1은 파랑, 2는 노랑이 됩니다.
    private static readonly int[][] Stage1Pattern =
    {
        new[] { 0, 0, 1, 1, 2, 2 },
        new[] { 0, 0, 1, 1, 2 },
        new[] { 0, 1, 2, 0, 1 },
        new[] { 2, 2, 1, 0 }
    };

#if UNITY_EDITOR
    // UNITY_EDITOR는 "Unity 에디터에서만 이 코드를 사용한다"는 뜻입니다.
    // 실제 게임으로 빌드하면 이 안의 코드는 포함되지 않습니다.

    // OnEnable은 이 컴포넌트가 켜질 때 실행됩니다.
    // 플레이 전에도 Game 창에서 버블을 미리 볼 수 있게 다시 생성합니다.
    private void OnEnable()
    {
        if (!Application.isPlaying)
        {
            UnityEditor.EditorApplication.delayCall += RebuildEditorBubbles;
            UnityEditor.SceneView.duringSceneGui += HideEditorPreviewLayerInSceneView;
            UnityEditor.EditorApplication.update += HideEditorPreviewBubblesInSceneView;

            // delayCall은 "지금 바로 실행하지 말고, Unity가 준비된 다음 한 번 실행해줘"라는 뜻입니다.
            // SceneView는 Scene 창이고, update는 에디터가 계속 반복 실행하는 흐름입니다.
        }
    }

    // OnDisable은 이 컴포넌트가 꺼질 때 실행됩니다.
    // 에디터 이벤트 연결을 해제해서 불필요하게 계속 실행되지 않게 합니다.
    private void OnDisable()
    {
        // +=로 연결했던 이벤트는 -=로 꼭 해제합니다.
        // 해제하지 않으면 스크립트가 꺼져도 계속 실행될 수 있습니다.
        UnityEditor.SceneView.duringSceneGui -= HideEditorPreviewLayerInSceneView;
        UnityEditor.EditorApplication.update -= HideEditorPreviewBubblesInSceneView;
    }

    // OnValidate는 Inspector에서 값을 바꿀 때 실행됩니다.
    // 예를 들어 Bubble Spacing 값을 바꾸면 버블 배치를 바로 다시 만듭니다.
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            UnityEditor.EditorApplication.delayCall += RebuildEditorBubbles;
        }
    }

    // 플레이 전 미리보기 버블을 다시 만드는 함수입니다.
    private void RebuildEditorBubbles()
    {
        if (this == null || Application.isPlaying)
        {
            // this == null은 이 스크립트가 삭제된 상태인지 확인하는 안전장치입니다.
            // Application.isPlaying은 현재 Play 버튼을 누른 상태인지 확인합니다.
            return;
        }

        // 기존에 만든 미리보기 버블을 먼저 지웁니다.
        ClearGeneratedBubblesImmediate();

        // 새 값으로 버블을 다시 만듭니다.
        CreateBubbles();
    }

    // SceneView는 Unity의 Scene 창을 뜻합니다.
    // 여기서는 Scene 창을 다시 그리게 해서 숨김 처리가 적용되도록 합니다.
    private void HideEditorPreviewLayerInSceneView(UnityEditor.SceneView sceneView)
    {
        sceneView.Repaint();
    }

    // 모든 Scene 창을 계속 다시 그리게 합니다.
    // 플레이 전에는 Scene 창에서 버블이 보이지 않도록 보조하는 역할입니다.
    private void HideEditorPreviewBubblesInSceneView()
    {
        // Scene 창이 여러 개 열려 있을 수도 있어서 모두 반복합니다.
        foreach (UnityEditor.SceneView sceneView in UnityEditor.SceneView.sceneViews)
        {
            sceneView.Repaint();
        }
    }
#endif

    // Start는 게임을 플레이할 때 처음 한 번 실행됩니다.
    private void Start()
    {
        // ExecuteAlways 때문에 플레이 전에도 Start가 호출될 수 있습니다.
        // 플레이 중이 아니면 여기서 멈춥니다.
        if (!Application.isPlaying)
        {
            return;
        }

        // 이전에 만들어진 버블을 지우고, 실제 플레이용 버블을 다시 만듭니다.
        ClearGeneratedBubbles();
        CreateBubbles();
    }

    // 플레이 중에 생성된 버블을 지우는 함수입니다.
    private void ClearGeneratedBubbles()
    {
        // 자식 오브젝트를 뒤에서부터 확인합니다.
        // 삭제할 때는 뒤에서부터 지우는 것이 안전합니다.
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            // transform.childCount는 WallsRoot 아래 자식 오브젝트 개수입니다.
            // GetChild(i)는 i번째 자식 오브젝트를 가져오는 함수입니다.
            Transform child = transform.GetChild(i);

            // 이름이 Bubble_로 시작하면 이 스크립트가 만든 버블입니다.
            if (child.name.StartsWith(GeneratedBubblePrefix))
            {
                Destroy(child.gameObject);
            }
        }
    }

    // 플레이 전 에디터 상태에서 생성된 버블을 즉시 지우는 함수입니다.
    private void ClearGeneratedBubblesImmediate()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            // 에디터 상태에서는 Destroy를 쓰면 오류가 날 수 있어서 DestroyImmediate를 씁니다.
            Transform child = transform.GetChild(i);

            if (child.name.StartsWith(GeneratedBubblePrefix))
            {
                // DestroyImmediate는 "바로 삭제"라는 뜻입니다.
                // 플레이 전 에디터 상태에서는 Destroy 대신 DestroyImmediate를 써야 합니다.
                DestroyImmediate(child.gameObject);
            }
        }
    }

    // 실제로 버블을 만드는 함수입니다.
    private void CreateBubbles()
    {
        // 실제로 사용할 버블 이미지 목록을 가져옵니다.
        // useGeneratedCircleSprites가 켜져 있으면 코드로 만든 원형 이미지를 사용합니다.
        Sprite[] activeBubbleSprites = GetActiveBubbleSprites();

        // 버블 이미지가 없으면 만들 수 없으므로 멈춥니다.
        if (activeBubbleSprites == null || activeBubbleSprites.Length == 0)
        {
            Debug.LogWarning("Bubble Sprites가 비어 있습니다. Inspector에서 빨강, 파랑, 노랑 이미지를 넣어주세요.");
            return;
        }

        // WallsRoot 안에 있는 벽 오브젝트를 이름으로 찾습니다.
        Transform leftWall = transform.Find("LeftWall");
        Transform rightWall = transform.Find("RightWall");
        Transform ceiling = transform.Find("Ceiling");

        // 벽 오브젝트가 없으면 배치 기준을 알 수 없으므로 멈춥니다.
        if (leftWall == null || rightWall == null || ceiling == null)
        {
            Debug.LogError("WallsRoot 안에 LeftWall, RightWall, Ceiling 오브젝트가 있어야 합니다.");
            return;
        }

        // LeftWall의 오른쪽 끝, RightWall의 왼쪽 끝, Ceiling의 아래쪽 끝을 구합니다.
        // 이렇게 해야 버블이 벽 안쪽에 배치됩니다.
        float leftX = transform.InverseTransformPoint(GetWorldBounds(leftWall).max).x + horizontalPadding;
        float rightX = transform.InverseTransformPoint(GetWorldBounds(rightWall).min).x - horizontalPadding;
        float ceilingY = transform.InverseTransformPoint(GetWorldBounds(ceiling).min).y - ceilingPadding;

        // InverseTransformPoint는 "월드 좌표"를 "WallsRoot 기준 좌표"로 바꿔줍니다.
        // 월드 좌표는 씬 전체 기준 위치이고, 로컬 좌표는 부모 오브젝트 기준 위치입니다.

        // 벽 안쪽의 실제 사용 가능한 가로 길이입니다.
        float availableWidth = Mathf.Max(rightX - leftX, bubbleSpacing);

        // Mathf.Max는 두 값 중 더 큰 값을 고르는 함수입니다.
        // 벽 사이 길이가 너무 작게 계산되어도 최소한 bubbleSpacing만큼은 확보합니다.

        // 버블 6개가 벽 안쪽을 꽉 채우도록 한 칸의 크기를 계산합니다.
        // 예: 벽 사이가 6칸이고 cols가 6이면, 한 칸 크기 = 벽 사이 너비 / 6 입니다.
        float spacingToFit = cols > 0 ? availableWidth / cols : availableWidth;

        // ? : 는 간단한 if문입니다.
        // cols가 0보다 크면 availableWidth / cols를 쓰고,
        // 아니면 availableWidth를 그대로 씁니다.

        // 최종 간격은 벽 안쪽을 꽉 채우는 값으로 사용합니다.
        // 이렇게 해야 첫 줄 6개가 LeftWall과 RightWall 사이에 일정하게 들어갑니다.
        float finalSpacing = spacingToFit;

        // finalSpacing은 버블 하나가 차지하는 "한 칸" 크기라고 생각하면 됩니다.

        // 버블이 한 칸 안에서 차지할 실제 크기입니다.
        // bubbleVisualScale은 이제 "직접 크기"가 아니라 "칸 크기 대비 비율"입니다.
        // 예: finalSpacing이 0.7이고 bubbleVisualScale이 0.9이면 실제 크기는 0.63입니다.
        // 예: finalSpacing이 0.7이고 bubbleVisualScale이 1.1이면 실제 크기는 0.77이라서 살짝 겹칩니다.
        // Clamp는 값을 정해진 범위 안으로 제한하는 함수입니다.
        // 여기서는 너무 작거나 너무 커지는 것을 막기 위해 0.1~1.3 사이로 제한합니다.
        float finalBubbleDiameter = finalSpacing * Mathf.Clamp(bubbleVisualScale, 0.1f, 1.3f);

        // bubbleVisualScale이 1이면 칸을 거의 꽉 채웁니다.
        // bubbleVisualScale이 0.9이면 칸보다 살짝 작게 보여서 버블 사이에 작은 틈이 생깁니다.

        // 첫 번째 줄의 Y 위치입니다.
        // 버블 중심을 천장 아래에 놓아야 하므로 버블 반지름(finalBubbleDiameter / 2)을 빼줍니다.
        float startY = ceilingY - finalBubbleDiameter / 2f - startYOffset;

        // 세로 간격입니다.
        // 지그재그 배치일 때는 버블슈터처럼 조금 더 촘촘한 세로 간격을 사용합니다.
        float verticalSpacing = useStaggeredRows ? finalSpacing * Mathf.Sqrt(3f) / 2f : finalSpacing;

        // Mathf.Sqrt는 제곱근을 구하는 함수입니다.
        // 버블슈터의 지그재그 배치에서는 세로 간격을 조금 줄여야 자연스럽게 붙습니다.

        // Stage 1은 기획서 기준 최대 4줄입니다.
        int totalRows = Mathf.Min(rows, Stage1Pattern.Length);

        // 줄 반복입니다.
        for (int row = 0; row < totalRows; row++)
        {
            // 현재 줄에 놓을 버블 개수입니다.
            int colsInRow = Mathf.Min(cols, Stage1Pattern[row].Length);

            // Stage1Pattern[row].Length는 현재 줄에 실제로 적혀 있는 버블 개수입니다.

            // 버블슈터 정렬의 핵심입니다.
            // 첫 번째 버블의 중심은 왼쪽벽 안쪽에서 반 칸 떨어진 곳에 둡니다.
            // 그래야 버블의 왼쪽 끝이 왼쪽벽 안쪽에 맞습니다.
            float startX = leftX + finalSpacing / 2f;

            // 버블슈터 정렬은 보통 홀수 줄만 반 칸 오른쪽으로 이동합니다.
            // row % 2 == 1은 "1번째 줄, 3번째 줄, 5번째 줄..."이라는 뜻입니다.
            // 이렇게 해야 윗줄 버블 사이에 아랫줄 버블이 딱 끼워지는 벌집 모양이 됩니다.
            if (useStaggeredRows && row % 2 == 1)
            {
                startX += finalSpacing / 2f;
            }

            // 열 반복입니다.
            for (int col = 0; col < colsInRow; col++)
            {
                // 이번 버블의 위치를 계산합니다.
                float x = startX + col * finalSpacing;
                float y = startY - row * verticalSpacing;

                // 새 GameObject를 만듭니다.
                GameObject bubble = new GameObject($"{GeneratedBubblePrefix}{row}_{col}");

                // $"문자{값}" 형태는 문자열 안에 변수 값을 쉽게 넣는 문법입니다.
                // 예: row가 0이고 col이 2이면 이름은 Bubble_0_2가 됩니다.

                // 버블을 WallsRoot의 자식으로 넣습니다.
                bubble.transform.SetParent(transform, false);

                // SetParent는 부모 오브젝트를 정하는 함수입니다.
                // false는 현재 localPosition을 부모 기준으로 그대로 쓰겠다는 뜻입니다.

                // 버블 위치와 크기를 적용합니다.
                bubble.transform.localPosition = new Vector3(x, y, 0f);
                // 일단 기본 크기로 둡니다.
                // 실제 크기는 SpriteRenderer에 이미지를 넣은 뒤에 정확히 맞춥니다.
                bubble.transform.localScale = Vector3.one;

                // localPosition은 부모인 WallsRoot 기준 위치입니다.
                // localScale은 크기입니다. Vector3.one은 (1,1,1)을 뜻합니다.

                // SpriteRenderer는 2D 이미지를 화면에 보여주는 컴포넌트입니다.
                SpriteRenderer spriteRenderer = bubble.AddComponent<SpriteRenderer>();

                // Stage1Pattern 숫자에 맞는 버블 이미지를 선택합니다.
                int spriteIndex = Stage1Pattern[row][col] % activeBubbleSprites.Length;
                spriteRenderer.sprite = activeBubbleSprites[spriteIndex];

                // 버블 크기를 최종 크기로 적용합니다.
                // Sprite의 원본 bounds 크기로 보정하지 않는 것이 중요합니다.
                // 투명 여백이 큰 PNG는 bounds가 커져서 보정하면 동그라미가 점처럼 작아집니다.
                bubble.transform.localScale = Vector3.one * finalBubbleDiameter;

                // %는 나머지를 구하는 기호입니다.
                // 이미지 개수가 부족해도 배열 범위를 넘지 않게 하는 안전장치입니다.

                // sortingOrder는 앞뒤 표시 순서입니다.
                // 숫자가 클수록 더 앞에 보입니다.
                spriteRenderer.sortingOrder = 1;

#if UNITY_EDITOR
                // 플레이 전에는 Game 창에서만 보이고 Scene 창에서는 안 보이게 하기 위한 컴포넌트입니다.
                if (!Application.isPlaying)
                {
                    bubble.AddComponent<GameViewOnlyRenderer>();
                }

                // AddComponent는 오브젝트에 새 컴포넌트를 붙이는 함수입니다.
                // 여기서는 Game 창에서만 보이게 하는 보조 스크립트를 붙입니다.

                // 플레이 전 미리보기 버블은 씬 파일에 저장하지 않습니다.
                if (!Application.isPlaying)
                {
                    bubble.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSaveInEditor;
                    UnityEditor.SceneVisibilityManager.instance.Hide(bubble, true);
                    UnityEditor.SceneVisibilityManager.instance.DisablePicking(bubble, true);

                    // HideInHierarchy는 Hierarchy 창에서 숨긴다는 뜻입니다.
                    // DontSaveInEditor는 이 미리보기 버블을 씬 파일에 저장하지 않는다는 뜻입니다.
                    // DisablePicking은 Scene 창에서 마우스로 선택되지 않게 한다는 뜻입니다.
                }
#endif
            }
        }
    }

    // 오브젝트의 실제 화면 크기 범위를 구하는 함수입니다.
    // Bounds는 "영역" 또는 "범위"라는 뜻입니다.
    private Bounds GetWorldBounds(Transform target)
    {
        // Renderer는 이미지를 화면에 그리는 컴포넌트입니다.
        Renderer targetRenderer = target.GetComponent<Renderer>();
        if (targetRenderer != null)
        {
            return targetRenderer.bounds;
        }

        // Collider2D는 2D 충돌 영역입니다.
        Collider2D targetCollider = target.GetComponent<Collider2D>();
        if (targetCollider != null)
        {
            return targetCollider.bounds;
        }

        // Collider는 3D 충돌 영역입니다. 혹시 3D Collider를 썼을 경우를 대비합니다.
        Collider targetCollider3D = target.GetComponent<Collider>();
        if (targetCollider3D != null)
        {
            return targetCollider3D.bounds;
        }

        // Renderer와 Collider가 둘 다 없으면 임시로 작은 범위를 만들어 반환합니다.
        return new Bounds(target.position, Vector3.one * 0.1f);
    }

    // 실제로 사용할 버블 Sprite 목록을 돌려주는 함수입니다.
    // Sprite는 "2D 그림 조각"이라는 뜻입니다.
    private Sprite[] GetActiveBubbleSprites()
    {
        // 정렬 테스트용 원형 버블을 쓰기로 했다면 코드로 만든 Sprite를 돌려줍니다.
        if (useGeneratedCircleSprites)
        {
            return GetGeneratedCircleSprites();
        }

        // 정렬 테스트용 원형 버블을 쓰지 않으면 Inspector에 넣은 PNG Sprite를 사용합니다.
        return bubbleSprites;
    }

    // 코드로 빨강, 파랑, 노랑 원형 버블 Sprite를 만드는 함수입니다.
    // PNG 파일의 투명 여백, 피벗(Pivot, 기준점), 원본 크기 문제를 피하기 위해 사용합니다.
    private Sprite[] GetGeneratedCircleSprites()
    {
        // 이미 한 번 만들었다면 다시 만들지 않고 그대로 사용합니다.
        if (generatedCircleSprites != null && generatedCircleSprites.Length == 3)
        {
            return generatedCircleSprites;
        }

        // Stage 1은 빨강, 파랑, 노랑 3색을 사용합니다.
        generatedCircleSprites = new[]
        {
            CreateCircleSprite(Color.red),
            CreateCircleSprite(Color.blue),
            CreateCircleSprite(Color.yellow)
        };

        return generatedCircleSprites;
    }

    // 단색 원형 Sprite를 만드는 함수입니다.
    // Texture2D는 "작은 그림판"이라고 생각하면 됩니다.
    private Sprite CreateCircleSprite(Color circleColor)
    {
        // 128x128 크기의 정사각형 그림판을 만듭니다.
        int textureSize = 128;
        Texture2D texture = new Texture2D(textureSize, textureSize);

        // 원의 중심과 반지름을 정합니다.
        float center = (textureSize - 1) / 2f;
        float radius = textureSize * 0.46f;

        // 모든 픽셀을 하나씩 검사해서 원 안쪽이면 색을 칠하고, 바깥쪽이면 투명하게 둡니다.
        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float distanceX = x - center;
                float distanceY = y - center;
                float distanceFromCenter = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY);

                if (distanceFromCenter <= radius)
                {
                    texture.SetPixel(x, y, circleColor);
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }

        // SetPixel로 칠한 내용을 실제 Texture에 적용합니다.
        texture.Apply();

        // Texture를 Sprite로 바꿉니다.
        // pivot을 (0.5, 0.5)로 주면 기준점이 정확히 가운데가 됩니다.
        return Sprite.Create(
            texture,
            new Rect(0f, 0f, textureSize, textureSize),
            new Vector2(0.5f, 0.5f),
            textureSize
        );
    }

    // 스테이지 버블의 실제 크기를 다른 스크립트가 읽을 수 있게 해주는 함수입니다.
    // BubbleCurrentController가 이 함수를 사용해서 스테이지 버블과 같은 크기를 맞춥니다.
    public float GetBubbleDiameter()
    {
        // 벽 안쪽 너비를 계산합니다.
        Transform leftWall = transform.Find("LeftWall");
        Transform rightWall = transform.Find("RightWall");

        if (leftWall == null || rightWall == null)
        {
            return bubbleVisualScale;
        }

        float leftX = transform.InverseTransformPoint(GetWorldBounds(leftWall).max).x + horizontalPadding;
        float rightX = transform.InverseTransformPoint(GetWorldBounds(rightWall).min).x - horizontalPadding;
        float availableWidth = Mathf.Max(rightX - leftX, bubbleSpacing);
        float spacingToFit = cols > 0 ? availableWidth / cols : availableWidth;
        float finalSpacing = spacingToFit;
        float finalBubbleDiameter = finalSpacing * Mathf.Clamp(bubbleVisualScale, 0.1f, 1.3f);

        return finalBubbleDiameter;
    }
}

#if UNITY_EDITOR
// 이 컴포넌트는 플레이 전 미리보기 버블을 Game 창에서만 보이게 하기 위한 보조 컴포넌트입니다.
// GameView는 "Game 창", SceneView는 "Scene 창"이라는 뜻입니다.
public class GameViewOnlyRenderer : MonoBehaviour
{
    // 이 오브젝트의 SpriteRenderer를 저장해둡니다.
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        // 카메라가 그리기 직전에 실행되는 이벤트에 연결합니다.
        Camera.onPreCull += UpdateRendererForCamera;
    }

    private void OnDisable()
    {
        // 오브젝트가 꺼질 때 이벤트 연결을 해제합니다.
        Camera.onPreCull -= UpdateRendererForCamera;
    }

    private void UpdateRendererForCamera(Camera targetCamera)
    {
        // 플레이 중에는 이 미리보기 숨김 처리를 사용하지 않습니다.
        if (Application.isPlaying || spriteRenderer == null || targetCamera == null)
        {
            return;
        }

        // 현재 카메라가 SceneView 카메라라면 버블을 숨깁니다.
        // SceneView는 Unity의 Scene 창입니다.
        spriteRenderer.enabled = targetCamera.cameraType != CameraType.SceneView;
    }
}
#endif
