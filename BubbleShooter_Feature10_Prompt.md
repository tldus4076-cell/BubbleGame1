# 기능 10 프롬프트: 버블을 제거하면 점수 올라가게 하기

아래 프롬프트를 그대로 복사해서 사용하세요.

```text
나는 초보자이고, Unity 6으로 2D 버블슈터 게임을 만들고 있어.

이번에는 기능 목록 10번인 "버블을 제거하면 점수가 올라가게 하기"만 만들고 싶어.

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
- 기능 9번 점수 숫자 넣기는 완료했어.
- ScoreController.cs가 있고 현재 점수 0을 화면에 보여줘.
- ScoreController.cs에는 AddScore(int amount), ResetScore(), GetCurrentScore() 함수가 있어.
- 아직 실제 버블 제거 기능은 완성되지 않았을 수 있어.
- 기존 배경, 타이머, 게이지, 점수 표시 기능은 절대 망가뜨리지 말아줘.
- EventSystem은 삭제된 상태야. 이번 기능에는 필요 없으면 만들지 마.

목표:
버블이 제거될 때 점수가 올라가게 만들고 싶어.
아직 실제 버블 제거 기능이 없다면 테스트용으로 키보드나 Inspector 버튼 없이 간단히 점수 증가를 확인할 수 있는 방법도 알려줘.

조건:
- Unity 6 2D 프로젝트 기준으로 설명해줘.
- 이번에는 기능 10번 "버블 제거 시 점수 증가"만 만들어줘.
- 실제 같은 색 3개 찾기, 버블 제거 로직은 아직 만들지 마. 그건 기능 31번~35번에서 만들 예정이야.
- 이번 기능에서는 점수 증가 시스템만 준비해줘.
- ScoreController.cs의 AddScore(int amount)를 활용해줘.
- 버블 1개 제거당 점수를 몇 점 줄지 Inspector에서 조절할 수 있게 해줘.
- 기본값은 버블 1개당 10점으로 해줘.
- 여러 개 버블이 제거되면 제거된 개수만큼 점수가 올라가게 해줘.
- 예: 3개 제거, 1개당 10점이면 30점 증가.
- 점수는 0보다 작아지면 안 돼.
- ScoreText 위치를 자동으로 바꾸지 마.
- 기존 ScoreController.cs의 점수 표시 기능은 유지해줘.
- 기존 TimerController.cs, TimerGaugeController.cs, StageBackgroundController.cs는 건드리지 마.
- 초보자도 이해할 수 있게 쉬운 말로 설명해줘.
- 어려운 영어 용어가 나오면 한국말 뜻도 같이 설명해줘.
- 전체 코드를 빠짐없이 작성해줘.
- 코드마다 주석을 초보자도 알아보기 쉽게 달아줘.
- 어느 GameObject(게임 오브젝트)에 스크립트를 붙이는지 알려줘.
- Inspector(인스펙터)에서 뭘 설정해야 하는지 알려줘.
- 자주 틀리는 부분도 알려줘.

코드 작성 조건:
- BubbleScoreManager.cs 같은 새 스크립트를 만들어줘.
- BubbleScoreManager.cs는 ScoreController를 연결해서 점수를 올리게 해줘.
- 변수는 scoreController, scorePerBubble 같은 이름을 사용해줘.
- AddBubbleScore(int removedBubbleCount) 함수를 만들어줘.
- removedBubbleCount가 0 이하이면 점수를 올리지 않게 해줘.
- 점수 계산은 removedBubbleCount * scorePerBubble 방식으로 해줘.
- 아직 실제 버블 제거 기능이 없으니 테스트용 함수도 만들어줘.
- 예: AddTestScoreForOneBubble(), AddTestScoreForThreeBubbles() 같은 public 함수 또는 키보드 T 키 테스트.
- 테스트용 키보드 입력을 넣는다면 나중에 쉽게 끌 수 있도록 useKeyboardTest 변수를 만들어줘.
- 기본 테스트 키는 T로 해서 T를 누르면 3개 제거된 것처럼 점수가 올라가게 해줘.
- 실제 게임 기능과 헷갈리지 않도록 테스트용이라는 주석을 확실히 달아줘.
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
- BubbleScoreManager.cs를 새로 만들어줘.
- 빈 GameObject를 만들고 이름은 BubbleScoreManager로 해줘.
- BubbleScoreManager.cs를 BubbleScoreManager 오브젝트에 붙여줘.
- Inspector에서 ScoreController를 연결하게 해줘.
- scorePerBubble 기본값은 10으로 해줘.
- useKeyboardTest를 켜면 Play 중 T 키를 눌렀을 때 3개 버블 제거 테스트로 30점이 올라가게 해줘.
- 나중에 실제 버블 제거 기능이 만들어지면 AddBubbleScore(제거된버블개수)를 호출하면 되게 해줘.
- 자동 세팅 메뉴가 필요하다면 Bubble Shooter > Setup Bubble Score Manager 메뉴를 만들어줘.
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
- 이번에는 기능 10번만 만들어줘.
- 실제 버블 제거, 같은 색 찾기, 매칭 로직은 만들지 마.
- 점수 UI 위치를 Play 때 자동으로 다시 고정하지 마.
- 기존 배경, 타이머, 게이지 코드는 건드리지 마.
- EventSystem은 다시 만들지 마.
```

## 사용 방법

1. 위 `text` 박스 안의 내용을 전부 복사합니다.
2. 다음 작업 요청으로 그대로 붙여넣습니다.
3. 기능 10번이 성공하면 다음에는 기능 11번 프롬프트를 만들면 됩니다.
