# 기능 17번 프롬프트: 조준선 벽 반사 만들기

나는 초보자이고, Unity 6으로 2D 버블슈터 게임을 만들고 있어.

이번에는 기능 17번인 **조준선 벽 반사** 기능만 만들고 싶어.

## 참고 문서 위치
- `C:\Users\admin\Documents\BubbleGame\BubbleShooter_Planning.md`
- `C:\Users\admin\Documents\BubbleGame\BubbleShooter_StagePlan.md`
- `C:\Users\admin\Documents\BubbleGame\BubbleShooter_FeatureList.md`

## 작업 폴더
- `C:\Users\admin\Documents\BubbleGame`

## Unity 프로젝트 폴더
- `C:\Users\admin\Documents\BubbleGame\BubbleGame`

## 현재 상태
- 기능 1번~4번 배경 기능은 완료했어.
- 기능 5번~8번 타이머 숫자/게이지 기능은 완료했어.
- 기능 9번~11번 점수 기능은 완료했어.
- 기능 12번 슈터 배치 기능은 완료했어.
- 기능 13번 마우스/키보드 조준 기능은 완료했어.
- 기능 14번 모바일 터치 조준은 패스했어.
- 기능 15번 아래쪽 조준 제한 기능은 완료했어.
- 기능 16번 조준선 표시 기능은 완료했어.
- 선행 기능으로 `WallsRoot`, `LeftWall`, `RightWall`, `Ceiling` 벽 오브젝트를 만들었어.
- `LeftWall`, `RightWall`, `Ceiling`에는 `BoxCollider2D`가 붙어 있어.
- `WallBoundsController.cs`는 `WallsRoot`에 붙어 있어.
- 자동 세팅 메뉴는 제거했어.
- 앞으로 자동 세팅 메뉴는 만들지 말고, 수동 세팅 방식으로 알려줘.
- `EventSystem`은 삭제된 상태야. 이번 기능에는 필요 없으니 만들지 마.
- 기존 기능은 절대 망가뜨리지 마.
- `ShooterRoot` 위치를 자동으로 바꾸지 마.

## 목표
현재 조준선은 직선으로만 표시돼.

이번 기능에서는 조준선이 왼쪽 벽 또는 오른쪽 벽에 닿으면, 벽에서 반사되어 꺾인 선으로 보이게 만들고 싶어.

나중에 실제 버블 발사와 버블 벽 반사 기능에서 같은 방향 계산을 사용할 수 있게 준비하고 싶어.

## 이번 기능에서 만들 것
- 조준선이 벽에 닿는지 검사하기.
- 벽에 닿으면 조준선을 꺾어서 표시하기.
- 반사 방향은 `Vector2.Reflect`로 계산하기.
- 벽 감지는 `Physics2D.Raycast`로 처리하기.
- 기본 반사 횟수는 1번으로 하기.
- Inspector에서 최대 반사 횟수를 조절할 수 있게 하기.
- Inspector에서 벽 감지용 LayerMask를 설정할 수 있게 하기.

## 이번 기능에서 만들지 말 것
- 버블 발사는 만들지 마.
- 실제 버블 이동은 만들지 마.
- 실제 버블 Rigidbody2D 벽 반사는 만들지 마.
- 천장에 버블 붙는 기능은 만들지 마.
- 자동 세팅 메뉴는 만들지 마.
- `EventSystem`은 만들지 마.
- 기존 배경, 타이머, 게이지, 점수, 슈터, 조준선 기본 기능은 망가뜨리지 마.

## 구현 조건
- Unity 6 2D 프로젝트 기준으로 작성해줘.
- 초보자도 이해할 수 있게 쉬운 말로 설명해줘.
- 어려운 영어 용어가 나오면 한국말 뜻도 같이 설명해줘.
- 기존 `ShooterAimLineController.cs`를 수정하는 방식으로 해줘.
- 기존 `LineRenderer` 점선 설정은 최대한 유지해줘.
- 기존 `ShooterAimController.cs`의 조준 각도 제한 기능은 유지해줘.
- 조준 방향은 기존처럼 수동 연결된 조준 방향 Transform 기준을 사용해줘.
- `Physics2D.Raycast`를 사용해 벽 충돌 지점을 찾게 해줘.
- `RaycastHit2D.normal`을 사용해 벽의 방향을 구해줘.
- `Vector2.Reflect`를 사용해 반사 방향을 계산해줘.
- `LineRenderer.positionCount`를 반사 지점 개수에 맞게 조절해줘.
- `LineRenderer.useWorldSpace = true`를 유지하거나 필요하면 명확히 설명해줘.
- `maxReflections` 변수를 만들어 Inspector에서 조절할 수 있게 해줘.
- `maxReflections` 기본값은 `1`로 해줘.
- `wallLayerMask` 변수를 만들어 Inspector에서 벽 레이어를 선택할 수 있게 해줘.
- 벽 오브젝트가 `Default` 레이어에 있어도 테스트할 수 있게 설명해줘.
- 코드는 한국어 주석을 충분히 달아줘.
- `[Header("한글 설명")]`과 `[Tooltip("초보자용 설명")]`을 사용해줘.
- 변수 이름은 Unity/C# 규칙 때문에 영어로 유지해줘.

## 추천 구현 방식
- `ShooterAimLineController.cs` 안에서 조준선 경로를 계산해줘.
- 시작점은 슈터 또는 조준선 시작 위치 Transform으로 해줘.
- 방향은 조준 방향 Transform의 `up` 또는 기존 코드에서 사용하던 방향 기준을 유지해줘.
- Raycast를 쏘고 벽에 닿으면 그 지점을 LineRenderer 점으로 추가해줘.
- 닿은 지점에서 살짝 앞으로 이동한 뒤 다시 Raycast를 쏴서 자기 자신에게 다시 맞는 문제를 피하게 해줘.
- 반사 횟수가 끝나면 남은 길이만큼 직선으로 조준선을 이어줘.

## 수동 세팅 조건
- 자동 세팅 메뉴를 만들지 마.
- Unity 상단 메뉴에 `Bubble Shooter` 같은 메뉴를 추가하지 마.
- Inspector에서 직접 값을 넣는 방법을 알려줘.
- 어떤 GameObject에 어떤 스크립트가 붙어 있어야 하는지 알려줘.
- `Wall Layer Mask`를 어떻게 설정하는지 초보자도 알 수 있게 설명해줘.

## 테스트 조건
- Scene 창에서 `WallsRoot` 아래에 `LeftWall`, `RightWall`, `Ceiling`이 있는지 확인할 수 있어야 해.
- 각 벽에 `BoxCollider2D`가 붙어 있어야 해.
- 조준선을 왼쪽 벽이나 오른쪽 벽으로 향하게 하면 벽에서 꺾여 보여야 해.
- 조준선을 천장 쪽으로 향하게 하면 천장에서 멈추거나 반사 처리되는지 확인할 수 있어야 해.
- `maxReflections` 값을 `1`로 했을 때 한 번만 꺾여야 해.
- `maxReflections` 값을 `2`로 했을 때 두 번까지 꺾일 수 있어야 해.
- 기존 조준 각도 제한 기능은 계속 정상 작동해야 해.
- 기존 배경, 타이머, 게이지, 점수, 슈터는 정상 작동해야 해.

## 자주 틀리는 부분도 알려줘
- 벽에 `BoxCollider2D`가 없으면 Raycast가 감지하지 못한다는 점.
- `Wall Layer Mask`가 벽 레이어를 포함하지 않으면 조준선이 반사되지 않는다는 점.
- `LineRenderer`의 `positionCount`를 제대로 설정하지 않으면 선이 이상하게 보인다는 점.
- Raycast 시작점이 벽 안쪽에 있으면 감지가 이상해질 수 있다는 점.
- 반사 후 다시 Raycast를 쏠 때 충돌 지점에서 아주 조금 떨어뜨려 시작해야 한다는 점.

## 반드시 아래 형식으로 답해줘

1. 기능 설명
2. 전체 코드
3. 코드 설명
4. 유니티 적용 방법
5. 오류 체크 포인트
6. 초보자용으로 필요한 C# 스크립트 파일 이름
7. Inspector에서 조절할 변수
8. 테스트 성공 기준
9. 다음 기능으로 넘어가기 전 체크리스트
