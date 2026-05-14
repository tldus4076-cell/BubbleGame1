# 기능 20 프롬프트: 발사 후 다음 버블이 현재 버블로 바뀌기

아래 프롬프트를 그대로 복사해서 사용하세요.

```text
나는 초보자이고, Unity 6으로 2D 버블슈터 게임을 만들고 있어.

이번에는 기능 목록 20번인 "발사 후 다음 버블이 현재 버블로 바뀌기"만 만들고 싶어.

참고 문서 위치:
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_Planning.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_StagePlan.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_FeatureList.md

작업 폴더:
- C:\Users\admin\Documents\BubbleGame

Unity 프로젝트 폴더:
- C:\Users\admin\Documents\BubbleGame\BubbleGame

현재 상태:
- 기능 1번~19번은 완료했어.
- BubbleCurrentController.cs가 있고 ShooterRoot에 붙어 있어.
- 현재 버블은 ShooterVisual 기준으로 슈터 위에 보여.
- BubbleNextController.cs가 있고 ShooterRoot에 붙어 있어.
- 다음 버블은 현재 버블 옆에 작게 보여.
- 다음 버블은 현재 버블과 다른 색으로 랜덤 선택됨.
- Bubble Sprites 배열에 빨강, 파랑, 노랑 이미지가 들어가 있어.
- WallsRoot, LeftWall, RightWall, Ceiling 벽 오브젝트가 준비되어 있어.
- 기존 배경, 타이머, 게이지, 점수, 슈터, 조준선, 현재 버블, 다음 버블 기능은 절대 망가뜨리지 말아줘.
- EventSystem은 삭제된 상태야. 이번 기능에는 필요 없으면 만들지 마.
- 앞으로 자동 세팅 메뉴는 만들지 말고, 수동 세팅 방식으로 알려줘.

목표:
한 발 쏜 뒤에 다음 버블이 슈터 위의 현재 버블로 바뀌게 하고 싶어.
지금은 버블 발사 기능(21번)은 아직 안 만들었지만, "다음 버블을 현재 버블로 바꾸는 로직"만 미리 준비하고 싶어.

조건:
- Unity 6 2D 프로젝트 기준으로 설명해줘.
- 이번에는 기능 20번 "다음 버블이 현재 버블로 바뀌기"만 만들어줘.
- 버블 발사는 아직 만들지 마. 그건 기능 21번에서 만들 거야.
- 기능 20번은 "바꾸기 로직"만 만들면 돼.
- 나중에 기능 21번에서 버블을 쏠 때 이 바꾸기 로직을 호출하면 됩니다.
- 기존 BubbleCurrentController.cs의 현재 버블 표시 기능은 유지해줘.
- 기존 BubbleNextController.cs의 다음 버블 표시 기능은 유지해줘.
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

바꾸기 로직 방식:
- BubbleSwapController.cs 같은 새 스크립트를 만들어줘.
- BubbleSwapController.cs는 ShooterRoot에 붙이는 방식으로 해줘.
- SwapBubbles() 함수를 만들어서 호출하면 다음 버블이 현재 버블로 바뀌게 해줘.
- SwapBubbles() 안에서:
  1. BubbleCurrentController의 SetNextBubble()을 호출해서 현재 버블 이미지를 새로 고칩니다.
  2. BubbleNextController의 SelectNewNextBubble()을 호출해서 다음 버블 이미지를 새로 고칩니다.
- 이렇게 하면 나중에 기능 21번에서 버블을 쏜 뒤에 SwapBubbles()를 한 번 호출하면 됩니다.
- 테스트용으로 키보드 키를 누르면 SwapBubbles()가 실행되게 해줘.
- 예: Space 키를 누르면 다음 버블이 현재 버블로 바뀌게 해줘.
- Inspector에서 테스트 키를 켜고 끌 수 있게 해줘.

코드 작성 조건:
- BubbleSwapController.cs를 새로 만들어줘.
- 기존 BubbleCurrentController.cs는 가능한 건드리지 마.
- 기존 BubbleNextController.cs는 가능한 건드리지 마.
- Awake/Start에서 초기화해줘.
- Update에서 키 입력을 확인해줘.
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
- 이번에는 기능 20번만 만들어줘.
- 버블 발사는 만들지 마.
- ShooterRoot 위치를 자동으로 바꾸지 마.
- 기존 배경, 타이머, 게이지, 점수 코드는 건드리지 마.
- EventSystem은 다시 만들지 마.
```

## 사용 방법

1. 위 `text` 박스 안의 내용을 전부 복사합니다.
2. 다음 작업 요청으로 그대로 붙여넣습니다.
3. 기능 20번이 성공하면 다음에는 기능 21번 프롬프트를 만들면 됩니다.
