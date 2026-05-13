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

    [Header("현재 버블 색 설정")]
    [Tooltip("버블에 입힐 색 목록입니다. Sprite가 흰색 버블일 때 색칠용으로 사용할 수 있습니다.")]
    [SerializeField] private Color[] bubbleColors = { Color.red, Color.blue, Color.yellow };

    [Tooltip("체크하면 버블 색을 랜덤으로 고릅니다. 색칠이 필요 없으면 꺼도 됩니다.")]
    [SerializeField] private bool useRandomColor = false;

    [Header("랜덤 선택 설정")]
    [Tooltip("체크하면 게임 시작 때 Bubble Sprites 목록 중 하나를 랜덤으로 고릅니다.")]
    [SerializeField] private bool useRandomBubble = true;

    [Header("표시 설정")]
    [Tooltip("체크되어 있으면 현재 발사할 버블을 화면에 보여줍니다.")]
    [SerializeField] private bool showCurrentBubble = true;

    [Tooltip("현재 버블이 배경과 Stage 버블보다 앞에 보이게 하는 순서입니다. 숫자가 클수록 앞에 보입니다.")]
    [SerializeField] private int sortingOrder = 200;

    [Header("위치와 크기 설정")]
    [Tooltip("ShooterVisual 기준으로 현재 버블을 어디에 보여줄지 정합니다. X는 좌우, Y는 위아래입니다.")]
    [SerializeField] private Vector2 bubbleLocalPosition = new Vector2(0f, 0.8f);

    [Tooltip("현재 버블의 화면 크기입니다. 너무 크거나 작으면 이 값을 조절하세요.")]
    [SerializeField] private float bubbleScale = 0.6f;

    // SpriteRenderer는 2D 이미지를 화면에 보여주는 Unity 컴포넌트입니다.
    // 여기서는 현재 발사할 버블 그림을 보여주는 역할을 합니다.
    private SpriteRenderer bubbleRenderer;

    // 실제로 이번에 사용할 현재 버블 Sprite를 저장합니다.
    // Inspector에 보일 필요는 없어서 private으로 둡니다.
    private Sprite selectedBubbleSprite;

    // Awake는 Start보다 먼저 한 번 호출됩니다.
    // 여기서는 현재 버블을 보여줄 오브젝트와 SpriteRenderer를 먼저 준비합니다.
    private void Awake()
    {
        PrepareBubbleDisplay();
    }

    // Start는 게임이 시작될 때 한 번 호출됩니다.
    // 여기서는 실제로 보여줄 현재 버블 Sprite와 색을 정합니다.
    private void Start()
    {
        SelectCurrentBubble();
        ApplyBubbleVisual();
    }

    // Update는 매 프레임 호출됩니다.
    // 이번 기능은 발사 기능이 아니므로 매 프레임 할 일은 많지 않습니다.
    // 다만 Inspector 값이 바뀌었을 때 위치와 크기가 반영되도록 계속 적용합니다.
    private void Update()
    {
        ApplyTransformSettings();
        ApplyRendererSettings();
    }

    // 현재 버블을 보여줄 오브젝트와 SpriteRenderer를 준비하는 함수입니다.
    private void PrepareBubbleDisplay()
    {
        // ShooterRoot 아래에 CurrentBubble이라는 자식 오브젝트가 있는지 찾습니다.
        Transform bubbleTransform = transform.Find("CurrentBubble");

        // CurrentBubble이 없으면 새로 만듭니다.
        if (bubbleTransform == null)
        {
            GameObject bubbleObject = new GameObject("CurrentBubble");

            // CurrentBubble을 ShooterRoot의 자식으로 넣습니다.
            // 이렇게 하면 ShooterRoot를 옮겼을 때 현재 버블도 같이 따라갑니다.
            bubbleObject.transform.SetParent(transform);

            // 새 오브젝트에 SpriteRenderer를 붙입니다.
            bubbleRenderer = bubbleObject.AddComponent<SpriteRenderer>();
        }
        else
        {
            // 이미 CurrentBubble이 있다면 그 오브젝트의 SpriteRenderer를 가져옵니다.
            bubbleRenderer = bubbleTransform.GetComponent<SpriteRenderer>();

            // SpriteRenderer가 없다면 새로 붙입니다.
            if (bubbleRenderer == null)
            {
                bubbleRenderer = bubbleTransform.gameObject.AddComponent<SpriteRenderer>();
            }
        }

        // 위치, 크기, 앞뒤 순서, 보임 여부를 적용합니다.
        ApplyTransformSettings();
        ApplyRendererSettings();
    }

    // 현재 사용할 버블 Sprite를 고르는 함수입니다.
    private void SelectCurrentBubble()
    {
        // Use Random Bubble이 켜져 있고 Sprite 목록이 있다면 랜덤으로 하나 고릅니다.
        if (useRandomBubble && bubbleSprites != null && bubbleSprites.Length > 0)
        {
            int randomIndex = Random.Range(0, bubbleSprites.Length);
            selectedBubbleSprite = bubbleSprites[randomIndex];
            return;
        }

        // 랜덤을 끈 상태라면 첫 번째 Sprite를 사용합니다.
        // Current Bubble Sprite 필드는 초보자에게 헷갈릴 수 있어서 Inspector에서 제거했습니다.
        if (bubbleSprites != null && bubbleSprites.Length > 0)
        {
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

        // 색 랜덤 기능을 켜면 bubbleColors 목록 중 하나를 골라서 Sprite에 색을 입힙니다.
        if (useRandomColor && bubbleColors != null && bubbleColors.Length > 0)
        {
            int randomColorIndex = Random.Range(0, bubbleColors.Length);
            bubbleRenderer.color = bubbleColors[randomColorIndex];
        }
        else
        {
            // 색 랜덤을 끄면 Sprite 원래 색이 보이도록 흰색을 사용합니다.
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

        // 현재 버블 위치는 Inspector에서 수동으로 조절합니다.
        // 단, 기준점은 ShooterRoot가 아니라 실제 슈터 그림인 ShooterVisual입니다.
        // 이유: ShooterRoot의 기준점과 ShooterVisual 그림 위치가 서로 떨어져 있으면
        // CurrentBubble이 슈터 바로 위가 아니라 옆이나 위쪽으로 비켜 보일 수 있기 때문입니다.
        Vector3 shooterVisualLocalPosition = GetShooterVisualLocalPosition();

        // ShooterVisual 위치에 사용자가 입력한 오프셋을 더합니다.
        // offset은 "기준 위치에서 얼마나 더 움직일지"라는 뜻입니다.
        bubbleTransform.localPosition = shooterVisualLocalPosition + new Vector3(bubbleLocalPosition.x, bubbleLocalPosition.y, 0f);

        // localScale은 크기입니다.
        // Vector3.one은 (1, 1, 1)을 뜻합니다.
        bubbleTransform.localScale = Vector3.one * bubbleScale;
    }

    // ShooterVisual의 위치를 가져오는 함수입니다.
    // ShooterVisual은 실제 슈터 그림이 들어있는 자식 오브젝트입니다.
    private Vector3 GetShooterVisualLocalPosition()
    {
        // ShooterRoot 아래에서 ShooterVisual이라는 이름의 자식 오브젝트를 찾습니다.
        Transform shooterVisual = transform.Find("ShooterVisual");

        // ShooterVisual을 찾으면 그 localPosition을 기준점으로 사용합니다.
        if (shooterVisual != null)
        {
            return shooterVisual.localPosition;
        }

        // ShooterVisual을 못 찾으면 기존처럼 ShooterRoot 기준점(Vector3.zero)을 사용합니다.
        // Vector3.zero는 (0, 0, 0)을 뜻합니다.
        return Vector3.zero;
    }

    // 현재 버블이 보이는지, 배경보다 앞에 있는지 설정하는 함수입니다.
    private void ApplyRendererSettings()
    {
        if (bubbleRenderer == null)
        {
            return;
        }

        bubbleRenderer.enabled = showCurrentBubble;
        bubbleRenderer.sortingOrder = sortingOrder;
    }

    // 나중에 발사 후 다음 버블을 현재 버블로 바꿀 때 사용할 수 있는 함수입니다.
    // 기능 20번에서 다시 사용할 수 있습니다.
    public void SetNextBubble()
    {
        SelectCurrentBubble();
        ApplyBubbleVisual();
    }
}
