using UnityEngine;

// BubbleSlot은 버블이 들어갈 수 있는 "한 칸" 정보입니다.
// Cell은 "칸"이라는 뜻이고, Slot은 "자리"라는 뜻입니다.
[System.Serializable]
public class BubbleSlot
{
    [Header("칸 위치 정보")]
    [Tooltip("몇 번째 줄인지 저장합니다. 0은 가장 위쪽 줄입니다.")]
    public int row;

    [Tooltip("몇 번째 칸인지 저장합니다. 0은 가장 왼쪽 칸입니다.")]
    public int col;

    [Tooltip("이 칸의 월드 좌표입니다. 발사 버블은 이 위치로 이동한 뒤 정확히 스냅됩니다.")]
    public Vector3 worldPosition;

    [Header("칸 상태")]
    [Tooltip("체크되어 있으면 이 칸에는 이미 버블이 있습니다.")]
    public bool occupied;

    [Tooltip("이 칸에 들어있는 버블 오브젝트입니다.")]
    public GameObject bubbleObject;

    public BubbleSlot(int row, int col, Vector3 worldPosition)
    {
        this.row = row;
        this.col = col;
        this.worldPosition = worldPosition;
        occupied = false;
        bubbleObject = null;
    }
}
