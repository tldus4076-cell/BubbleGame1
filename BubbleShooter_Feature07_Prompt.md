# 기능 7 프롬프트: 타이머 게이지 넣기

아래 프롬프트를 그대로 복사해서 사용하세요.

```text
나는 초보자이고, Unity 6으로 2D 버블슈터 게임을 만들고 있어.

이번에는 기능 목록 7번인 "타이머 게이지 넣기"만 만들고 싶어.

참고 문서 위치:
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_Planning.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_StagePlan.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_FeatureList.md

작업 폴더:
- C:\Users\admin\Documents\BubbleGame

Unity 프로젝트 폴더:
- C:\Users\admin\Documents\BubbleGame\BubbleGame

앞으로 만드는 코드와 프롬프트는 가능하면 C:\Users\admin\Documents\BubbleGame 폴더 안에 저장해줘.

현재 상태:
- 기능 1번~4번 배경 기능은 완료했어.
- 기능 5번 타이머 숫자 기능도 완료했어.
- 기능 6번 타이머 숫자 위치 배치도 완료했어.
- TimerController.cs가 있고 타이머가 4:00, 3:59처럼 분:초 형식으로 줄어들어.
- TimerText는 내가 직접 Scene 창에서 원하는 위치로 옮겨서 사용하고 있어.
- 직접 배치한 TimerText 위치는 절대 다시 중앙으로 고정하지 말아줘.
- 기존 타이머 숫자 감소 기능과 배경 기능은 절대 망가뜨리지 말아줘.

목표:
남은 시간을 숫자뿐만 아니라 게이지로도 보여주고 싶어.
시간이 줄어들수록 게이지가 점점 짧아지면 돼.

조건:
- Unity 6 2D 프로젝트 기준으로 설명해줘.
- 이번에는 기능 7번 "타이머 게이지 넣기"만 만들어줘.
- 기능 8번 "배경 색과 겹치지 않게 게이지 색 정하기"는 아직 만들지 마. 색 조정은 다음 기능에서 할 거야.
- 아직 실패 처리는 만들지 마. 시간이 0이 되었을 때 실패하는 기능은 기능 64번에서 만들 거야.
- 기존 TimerController.cs의 타이머 숫자 감소 기능은 유지해줘.
- TimerText 위치를 자동으로 바꾸지 마.
- 기존 StageBackgroundController.cs는 건드리지 마.
- 게이지는 Canvas 안에 UI Image로 만들어줘.
- 게이지는 배경보다 앞에 보여야 해.
- 게이지는 타이머 숫자 근처 또는 화면 위쪽에 보이게 해줘.
- 게이지는 시간이 100% 남았을 때 가득 차 있어야 해.
- 시간이 절반 남으면 게이지 길이도 절반 정도로 줄어야 해.
- 시간이 0이 되면 게이지가 비어 있어야 해.
- 게이지는 왼쪽에서 오른쪽 또는 오른쪽에서 왼쪽 중 쉬운 방식으로 줄어들게 해줘.
- 초보자에게 쉬운 방식이면 Image Type을 Filled로 쓰거나 RectTransform width를 줄이는 방식 중 하나를 선택해줘.
- 추천은 UI Image의 fillAmount를 사용하는 방식이야.
- 초보자도 이해할 수 있게 쉬운 말로 설명해줘.
- 어려운 영어 용어가 나오면 한국말 뜻도 같이 설명해줘.
- 전체 코드를 빠짐없이 작성해줘.
- 코드마다 주석을 초보자도 알아보기 쉽게 달아줘.
- 어느 GameObject(게임 오브젝트)에 스크립트를 붙이는지 알려줘.
- Inspector(인스펙터)에서 뭘 설정해야 하는지 알려줘.
- 자주 틀리는 부분도 알려줘.

UI 조건:
- Canvas 안에 TimerGaugeBackground와 TimerGaugeFill을 만들어줘.
- TimerGaugeBackground는 게이지의 뒤쪽 배경 막대야.
- TimerGaugeFill은 실제로 줄어드는 앞쪽 막대야.
- TimerGaugeFill에는 Image 컴포넌트를 사용해줘.
- TimerGaugeFill의 Image Type을 Filled로 설정하는 방법을 알려줘.
- Fill Method는 Horizontal(가로)로 해줘.
- Fill Origin은 Left(왼쪽)로 해줘.
- 시간이 줄어들 때 fillAmount가 1에서 0으로 줄어들게 해줘.
- 게이지 위치는 Inspector에서 직접 조절할 수 있게 해줘.
- 자동 세팅 메뉴가 있다면 기본 위치만 잡아주고, 이후 내가 직접 옮길 수 있게 해줘.

코드 작성 조건:
- TimerGaugeController.cs 같은 새 스크립트를 만들어줘.
- 기존 TimerController.cs를 수정해야 한다면 최소한으로만 수정해줘.
- TimerGaugeController가 TimerController의 현재 시간과 시작 시간을 읽어서 게이지를 갱신하게 해줘.
- TimerController에 currentTime과 startTime을 읽을 수 있는 public getter 함수를 추가해도 돼.
- 예: GetCurrentTime(), GetStartTime()
- TimerController의 타이머 감소 로직은 바꾸지 마.
- TimerGaugeController에는 timerController, gaugeFillImage 변수를 만들어줘.
- gaugeFillImage.fillAmount를 사용해서 게이지를 줄여줘.
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
- 가능하면 Project Settings(프로젝트 설정)나 Scene(씬) 저장이 필요한 부분을 체크리스트에 넣어줘.

이번 기능에서 원하는 쉬운 구현 방향:
- TimerController.cs에 GetCurrentTime(), GetStartTime() 함수만 추가해줘.
- TimerGaugeController.cs를 새로 만들어줘.
- TimerGaugeController.cs는 TimerGaugeFill Image의 fillAmount를 매 프레임 갱신하게 해줘.
- Canvas 아래에 TimerGaugeBackground와 TimerGaugeFill을 만들게 해줘.
- TimerGaugeFill은 TimerGaugeBackground의 자식으로 넣는 방식을 추천해줘.
- 자동 세팅 스크립트가 있다면 Bubble Shooter > Setup Timer Gauge 메뉴를 만들어줘.
- 자동 세팅 메뉴를 누르면 TimerGaugeBackground, TimerGaugeFill, TimerGaugeController가 만들어지게 해줘.
- 단, 내가 나중에 게이지 위치를 직접 옮길 수 있어야 해.
- Play를 누르면 시간에 맞춰 게이지가 점점 줄어야 해.

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
- 이번에는 기능 7번만 만들어줘.
- 게이지 색을 배경별로 최적화하는 기능은 만들지 마. 그건 기능 8번에서 할 거야.
- 시간이 0이 되었을 때 실패 처리는 만들지 마.
- 점수 UI는 만들지 마.
- 기존 배경 관련 코드는 건드리지 마.
- TimerText 위치를 자동으로 변경하지 마.
```

## 사용 방법

1. 위 `text` 박스 안의 내용을 전부 복사합니다.
2. 다음 작업 요청으로 그대로 붙여넣습니다.
3. 답변을 받은 뒤 Unity 6 프로젝트에 적용합니다.
4. 기능 7번이 성공하면 다음에는 기능 8번 프롬프트를 만들면 됩니다.
