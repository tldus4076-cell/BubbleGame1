# 선행 기능 프롬프트: 벽 만들기

기능 목록에는 "벽 만들기"가 별도 번호로 없지만, 기능 17번 조준선 반사와 기능 25~27번 벽 반사를 만들기 전에 필요합니다.

아래 프롬프트를 그대로 복사해서 사용하세요.

```text
나는 초보자이고, Unity 6으로 2D 버블슈터 게임을 만들고 있어.

기능 목록에는 "벽 만들기"가 따로 없지만, 기능 17번 조준선 벽 반사와 기능 25~27번 버블 벽 반사를 만들기 전에 왼쪽 벽, 오른쪽 벽, 천장 같은 충돌 영역이 필요해 보여.

이번에는 선행 기능으로 "게임 화면 양쪽 벽과 천장 만들기"만 만들고 싶어.

참고 문서 위치:
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_Planning.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_StagePlan.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_FeatureList.md

작업 폴더:
- C:\Users\admin\Documents\BubbleGame

Unity 프로젝트 폴더:
- C:\Users\admin\Documents\BubbleGame\BubbleGame

현재 상태:
- 기능 1번~4번 배경 기능은 완료했어.
- 기능 5번~8번 타이머 숫자/게이지 기능은 완료했어.
- 기능 9번~11번 점수 기능은 완료했어.
- 기능 12번 슈터 배치 기능은 완료했어.
- 기능 13번 마우스/키보드 조준 기능은 완료했어.
- 기능 14번 모바일 터치 조준은 패스했어.
- 기능 15번 아래쪽 조준 제한 기능은 완료했어.
- 기능 16번 조준선 표시 기능은 완료했어.
- 기존 기능은 절대 망가뜨리지 말아줘.
- EventSystem은 삭제된 상태야. 이번 기능에는 필요 없으면 만들지 마.

목표:
화면 왼쪽, 오른쪽, 위쪽에 벽 충돌 영역을 만들고 싶어.
나중에 조준선 반사와 버블 반사 기능에서 사용할 수 있게 준비하고 싶어.

조건:
- Unity 6 2D 프로젝트 기준으로 설명해줘.
- 이번 기능은 기능 목록 번호에 없는 선행 기능이야.
- 왼쪽 벽, 오른쪽 벽, 천장을 만들어줘.
- 벽은 화면 밖 또는 화면 가장자리에 배치해줘.
- 벽은 플레이어 눈에 보이지 않아도 돼.
- 디버그용으로 보이게 할지 숨길지 Inspector에서 선택할 수 있으면 좋아.
- 벽에는 Collider2D를 붙여줘.
- 추천은 BoxCollider2D를 사용해줘.
- 벽은 Rigidbody2D 없이 고정된 Collider로 만들어줘.
- 나중에 버블 Rigidbody2D가 벽에 부딪힐 수 있어야 해.
- 왼쪽 벽과 오른쪽 벽은 나중에 벽 반사에 사용돼.
- 천장은 나중에 버블이 천장에 붙는 기능에서 사용돼.
- 기존 배경, 타이머, 게이지, 점수, 슈터, 조준선 코드는 건드리지 마.
- ShooterRoot 위치를 자동으로 바꾸지 마.
- 초보자도 이해할 수 있게 쉬운 말로 설명해줘.
- 어려운 영어 용어가 나오면 한국말 뜻도 같이 설명해줘.
- 전체 코드를 빠짐없이 작성해줘.
- 코드마다 주석을 초보자도 알아보기 쉽게 달아줘.
- 어느 GameObject에 스크립트를 붙이는지 알려줘.
- Inspector에서 뭘 설정해야 하는지 알려줘.
- 자주 틀리는 부분도 알려줘.

오브젝트 조건:
- WallsRoot라는 빈 GameObject를 만들어줘.
- WallsRoot 아래에 LeftWall, RightWall, Ceiling 오브젝트를 만들어줘.
- LeftWall에는 BoxCollider2D를 붙여줘.
- RightWall에는 BoxCollider2D를 붙여줘.
- Ceiling에는 BoxCollider2D를 붙여줘.
- 각 벽의 위치와 크기는 카메라 화면 기준으로 자동 계산되게 해줘.
- 벽 두께는 Inspector에서 조절할 수 있게 해줘.
- 벽이 화면보다 살짝 밖에 있어도 괜찮아.
- 카메라가 Orthographic인 2D 기준으로 계산해줘.

코드 작성 조건:
- WallBoundsController.cs 같은 새 스크립트를 만들어줘.
- WallBoundsController.cs는 WallsRoot에 붙이는 방식으로 해줘.
- targetCamera, wallThickness, extraHeight, showDebugVisuals 같은 변수를 만들어줘.
- targetCamera는 비워두면 Main Camera를 자동으로 찾게 해줘.
- SetupWalls() 함수를 만들어줘.
- SetupWalls()는 왼쪽 벽, 오른쪽 벽, 천장을 만들거나 찾아서 위치와 크기를 맞추게 해줘.
- ContextMenu를 사용해서 Inspector에서 "벽 다시 맞추기" 같은 메뉴를 실행할 수 있게 해줘.
- Play 시작 때 자동으로 벽을 맞출지 autoSetupOnStart bool로 선택할 수 있게 해줘.
- 기본값은 true로 해줘.
- 단, 사용자가 직접 위치를 바꾸고 싶으면 autoSetupOnStart를 끄면 된다고 설명해줘.
- 각 벽에 BoxCollider2D를 붙여줘.
- Debug용 SpriteRenderer는 선택사항이야. 너무 복잡하면 만들지 마.
- 만약 Debug용으로 보이게 한다면 SpriteRenderer나 Gizmos 중 쉬운 방법으로 해줘.
- [Header("한글 설명")]을 사용해줘.
- [Tooltip("초보자용 설명")]도 한글로 추가해줘.
- 변수 이름은 Unity/C# 규칙 때문에 영어로 유지해줘.
- 코드 주석은 한글로 작성해줘.
- 어려운 문법보다 쉬운 문법으로 작성해줘.
- 이 함수는 언제 호출되는지 설명해줘.
- 이 변수가 어떤 역할인지 설명해줘.
- 코드 실행 흐름을 순서대로 알려줘.
- 초보자가 실수하기 쉬운 부분도 알려줘.

자동 세팅 조건:
- 자동 세팅 메뉴가 필요하다면 Bubble Shooter > Setup Walls 메뉴를 만들어줘.
- 메뉴를 누르면 WallsRoot, LeftWall, RightWall, Ceiling이 만들어지게 해줘.
- EventSystem은 만들지 마.

테스트 조건:
- Scene 창에서 LeftWall, RightWall, Ceiling이 있는지 확인할 수 있어야 해.
- BoxCollider2D가 붙어 있어야 해.
- 벽 위치가 카메라 화면 왼쪽, 오른쪽, 위쪽에 맞게 있어야 해.
- 아직 버블 발사 기능이 없으니 실제 반사 테스트는 하지 않아도 돼.
- 이번 테스트는 벽 오브젝트와 Collider가 제대로 만들어졌는지 확인하면 돼.

반드시 아래 형식으로 답해줘:

1. 기능 설명
2. 전체 코드
3. 코드 설명
4. 유니티 적용 방법
5. 오류 체크 포인트
6. 초보자용으로 필요한 C# 스크립트 파일 이름
7. Inspector에서 조절할 변수
8. 테스트 성공 기준
9. 다음 기능으로 넘어가기 전 체크리스트

주의:
- 이번에는 벽 만들기만 해줘.
- 조준선 벽 반사는 만들지 마. 그건 기능 17번에서 만들 거야.
- 버블 벽 반사는 만들지 마. 그건 기능 25~27번에서 만들 거야.
- 버블 발사는 만들지 마.
- 기존 배경, 타이머, 게이지, 점수, 슈터, 조준선 코드는 건드리지 마.
- EventSystem은 다시 만들지 마.
```

## 진행 메모

- 기능 목록에는 벽 만들기가 별도 번호로 없습니다.
- 이 프롬프트는 기능 17번과 기능 25~28번 전에 필요한 선행 준비 기능입니다.
