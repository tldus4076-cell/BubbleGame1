using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// ShooterAimController는 마우스 위치를 따라 슈터가 회전하게 만드는 스크립트입니다.
// 슈터의 위치는 절대 바꾸지 않고, Z축 회전만 바꿉니다.
public class ShooterAimController : MonoBehaviour
{
    [Header("카메라 설정")]
    [Tooltip("마우스 화면 좌표를 월드 좌표로 바꿀 때 사용할 카메라입니다. 비워두면 Main Camera를 자동으로 찾습니다.")]
    [SerializeField] private Camera targetCamera;

    [Header("회전 대상 설정")]
    [Tooltip("실제로 회전할 Transform입니다. 비워두면 이 스크립트가 붙은 ShooterRoot를 회전합니다.")]
    [SerializeField] private Transform rotationTarget;

    [Header("방향 보정 설정")]
    [Tooltip("슈터 이미지가 바라보는 기본 방향을 보정하는 각도입니다. 이미지가 위쪽을 보고 있으면 보통 -90 또는 90을 사용합니다.")]
    [SerializeField] private float angleOffset = -90f;

    [Tooltip("0이면 즉시 회전합니다. 0보다 크면 숫자가 클수록 더 빠르게 부드럽게 회전합니다.")]
    [SerializeField] private float rotateSpeed = 0f;

    [Header("조준 사용 설정")]
    [Tooltip("체크되어 있으면 조준 기능 전체가 켜집니다.")]
    [SerializeField] private bool aimEnabled = true;

    [Tooltip("체크되어 있으면 마우스 위치를 따라 슈터가 회전합니다.")]
    [SerializeField] private bool useMouseAim = true;

    [Tooltip("체크되어 있으면 방향키와 WASD 키로 슈터를 회전할 수 있습니다.")]
    [SerializeField] private bool useKeyboardAim = true;

    [Header("키보드 조준 설정")]
    [Tooltip("키보드를 누를 때 초당 몇 도씩 회전할지 정합니다.")]
    [SerializeField] private float keyboardRotationSpeed = 120f;

    [Tooltip("W 키나 위쪽 방향키를 눌렀을 때 맞출 위쪽 각도입니다.")]
    [SerializeField] private float keyboardUpAngle = 90f;

    [Header("아래쪽 조준 제한 설정")]
    [Tooltip("체크되어 있으면 슈터가 아래쪽으로 조준되지 않도록 각도를 제한합니다.")]
    [SerializeField] private bool useAimLimit = true;

    [Tooltip("오른쪽 아래로 너무 내려가지 않게 막는 최소 각도입니다. 추천값은 30입니다.")]
    [SerializeField] private float minAimAngle = 30f;

    [Tooltip("왼쪽 아래로 너무 내려가지 않게 막는 최대 각도입니다. 추천값은 150입니다.")]
    [SerializeField] private float maxAimAngle = 150f;

    // 키보드 조준에서 현재 목표 각도를 기억합니다.
    private float currentAimAngle;

    // Awake는 게임이 시작될 때 Start보다 먼저 한 번 호출됩니다.
    private void Awake()
    {
        // 필요한 카메라와 회전 대상을 준비합니다.
        PrepareReferences();

        // 현재 회전값을 키보드 조준 각도로 기억합니다.
        SyncCurrentAimAngleWithRotation();
    }

    // Update는 게임이 실행되는 동안 매 프레임 호출됩니다.
    private void Update()
    {
        // 조준 기능이 꺼져 있으면 아무것도 하지 않습니다.
        if (!aimEnabled)
        {
            return;
        }

        // 연결이 비어 있으면 다시 찾아봅니다.
        PrepareReferences();

        // 키보드가 눌렸는지 먼저 확인합니다.
        bool keyboardUsed = false;

        if (useKeyboardAim)
        {
            keyboardUsed = AimWithKeyboard();
        }

        // 키보드를 누르지 않았을 때만 마우스 조준을 사용합니다.
        // 이렇게 하면 키보드 조작 중 마우스가 회전을 다시 덮어쓰지 않습니다.
        if (!keyboardUsed && useMouseAim)
        {
            AimAtMousePosition();
        }
    }

    // 필요한 연결을 자동으로 찾는 함수입니다.
    private void PrepareReferences()
    {
        // 카메라가 비어 있으면 Main Camera를 찾습니다.
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        // 회전 대상이 비어 있으면 이 오브젝트 자신을 회전 대상으로 사용합니다.
        if (rotationTarget == null)
        {
            rotationTarget = transform;
        }
    }

    // 마우스 위치를 바라보도록 슈터를 회전하는 함수입니다.
    private void AimAtMousePosition()
    {
        // 카메라나 회전 대상이 없으면 회전할 수 없으므로 멈춥니다.
        if (targetCamera == null || rotationTarget == null)
        {
            return;
        }

        // 마우스의 화면 좌표를 가져옵니다.
        Vector3 mouseScreenPosition = Input.mousePosition;

        // 카메라와 같은 깊이 문제를 피하기 위해 Z값을 카메라와 슈터 사이 거리로 맞춥니다.
        mouseScreenPosition.z = Mathf.Abs(targetCamera.transform.position.z - rotationTarget.position.z);

        // 화면 좌표를 월드 좌표로 바꿉니다.
        Vector3 mouseWorldPosition = targetCamera.ScreenToWorldPoint(mouseScreenPosition);

        // 슈터 위치에서 마우스 위치까지의 방향을 구합니다.
        Vector2 direction = mouseWorldPosition - rotationTarget.position;

        // 방향이 너무 작으면 각도 계산이 불안정하므로 멈춥니다.
        if (direction.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        // Atan2는 방향 벡터를 각도로 바꿔줍니다.
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 마우스가 아래쪽에 있어도 허용 각도 범위 안으로 제한합니다.
        angle = ApplyAimLimit(angle);

        // 마우스 각도를 현재 키보드 조준 각도에도 저장합니다.
        currentAimAngle = angle;

        // 계산한 각도로 회전합니다.
        ApplyRotation(angle);
    }

    // 키보드 방향키와 WASD로 슈터를 조준하는 함수입니다.
    // 키보드를 사용했으면 true, 사용하지 않았으면 false를 돌려줍니다.
    private bool AimWithKeyboard()
    {
        // 회전 대상이 없으면 조준할 수 없습니다.
        if (rotationTarget == null)
        {
            return false;
        }

        // 왼쪽/오른쪽 입력 값을 구합니다.
        float horizontalInput = GetKeyboardHorizontalInput();

        // W 또는 위쪽 방향키가 눌렸는지 확인합니다.
        bool upPressed = IsKeyboardUpPressed();

        // 아무 키도 누르지 않았으면 키보드 조준을 하지 않은 것입니다.
        if (Mathf.Approximately(horizontalInput, 0f) && !upPressed)
        {
            return false;
        }

        // 키보드를 처음 누를 때 현재 슈터 회전과 각도를 맞춥니다.
        SyncCurrentAimAngleWithRotation();

        // 왼쪽/오른쪽 키를 누르면 각도를 조금씩 돌립니다.
        currentAimAngle += horizontalInput * keyboardRotationSpeed * Time.deltaTime;

        // W 또는 위쪽 방향키를 누르면 위쪽 각도로 맞춥니다.
        if (upPressed)
        {
            currentAimAngle = keyboardUpAngle;
        }

        // 키보드로 돌려도 허용 각도 범위 밖으로 나가지 않게 제한합니다.
        currentAimAngle = ApplyAimLimit(currentAimAngle);

        // 계산한 각도로 회전합니다.
        ApplyRotation(currentAimAngle);

        return true;
    }

    // 실제 회전을 적용하는 함수입니다.
    private void ApplyRotation(float aimAngle)
    {
        // 슈터 이미지 기본 방향에 맞게 보정 각도를 더합니다.
        float finalAngle = aimAngle + angleOffset;

        // 2D에서는 Z축 회전만 사용합니다.
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, finalAngle);

        // rotateSpeed가 0 이하면 즉시 회전합니다.
        if (rotateSpeed <= 0f)
        {
            rotationTarget.rotation = targetRotation;
        }
        else
        {
            // rotateSpeed가 0보다 크면 부드럽게 회전합니다.
            rotationTarget.rotation = Quaternion.Lerp(rotationTarget.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }

    // 조준 각도를 허용 범위 안으로 제한하는 함수입니다.
    private float ApplyAimLimit(float angle)
    {
        // 제한 기능이 꺼져 있으면 원래 각도를 그대로 사용합니다.
        if (!useAimLimit)
        {
            return angle;
        }

        // 각도를 -180~180 범위로 정리합니다.
        float normalizedAngle = NormalizeAngle(angle);

        // Mathf.Clamp는 값을 최소값과 최대값 사이로 막아줍니다.
        // 예: 10도는 30도로 고치고, 170도는 150도로 고칩니다.
        return Mathf.Clamp(normalizedAngle, minAimAngle, maxAimAngle);
    }

    // 각도를 -180도부터 180도 사이로 정리하는 함수입니다.
    private float NormalizeAngle(float angle)
    {
        // 180보다 크면 360을 빼서 같은 방향의 작은 각도로 바꿉니다.
        while (angle > 180f)
        {
            angle -= 360f;
        }

        // -180보다 작으면 360을 더해서 같은 방향의 작은 각도로 바꿉니다.
        while (angle < -180f)
        {
            angle += 360f;
        }

        return angle;
    }

    // 현재 Transform 회전값을 currentAimAngle에 맞추는 함수입니다.
    private void SyncCurrentAimAngleWithRotation()
    {
        if (rotationTarget == null)
        {
            return;
        }

        // 현재 Z 회전값에서 angleOffset을 빼면 조준 각도가 됩니다.
        currentAimAngle = rotationTarget.eulerAngles.z - angleOffset;
    }

    // 다른 스크립트가 현재 조준 방향을 읽을 수 있게 해주는 함수입니다.
    // BubbleLauncherController가 이 함수를 사용하면 슈터 이미지 보정 각도(angleOffset)에 영향을 받지 않습니다.
    public Vector2 GetCurrentAimDirection()
    {
        float safeAimAngle = ApplyAimLimit(currentAimAngle);
        float angleRadian = safeAimAngle * Mathf.Deg2Rad;

        return new Vector2(Mathf.Cos(angleRadian), Mathf.Sin(angleRadian)).normalized;
    }

    // 키보드 좌우 입력을 구하는 함수입니다.
    private float GetKeyboardHorizontalInput()
    {
        float input = 0f;

        if (IsKeyPressedLeft())
        {
            input += 1f;
        }

        if (IsKeyPressedRight())
        {
            input -= 1f;
        }

        return input;
    }

    // 왼쪽 방향키 또는 A 키가 눌렸는지 확인합니다.
    private bool IsKeyPressedLeft()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed))
        {
            return true;
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
#else
        return false;
#endif
    }

    // 오른쪽 방향키 또는 D 키가 눌렸는지 확인합니다.
    private bool IsKeyPressedRight()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed))
        {
            return true;
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
#else
        return false;
#endif
    }

    // 위쪽 방향키 또는 W 키가 눌렸는지 확인합니다.
    private bool IsKeyboardUpPressed()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && (Keyboard.current.upArrowKey.isPressed || Keyboard.current.wKey.isPressed))
        {
            return true;
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
#else
        return false;
#endif
    }

}
