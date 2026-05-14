using UnityEngine;

// BubbleCurrentController는 "현재 발사할 버블"을 슈터 위에 보여주는 스크립트입니다.
// 기능 18번은 버블을 실제로 발사하지 않습니다.
// 이번 기능에서는 "발사 전에 어떤 색 버블이 준비되어 있는지 보여주기"만 합니다.
// 이 스크립트는 ShooterRoot 오브젝트에 붙여서 사용합니다.
public class BubbleCurrentController : MonoBehaviour
{
    [Header("현재 버블 이미지 설정")]
    [Tooltip("현재 버블로 사용할 Sprite 목록입니다. 빨강, 파랑, 노랑 버블 이미지를 넣어주세요.")]
    [SerializeField] private Sprite[] bubbleSprites;

    [Tooltip("체크하면 PNG 대신 코드로 만든 깨끗한 원형 버블을 사용합니다. 위치가 안 맞으면 체크하세요.")]
    [SerializeField] private bool useGeneratedCircleSprites = true;

    [Header("현재 버블 색 설정")]
    [Tooltip("버블에 입힐 색 목록입니다. 코드로 만든 원형 버블일 때 이 색을 사용합니다.")]
    [SerializeField] private Color[] bubbleColors = { Color.red, Color.blue, Color.yellow };

    [Tooltip("체크하면 버블 색을 랜덤으로 고릅니다.")]
    [SerializeField] private bool useRandomColor = false;

    [Header("랜덤 선택 설정")]
    [Tooltip("체크하면 게임 시작 때 버블을 랜덤으로 고릅니다.")]
    [SerializeField] private bool useRandomBubble = true;

    [Header("표시 설정")]
    [Tooltip("체크되어 있으면 현재 발사할 버블을 화면에 보여줍니다.")]
    [SerializeField] private bool showCurrentBubble = true;

    [Tooltip("현재 버블이 배경과 Stage 버블보다 앞에 보이게 하는 순서입니다. 숫자가 클수록 앞에 보입니다.")]
    [SerializeField] private int sortingOrder = 200;

    [Header("위치와 크기 설정")]
    [Tooltip("ShooterVisual 기준으로 현재 버블을 어디에 보여줄지 정합니다. X는 좌우, Y는 위아래입니다.")]
    [SerializeField] private Vector2 bubbleLocalPosition = new Vector2(0f, 0.8f);

    [Tooltip("체크하면 스테이지 버블과 같은 크기로 자동 맞춥니다.")]
    [SerializeField] private bool matchStageBubbleSize = true;

    [Tooltip("Match Stage Bubble Size가 꺼져 있을 때 사용할 직접 크기입니다.")]
    [SerializeField] private float bubbleScale = 0.6f;

    // SpriteRenderer는 2D 이미지를 화면에 보여주는 Unity 컴포넌트입니다.
    private SpriteRenderer bubbleRenderer;

    // 실제로 이번에 사용할 현재 버블 Sprite를 저장합니다.
    private Sprite selectedBubbleSprite;

    // 코드로 만든 원형 버블 Sprite를 저장합니다.
    private Sprite[] generatedCircleSprites;

    // 현재 선택된 색 인덱스를 저장합니다.
    private int selectedColorIndex = 0;

    // Awake는 Start보다 먼저 한 번 호출됩니다.
    private void Awake()
    {
        PrepareBubbleDisplay();
    }

    // Start는 게임이 시작될 때 한 번 호출됩니다.
    private void Start()
    {
        SelectCurrentBubble();
        ApplyBubbleVisual();
    }

    // Update는 매 프레임 호출됩니다.
    private void Update()
    {
        ApplyTransformSettings();
        ApplyRendererSettings();
    }

    // 현재 버블을 보여줄 오브젝트와 SpriteRenderer를 준비하는 함수입니다.
    private void PrepareBubbleDisplay()
    {
        // ShooterVisual을 찾습니다.
        // ShooterVisual은 실제 슈터 그림이 들어있는 자식 오브젝트입니다.
        Transform shooterVisual = transform.Find("ShooterVisual");

        // ShooterVisual이 없으면 ShooterRoot를 기준으로 합니다.
        Transform parentForBubble = shooterVisual != null ? shooterVisual : transform;

        // ShooterVisual 아래에서 CurrentBubble을 찾습니다.
        Transform bubbleTransform = parentForBubble.Find("CurrentBubble");

        if (bubbleTransform == null)
        {
            GameObject bubbleObject = new GameObject("CurrentBubble");

            // CurrentBubble을 ShooterVisual의 자식으로 넣습니다.
            // 이렇게 하면 슈터가 회전할 때 현재 버블도 같이 회전합니다.
            bubbleObject.transform.SetParent(parentForBubble);
            bubbleRenderer = bubbleObject.AddComponent<SpriteRenderer>();
        }
        else
        {
            bubbleRenderer = bubbleTransform.GetComponent<SpriteRenderer>();
            if (bubbleRenderer == null)
            {
                bubbleRenderer = bubbleTransform.gameObject.AddComponent<SpriteRenderer>();
            }
        }

        ApplyTransformSettings();
        ApplyRendererSettings();
    }

    // 현재 사용할 버블을 고르는 함수입니다.
    private void SelectCurrentBubble()
    {
        // 코드로 만든 원형 버블을 사용하는 경우
        if (useGeneratedCircleSprites)
        {
            Sprite[] circleSprites = GetGeneratedCircleSprites();
            if (circleSprites != null && circleSprites.Length > 0)
            {
                if (useRandomBubble)
                {
                    selectedColorIndex = Random.Range(0, circleSprites.Length);
                }
                else
                {
                    selectedColorIndex = 0;
                }
                selectedBubbleSprite = circleSprites[selectedColorIndex];
            }
            return;
        }

        // PNG Sprite를 사용하는 경우
        if (useRandomBubble && bubbleSprites != null && bubbleSprites.Length > 0)
        {
            selectedColorIndex = Random.Range(0, bubbleSprites.Length);
            selectedBubbleSprite = bubbleSprites[selectedColorIndex];
            return;
        }

        if (bubbleSprites != null && bubbleSprites.Length > 0)
        {
            selectedColorIndex = 0;
            selectedBubbleSprite = bubbleSprites[0];
        }
    }

    // 현재 버블의 Sprite와 색을 화면에 적용하는 함수입니다.
    private void ApplyBubbleVisual()
    {
        if (bubbleRenderer == null)
        {
            return;
        }

        bubbleRenderer.sprite = selectedBubbleSprite;

        // 코드로 만든 원형 버블이면 색을 직접 입힙니다.
        if (useGeneratedCircleSprites && bubbleColors != null && selectedColorIndex < bubbleColors.Length)
        {
            bubbleRenderer.color = bubbleColors[selectedColorIndex];
        }
        else if (useRandomColor && bubbleColors != null && bubbleColors.Length > 0)
        {
            int randomColorIndex = Random.Range(0, bubbleColors.Length);
            bubbleRenderer.color = bubbleColors[randomColorIndex];
        }
        else
        {
            bubbleRenderer.color = Color.white;
        }
    }

    // 현재 버블 위치와 크기를 적용하는 함수입니다.
    private void ApplyTransformSettings()
    {
        if (bubbleRenderer == null)
        {
            return;
        }

        Transform bubbleTransform = bubbleRenderer.transform;

        // 현재 버블은 ShooterVisual의 자식이므로, localPosition은 ShooterVisual 기준입니다.
        // X는 좌우, Y는 위아래입니다.
        bubbleTransform.localPosition = new Vector3(bubbleLocalPosition.x, bubbleLocalPosition.y, 0f);

        // 크기를 결정합니다.
        float finalScale = bubbleScale;

        // 스테이지 버블과 같은 크기로 맞추는 옵션이 켜져 있으면 StageBubbleLayout에서 크기를 가져옵니다.
        if (matchStageBubbleSize)
        {
            StageBubbleLayout stageLayout = FindFirstObjectByType<StageBubbleLayout>();
            if (stageLayout != null)
            {
                finalScale = stageLayout.GetBubbleDiameter();
            }
        }

        bubbleTransform.localScale = Vector3.one * finalScale;
    }



    // 현재 버블이 보이는지 설정하는 함수입니다.
    private void ApplyRendererSettings()
    {
        if (bubbleRenderer == null)
        {
            return;
        }

        bubbleRenderer.enabled = showCurrentBubble;
        bubbleRenderer.sortingOrder = sortingOrder;
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

    // 나중에 발사 후 다음 버블을 현재 버블로 바꿀 때 사용하는 함수입니다.
    public void SetNextBubble(Sprite newSprite = null, int newColorIndex = -1)
    {
        if (newSprite != null)
        {
            selectedBubbleSprite = newSprite;

            // 색 인덱스도 함께 업데이트합니다.
            // newColorIndex가 0 이상이면 그 인덱스를 사용합니다.
            if (newColorIndex >= 0)
            {
                selectedColorIndex = newColorIndex;
            }
        }
        else
        {
            SelectCurrentBubble();
        }

        ApplyBubbleVisual();
    }

    // 현재 선택된 버블 Sprite를 다른 스크립트가 읽을 수 있게 해주는 함수입니다.
    public Sprite GetSelectedBubbleSprite()
    {
        return selectedBubbleSprite;
    }

    // 현재 사용 중인 Bubble Sprites 목록을 다른 스크립트가 읽을 수 있게 해주는 함수입니다.
    public Sprite[] GetBubbleSprites()
    {
        return bubbleSprites;
    }

    // 코드로 만든 원형 버블의 색 인덱스를 다른 스크립트가 읽을 수 있게 해주는 함수입니다.
    public int GetSelectedColorIndex()
    {
        return selectedColorIndex;
    }
}
