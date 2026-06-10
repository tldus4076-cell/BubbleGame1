# 기능 37 프롬프트: 천장과 연결된 버블 찾기

아래 프롬프트를 그대로 복사해서 사용하세요.

```text
나는 초보자이고, Unity 6으로 2D 버블슈터 게임을 만들고 있어.

이번에는 기능 목록 37번인 "천장과 연결된 버블 찾기"만 만들고 싶어.

참고 문서 위치:
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_Planning.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_StagePlan.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_FeatureList.md

작업 폴더:
- C:\Users\admin\Documents\BubbleGame

Unity 프로젝트 폴더:
- C:\Users\admin\Documents\BubbleGame\BubbleGame

현재 상태:
- 기능 1번~36번은 완료했어. (22번 모바일 터치는 패스)
- 기능 36번 "버블 제거 효과" 테스트는 완료했어.
- 현재 발사 시스템은 Grid 기반으로 동작해:
  - ShooterController.cs가 발사를 담당해.
  - BubbleGridManager.cs가 격자 칸(BubbleSlot) 관리, 버블 등록, 같은 색 찾기, 매칭 규칙 확인, 버블 제거를 담당해.
  - BubbleSlot에 occupied (true/false) 와 bubbleObject 필드가 있어.
  - GetNeighborOffsets(row)로 한 버블의 이웃 6칸을 찾을 수 있어.
  - row 0(최상단)이 천장 줄이야. 이 줄에 있는 버블이 천장에 붙은 거야.
  - 기능 32에서 BFS로 같은 색 연결을 찾는 로직이 이미 있어 (CheckMatchRule).
- 이번 기능 37에서는 "천장에 연결된 모든 버블"을 찾는 BFS를 새로 만들어야 해.
- 같은 색이 아니라 "아무 색이든" 천장과 연결된 버블을 다 찾아야 해.
- 아직 천장과 연결 안 된 버블(떨어질 버블)을 떨어뜨리지는 마. 그건 기능 39번~40번에서 할 거야.
- 기존 배경, 타이머, 게이지, 점수, 슈터, 조준선, 현재 버블, 다음 버블, 발사, 격자, 같은 색 찾기, 버블 제거, 점수 증가, 제거 효과 기능은 절대 망가뜨리지 말아줘.
- EventSystem은 삭제된 상태야. 이번 기능에는 필요 없으면 만들지 마.

목표:
천장(row 0)에 붙은 버블들에서 시작해서, 그 옆에 붙은 버블들을 차례로 따라가며
천장과 연결된 모든 버블을 찾고 싶어. 이때 색깔은 상관없어.

조건:
- Unity 6 2D 프로젝트 기준으로 설명해줘.
- 이번에는 기능 37번 "천장과 연결된 버블 찾기"만 만들어줘.
- 천장과 연결 안 된 버블을 떨어뜨리지는 마. 떨어뜨리는 건 기능 39번~40번에서 할 거야.
- 점수 증가는 하지 마. (기능 35에서 이미 처리됨)
- 같은 색 찾기(CheckMatchRule)는 건드리지 마.
- BFS(너비 우선 탐색) 알고리즘을 사용해서 천장과 연결된 모든 버블을 찾아줘.
- 시작점: row 0(천장 줄)의 모든 occupied 칸
- 끝점: 더 이상 연결된 occupied 칸이 없을 때까지
- 색깔에 관계없이 모든 버블을 포함해줘.
- BubbleGridManager.cs에 새 함수를 만들어줘.
- 함수 이름은 초보자가 읽기 쉬운 이름으로 해줘. 예: FindCeilingConnectedBubbles()
- 찾은 결과는 BubbleSlot 리스트로 돌려줘.
- 결과는 public 함수로 만들어서 Inspector나 다른 스크립트에서도 확인할 수 있게 해줘.
- 실제 버블을 제거하거나 점수를 올리지는 마. 찾기만 해.
- 기존 ShooterRoot 위치는 자동으로 바꾸지 마.
- 기존 벽 오브젝트 위치도 자동으로 바꾸지 마.
- 자동 세팅 메뉴는 만들지 마.
- 초보자도 이해할 수 있게 쉬운 말로 설명해줘.
- 어려운 영어 용어가 나오면 한국말 뜻도 같이 설명해줘.

코드 작성 조건:
- BubbleGridManager.cs만 수정해줘.
- 새 스크립트가 꼭 필요하지 않으면 만들지 마.
- BFS 탐색은 기존 searchQueue와 visited 배열을 재사용할 수 있어.
- 새 함수 예시:
  - public System.Collections.Generic.List<BubbleSlot> FindCeilingConnectedBubbles()
  - private void AddOccupiedNeighborSlots(BubbleSlot currentSlot, bool[,] visited)
- CheckMatchRule()과 같은 색을 비교하지 않고, occupied면 무조건 큐에 넣어줘.
- [Header("한글 설명")]을 사용할 수 있는 새 Inspector 변수는 이번 기능에 필요 없어.
- 함수 이름은 한글이 아니라 영어지만, 주석에 한글로 설명을 달아줘.
- 변수 이름은 Unity/C# 규칙 때문에 영어로 유지해줘.
- 코드 주석은 한글로 많이 작성해줘.
- 어려운 문법보다 쉬운 문법으로 작성해줘.
- 기존 기능을 크게 갈아엎지 말고, 필요한 부분만 최소 수정해줘.
- 기존 배경, 타이머, 게이지, 슈터 조준, 조준선, 현재/다음 버블 기능 파일은 가능하면 건드리지 마.
- GameObject.Find() 사용 금지.
- 하드코딩된 Tag 사용 금지.
- 외부 객체 참조는 반드시 [SerializeField] 또는 Interface 사용.

중요한 테스트 상황:
- 천장 줄(row 0)에 버블이 있을 때, 그 버블이 결과에 포함되는지 확인.
- 천장 줄 버블 옆의 같은 색/다른 색 버블도 결과에 포함되는지 확인.
- 천장과 연결되지 않은 버블(떠 있는 버블)은 결과에 포함되지 않아야 해.
- 결과를 Console에 Debug.Log로 몇 개인지 출력해줘. 예: "[기능 37] 천장 연결 버블: N개"
- 0개여도 정상 동작해야 해.
- 점수는 이번 기능에서 안 올라가도 돼.
- 버블이 실제로 제거되거나 떨어지지 않아야 해.
- 기존 발사/정렬/같은 색 찾기/버블 제거/점수 증가/제거 효과 기능이 정상 작동하는지 확인.

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
- BFS가 무엇인지 쉽게 설명해줘.
- 왜 천장과 연결된 버블을 찾아야 하는지 쉽게 설명해줘.
- 같은 색 찾기와 어떻게 다른지 알려줘.
- 함수별 설명을 해줘.
- 이 코드가 왜 이렇게 동작하는지 초보자도 이해하게 설명해줘.
- 실행 순서를 번호로 설명해줘.
- 변수, 함수, if문이 각각 무슨 역할인지 알려줘.
- 마지막에 내가 외워야 할 핵심만 짧게 뽑아줘.

주의:
- 이번에는 기능 37번만 만들어줘.
- 점수 증가는 만들지 마.
- 제거 효과는 만들지 마.
- 떨어뜨리기는 만들지 마.
- 기존 발사 기능과 정렬 기능을 망가뜨리지 마.
- ShooterRoot, WallsRoot, LeftWall, RightWall, Ceiling 위치를 자동으로 바꾸지 마.
- 자동 세팅 메뉴는 만들지 마.
- EventSystem은 다시 만들지 마.
- BubbleLauncherController.cs, BubbleSwapController.cs, BubbleCurrentController.cs는 새로 만들지 마.
```

## 사용 방법

1. 위 `text` 박스 안의 내용을 전부 복사합니다.
2. 다음 작업 요청으로 그대로 붙여넣습니다.
3. 기능 37번이 성공하면 다음에는 기능 38번 프롬프트를 만들면 됩니다.
