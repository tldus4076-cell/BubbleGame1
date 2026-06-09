# 기능 34 프롬프트: 같은 색 3개 이상이면 버블 제거하기

아래 프롬프트를 그대로 복사해서 사용하세요.

```text
나는 초보자이고, Unity 6으로 2D 버블슈터 게임을 만들고 있어.

이번에는 기능 목록 34번인 "버블 제거"만 만들고 싶어.

참고 문서 위치:
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_Planning.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_StagePlan.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_FeatureList.md

작업 폴더:
- C:\Users\admin\Documents\BubbleGame

Unity 프로젝트 폴더:
- C:\Users\admin\Documents\BubbleGame\BubbleGame

현재 상태:
- 기능 1번~33번은 완료했어. (22번 모바일 터치는 패스)
- 기능 33번 "같은 색이 2개 이하면 제거하지 않기" 테스트는 완료했어.
- 현재 발사 시스템은 Grid 기반으로 동작해:
  - ShooterController.cs가 발사를 담당해.
  - BubbleGridManager.cs가 격자 칸(BubbleSlot) 관리, 버블 등록, 같은 색 찾기, 매칭 규칙 확인을 담당해.
  - BubbleProjectile.cs가 target cell까지 이동한 뒤 gridManager.RegisterBubble(targetSlot, gameObject)를 호출해.
  - RegisterBubble() 안에서 CheckMatchRule(slot)이 호출돼.
  - 기능 33에서 연결된 같은 색 개수를 세고, 3개 이상이면 "나중에 제거 대상", 2개 이하면 "제거하지 않음" 로그가 출력돼.
- BubbleLauncherController.cs는 구식 물리 발사 스크립트였고 현재는 삭제됐어. 새로 만들지 마.
- BubbleSwapController.cs도 테스트용 구식 스크립트였고 현재는 삭제됐어. 새로 만들지 마.
- BubbleCurrentController.cs는 이미 제거됐어. 새로 만들지 마.
- 기존 배경, 타이머, 게이지, 점수, 슈터, 조준선, 현재 버블, 다음 버블, 발사, 격자, 같은 색 찾기 기능은 절대 망가뜨리지 말아줘.
- EventSystem은 삭제된 상태야. 이번 기능에는 필요 없으면 만들지 마.

목표:
같은 색으로 연결된 버블이 3개 이상이면 그 버블들을 실제로 제거하고 싶어.
같은 색으로 연결된 버블이 1개 또는 2개뿐이면 그대로 남아 있어야 해.

조건:
- Unity 6 2D 프로젝트 기준으로 설명해줘.
- 이번에는 기능 34번 "같은 색 3개 이상이면 버블 제거"만 만들어줘.
- 점수 증가는 아직 만들지 마. 점수는 기능 35번 또는 37번에서 할 거야.
- 제거 효과도 아직 만들지 마. 제거 효과는 기능 36번에서 할 거야.
- 연결 끊긴 버블 찾기/떨어뜨리기는 아직 만들지 마. 그건 기능 38번~40번에서 할 거야.
- BubbleGridManager.cs의 CheckMatchRule() 또는 같은 역할을 하는 함수를 수정해줘.
- 연결 개수가 3개 이상이면 connectedSameColorSlots에 들어 있는 버블 GameObject를 제거해줘.
- 연결 개수가 1개 또는 2개이면 아무것도 제거하지 말고 "제거하지 않음" 로그만 출력해줘.
- 제거할 때 BubbleSlot의 occupied는 false로 바꾸고 bubbleObject는 null로 비워줘.
- 제거할 때는 매칭된 버블만 제거해줘. 다른 색 버블은 절대 제거하지 마.
- 기존 ShooterRoot 위치는 자동으로 바꾸지 마.
- 기존 벽 오브젝트 위치도 자동으로 바꾸지 마.
- 자동 세팅 메뉴는 만들지 마.
- 초보자도 이해할 수 있게 쉬운 말로 설명해줘.
- 어려운 영어 용어가 나오면 한국말 뜻도 같이 설명해줘.

코드 작성 조건:
- 가능하면 BubbleGridManager.cs만 수정해줘.
- 새 스크립트가 꼭 필요하지 않으면 만들지 마.
- CheckMatchRule()을 초보자가 이해하기 쉽게 함수로 나눠줘.
  예:
  - CheckMatchRule(BubbleSlot startSlot)
  - ShouldRemoveMatchedBubbles(int connectedCount)
  - RemoveMatchedBubbles()
  - ClearBubbleSlot(BubbleSlot slot)
- 같은 색 연결 목록이 3개 이상인지 2개 이하인지 명확하게 나눠줘.
- 2개 이하면 반드시 아무 동작도 하지 않고 로그만 출력해줘.
- 실제 Destroy()는 3개 이상일 때만 사용해줘.
- BubbleGridManager.cs의 IsSameBubbleColor(), GetBubbleColorName(), AddSameColorNeighborSlots() 함수는 유지해줘.
- 기능 33에서 고친 "Renderer Color를 먼저 비교하는 색 판별 방식"은 유지해줘.
- [Header("한글 설명")]을 사용할 수 있는 새 Inspector 변수에는 한글 설명을 넣어줘.
- [Tooltip("초보자용 설명")]도 한글로 추가해줘.
- 변수 이름은 Unity/C# 규칙 때문에 영어로 유지해줘.
- 코드 주석은 한글로 많이 작성해줘.
- 어려운 문법보다 쉬운 문법으로 작성해줘.
- 기존 기능을 크게 갈아엎지 말고, 필요한 부분만 최소 수정해줘.
- 기존 배경, 타이머, 게이지, 점수, 슈터 조준, 조준선, 현재/다음 버블 기능 파일은 가능하면 건드리지 마.
- GameObject.Find() 사용 금지.
- 하드코딩된 Tag 사용 금지.
- 외부 객체 참조는 반드시 [SerializeField] 또는 Interface 사용.

C# 이벤트(event) 조건:
- 버블이 제거되면 나중에 점수 기능이 구독할 수 있도록 event를 만들어줘.
- 예: public event System.Action<int> MatchedBubblesRemoved;
- 3개 이상 제거가 끝난 뒤 제거한 개수를 event로 Invoke해줘.
- 이번 기능에서는 점수 처리를 직접 하지 마.
- ScoreController나 BubbleScoreManager를 직접 호출하지 마.
- 이벤트만 발생시키고, 실제 점수 증가는 다음 기능에서 처리할 거야.

중요한 테스트 상황:
- 같은 색 1개만 있으면 Console에 "제거하지 않음"이 출력되고 버블이 남아 있는지 확인.
- 같은 색 2개만 연결되면 Console에 "제거하지 않음"이 출력되고 버블 2개가 남아 있는지 확인.
- 같은 색 3개 이상 연결되면 그 버블들이 실제로 사라지는지 확인.
- 같은 색 3개 이상 제거 시 Console에 "버블 제거" 또는 "제거 완료" 로그가 출력되는지 확인.
- 3개 제거 시 다른 색 버블은 그대로 남아 있어야 해.
- 제거 후 해당 BubbleSlot이 비워져서 다음 발사 버블이 그 칸에 다시 들어갈 수 있어야 해.
- 점수는 이번 기능에서 올라가면 안 돼.
- 제거 효과는 이번 기능에서 나오면 안 돼.
- 기존 발사/정렬/같은 색 찾기/2개 이하 유지 기능이 정상 작동하는지 확인.

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
- 왜 3개 이상일 때만 제거하는지 쉽게 설명해줘.
- 왜 2개 이하는 제거하지 않는지 쉽게 설명해줘.
- 함수별 설명을 해줘.
- 이 코드가 왜 이렇게 동작하는지 초보자도 이해하게 설명해줘.
- 실행 순서를 번호로 설명해줘.
- 변수, 함수, if문이 각각 무슨 역할인지 알려줘.
- 마지막에 내가 외워야 할 핵심만 짧게 뽑아줘.

주의:
- 이번에는 기능 34번만 만들어줘.
- 점수 증가는 만들지 마.
- 제거 효과는 만들지 마.
- 연결 끊긴 버블 떨어뜨리기는 만들지 마.
- 기존 발사 기능과 정렬 기능을 망가뜨리지 마.
- ShooterRoot, WallsRoot, LeftWall, RightWall, Ceiling 위치를 자동으로 바꾸지 마.
- 자동 세팅 메뉴는 만들지 마.
- EventSystem은 다시 만들지 마.
- BubbleLauncherController.cs, BubbleSwapController.cs, BubbleCurrentController.cs는 새로 만들지 마.
```

## 사용 방법

1. 위 `text` 박스 안의 내용을 전부 복사합니다.
2. 다음 작업 요청으로 그대로 붙여넣습니다.
3. 기능 34번이 성공하면 다음에는 기능 35번 프롬프트를 만들면 됩니다.
