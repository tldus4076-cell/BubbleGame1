# 기능 21 프롬프트: 버블 발사하기

아래 프롬프트를 그대로 복사해서 사용하세요.

```text
나는 초보자이고, Unity 6으로 2D 버블슈터 게임을 만들고 있어.

이번에는 기능 목록 21번인 "버블 발사"만 만들고 싶어.

참고 문서 위치:
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_Planning.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_StagePlan.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_FeatureList.md

작업 폴더:
- C:\Users\admin\Documents\BubbleGame

Unity 프로젝트 폴더:
- C:\Users\admin\Documents\BubbleGame\BubbleGame

현재 상태:
- 기능 1번~20번은 완료했어.
- BubbleCurrentController.cs가 있고 ShooterRoot에 붙어 있어.
- 현재 버블은 ShooterVisual 기준으로 슈터 위에 보여.
- BubbleNextController.cs가 있고 다음 버블은 현재 버블 옆에 작게 보여.
- BubbleSwapController.cs가 있고 Space 키로 다음 버블을 현재 버블로 바꿀 수 있어.
- ShooterAimController.cs가 있고 슈터가 마우스/키보드로 조준해.
- ShooterAimLineController.cs가 있고 조준선이 벽에 닿으면 꺾여서 보여.
- WallsRoot, LeftWall, RightWall, Ceiling 벽 오브젝트가 준비되어 있고 BoxCollider2D가 붙어 있어.
- Bubble Sprites 배열에 빨강, 파랑, 노랑 이미지가 들어가 있어.
- 기존 배경, 타이머, 게이지, 점수, 슈터, 조준선, 현재 버블, 다음 버블, 바꾸기 기능은 절대 망가뜨리지 말아줘.
- EventSystem은 삭제된 상태야. 이번 기능에는 필요 없으면 만들지 마.
- 앞으로 자동 세팅 메뉴는 만들지 말고, 수동 세팅 방식으로 알려줘.

목표:
마우스 왼쪽 버튼을 클릭하면 현재 버블이 조준 방향으로 날아가게 하고 싶어.
버블이 벽에 닿으면 반사되고, 다른 버블이나 천장에 닿으면 멈추게 하고 싶어.

조건:
- Unity 6 2D 프로젝트 기준으로 설명해줘.
- 이번에는 기능 21번 "버블 발사"만 만들어줘.
- 버블 발사 후 매칭(3개 이상 같은 색 제거)은 아직 만들지 마. 그건 기능 31번~34번에서 만들 거야.
- 연결 끊긴 버블 떨어뜨리기도 아직 만들지 마. 그건 기능 37번~39번에서 만들 거야.
- 마우스 왼쪽 버튼을 클릭하면 현재 버블이 조준 방향으로 날아가게 해줘.
- 발사된 버블은 Rigidbody2D와 Collider2D를 사용해서 물리적으로 움직이게 해줘.
- 발사된 버블은 벽(WallsRoot 안의 LeftWall, RightWall, Ceiling)에 닿으면 반사돼야 해.
- 발사된 버블은 스테이지에 있는 버블(StageBubbleLayout이 만든 버블)에 닿으면 그 자리에 멈춰야 해.
- 발사된 버블은 천장(Ceiling)에 닿으면 그 자리에 멈춰야 해.
- 발사 후에는 BubbleSwapController의 SwapBubbles()를 호출해서 다음 버블을 현재 버블로 바꿔야 해.
- 기존 조준/조준선/배경/타이머/점수 기능은 유지해줘.
- ShooterRoot 위치는 자동으로 바꾸지 마.
- 기존 StageBackgroundController.cs, TimerController.cs, TimerGaugeController.cs, ScoreController.cs, ShooterAimController.cs, ShooterAimLineController.cs 등은 건드리지 마.
- 초보자도 이해할 수 있게 쉬운 말로 설명해줘.
- 어려운 영어 용어가 나오면 한국말 뜻도 같이 설명해줘.
- 전체 코드를 빠짐없이 작성해줘.
- 코드마다 주석을 초보자도 알아보기 쉽게 달아줘.
- 어느 GameObject에 스크립트를 붙이는지 알려줘.
- Inspector에서 뭘 설정해야 하는지 알려줘.
- 자주 틀리는 부분도 알려줘.

버블 발사 방식:
- BubbleLauncherController.cs 같은 새 스크립트를 만들어줘.
- BubbleLauncherController.cs는 ShooterRoot에 붙이는 방식으로 해줘.
- 마우스 왼쪽 버튼을 클릭하면 현재 버블을 발사하게 해줘.
- 발사할 때 현재 버블의 Sprite와 색을 복사해서 새 GameObject를 만들어줘.
- 새 GameObject에는 Rigidbody2D를 붙여서 물리적으로 움직이게 해줘.
- 새 GameObject에는 CircleCollider2D를 붙여서 충돌 감지를 하게 해줘.
- 발사 방향은 ShooterAimController의 조준 방향을 사용해줘.
- 발사 속도는 launchSpeed 변수로 Inspector에서 조절하게 해줘.
- 발사 후에는 BubbleSwapController의 SwapBubbles()를 호출해서 다음 버블을 현재 버블로 바꿔줘.

벽 반사 조건:
- 발사된 버블이 LeftWall이나 RightWall에 닿으면 반사돼야 해.
- Rigidbody2D의 Velocity를 사용해서 반사 방향을 계산해줘.
- 또는 Physics Material 2D를 사용해서 벽 반사를 구현해도 돼.

멈춤 조건:
- 발사된 버블이 Ceiling에 닿으면 그 자리에 멈춰야 해.
- 발사된 버블이 스테이지 버블에 닿으면 그 자리에 멈춰야 해.
- 멈춘 뒤에는 Rigidbody2D를 비활성화하거나 Velocity를 0으로 만들어줘.
- 멈춘 버블은 나중에 매칭/제거 기능에서 사용할 수 있도록 StageBubbleLayout의 자식으로 넣어줘.

코드 작성 조건:
- BubbleLauncherController.cs를 새로 만들어줘.
- 기존 BubbleCurrentController.cs는 가능한 건드리지 마.
- 기존 BubbleNextController.cs는 가능한 건드리지 마.
- 기존 BubbleSwapController.cs는 가능한 건드리지 마.
- Awake/Start에서 초기화해줘.
- Update에서 마우스 입력을 확인해줘.
- [Header("한글 설명")]을 사용해줘.
- [Tooltip("초보자용 설명")]도 한글로 추가해줘.
- 변수 이름은 Unity/C# 규칙 때문에 영어로 유지해줘.
- 인스펙터에 보이는 설명은 초보자가 이해하기 쉽게 한글로 작성해줘.
- 코드 주석은 한글로 작성해줘.
- 어려운 문법보다 쉬운 문법으로 작성해줘.
- 이 함수는 언제 호출되는지 설명해줘.
- 이 변수가 어떤 역할인지 설명해줘.
- 코드 실행 흐름을 순서대로 알려줘.
- 초보자가 실수하기 쉬운 부분도 알려줘.

수동 세팅 조건:
- 자동 세팅 메뉴를 만들지 마.
- Unity 상단 메뉴에 Bubble Shooter 같은 메뉴를 추가하지 마.
- Inspector에서 직접 값을 넣는 방법을 알려줘.
- 어떤 GameObject에 어떤 스크립트가 붙어 있어야 하는지 알려줘.

자동저장 조건:
- 기능 하나가 완성되면 Unity에서 Scene(씬)을 저장하라고 알려줘.
- Ctrl + S로 저장하는 방법도 알려줘.

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

설명 스타일:
- 초등학생도 이해할 수 있게 차근차근 설명해줘.
- 코드만 알려주지 말고 코드 설명도 해줘.
- 한 줄씩 설명하고 설치 순서도 알려줘.
- 오브젝트 세팅 순서도 알려줘.
- 오류날 수 있는 부분도 알려줘.
- 함수별 설명을 해줘.
- 이 코드가 왜 이렇게 동작하는지 초보자도 이해하게 설명해줘.
- 실행 순서를 번호로 설명해줘.
- 변수, 함수, if문이 각각 무슨 역할인지 알려줘.
- 그림처럼 비유해서 설명해줘.
- 마지막에 내가 외워야 할 핵심만 짧게 뽑아줘.

주의:
- 이번에는 기능 21번만 만들어줘.
- 매칭/제거 기능은 만들지 마.
- 연결 끊긴 버블 떨어뜨리기도 만들지 마.
- ShooterRoot 위치를 자동으로 바꾸지 마.
- 기존 배경, 타이머, 게이지, 점수 코드는 건드리지 마.
- EventSystem은 다시 만들지 마.
```

## 사용 방법

1. 위 `text` 박스 안의 내용을 전부 복사합니다.
2. 다음 작업 요청으로 그대로 붙여넣습니다.
3. 기능 21번이 성공하면 다음에는 기능 22번 프롬프트를 만들면 됩니다.
