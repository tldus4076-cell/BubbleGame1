using UnityEngine;

// BubbleNextController는 "다음에 나올 버블"을 현재 버블 옆에 작게 보여주는 스크립트입니다.
// 기능 19번은 버블을 실제로 발사하지 않습니다.
// 이번 기능에서는 "다음에 어떤 색 버블이 올지 미리 보여주기"만 합니다.
// 이 스크립트는 ShooterRoot 오브젝트에 붙여서 사용합니다.
public class BubbleNextController : MonoBehaviour
{
    [Header("다음 버블 이미지 설정")]
    [Tooltip("다음 버블로 사용할 Sprite 목록입니다. 빨강, 파랑, 노랑 버블 이미지를 넣어주세요.")]
    [SerializeField] private Sprite[] bubbleSprites;

    [Tooltip("체크하면 PNG 대신 코드로 만든 깨끗한 원형 버블을 사용합니다. 위치가 안 맞으면 체크하세요.")]
    [SerializeField] private bool useGeneratedCircleSprites = true;

    [Header("다음 버블 색 설정")]
    [Tooltip("버블에 입힐 색 목록입니다. 코드로 만든 원형 버블일 때 이 색을 사용합니다.")]
    [SerializeField] private Color[] bubbleColors = { Color.red, Color.blue, Color.yellow };

    [Tooltip("체크하면 버블 색을 랜덤으로 고릅니다.")]
    [SerializeField] private bool useRandomColor = false;

    [Header("랜덤 선택 설정")]
    [Tooltip("체크하면 게임 시작 때 버블을 랜덤으로 고릅니다.")]
    [SerializeField] private bool useRandomBubble = true;

    [Header("표시 설정")]
    [Tooltip("체크되어 있으면 다음 버블을 화면에 보여줍니다.")]
    [SerializeField] private bool showNextBubble = true;

    [Tooltip("다음 버블이 배경보다 앞에 보이게 하는 순서입니다. 숫자가 클수록 앞에 보입니다.")]
    [SerializeField] private int sortingOrder = 200;

    [Header("위치와 크기 설정")]
    [Tooltip("ShooterVisual 기준으로 다음 버블을 어디에 보여줄지 정합니다. X는 좌우, Y는 위아래입니다.")]
    [SerializeField] private Vector2 nextBubbleLocalPosition = new Vector2(0.7f, 0.45f);

    [Tooltip("체크하면 스테이지 버블과 같은 크기 비율로 자동 맞춥니다.")]
    [SerializeField] private bool matchStageBubbleSize = true;

    [Tooltip("다음 버블이 현재 버블보다 얼마나 작을지 비율입니다. 0.7이면 현재 버블의 70% 크기입니다.")]
    [SerializeField] private float nextBubbleSizeRatio = 0.7f;

    [Tooltip("Match Stage Bubble Size가 꺼져 있을 때 사용할 직접 크기입니다.")]
    [SerializeField] private float nextBubbleScale = 0.4f;

    // SpriteRenderer는 2D 이미지를 화면에 보여주는 Unity 컴포넌트입니다.
    private SpriteRenderer nextBubbleRenderer;

    // 실제로 이번에 사용할 다음 버블 Sprite를 저장합니다.
    private Sprite selectedNextBubbleSprite;

    // 코드로 만든 원형 버블 Sprite를 저장합니다.
    private Sprite[] generatedCircleSprites;

    // 현재 선택된 색 인덱스를 저장합니다.
    private int selectedColorIndex = 0;

    // Awake는 Start보다 먼저 한 번 호출됩니다.
    private void Awake()
    {
        PrepareNextBubbleDisplay();
    }

    // Start는 게임이 시작될 때 한 번 호출됩니다.
    private void Start()
    {
        SelectNextBubble();
        ApplyNextBubbleVisual();
    }

    // Update는 매 프레임 호출됩니다.
    private void Update()
    {
        ApplyTransformSettings();
        ApplyRendererSettings();
    }

    // 다음 버블을 보여줄 오브젝트와 SpriteRenderer를 준비하는 함수입니다.
    private void PrepareNextBubbleDisplay()
    {
        // ShooterVisual을 찾습니다.
        Transform shooterVisual = transform.Find("ShooterVisual");
        Transform parentForBubble = shooterVisual != null ? shooterVisual : transform;

        // ShooterVisual 아래에서 NextBubble을 찾습니다.
        Transform nextBubbleTransform = parentForBubble.Find("NextBubble");

        if (nextBubbleTransform == null)
        {
            GameObject nextBubbleObject = new GameObject("NextBubble");

            // NextBubble을 ShooterVisual의 자식으로 넣습니다.
            // 이렇게 하면 슈터가 회전할 때 다음 버블도 같이 회전합니다.
            nextBubbleObject.transform.SetParent(parentForBubble);
            nextBubbleRenderer = nextBubbleObject.AddComponent<SpriteRenderer>();
        }
        else
        {
            nextBubbleRenderer = nextBubbleTransform.GetComponent<SpriteRenderer>();
            if (nextBubbleRenderer == null)
            {
                nextBubbleRenderer = nextBubbleTransform.gameObject.AddComponent<SpriteRenderer>();
            }
        }

        ApplyTransformSettings();
        ApplyRendererSettings();
    }

    // 다음 사용할 버블을 고르는 함수입니다.
    // 현재 버블과 다른 색이 나올 때까지 랜덤을 시도합니다.
    private void SelectNextBubble()
    {
        // BubbleCurrentController가 같은 오브젝트에 붙어 있는지 확인합니다.
        BubbleCurrentController currentController = GetComponent<BubbleCurrentController>();

        // 코드로 만든 원형 버블을 사용하는 경우
        if (useGeneratedCircleSprites)
        {
            Sprite[] circleSprites = GetGeneratedCircleSprites();
            if (circleSprites != null && circleSprites.Length > 1)
            {
                // 현재 버블과 다른 색을 찾습니다.
                int currentColorIndex = currentController != null ? currentController.GetSelectedColorIndex() : -1;
                int tryCount = 0;
                int randomIndex = Random.Range(0, circleSprites.Length);

                while (randomIndex == currentColorIndex && tryCount < 20)
                {
                    randomIndex = Random.Range(0, circleSprites.Length);
                    tryCount++;
                }

                selectedColorIndex = randomIndex;
                selectedNextBubbleSprite = circleSprites[randomIndex];
            }
            return;
        }

        // PNG Sprite를 사용하는 경우
        Sprite currentSprite = currentController != null ? currentController.GetSelectedBubbleSprite() : null;

        if (useRandomBubble && bubbleSprites != null && bubbleSprites.Length > 1)
        {
            int tryCount = 0;
            int randomIndex = Random.Range(0, bubbleSprites.Length);

            while (bubbleSprites[randomIndex] == currentSprite && tryCount < 20)
            {
                randomIndex = Random.Range(0, bubbleSprites.Length);
                tryCount++;
            }

            selectedColorIndex = randomIndex;
            selectedNextBubbleSprite = bubbleSprites[randomIndex];
            return;
        }

        if (bubbleSprites != null && bubbleSprites.Length > 1)
        {
            for (int i = 0; i < bubbleSprites.Length; i++)
            {
                if (bubbleSprites[i] != currentSprite)
                {
                    selectedColorIndex = i;
                    selectedNextBubbleSprite = bubbleSprites[i];
                    return;
                }
            }
        }

        if (bubbleSprites != null && bubbleSprites.Length > 0)
        {
            selectedColorIndex = 0;
            selectedNextBubbleSprite = bubbleSprites[0];
        }
    }

    // 다음 버블의 Sprite와 색을 화면에 적용하는 함수입니다.
    private void ApplyNextBubbleVisual()
    {
        if (nextBubbleRenderer == null)
        {
            return;
        }

        nextBubbleRenderer.sprite = selectedNextBubbleSprite;

        // 코드로 만든 원형 버블이면 색을 직접 입힙니다.
        if (useGeneratedCircleSprites && bubbleColors != null && selectedColorIndex < bubbleColors.Length)
        {
            nextBubbleRenderer.color = bubbleColors[selectedColorIndex];
        }
        else if (useRandomColor && bubbleColors != null && bubbleColors.Length > 0)
        {
            int randomColorIndex = Random.Range(0, bubbleColors.Length);
            nextBubbleRenderer.color = bubbleColors[randomColorIndex];
        }
        else
        {
            nextBubbleRenderer.color = Color.white;
        }
    }

    // 다음 버블 위치와 크기를 적용하는 함수입니다.
    private void ApplyTransformSettings()
    {
        if (nextBubbleRenderer == null)
        {
            return;
        }

        Transform nextBubbleTransform = nextBubbleRenderer.transform;

        // ShooterVisual 기준으로 위치를 정합니다.
        Vector3 shooterVisualLocalPosition = GetShooterVisualLocalPosition();
        nextBubbleTransform.localPosition = shooterVisualLocalPosition + new Vector3(nextBubbleLocalPosition.x, nextBubbleLocalPosition.y, 0f);

        // 크기를 결정합니다.
        float finalScale = nextBubbleScale;

        // 스테이지 버블과 같은 크기 비율로 맞추는 옵션이 켜져 있으면 StageBubbleLayout에서 크기를 가져옵니다.
        if (matchStageBubbleSize)
        {
            StageBubbleLayout stageLayout = FindFirstObjectByType<StageBubbleLayout>();
            if (stageLayout != null)
            {
                float stageBubbleDiameter = stageLayout.GetBubbleDiameter();
                finalScale = stageBubbleDiameter * nextBubbleSizeRatio;
            }
        }

        nextBubbleTransform.localScale = Vector3.one * finalScale;
    }

    // 다음 버블이 보이는지 설정하는 함수입니다.
    private void ApplyRendererSettings()
    {
        if (nextBubbleRenderer == null)
        {
            return;
        }

        nextBubbleRenderer.enabled = showNextBubble;
        nextBubbleRenderer.sortingOrder = sortingOrder;
    }

    // ShooterVisual의 위치를 가져오는 함수입니다.
    private Vector3 GetShooterVisualLocalPosition()
    {
        Transform shooterVisual = transform.Find("ShooterVisual");
        if (shooterVisual != null)
        {
            return shooterVisual.localPosition;
        }
        return Vector3.zero;
    }

    // 코드로 빨강, 파랑, 노랑 원형 버블 Sprite를 만드는 함수입니다.
    private Sprite[] GetGeneratedCircleSprites()
    {
        if (generatedCircleSprites != null && generatedCircleSprites.Length == 3)
        {
            return generatedCircleSprites;
        }

        generatedCircleSprites = new[]
        {
            CreateCircleSprite(Color.red),
            CreateCircleSprite(Color.blue),
            CreateCircleSprite(Color.yellow)
        };

        return generatedCircleSprites;
    }

    // 단색 원형 Sprite를 만드는 함수입니다.
    private Sprite CreateCircleSprite(Color circleColor)
    {
        int textureSize = 128;
        Texture2D texture = new Texture2D(textureSize, textureSize);

        float center = (textureSize - 1) / 2f;
        float radius = textureSize * 0.46f;

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

        texture.Apply();

        return Sprite.Create(
            texture,
            new Rect(0f, 0f, textureSize, textureSize),
            new Vector2(0.5f, 0.5f),
            textureSize
        );
    }

    // 나중에 발사 후 다음 버블을 새로 고를 때 사용하는 함수입니다.
    public void SelectNewNextBubble()
    {
        SelectNextBubble();
        ApplyNextBubbleVisual();
    }

    // 현재 선택된 다음 버블 Sprite를 다른 스크립트가 읽을 수 있게 해주는 함수입니다.
    public Sprite GetSelectedNextBubbleSprite()
    {
        return selectedNextBubbleSprite;
    }

    // 코드로 만든 원형 버블의 색 인덱스를 다른 스크립트가 읽을 수 있게 해주는 함수입니다.
    public int GetSelectedColorIndex()
    {
        return selectedColorIndex;
    }
}
