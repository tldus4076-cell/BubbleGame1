# 기능 40 프롬프트: 떨어진 버블에도 점수 주기

아래 프롬프트를 그대로 복사해서 사용하세요.

```text
나는 초보자이고, Unity 6으로 2D 버블슈터 게임을 만들고 있어.

이번에는 기능 목록 40번인 "떨어진 버블에도 점수 주기"만 만들고 싶어.

참고 문서 위치:
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_Planning.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_StagePlan.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_FeatureList.md

작업 폴더:
- C:\Users\admin\Documents\BubbleGame

Unity 프로젝트 폴더:
- C:\Users\admin\Documents\BubbleGame\BubbleGame

현재 상태:
- 기능 1번~39번은 완료했어. (22번 모바일 터치는 패스)
- 기능 39번 "연결 끊긴 버블 떨어뜨리기" 테스트는 완료했어.
- 현재 발사 시스템은 Grid 기반으로 동작해:
  - ShooterController.cs가 발사를 담당해.
  - BubbleGridManager.cs가 격자 칸(BubbleSlot) 관리, 버블 등록, 같은 색 찾기, 매칭 규칙 확인, 버블 제거, 제거 효과, 천장 연결 버블 찾기, 떠 있는 버블 찾기, 떠 있는 버블 떨어뜨리기를 담당해.
  - BubbleGridManager.cs에 public event System.Action<int> MatchedBubblesRemoved 이벤트가 이미 있어.
  - 이 이벤트는 같은 색 3개 이상 버블이 제거된 뒤, 제거된 버블 개수를 Invoke로 알려줘.
  - 기능 39에서 DropFloatingBubbles() 함수가 떠 있는 버블을 아래로 떨어뜨려.
  - DropBubbleObject() Coroutine이 버블을 아래로 이동시킨 뒤 Destroy() 해.
  - 현재 떨어진 버블에는 점수가 없어.
- BubbleScoreManager.cs가 이미 있어서, AddBubbleScore(int removedBubbleCount) 함수가 있어.
- BubbleScoreManager.cs는 scorePerBubble(버블 1개당 점수)과 bonusForFourBubbles, bonusForFiveOrMoreBubbles를 Inspector에서 조절할 수 있어.
- ScoreController.cs가 이미 있어서, AddScore(int amount) 함수가 있어.
- 아직 떨어진 버블에 점수를 주는 기능이 없어.
- 기존 배경, 타이머, 게이지, 점수, 슈터, 조준선, 현재 버블, 다음 버블, 발사, 격자, 같은 색 찾기, 버블 제거, 점수 증가, 제거 효과, 천장 연결 찾기, 떠 있는 버블 찾기, 떠 있는 버블 떨어뜨리기 기능은 절대 망가뜨리지 말아줘.
- EventSystem은 삭제된 상태야. 이번 기능에는 필요 없으면 만들지 마.

목표:
천장과 연결이 끊겨서 떨어진 버블에도 점수를 주고 싶어.
떨어진 버블 수만큼 추가 점수가 올라가게 하고 싶어.

조건:
- Unity 6 2D 프로젝트 기준으로 설명해줘.
- 이번에는 기능 40번 "떨어진 버블에도 점수 주기"만 만들어줘.
- 떨어진 버블 수만큼 BubbleScoreManager의 AddBubbleScore()를 호출해서 점수를 올려줘.
- BubbleGridManager.cs에 새 이벤트를 만들어줘. 예: public event System.Action<int> FloatingBubblesDropped;
- 떨어진 버블 개수를 이벤트로 Invoke 해줘.
- BubbleScoreManager.cs에서 이 이벤트를 구독해서 AddBubbleScore()를 호출해줘.
- ScoreController.cs는 수정하지 마.
- BubbleGridManager.cs의 DropFloatingBubbles()에서 떨어뜨린 버블 개수를 이벤트로 알려줘.
- 기능 35의 MatchedBubblesRemoved 이벤트는 건드리지 마.
- 기능 39의 떨어뜨리기 로직은 건드리지 마.
- 기존 ShooterRoot 위치는 자동으로 바꾸지 마.
- 기존 벽 오브젝트 위치도 자동으로 바꾸지 마.
- 자동 세팅 메뉴는 만들지 마.
- 초보자도 이해할 수 있게 쉬운 말로 설명해줘.
- 어려운 영어 용어가 나오면 한국말 뜻도 같이 설명해줘.

코드 작성 조건:
- BubbleGridManager.cs를 수정해줘.
- BubbleScoreManager.cs를 수정해줘.
- 새 스크립트가 꼭 필요하지 않으면 만들지 마.
- BubbleGridManager.cs에 새 이벤트를 만들어줘:
  - public event System.Action<int> FloatingBubblesDropped;
- DropFloatingBubbles()에서 떠 있는 버블을 떨어뜨린 뒤, FloatingBubblesDropped?.Invoke(floatingSlots.Count)를 호출해줘.
- BubbleScoreManager.cs에서 FloatingBubblesDropped 이벤트를 구독해줘.
- SubscribeToGridManagerEvent()에서 FloatingBubblesDropped도 같이 구독해줘.
- UnsubscribeFromGridManagerEvent()에서 FloatingBubblesDropped도 같이 해제해줘.
- OnBubblesDropped() 함수를 만들어서 AddBubbleScore(droppedCount)를 호출해줘.
- [Header("한글 설명")]을 사용할 수 있는 새 Inspector 변수에는 한글 설명을 넣어줘.
- [Tooltip("초보자용 설명")]도 한글로 추가해줘.
- 변수 이름은 Unity/C# 규칙 때문에 영어로 유지해줘.
- 코드 주석은 한글로 많이 작성해줘.
- 어려운 문법보다 쉬운 문법으로 작성해줘.
- 기존 기능을 크게 갈아엎지 말고, 필요한 부분만 최소 수정해줘.
- GameObject.Find() 사용 금지.
- 하드코딩된 Tag 사용 금지.
- 외부 객체 참조는 반드시 [SerializeField] 또는 Interface 사용.

C# 이벤트(event) 조건:
- BubbleGridManager.cs에 새 이벤트를 만들어줘.
- BubbleScoreManager.cs가 이 이벤트를 구독해서 점수를 올려줘.
- 직접 호출하지 마. 이벤트로만 연결해줘.

중요한 테스트 상황:
- 같은 색 3개 이상 제거 후 떠 있는 버블이 떨어지면 점수가 올라가는지 확인.
- 떨어진 버블 1개당 점수가 올라가는지 확인. (예: 버블 1개당 10점이면 1개 떨어지면 +10점)
- 떨어진 버블이 0개이면 점수가 안 올라가는지 확인.
- 같은 색 3개 이상 제거 시 제거 점수 + 떨어진 버블 점수가 합산되는지 확인.
- 천장과 연결된 버블은 떨어지지 않고 점수도 안 올라가는지 확인.
- 기존 발사/정렬/같은 색 찾기/버블 제거/점수 증가/제거 효과/천장 연결 찾기/떠 있는 버블 찾기/떠 있는 버블 떨어뜨리기 기능이 정상 작동하는지 확인.

자동저장 조건:
- 기능 하나가 완성되면 Unity에서 Scene(씬)을 저장하라고 알려줘.
- Ctrl + S로 저장하는 방법도 알려줘.
- Unity 상단 메뉴에 자동 저장 메뉴를 새로 만들지는 마.

반드시 아래 형식으로 답해줘:

1. 기능 설명
2. 수정한 파일
3. 전체 코드 또는 수정된 주요 코드
4. 코드 설명
5. 유니티 적용 방법
6. 오류 체크 포인트
7. Inspector에서 조절할 변수
8. 테스트 성공 기준
9. 다음 기능으로 넘어가기 전 체크리스트

설명 스타일:
- 초등학생도 이해할 수 있게 차근차근 설명해줘.
- 코드만 알려주지 말고 코드 설명도 해줘.
- 왜 떨어진 버블에도 점수를 줘야 하는지 알려줘.
- 이벤트로 연결하는 이유를 쉽게 설명해줘.
- OnDestroy에서 이벤트를 해지하는 이유를 쉽게 설명해줘.
- 함수별 설명을 해줘.
- 이 코드가 왜 이렇게 동작하는지 초보자도 이해하게 설명해줘.
- 실행 순서를 번호로 설명해줘.
- 변수, 함수, if문이 각각 무슨 역할인지 알려줘.
- 마지막에 내가 외워야 할 핵심만 짧게 뽑아줘.

주의:
- 이번에는 기능 40번만 만들어줘.
- 기존 점수 기능(기능 35)을 건드리지 마.
- 기존 떨어뜨리기 기능(기능 39)을 건드리지 마.
- ScoreController.cs는 수정하지 마.
- 기존 발사 기능과 정렬 기능을 망가뜨리지 마.
- ShooterRoot, WallsRoot, LeftWall, RightWall, Ceiling 위치를 자동으로 바꾸지 마.
- 자동 세팅 메뉴는 만들지 마.
- EventSystem은 다시 만들지 마.
- BubbleLauncherController.cs, BubbleSwapController.cs, BubbleCurrentController.cs는 새로 만들지 마.
```

## 사용 방법

1. 위 `text` 박스 안의 내용을 전부 복사합니다.
2. 다음 작업 요청으로 그대로 붙여넣습니다.
3. 기능 40번이 성공하면 다음에는 기능 41번 프롬프트를 만들면 됩니다.
