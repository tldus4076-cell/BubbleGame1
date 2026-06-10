# 기능 39 프롬프트: 연결 끊긴 버블 아래로 떨어뜨리기

아래 프롬프트를 그대로 복사해서 사용하세요.

```text
나는 초보자이고, Unity 6으로 2D 버블슈터 게임을 만들고 있어.

이번에는 기능 목록 39번인 "연결 끊긴 버블 떨어뜨리기"만 만들고 싶어.

참고 문서 위치:
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_Planning.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_StagePlan.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_FeatureList.md

작업 폴더:
- C:\Users\admin\Documents\BubbleGame

Unity 프로젝트 폴더:
- C:\Users\admin\Documents\BubbleGame\BubbleGame

현재 상태:
- 기능 1번~38번은 완료했어. (22번 모바일 터치는 패스)
- 기능 38번 "천장과 연결되지 않은 버블 찾기" 테스트는 완료했어.
- 현재 발사 시스템은 Grid 기반으로 동작해:
  - ShooterController.cs가 발사를 담당해.
  - BubbleGridManager.cs가 격자 칸(BubbleSlot) 관리, 버블 등록, 같은 색 찾기, 매칭 규칙 확인, 버블 제거, 제거 효과, 천장 연결 버블 찾기, 떠 있는 버블 찾기를 담당해.
  - 기능 37에서 FindCeilingConnectedBubbles() 함수가 있어.
  - 기능 38에서 FindFloatingBubbles() 함수가 있어.
  - FindFloatingBubbles()는 천장과 연결되지 않은 BubbleSlot 리스트를 돌려줘.
- 이번 기능 39에서는 FindFloatingBubbles()가 찾은 버블들을 아래로 떨어뜨리고 싶어.
- 아직 떨어진 버블 점수는 주지 마. 떨어진 버블 점수는 기능 40번에서 할 거야.
- 기존 배경, 타이머, 게이지, 점수, 슈터, 조준선, 현재 버블, 다음 버블, 발사, 격자, 같은 색 찾기, 버블 제거, 점수 증가, 제거 효과, 천장 연결 찾기, 떠 있는 버블 찾기 기능은 절대 망가뜨리지 말아줘.
- EventSystem은 삭제된 상태야. 이번 기능에는 필요 없으면 만들지 마.

목표:
같은 색 3개 이상이 제거된 뒤 천장과 연결되지 않은 버블이 생기면,
그 버블들이 아래로 떨어지게 만들고 싶어.

조건:
- Unity 6 2D 프로젝트 기준으로 설명해줘.
- 이번에는 기능 39번 "연결 끊긴 버블 아래로 떨어뜨리기"만 만들어줘.
- 떨어진 버블에 점수는 아직 주지 마. 점수는 기능 40번에서 할 거야.
- FindFloatingBubbles()로 떠 있는 버블 목록을 가져와서 사용해줘.
- 떠 있는 버블은 더 이상 격자에 붙어 있으면 안 돼.
- 떨어뜨리기 전에 BubbleSlot의 occupied는 false, bubbleObject는 null로 비워줘.
- 떨어지는 버블은 화면 아래로 이동한 뒤 Destroy() 해줘.
- 물리 Rigidbody2D를 꼭 쓰지 않아도 돼. 초보자에게 쉬운 Coroutine 방식으로 아래로 이동해줘.
- 기존 ShooterRoot 위치는 자동으로 바꾸지 마.
- 기존 벽 오브젝트 위치도 자동으로 바꾸지 마.
- 자동 세팅 메뉴는 만들지 마.
- 초보자도 이해할 수 있게 쉬운 말로 설명해줘.
- 어려운 영어 용어가 나오면 한국말 뜻도 같이 설명해줘.

코드 작성 조건:
- 가능하면 BubbleGridManager.cs만 수정해줘.
- 새 스크립트가 꼭 필요하지 않으면 만들지 마.
- Coroutine을 사용해서 떠 있는 버블을 아래로 부드럽게 이동시켜줘.
- 새 함수 예시:
  - private void DropFloatingBubbles()
  - private IEnumerator DropBubbleObject(GameObject bubbleObject)
  - private void ClearFloatingBubbleSlot(BubbleSlot slot)
- RemoveMatchedBubbles()에서 같은 색 버블 제거가 끝난 뒤 FindFloatingBubbles()를 호출하고, 결과가 있으면 DropFloatingBubbles()를 실행해줘.
- FindFloatingBubbles() 함수 자체는 유지해줘.
- 기능 37의 FindCeilingConnectedBubbles() 함수는 건드리지 마.
- 기능 38의 FindFloatingBubbles() 함수는 필요한 최소 수정만 해줘.
- [Header("한글 설명")]을 사용할 수 있는 새 Inspector 변수에는 한글 설명을 넣어줘.
- [Tooltip("초보자용 설명")]도 한글로 추가해줘.
- 변수 이름은 Unity/C# 규칙 때문에 영어로 유지해줘.
- 코드 주석은 한글로 많이 작성해줘.
- 어려운 문법보다 쉬운 문법으로 작성해줘.
- 기존 기능을 크게 갈아엎지 말고, 필요한 부분만 최소 수정해줘.
- 기존 배경, 타이머, 게이지, 슈터 조준, 조준선, 현재/다음 버블 기능 파일은 가능하면 건드리지 마.
- GameObject.Find() 사용 금지.
- 하드코딩된 Tag 사용 금지.
- 외부 객체 참조는 반드시 [SerializeField] 또는 Interface 사용.

C# 이벤트 조건:
- 이번 기능에서는 점수를 직접 올리지 마.
- 떨어진 버블 개수 이벤트는 기능 40번에서 만들 예정이므로 이번에는 만들지 않아도 돼.
- 이미 있는 MatchedBubblesRemoved 점수 이벤트는 건드리지 마.

중요한 테스트 상황:
- 같은 색 3개 이상 제거 후 떠 있는 버블이 생기면 아래로 떨어지는지 확인.
- 떨어지는 버블은 격자에서 비워져야 해.
- 떨어지는 버블은 화면 아래로 내려간 뒤 Destroy() 되어야 해.
- 떨어진 버블 때문에 점수가 추가로 오르면 안 돼. 점수는 기능 40번에서 처리할 거야.
- 천장과 연결된 버블은 떨어지면 안 돼.
- 떠 있는 버블이 0개이면 아무 일도 일어나지 않아야 해.
- 기존 발사/정렬/같은 색 찾기/버블 제거/점수 증가/제거 효과/천장 연결 찾기/떠 있는 버블 찾기 기능이 정상 작동하는지 확인.

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
- 왜 떠 있는 버블을 떨어뜨려야 하는지 알려줘.
- Coroutine이 무엇인지 쉽게 설명해줘.
- Slot을 먼저 비우는 이유를 쉽게 설명해줘.
- 함수별 설명을 해줘.
- 이 코드가 왜 이렇게 동작하는지 초보자도 이해하게 설명해줘.
- 실행 순서를 번호로 설명해줘.
- 변수, 함수, if문이 각각 무슨 역할인지 알려줘.
- 마지막에 내가 외워야 할 핵심만 짧게 뽑아줘.

주의:
- 이번에는 기능 39번만 만들어줘.
- 떨어진 버블 점수는 만들지 마.
- 새로운 점수 이벤트도 만들지 마.
- 기존 점수 기능을 건드리지 마.
- 기존 발사 기능과 정렬 기능을 망가뜨리지 마.
- ShooterRoot, WallsRoot, LeftWall, RightWall, Ceiling 위치를 자동으로 바꾸지 마.
- 자동 세팅 메뉴는 만들지 마.
- EventSystem은 다시 만들지 마.
- BubbleLauncherController.cs, BubbleSwapController.cs, BubbleCurrentController.cs는 새로 만들지 마.
```

## 사용 방법

1. 위 `text` 박스 안의 내용을 전부 복사합니다.
2. 다음 작업 요청으로 그대로 붙여넣습니다.
3. 기능 39번이 성공하면 다음에는 기능 40번 프롬프트를 만들면 됩니다.
