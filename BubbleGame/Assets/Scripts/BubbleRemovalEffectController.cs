using System.Collections;
using UnityEngine;

// ============================================================
// BubbleRemovalEffectController는 버블이 제거될 때 시각적 효과를 보여주는 스크립트입니다.
//
// [어디에 붙이나요?]
// ShooterRoot 오브젝트 또는 별도의 빈 오브젝트에 붙여서 사용합니다.
//
// [Inspector에서 연결할 것]
// - Grid Manager: 버블 제거 이벤트를 보내주는 BubbleGridManager
//   (또는 BubbleGridManager에서 직접 연결해도 됩니다)
//
// [동작 방식]
// 1. BubbleGridManager가 버블을 제거하기 직전에 PlayRemovalEffect()를 호출합니다.
// 2. PlayRemovalEffect()는 임시 동그라미 SpriteRenderer를 만들어서 버블이 있던 위치에 표시합니다.
// 3. Coroutine이 0.4초 동안 스케일을 키웠다가 줄이는 애니메이션을 실행합니다.
// 4. 애니메이션이 끝나면 임시 오브젝트를 Destroy()로 제거합니다.
//
// [왜 필요한가요?]
// 버블이 갑자기 사라지면 화면이 너무 휑합니다.
// 살짝 커졌다 작아지면서 사라지면 "방금 사라졌구나"라고 한눈에 알 수 있습니다.
// ============================================================
public class BubbleRemovalEffectController : MonoBehaviour
{
    [Header("격자 매니저 연결")]
    [Tooltip("버블 제거 시 효과를 발동시키는 BubbleGridManager를 연결합니다.")]
    [SerializeField] private BubbleGridManager gridManager;

    [Header("효과 시간 설정")]
    [Tooltip("효과가 보이는 총 시간(초)입니다. 0.4초 정도가 적당합니다.")]
    [SerializeField] private float effectDuration = 0.4f;

    [Tooltip("효과가 가장 커지는 배율입니다. 1.5배 정도가 적당합니다.")]
    [SerializeField] private float maxScaleMultiplier = 1.5f;

    [Tooltip("버블 원래 크기입니다. 스테이지 버블 크기와 비슷하게 설정하세요.")]
    [SerializeField] private float baseEffectSize = 0.5f;

    [Header("효과 정렬 설정")]
    [Tooltip("효과가 다른 오브젝트보다 앞에 보이게 하는 정렬 순서입니다.")]
    [SerializeField] private int effectSortingOrder = 100;

    // ============================================================
    // Start는 게임이 시작될 때 한 번 호출됩니다.
    // ============================================================
    private void Start()
    {
        // GridManager가 연결되어 있으면, GridManager의 효과를 발동시키는 함수를
        // PlayRemovalEffect로 자동으로 연결합니다.
        // (GridManager가 직접 호출하지 않을 때 대비용)
    }

    // ============================================================
    // 버블이 제거될 때 호출되는 함수입니다.
    // BubbleGridManager.ClearBubbleSlot()에서 호출됩니다.
    //
    // worldPosition: 효과가 보일 월드 좌표 (버블이 있던 위치)
    // bubbleColor: 버블 색 (효과 색으로 사용)
    // ============================================================
    public void PlayRemovalEffect(Vector3 worldPosition, Color bubbleColor)
    {
        // 효과 오브젝트를 만듭니다.
        GameObject effectObject = CreateEffectObject(worldPosition, bubbleColor);

        // 효과 애니메이션을 Coroutine으로 실행합니다.
        StartCoroutine(AnimateEffect(effectObject));
    }

    // ============================================================
    // 효과 오브젝트를 만드는 함수입니다.
    // 임시 GameObject + 임시 동그라미 Sprite + SpriteRenderer로 구성됩니다.
    // ============================================================
    private GameObject CreateEffectObject(Vector3 worldPosition, Color bubbleColor)
    {
        // 빈 오브젝트를 만듭니다.
        GameObject effectObject = new GameObject("BubbleRemovalEffect");
        effectObject.transform.position = worldPosition;
        effectObject.transform.localScale = Vector3.one * baseEffectSize;

        // 임시 동그라미 Sprite를 만들어서 SpriteRenderer에 넣습니다.
        SpriteRenderer renderer = effectObject.AddComponent<SpriteRenderer>();
        renderer.sprite = CreateCircleSprite();
        renderer.color = bubbleColor;
        renderer.sortingOrder = effectSortingOrder;

        return effectObject;
    }

    // ============================================================
    // 임시 동그라미 Sprite를 만드는 함수입니다.
    // (ShooterController의 CreateTemporaryShooterSprite()와 비슷한 방식)
    // ============================================================
    private Sprite CreateCircleSprite()
    {
        int size = 64;
        Texture2D texture = new Texture2D(size, size);
        float center = (size - 1) / 2f;
        float radius = size * 0.45f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distanceX = x - center;
                float distanceY = y - center;
                float distanceFromCenter = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY);

                if (distanceFromCenter <= radius)
                {
                    texture.SetPixel(x, y, Color.white);
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
    }

    // ============================================================
    // 효과 애니메이션을 실행하는 Coroutine입니다.
    // 첫 절반: 크기가 maxScaleMultiplier까지 커짐
    // 두 번째 절반: 크기가 0까지 작아짐
    // 끝나면 오브젝트를 Destroy()로 제거
    // ============================================================
    private System.Collections.IEnumerator AnimateEffect(GameObject effectObject)
    {
        if (effectObject == null)
        {
            yield break;
        }

        float halfDuration = effectDuration * 0.5f;
        float elapsed = 0f;

        // 1단계: 크기가 1배에서 maxScaleMultiplier배까지 커집니다.
        while (elapsed < halfDuration)
        {
            if (effectObject == null)
            {
                yield break;
            }

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / halfDuration);
            float scale = Mathf.Lerp(1f, maxScaleMultiplier, t);
            effectObject.transform.localScale = Vector3.one * baseEffectSize * scale;
            yield return null;
        }

        elapsed = 0f;

        // 2단계: 크기가 maxScaleMultiplier배에서 0배까지 작아집니다.
        while (elapsed < halfDuration)
        {
            if (effectObject == null)
            {
                yield break;
            }

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / halfDuration);
            float scale = Mathf.Lerp(maxScaleMultiplier, 0f, t);
            effectObject.transform.localScale = Vector3.one * baseEffectSize * scale;
            yield return null;
        }

        // 애니메이션이 끝났으면 오브젝트를 제거합니다.
        if (effectObject != null)
        {
            Destroy(effectObject);
        }
    }
}
