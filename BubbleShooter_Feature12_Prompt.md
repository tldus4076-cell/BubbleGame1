# 기능 12 프롬프트: 화면 아래 중앙에 슈터 만들기

아래 프롬프트를 그대로 복사해서 사용하세요.

```text
나는 초보자이고, Unity 6으로 2D 버블슈터 게임을 만들고 있어.

이번에는 기능 목록 12번인 "화면 아래 중앙에 슈터 놓기"만 만들고 싶어.

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
- 기존 배경, 타이머, 게이지, 점수 기능은 절대 망가뜨리지 말아줘.
- EventSystem은 삭제된 상태야. 이번 기능에는 필요 없으면 만들지 마.

목표:
게임 화면 아래 중앙에 슈터를 놓고 싶어.
게임 시작 시 슈터가 아래쪽에 보이면 성공이야.

조건:
- Unity 6 2D 프로젝트 기준으로 설명해줘.
- 이번에는 기능 12번 "슈터 만들기"만 만들어줘.
- 아직 슈터 조준 기능은 만들지 마. 그건 기능 13번에서 만들 거야.
- 아직 터치 조준 기능은 만들지 마. 그건 기능 14번에서 만들 거야.
- 아직 아래쪽 조준 제한 기능은 만들지 마. 그건 기능 15번에서 만들 거야.
- 아직 버블 발사 기능은 만들지 마. 그건 기능 21번에서 만들 거야.
- 이번에는 슈터가 화면 아래 중앙에 보이게 배치하는 것만 만들면 돼.
- 슈터는 배경보다 앞에 보여야 해.
- 슈터는 UI보다 뒤에 있어도 괜찮아. UI는 Canvas라서 보통 앞에 보여.
- 슈터는 나중에 회전할 수 있도록 별도 GameObject로 만들어줘.
- 슈터 이미지는 SpriteRenderer로 보여줘.
- 내가 슈터 Sprite를 준비하지 않았을 수도 있으니, 없으면 임시 Sprite나 기본 모양으로 테스트할 수 있게 해줘.
- 나중에 내가 준비한 Shooter Sprite로 쉽게 바꿀 수 있게 Inspector에서 연결하게 해줘.
- 화면 해상도가 바뀌어도 아래 중앙 근처에 놓일 수 있게 카메라 기준 위치 계산을 해줘.
- 너무 복잡하면 처음에는 Inspector에서 위치를 조절할 수 있게 하고, 자동 배치 함수도 같이 제공해줘.
- 초보자도 이해할 수 있게 쉬운 말로 설명해줘.
- 어려운 영어 용어가 나오면 한국말 뜻도 같이 설명해줘.
- 전체 코드를 빠짐없이 작성해줘.
- 코드마다 주석을 초보자도 알아보기 쉽게 달아줘.
- 어느 GameObject(게임 오브젝트)에 스크립트를 붙이는지 알려줘.
- Inspector(인스펙터)에서 뭘 설정해야 하는지 알려줘.
- 자주 틀리는 부분도 알려줘.

오브젝트 조건:
- ShooterRoot라는 빈 GameObject를 만들어줘.
- ShooterRoot는 화면 아래 중앙 위치를 담당하게 해줘.
- ShooterRoot 아래에 ShooterVisual이라는 자식 GameObject를 만들어줘.
- ShooterVisual에는 SpriteRenderer를 붙여줘.
- 나중에 회전 기능을 만들 때 ShooterRoot 또는 ShooterVisual을 회전할 수 있게 구조를 단순하게 유지해줘.
- Sorting Order를 배경보다 크게 설정해서 배경 앞에 보이게 해줘.
- 배경은 현재 Sorting Order가 -100일 수 있으니, 슈터는 10 정도를 추천해줘.

코드 작성 조건:
- ShooterController.cs 같은 새 스크립트를 만들어줘.
- ShooterController.cs는 ShooterRoot에 붙이는 방식으로 해줘.
- shooterSprite, shooterRenderer, targetCamera, bottomOffset, sortingOrder 같은 변수를 만들어줘.
- shooterSprite는 Inspector에서 연결할 수 있게 해줘.
- shooterSprite가 없으면 임시 모양을 만들거나 경고를 보여줘.
- 화면 아래 중앙 배치를 위한 PositionShooterAtBottomCenter() 함수를 만들어줘.
- 카메라가 Orthographic인지 확인해서 2D 화면 아래 위치를 계산해줘.
- OnValidate나 ContextMenu를 사용해서 Editor에서 위치를 다시 맞출 수 있게 해줘.
- 단, 내가 나중에 위치를 직접 옮기고 싶을 수 있으니 autoPositionOnStart 같은 bool 변수로 자동 배치를 켜고 끌 수 있게 해줘.
- 기본값은 autoPositionOnStart = true로 해줘.
- [Header("한글 설명")]을 사용해줘.
- 필요하면 [Tooltip("초보자용 설명")]도 한글로 추가해줘.
- 변수 이름은 Unity/C# 규칙 때문에 영어로 유지해줘.
- 인스펙터에 보이는 설명은 초보자가 이해하기 쉽게 한글로 작성해줘.
- 코드 주석도 계속 한글로 작성해줘.
- 어려운 문법보다 쉬운 문법으로 작성해줘.
- 이 코드를 함수별로 나눠줘.
- 이 코드가 왜 필요한지 설명해줘.
- 이 함수는 언제 호출되는지 설명해줘.
- 이 변수가 어떤 역할인지 설명해줘.
- 이 코드가 실행되는 흐름을 순서대로 알려줘.
- 초보자가 실수하기 쉬운 부분도 알려줘.
- 실행 순서를 번호로 설명해줘.
- 이 코드에서 변수, 함수, if문이 각각 무슨 역할인지 알려줘.
- 내가 외워야 할 핵심만 뽑아줘.

자동저장 조건:
- 기능 하나가 완성되면 Unity에서 Scene(씬)을 저장하라고 알려줘.
- Ctrl + S로 저장하는 방법도 알려줘.
- 가능하면 Scene 저장이 필요한 부분을 체크리스트에 넣어줘.

이번 기능에서 원하는 쉬운 구현 방향:
- ShooterController.cs를 새로 만들어줘.
- ShooterRoot 오브젝트에 ShooterController.cs를 붙여줘.
- ShooterVisual 자식 오브젝트를 만들고 SpriteRenderer를 붙여줘.
- 자동 세팅 메뉴가 필요하다면 Bubble Shooter > Setup Shooter 메뉴를 만들어줘.
- 자동 세팅 메뉴를 누르면 ShooterRoot, ShooterVisual, ShooterController가 만들어지게 해줘.
- shooterSprite가 없으면 일단 기본 흰색 사각형 Sprite나 임시 Sprite로 보이게 해줘.
- EventSystem은 만들지 마.

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
- 이번에는 기능 12번만 만들어줘.
- 조준, 회전, 터치, 발사 기능은 만들지 마.
- 기존 배경, 타이머, 게이지, 점수 코드는 건드리지 마.
- EventSystem은 다시 만들지 마.
```

## 사용 방법

1. 위 `text` 박스 안의 내용을 전부 복사합니다.
2. 다음 작업 요청으로 그대로 붙여넣습니다.
3. 기능 12번이 성공하면 다음에는 기능 13번 프롬프트를 만들면 됩니다.
