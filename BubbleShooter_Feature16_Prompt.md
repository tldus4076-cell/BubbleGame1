# 기능 16 프롬프트: 슈터 앞에 조준선 표시하기

아래 프롬프트를 그대로 복사해서 사용하세요.

```text
나는 초보자이고, Unity 6으로 2D 버블슈터 게임을 만들고 있어.

이번에는 기능 목록 16번인 "슈터 앞에 조준선 표시하기"만 만들고 싶어.

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
- ShooterAimController.cs가 있고 슈터가 위쪽 범위 안에서만 회전해.
- ShooterRoot 위치는 내가 직접 Scene 창에서 조절하는 방식이야.
- 현재 추천 구조는 ShooterRoot는 위치 담당, ShooterVisual은 회전 담당이야.
- 기존 배경, 타이머, 게이지, 점수, 슈터 조준 기능은 절대 망가뜨리지 말아줘.
- EventSystem은 삭제된 상태야. 이번 기능에는 필요 없으면 만들지 마.

목표:
슈터 앞에 조준 방향을 보여주는 선을 표시하고 싶어.
마우스나 키보드로 슈터를 움직이면 조준선도 같은 방향으로 따라 움직이면 돼.

조건:
- Unity 6 2D 프로젝트 기준으로 설명해줘.
- 이번에는 기능 16번 "조준선 표시"만 만들어줘.
- 벽에 닿으면 튕길 방향까지 보여주는 기능은 아직 만들지 마. 그건 기능 17번에서 만들 거야.
- 버블 발사는 아직 만들지 마. 그건 기능 21번에서 만들 거야.
- 조준선은 슈터 앞에서 시작해서 앞으로 직선으로 보여야 해.
- 조준선은 배경보다 앞에 보여야 해.
- 조준선은 UI보다 뒤에 있어도 돼.
- 조준선은 마우스/키보드 조준 방향과 항상 같은 방향이어야 해.
- 기존 ShooterAimController.cs의 조준 기능은 유지해줘.
- 기존 아래쪽 조준 제한도 유지해줘.
- ShooterRoot 위치는 자동으로 바꾸지 마.
- 기존 StageBackgroundController.cs, TimerController.cs, TimerGaugeController.cs, ScoreController.cs 등은 건드리지 마.
- 초보자도 이해할 수 있게 쉬운 말로 설명해줘.
- 어려운 영어 용어가 나오면 한국말 뜻도 같이 설명해줘.
- 전체 코드를 빠짐없이 작성해줘.
- 코드마다 주석을 초보자도 알아보기 쉽게 달아줘.
- 어느 GameObject에 스크립트를 붙이는지 알려줘.
- Inspector에서 뭘 설정해야 하는지 알려줘.
- 자주 틀리는 부분도 알려줘.

조준선 표시 방식:
- LineRenderer를 사용하는 방식으로 만들어줘.
- LineRenderer는 선을 그리는 Unity 컴포넌트라는 설명을 해줘.
- ShooterAimLineController.cs 같은 새 스크립트를 만들어줘.
- ShooterAimLineController.cs는 ShooterRoot 또는 별도 AimLine 오브젝트에 붙일 수 있게 해줘.
- 추천은 ShooterRoot에 붙이는 방식이야.
- 조준선 시작 위치는 슈터 위치에서 살짝 앞쪽으로 떨어진 곳이면 좋아.
- lineStartOffset 변수를 만들어서 슈터 중심에서 얼마나 앞에서 시작할지 조절하게 해줘.
- lineLength 변수를 만들어서 조준선 길이를 조절하게 해줘.
- lineWidth 변수를 만들어서 조준선 두께를 조절하게 해줘.
- lineColor 변수를 만들어서 조준선 색을 조절하게 해줘.
- sortingOrder 변수를 만들어서 배경보다 앞에 보이게 해줘.

방향 계산 조건:
- 조준선 방향은 회전 대상의 방향을 기준으로 계산해줘.
- 현재 회전 대상이 ShooterVisual일 수 있으니 aimDirectionSource 같은 Transform 변수를 Inspector에서 연결할 수 있게 해줘.
- aimDirectionSource가 비어 있으면 이 스크립트가 붙은 Transform을 사용해줘.
- 슈터 이미지가 위쪽을 바라보는 기준이라면 aimLocalDirection 기본값은 Vector2.up으로 해줘.
- 만약 이미지가 오른쪽을 기준으로 되어 있으면 Inspector에서 Vector2.right로 바꿀 수 있게 설명해줘.
- 너무 복잡하면 aimLocalDirection을 Vector2.up 기본값으로 두고 설명으로 보정 방법을 알려줘.

코드 작성 조건:
- ShooterAimLineController.cs를 새로 만들어줘.
- 기존 ShooterAimController.cs는 가능하면 건드리지 마.
- LineRenderer를 자동으로 찾거나 없으면 만들어줘.
- startPoint와 endPoint를 계산해서 lineRenderer.SetPosition(0, startPoint), SetPosition(1, endPoint)로 선을 그려줘.
- Update()에서 매 프레임 조준선 위치를 갱신해줘.
- 조준선 켜기/끄기를 위한 showAimLine bool 변수를 만들어줘.
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

자동저장 조건:
- 기능 하나가 완성되면 Unity에서 Scene(씬)을 저장하라고 알려줘.
- Ctrl + S로 저장하는 방법도 알려줘.

이번 기능에서 원하는 쉬운 구현 방향:
- ShooterAimLineController.cs를 새로 만들어줘.
- ShooterRoot에 ShooterAimLineController.cs를 붙여줘.
- 자동 세팅 메뉴가 필요하다면 Bubble Shooter > Setup Shooter Aim Line 메뉴를 만들어줘.
- 자동 세팅 메뉴를 누르면 ShooterRoot에 ShooterAimLineController가 붙고 LineRenderer가 준비되게 해줘.
- 기존 ShooterRoot 위치는 절대 바꾸지 마.
- Play를 누르고 마우스/키보드로 조준하면 조준선도 같이 회전하게 해줘.
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
- 이번에는 기능 16번만 만들어줘.
- 벽 반사 조준선은 만들지 마.
- 버블 발사는 만들지 마.
- ShooterRoot 위치를 자동으로 바꾸지 마.
- 기존 배경, 타이머, 게이지, 점수 코드는 건드리지 마.
- EventSystem은 다시 만들지 마.
```

## 사용 방법

1. 위 `text` 박스 안의 내용을 전부 복사합니다.
2. 다음 작업 요청으로 그대로 붙여넣습니다.
3. 기능 16번이 성공하면 다음에는 기능 17번 프롬프트를 만들면 됩니다.
