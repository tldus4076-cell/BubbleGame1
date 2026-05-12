# 기능 13 추가 프롬프트: 방향키와 WASD로 슈터 조준하기

아래 프롬프트를 그대로 복사해서 사용하세요.

```text
나는 초보자이고, Unity 6으로 2D 버블슈터 게임을 만들고 있어.

기능 13번 "마우스 위치를 따라 슈터가 회전하게 하기"는 이미 만들었어.
이번에는 기능 13번에 추가로 "방향키와 WASD 키를 눌러도 슈터 조준이 되게 하기"를 만들고 싶어.

참고 문서 위치:
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_Planning.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_StagePlan.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_FeatureList.md

작업 폴더:
- C:\Users\admin\Documents\BubbleGame

Unity 프로젝트 폴더:
- C:\Users\admin\Documents\BubbleGame\BubbleGame

현재 상태:
- ShooterRoot와 ShooterVisual이 있어.
- ShooterController.cs는 슈터 이미지와 정렬만 담당해.
- ShooterAimController.cs가 있고 마우스를 따라 슈터가 회전해.
- ShooterRoot 위치는 내가 직접 Scene 창에서 조절하는 방식이야.
- ShooterRoot 위치는 절대 자동으로 바뀌면 안 돼.
- 현재 추천 구조는 ShooterRoot는 위치 담당, ShooterVisual은 회전 담당이야.
- EventSystem은 삭제된 상태야. 이번 기능에는 필요 없으면 만들지 마.
- Player Settings의 Active Input Handling은 Both로 바꿔둔 상태야.
- 기능 14번 모바일 터치 조준은 패스할 예정이야. 이번에 만들지 마.

목표:
마우스 조준뿐만 아니라 키보드 방향키와 WASD 키로도 슈터 조준을 할 수 있게 만들고 싶어.

원하는 조작:
- 왼쪽 방향키 또는 A 키를 누르면 슈터가 왼쪽으로 회전
- 오른쪽 방향키 또는 D 키를 누르면 슈터가 오른쪽으로 회전
- 위쪽 방향키 또는 W 키를 누르면 슈터가 위쪽 방향을 향하는 데 도움이 되게 처리
- 아래쪽 방향키 또는 S 키는 이번 기능에서는 사용하지 않거나, 아직 아래쪽 조준 제한 기능이 없으니 조심해서 설명해줘

조건:
- Unity 6 2D 프로젝트 기준으로 설명해줘.
- 이번에는 기능 13번 확장만 만들어줘.
- 기능 14번 모바일 터치 조준은 만들지 마. 패스할 거야.
- 기능 15번 아래쪽 조준 제한은 아직 만들지 마.
- 조준선은 만들지 마.
- 버블 발사는 만들지 마.
- 기존 마우스 조준은 유지해줘.
- 키보드 조준을 켜고 끌 수 있게 해줘.
- 마우스 조준과 키보드 조준 중 어떤 것을 사용할지 Inspector에서 선택할 수 있게 해줘.
- 예: Aim Input Mode를 Mouse, Keyboard, MouseAndKeyboard 중 선택하게 하거나, useMouseAim/useKeyboardAim bool로 나눠도 돼.
- 초보자에게 쉬운 방식으로 만들어줘.
- ShooterRoot 위치는 자동으로 바꾸지 마.
- 회전 대상은 Inspector의 rotationTarget을 그대로 사용해줘.
- 기존 배경, 타이머, 게이지, 점수 코드는 건드리지 마.
- 기존 ShooterController.cs도 가능하면 건드리지 마.
- ShooterAimController.cs를 수정하는 방식으로 해줘.
- 키보드 입력은 현재 Active Input Handling이 Both라서 UnityEngine.Input 방식으로 해도 돼.
- 단, 나중에 새 Input System만 사용할 수도 있으니 가능하면 Both/새 Input System에서도 안전하게 동작하는 방식이면 좋아.
- 너무 복잡하면 UnityEngine.Input 방식으로 하고, Player Settings가 Both여야 한다고 설명해줘.
- 전체 코드를 빠짐없이 작성해줘.
- 코드마다 주석을 초보자도 알아보기 쉽게 달아줘.
- 어느 GameObject에 스크립트를 붙이는지 알려줘.
- Inspector에서 뭘 설정해야 하는지 알려줘.
- 자주 틀리는 부분도 알려줘.

키보드 조준 방식:
- 키를 누르면 각도가 조금씩 변하는 방식으로 해줘.
- 예: 현재 각도에서 왼쪽 키를 누르면 +rotationSpeed 만큼 회전, 오른쪽 키를 누르면 -rotationSpeed 만큼 회전.
- rotationSpeed는 Inspector에서 조절할 수 있게 해줘.
- 기본값은 120 정도로 해줘.
- 키보드 조준용 현재 각도를 저장하는 currentAimAngle 같은 변수를 사용해도 돼.
- 마우스 조준을 사용하다가 키보드를 누르면 키보드 방향으로 자연스럽게 이어지게 해줘.
- 아직 아래쪽 조준 제한은 만들지 말아줘. 그건 기능 15번에서 만들 거야.

코드 작성 조건:
- 기존 ShooterAimController.cs를 수정해줘.
- targetCamera, rotationTarget, angleOffset, rotateSpeed는 유지해줘.
- keyboardRotationSpeed 같은 변수를 추가해줘.
- useMouseAim, useKeyboardAim 같은 bool 변수를 추가해줘.
- 또는 enum AimInputMode { Mouse, Keyboard, MouseAndKeyboard }를 사용해도 되지만, 초보자가 이해하기 쉬운 방식으로 해줘.
- 방향키와 WASD 입력을 모두 받을 수 있게 해줘.
- 키보드 입력은 왼쪽/오른쪽 회전 중심으로 단순하게 만들어줘.
- W/S는 지금 당장 복잡하게 만들지 말고, W는 위쪽으로 보정하거나 설명만 해줘도 돼.
- Input.GetKey를 사용할 경우 Player Settings가 Both 또는 Old Input Manager여야 한다는 설명을 해줘.
- 새 Input System을 함께 대응한다면 #if ENABLE_INPUT_SYSTEM을 사용해줘.
- [Header("한글 설명")]을 사용해줘.
- [Tooltip("초보자용 설명")]도 한글로 추가해줘.
- 변수 이름은 Unity/C# 규칙 때문에 영어로 유지해줘.
- 코드 주석은 한글로 작성해줘.
- 어려운 문법보다 쉬운 문법으로 작성해줘.
- 이 함수는 언제 호출되는지 설명해줘.
- 이 변수가 어떤 역할인지 설명해줘.
- 코드 실행 흐름을 순서대로 알려줘.
- 초보자가 실수하기 쉬운 부분도 알려줘.

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

주의:
- 이번에는 기능 13번 키보드 조준 추가만 만들어줘.
- 기능 14번 모바일 터치 조준은 만들지 마.
- 기능 15번 아래쪽 조준 제한은 만들지 마.
- ShooterRoot 위치를 자동으로 바꾸지 마.
- 기존 배경, 타이머, 게이지, 점수 코드는 건드리지 마.
- EventSystem은 다시 만들지 마.
```

## 진행 메모

- 기능 14번 모바일 터치 조준은 사용자가 패스한다고 했습니다.
- 기능 13번 키보드 조준 추가가 끝나면 다음 프롬프트는 기능 15번으로 진행하면 됩니다.
