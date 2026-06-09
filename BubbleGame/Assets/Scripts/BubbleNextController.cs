using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// ============================================================
// BubbleNextController는 "다음에 나올 버블"을 화면에 보여주는 스크립트입니다.
//
// [기능 19]
// - 버블을 발사하지 않습니다.
// - 현재 버블 옆에 "다음 버블" 그림만 작게 보여줍니다.
// - ShooterRoot 오브젝트에 붙여서 사용합니다.
// - NextBubble은 ShooterVisual 자식으로 만들어서 슈터 회전에 같이 따라갑니다.
//
// [ExecuteAlways]
// - Play를 하지 않아도 Game 창에 다음 버블이 보입니다.
// - Inspector에서 위치/크기를 바로 확인할 수 있습니다.
// - Scene 창에서 직접 움직이면 Inspector 값에 자동 반영됩니다.
// ============================================================
[ExecuteAlways]
public class BubbleNextController : MonoBehaviour
{
    [Header("다음 버블 이미지 설정")]
    [Tooltip("다음 버블로 사용할 이미지 목록입니다. 빨강, 파랑, 노랑 Sprite를 넣어주세요.")]
    [SerializeField] private Sprite[] bubbleSprites;

    [Tooltip("체크하면 Bubble Sprites 목록에서 랜덤으로 하나를 고릅니다.")]
    [SerializeField] private bool useRandomBubble = true;

    [Tooltip("NextBubble을 어느 오브젝트 아래에 둘지 정합니다. 보통 ShooterVisual을 넣으면 슈터 회전에 같이 따라갑니다.")]
    [SerializeField] private Transform nextBubbleParent;

    [Header("표시 설정")]
    [Tooltip("체크하면 다음 버블을 화면에 보여줍니다. 끄면 숨겨집니다.")]
    [SerializeField] private bool showNextBubble = true;

    [Tooltip("다음 버블이 배경보다 앞에 보이게 하는 순서입니다. 숫자가 클수록 앞에 보입니다.")]
    [SerializeField] private int sortingOrder = 200;

    [Header("위치와 크기 설정")]
    [Tooltip("ShooterVisual 기준으로 다음 버블을 어디에 보여줄지 정합니다. X는 좌우, Y는 위아래입니다.")]
    [SerializeField] private Vector2 nextBubbleLocalPosition = new Vector2(0.45f, 0f);

    [Tooltip("다음 버블의 직접 크기입니다.")]
    [SerializeField] private float nextBubbleScale = 0.2f;

    [Header("색 설정")]
    [Tooltip("Sprite 자체 색을 그대로 쓰고 싶으면 흰색으로 두세요.")]
    [SerializeField] private Color bubbleTintColor = Color.white;

    // SpriteRenderer는 2D 그림을 화면에 보여주는 Unity 컴포넌트입니다.
    private SpriteRenderer nextBubbleRenderer;

    // 이번에 선택된 다음 버블 Sprite입니다.
    private Sprite selectedNextBubbleSprite;

    // 이번에 선택된 다음 버블의 배열 번호입니다.
    private int selectedBubbleIndex = -1;

    // 다음 버블 Sprite가 바뀌면 다른 스크립트에게 알려주는 이벤트입니다.
    // ShooterController는 이 이벤트를 구독해서 실제 발사 색을 작은 버블 색과 맞춥니다.
    public event System.Action<Sprite> NextBubbleSpriteChanged;

    // ============================================================
    // OnEnable은 스크립트가 켜질 때 호출됩니다.
    // ExecuteAlways와 함께 쓰면 Play 전에도 실행됩니다.
    // ============================================================
    private void OnEnable()
    {
        EnsureNextBubbleExists();
        EnsureSelectedNextBubble();
        ApplyAll();
    }

    // ============================================================
    // Start는 Play를 누를 때 한 번 호출됩니다.
    // ============================================================
    private void Start()
    {
        EnsureNextBubbleExists();
        EnsureSelectedNextBubble();
        ApplyAll();
    }

    // ============================================================
    // Update는 매 프레임 호출됩니다.
    // Play 전/후 모두 실행됩니다.
    // ============================================================
    private void Update()
    {
        EnsureNextBubbleExists();

        if (selectedNextBubbleSprite == null)
        {
            EnsureSelectedNextBubble();
        }

        // Scene 창에서 NextBubble을 직접 움직이면 Inspector 값에 반영합니다.
        if (!TryReadSceneHandlePosition())
        {
            // Scene 창에서 움직이지 않았으면 Inspector 값으로 위치를 적용합니다.
            ApplyAll();
        }
        else
        {
            // Scene 창에서 움직였으면 나머지 설정만 적용합니다.
            ApplyRendererOnly();
        }
    }

    // ============================================================
    // Inspector 값이 바뀔 때 호출됩니다.
    // ============================================================
    private void OnValidate()
    {
        if (nextBubbleScale < 0f)
        {
            nextBubbleScale = 0f;
        }

        EnsureNextBubbleExists();
        EnsureSelectedNextBubble();
        ApplyAll();
    }

    // ============================================================
    // OnDisable은 스크립트가 꺼질 때 호출됩니다.
    // ============================================================
    private void OnDisable()
    {
        CleanupPreviewObject();
    }

    // ============================================================
    // OnDestroy는 오브젝트가 삭제될 때 호출됩니다.
    // ============================================================
    private void OnDestroy()
    {
        CleanupPreviewObject();
    }

    // ============================================================
    // NextBubble 오브젝트가 없으면 만듭니다.
    // ShooterVisual 자식으로 만들어서 슈터 회전에 같이 따라가게 합니다.
    // ============================================================
    private void EnsureNextBubbleExists()
    {
        if (nextBubbleRenderer != null)
        {
            return;
        }

        // 외부 오브젝트는 Inspector에서 연결합니다.
        // 연결이 비어 있으면 자기 자신 아래에 NextBubble을 만듭니다.
        Transform parentForNextBubble = nextBubbleParent != null ? nextBubbleParent : transform;

        // 이미 부모 아래에 NextBubble이 있으면 재사용합니다.
        Transform existing = parentForNextBubble.Find("NextBubble");
        if (existing != null)
        {
            nextBubbleRenderer = existing.GetComponent<SpriteRenderer>();
            if (nextBubbleRenderer == null)
            {
                nextBubbleRenderer = existing.gameObject.AddComponent<SpriteRenderer>();
            }
            return;
        }

        // 없으면 새로 만듭니다.
        GameObject nextBubbleObject = new GameObject("NextBubble");
        nextBubbleObject.transform.SetParent(parentForNextBubble, false);

        // Play 전 미리보기는 씬 파일에 저장하지 않습니다.
        if (!Application.isPlaying)
        {
            nextBubbleObject.hideFlags = HideFlags.DontSaveInEditor;
        }

        nextBubbleRenderer = nextBubbleObject.AddComponent<SpriteRenderer>();
    }

    // ============================================================
    // 다음 버블 Sprite를 고릅니다.
    // ============================================================
    private void SelectNextBubble()
    {
        Sprite previousSprite = selectedNextBubbleSprite;
        int previousIndex = selectedBubbleIndex;

        if (bubbleSprites == null || bubbleSprites.Length == 0)
        {
            selectedNextBubbleSprite = null;
            selectedBubbleIndex = -1;
            NotifyNextBubbleSpriteChangedIfNeeded(previousSprite, previousIndex);
            return;
        }

        if (!useRandomBubble)
        {
            SelectFirstValidBubble();
            NotifyNextBubbleSpriteChangedIfNeeded(previousSprite, previousIndex);
            return;
        }

        SelectRandomBubble();
        NotifyNextBubbleSpriteChangedIfNeeded(previousSprite, previousIndex);
    }

    // 이미 다음 버블을 골랐다면 다시 랜덤으로 뽑지 않습니다.
    // Start 순서 때문에 작은 버블 색과 실제 발사 색이 어긋나는 것을 막습니다.
    private void EnsureSelectedNextBubble()
    {
        if (IsSelectedNextBubbleValid())
        {
            return;
        }

        SelectNextBubble();
    }

    // 현재 고른 Sprite가 Bubble Sprites 안에 아직 있는지 확인합니다.
    private bool IsSelectedNextBubbleValid()
    {
        if (selectedNextBubbleSprite == null || bubbleSprites == null || bubbleSprites.Length == 0)
        {
            return false;
        }

        if (selectedBubbleIndex >= 0 && selectedBubbleIndex < bubbleSprites.Length && bubbleSprites[selectedBubbleIndex] == selectedNextBubbleSprite)
        {
            return true;
        }

        for (int i = 0; i < bubbleSprites.Length; i++)
        {
            if (bubbleSprites[i] == selectedNextBubbleSprite)
            {
                selectedBubbleIndex = i;
                return true;
            }
        }

        return false;
    }

    // 이벤트는 Sprite가 실제로 바뀌었을 때만 보냅니다.
    private void NotifyNextBubbleSpriteChangedIfNeeded(Sprite previousSprite, int previousIndex)
    {
        if (previousSprite == selectedNextBubbleSprite && previousIndex == selectedBubbleIndex)
        {
            return;
        }

        NextBubbleSpriteChanged?.Invoke(selectedNextBubbleSprite);
    }

    // 랜덤을 쓰지 않을 때, 첫 번째로 비어 있지 않은 Sprite를 고릅니다.
    private void SelectFirstValidBubble()
    {
        for (int i = 0; i < bubbleSprites.Length; i++)
        {
            if (bubbleSprites[i] != null)
            {
                selectedBubbleIndex = i;
                selectedNextBubbleSprite = bubbleSprites[i];
                return;
            }
        }

        selectedBubbleIndex = -1;
        selectedNextBubbleSprite = null;
    }

    // 랜덤으로 Sprite를 고릅니다.
    private void SelectRandomBubble()
    {
        int tryCount = 0;
        int randomIndex = Random.Range(0, bubbleSprites.Length);

        while (bubbleSprites[randomIndex] == null && bubbleSprites.Length > 1 && tryCount < 20)
        {
            randomIndex = Random.Range(0, bubbleSprites.Length);
            tryCount++;
        }

        selectedBubbleIndex = randomIndex;
        selectedNextBubbleSprite = bubbleSprites[randomIndex];
    }

    // ============================================================
    // Scene 창에서 NextBubble을 직접 움직이면 Inspector 값에 반영합니다.
    // Play 전 Scene 창에서 마우스로 위치를 조절할 때 사용됩니다.
    // ============================================================
    private bool TryReadSceneHandlePosition()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            return false;
        }

        if (nextBubbleRenderer == null)
        {
            return false;
        }

        // 지금 선택된 오브젝트가 NextBubble일 때만 Scene 조작 값을 읽습니다.
        if (Selection.activeTransform != nextBubbleRenderer.transform)
        {
            return false;
        }

        // NextBubble의 현재 localPosition을 읽어서 Inspector 값에 반영합니다.
        Vector3 localPosition = nextBubbleRenderer.transform.localPosition;
        Vector2 newLocalPosition = new Vector2(localPosition.x, localPosition.y);

        if (Vector2.Distance(nextBubbleLocalPosition, newLocalPosition) > 0.0001f)
        {
            nextBubbleLocalPosition = newLocalPosition;
        }

        // 크기도 Scene 창에서 바꾸면 Inspector 값에 반영합니다.
        float newScale = nextBubbleRenderer.transform.localScale.x;
        if (!Mathf.Approximately(nextBubbleScale, newScale))
        {
            nextBubbleScale = Mathf.Max(0f, newScale);
        }

        return true;
#else
        return false;
#endif
    }

    // ============================================================
    // 위치를 제외한 나머지 설정만 적용합니다.
    // Scene 창에서 움직인 후 호출됩니다.
    // ============================================================
    private void ApplyRendererOnly()
    {
        if (nextBubbleRenderer == null)
        {
            return;
        }

        nextBubbleRenderer.sprite = selectedNextBubbleSprite;
        nextBubbleRenderer.color = bubbleTintColor;
        nextBubbleRenderer.enabled = showNextBubble;
        nextBubbleRenderer.sortingOrder = sortingOrder;
        nextBubbleRenderer.transform.localScale = Vector3.one * nextBubbleScale;
    }

    // ============================================================
    // 위치, 크기, 표시, 색을 한 번에 적용합니다.
    // ============================================================
    private void ApplyAll()
    {
        if (nextBubbleRenderer == null)
        {
            return;
        }

        nextBubbleRenderer.sprite = selectedNextBubbleSprite;
        nextBubbleRenderer.color = bubbleTintColor;
        nextBubbleRenderer.enabled = showNextBubble;
        nextBubbleRenderer.sortingOrder = sortingOrder;
        nextBubbleRenderer.transform.localPosition = new Vector3(nextBubbleLocalPosition.x, nextBubbleLocalPosition.y, 0f);
        nextBubbleRenderer.transform.localScale = Vector3.one * nextBubbleScale;
    }

    // ============================================================
    // Play 전 미리보기 오브젝트를 정리합니다.
    // ============================================================
    private void CleanupPreviewObject()
    {
        if (nextBubbleRenderer == null)
        {
            return;
        }

        if (nextBubbleRenderer.gameObject != null)
        {
            if (Application.isPlaying)
            {
                Destroy(nextBubbleRenderer.gameObject);
            }
            else
            {
                DestroyImmediate(nextBubbleRenderer.gameObject);
            }
        }

        nextBubbleRenderer = null;
    }

    // ============================================================
    // 다른 스크립트가 다음 버블 Sprite를 읽을 수 있게 해주는 함수입니다.
    // ============================================================
    public Sprite GetSelectedNextBubbleSprite()
    {
        return selectedNextBubbleSprite;
    }

    // ============================================================
    // 다른 스크립트가 다음 버블 배열 번호를 읽을 수 있게 해주는 함수입니다.
    // ============================================================
    public int GetSelectedBubbleIndex()
    {
        return selectedBubbleIndex;
    }

    // ============================================================
    // 기존 코드 호환용 함수입니다.
    // ============================================================
    public int GetSelectedColorIndex()
    {
        return selectedBubbleIndex;
    }

    // ============================================================
    // 다음 버블을 새로 뽑고 싶을 때 호출하는 함수입니다.
    // ============================================================
    public void SelectNewNextBubble()
    {
        SelectNextBubble();
        ApplyAll();
    }

    // ============================================================
    // 기존 코드 호환용 함수입니다.
    // ============================================================
    public void RerollNextBubble()
    {
        SelectNewNextBubble();
    }

    // ============================================================
    // Inspector에서 다음 버블 Sprite 목록을 읽을 수 있게 해주는 함수입니다.
    // ============================================================
    public Sprite[] GetBubbleSprites()
    {
        return bubbleSprites;
    }

    // ============================================================
    // 실제 발사 버블도 같은 색 보정을 쓸 수 있게 해주는 함수입니다.
    // ============================================================
    public Color GetBubbleTintColor()
    {
        return bubbleTintColor;
    }
}
