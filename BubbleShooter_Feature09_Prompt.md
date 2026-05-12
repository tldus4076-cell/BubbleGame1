# 기능 9 프롬프트: 점수 숫자 넣기

아래 프롬프트를 그대로 복사해서 사용하세요.

```text
나는 초보자이고, Unity 6으로 2D 버블슈터 게임을 만들고 있어.

이번에는 기능 목록 9번인 "점수 숫자 넣기"만 만들고 싶어.

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
- 기능 5번 타이머 숫자 기능은 완료했어.
- 기능 6번 타이머 숫자 위치 배치는 완료했어.
- 기능 7번 타이머 게이지 기능은 완료했어.
- 기능 8번 타이머 게이지 색 조정 기능은 완료했어.
- TimerController.cs가 있고 타이머가 4:00, 3:59처럼 분:초 형식으로 줄어들어.
- TimerGaugeController.cs가 있고 게이지가 시간에 맞춰 줄어들어.
- TimerGaugeStyleController.cs가 있고 게이지 색/외곽선/그림자를 조절할 수 있어.
- EventSystem은 이미 삭제했어. 이번 점수 표시 기능에는 필요 없지만, 나중에 버튼 기능에서 필요하면 다시 만들어줘.
- 기존 배경, 타이머, 게이지 기능은 절대 망가뜨리지 말아줘.

목표:
게임 화면에 현재 점수를 숫자로 표시하고 싶어.
처음 게임 시작 시 점수가 0으로 보이면 돼.

조건:
- Unity 6 2D 프로젝트 기준으로 설명해줘.
- 이번에는 기능 9번 "점수 숫자 넣기"만 만들어줘.
- 아직 버블 제거 시 점수가 올라가는 기능은 만들지 마. 그건 기능 10번에서 만들 거야.
- 이번에는 처음 점수 0을 화면에 보여주는 것만 만들면 돼.
- 점수는 Canvas 안의 UI Text로 보여줘.
- TextMeshPro 오류가 있었으므로 Unity 기본 UI Text를 사용해줘.
- 점수 숫자는 배경보다 앞에 보여야 해.
- 점수 위치는 화면 위쪽 또는 내가 직접 배치할 수 있는 위치에 만들어줘.
- 내가 Scene 창에서 직접 위치를 옮길 수 있어야 해.
- Play를 눌러도 내가 직접 옮긴 ScoreText 위치가 자동으로 중앙이나 다른 곳으로 돌아가면 안 돼.
- 기존 TimerText 위치를 건드리지 마.
- 기존 TimerGaugeFill 위치와 크기를 건드리지 마.
- 기존 StageBackgroundController.cs는 건드리지 마.
- 기존 TimerController.cs는 건드리지 마.
- 기존 TimerGaugeController.cs는 건드리지 마.
- 기존 TimerGaugeStyleController.cs는 건드리지 마.
- 초보자도 이해할 수 있게 쉬운 말로 설명해줘.
- 어려운 영어 용어가 나오면 한국말 뜻도 같이 설명해줘.
- 전체 코드를 빠짐없이 작성해줘.
- 코드마다 주석을 초보자도 알아보기 쉽게 달아줘.
- 어느 GameObject(게임 오브젝트)에 스크립트를 붙이는지 알려줘.
- Inspector(인스펙터)에서 뭘 설정해야 하는지 알려줘.
- 자주 틀리는 부분도 알려줘.

UI 조건:
- GameCanvas 안에 ScoreText를 만들어줘.
- ScoreText에는 처음에 0이 보이게 해줘.
- 원하면 "SCORE 0" 또는 "0" 중 쉬운 방식을 선택해줘.
- 이번에는 배경 이미지에 이미 SCORE 칸이 있을 수 있으니 숫자만 "0"으로 표시하는 방식을 추천해줘.
- ScoreText 글자 색과 크기는 Inspector에서 조절할 수 있게 해줘.
- 밝은 배경에서도 보이도록 Shadow(그림자)를 사용할 수 있게 해줘.
- ScoreText 위치는 자동으로 계속 고정하지 말고, 처음 생성할 때만 기본 위치를 잡아줘.
- 이후에는 내가 직접 옮긴 위치가 유지되게 해줘.

코드 작성 조건:
- ScoreController.cs 같은 새 스크립트를 만들어줘.
- ScoreController.cs는 현재 점수 int currentScore를 가지고 있게 해줘.
- 처음 점수는 0으로 시작하게 해줘.
- ScoreText UI Text를 연결해서 화면에 점수를 보여주게 해줘.
- UpdateScoreText() 함수를 만들어줘.
- AddScore(int amount) 함수를 만들어줘. 이번 기능에서는 테스트용으로만 준비하고 실제 버블 제거와 연결하지는 마.
- ResetScore() 함수를 만들어줘.
- GetCurrentScore() 함수도 만들어줘.
- 점수가 음수가 되지 않게 해줘.
- ScoreText 위치를 자동으로 바꾸는 코드는 넣지 마.
- ScoreText 스타일을 위한 스크립트가 필요하다면 ScoreUIStyleController.cs 같은 새 스크립트로 분리해줘.
- 하지만 너무 복잡하면 ScoreController.cs 안에서 Text 색과 그림자 정도만 다뤄도 돼.
- 자동 세팅 메뉴가 있다면 Bubble Shooter > Setup Score Text 메뉴를 만들어줘.
- 자동 세팅 메뉴는 GameCanvas, ScoreText, ScoreController를 만들어주게 해줘.
- 자동 세팅 메뉴를 다시 눌러도 기존 ScoreText 위치와 크기는 함부로 덮어쓰지 마.
- EventSystem은 만들지 마. 이번 기능에는 필요 없어.
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
- ScoreController.cs를 새로 만들어줘.
- GameCanvas 아래에 ScoreText UI Text를 만들어줘.
- ScoreText는 기본 위치만 화면 위쪽 오른쪽 근처로 잡아줘.
- 하지만 자동 세팅 후에는 내가 직접 위치를 옮길 수 있어야 해.
- Play를 누르면 ScoreText에 0이 보여야 해.
- ScoreController의 scoreText 변수에 ScoreText를 연결해줘.
- ScoreText에 Shadow를 붙여 밝은 배경에서도 잘 보이게 해줘.
- Bubble Shooter > Setup Score Text 메뉴를 만들어 자동 세팅할 수 있게 해줘.
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
- 이번에는 기능 9번만 만들어줘.
- 버블 제거 시 점수 증가 기능은 만들지 마. 그건 기능 10번에서 만들 거야.
- 점수 UI 위치를 Play 때 자동으로 다시 고정하지 마.
- 기존 배경, 타이머, 게이지 코드는 건드리지 마.
- EventSystem은 다시 만들지 마.
```

## 사용 방법

1. 위 `text` 박스 안의 내용을 전부 복사합니다.
2. 다음 작업 요청으로 그대로 붙여넣습니다.
3. 답변을 받은 뒤 Unity 6 프로젝트에 적용합니다.
4. 기능 9번이 성공하면 다음에는 기능 10번 프롬프트를 만들면 됩니다.
