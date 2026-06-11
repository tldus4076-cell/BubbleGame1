# 기능 53 프롬프트: Stage 2 난이도 - 시작 배치를 조금 복잡하게 만들기

아래 프롬프트를 그대로 복사해서 사용하세요.

```text
나는 초보자이고, Unity 6으로 2D 버블슈터 게임을 만들고 있어.

이번에는 기능 목록 53번인 "Stage 2 난이도 - 시작 배치를 조금 복잡하게 만들기"만 만들고 싶어.

참고 문서 위치:
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_Planning.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_StagePlan.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_FeatureList.md

작업 폴더:
- C:\Users\admin\Documents\BubbleGame

Unity 프로젝트 폴더:
- C:\Users\admin\Documents\BubbleGame\BubbleGame

현재 상태:
- 기능 1번~52번은 완료했어. (22번 모바일 터치는 패스)
- 기능 52번 "Stage 2 난이도 - 색을 4종류 사용" 테스트 완료했어.
- Stage 2 씬이 있고, Stage 2에서는 빨강/파랑/노랑/초록 4색이 나와.
- StageDataController.cs에는 startRows, startCols, startBubblePattern, GetStartRows(), GetStartCols(), GetStartBubblePattern()이 이미 있어.
- StageBubbleLayout.cs는 현재 시작 버블 배치를 만드는 스크립트야.
- StageBubbleLayout.cs는 아직 StageDataController의 startBubblePattern을 실제 배치에 완전히 적용하지 못할 수 있어.
- Stage 2는 Stage 1보다 조금 복잡한 배치가 필요해.
- 기존 배경, 타이머, 게이지, 점수, 슈터, 조준선, 현재 버블, 다음 버블, 발사, 격자, 같은 색 찾기, 버블 제거, 점수 증가, 제거 효과, 천장 연결 찾기, 떠 있는 버블 찾기, 떠 있는 버블 떨어뜨리기, 떨어진 버블 점수, 스테이지 이름, 배경 이미지, 색 종류, 배치 저장, 제한 샷 수, 제한 시간, 장애물, Stage 1 난이도 기능은 절대 망가뜨리지 말아줘.
- EventSystem은 삭제된 상태야. 이번 기능에는 필요 없으면 만들지 마.

목표:
Stage 2의 시작 배치를 Stage 1보다 조금 복잡하게 만들고 싶어.
가운데는 조금 막혀 있고, 벽 반사를 사용하면 옆쪽 버블을 맞추기 쉬운 배치로 만들고 싶어.

조건:
- Unity 6 2D 프로젝트 기준으로 설명해줘.
- 이번에는 기능 53번 "Stage 2 난이도 - 시작 배치를 조금 복잡하게 만들기"만 만들어줘.
- StageDataController.cs는 수정하지 마. 이미 필요한 데이터가 있어.
- StageBubbleLayout.cs만 필요한 만큼 최소 수정해줘.
- BubbleGridManager.cs는 수정하지 마.
- BubbleNextController.cs는 수정하지 마.
- ShooterController.cs는 수정하지 마.
- StageBubbleLayout.cs에 StageDataController 참조를 Inspector에서 연결할 수 있게 추가해줘:
  - [SerializeField] private StageDataController stageDataController;
- GameObject.Find() 사용 금지.
- 하드코딩된 Tag 사용 금지.
- 외부 참조는 [SerializeField]로 연결해줘.
- stageDataController가 연결되어 있고 startBubblePattern이 비어 있지 않으면, 그 패턴을 시작 배치에 사용해줘.
- stageDataController가 연결되어 있지 않거나 startBubblePattern이 비어 있으면, 기존 Stage1Pattern을 예비로 사용해줘.
- Stage 2 추천 설정:
  - Start Rows: 5
  - Start Cols: 6
  - Start Bubble Pattern 크기: 24
  - 0=빨강, 1=파랑, 2=노랑, 3=초록
- 추천 Stage 2 배치 패턴:
  - 1번째 줄 6개: 0, 1, 2, 3, 1, 0
  - 2번째 줄 5개: 3, 2, 1, 0, 3
  - 3번째 줄 5개: 0, 3, 2, 1, 0
  - 4번째 줄 4개: 1, 2, 3, 1
  - 5번째 줄 4개: 2, 0, 3, 2
- 이 패턴은 색이 조금 섞여 있어서 Stage 1보다 어렵고, 양쪽 가장자리 색을 맞출 때 벽 반사가 도움이 됩니다.
- 기존 ShooterRoot 위치는 자동으로 바꾸지 마.
- 기존 벽 오브젝트 위치도 자동으로 바꾸지 마.
- 자동 세팅 메뉴는 만들지 마.
- 초보자도 이해할 수 있게 쉬운 말로 설명해줘.
- 어려운 영어 용어가 나오면 한국말 뜻도 같이 설명해줘.

코드 작성 조건:
- StageBubbleLayout.cs만 수정해줘.
- 새 스크립트는 만들지 마.
- 기존 기능을 크게 갈아엎지 말고, 필요한 부분만 최소 수정해줘.
- [Header("한글 설명")]을 사용할 수 있는 새 Inspector 변수에는 한글 설명을 넣어줘.
- [Tooltip("초보자용 설명")]도 한글로 추가해줘.
- 변수 이름은 Unity/C# 규칙 때문에 영어로 유지해줘.
- 코드 주석은 한글로 많이 작성해줘.
- 어려운 문법보다 쉬운 문법으로 작성해줘.
- SRP(단일 책임 원칙)를 지켜줘. StageBubbleLayout은 버블 배치만 담당해.
- StageDataController는 데이터 저장만 담당하고, 실제 생성은 StageBubbleLayout이 담당해.

구현 힌트:
- StageBubbleLayout.cs에 private int GetPatternValue(int row, int col, int fallbackValue) 같은 쉬운 도우미 함수를 만들어도 돼.
- StageDataController에서 받은 1차원 startBubblePattern을 row/col 위치에 맞게 읽어줘.
- Stage 2 패턴의 줄 길이는 6,5,5,4,4 총 24개입니다.
- 각 줄 시작 인덱스는 0, 6, 11, 16, 20으로 계산할 수 있어.
- 너무 복잡하면 간단한 방식으로:
  - 현재까지 지나온 칸 수를 patternIndex로 세면서 startBubblePattern[patternIndex]를 순서대로 읽어도 돼.

중요한 테스트 상황:
- Stage 2 씬에서 StageDataManager의 Start Rows를 5로 설정.
- Start Cols를 6으로 설정.
- Start Bubble Pattern 크기를 24로 설정.
- 위 추천 패턴 24개 숫자를 입력.
- StageBubbleLayout의 Stage Data Controller 칸에 StageDataManager를 연결.
- Play하면 Stage 2 시작 배치가 Stage 1보다 더 섞여 있는지 확인.
- 빨강/파랑/노랑/초록 4색이 모두 보이는지 확인.
- 벽 반사로 양쪽 가장자리 버블을 맞추기 쉬운지 확인.
- 기존 발사/정렬/같은 색 찾기/버블 제거/점수 증가/제거 효과/천장 연결 찾기/떠 있는 버블 찾기/떠 있는 버블 떨어뜨리기/떨어진 버블 점수/스테이지 이름/배경 이미지/색 종류/제한 샷 수/제한 시간/장애물 기능이 정상 작동하는지 확인.

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
- 왜 Stage 2에서 배치가 조금 복잡해야 하는지 알려줘.
- 왜 벽 반사를 쓰면 쉬워지는지 알려줘.
- startBubblePattern 숫자 배열을 그림처럼 설명해줘.
- StageDataController와 StageBubbleLayout이 어떻게 연결되는지 쉽게 설명해줘.
- 함수별 설명을 해줘.
- 이 코드가 왜 이렇게 동작하는지 초보자도 이해하게 설명해줘.
- 실행 순서를 번호로 설명해줘.
- 변수, 함수, if문이 각각 무슨 역할인지 알려줘.
- 마지막에 내가 외워야 할 핵심만 짧게 뽑아줘.

주의:
- 이번에는 기능 53번만 만들어줘.
- StageBubbleLayout.cs 외 다른 스크립트는 수정하지 마.
- 기존 발사 기능과 정렬 기능을 망가뜨리지 마.
- ShooterRoot, WallsRoot, LeftWall, RightWall, Ceiling 위치를 자동으로 바꾸지 마.
- 자동 세팅 메뉴는 만들지 마.
- EventSystem은 다시 만들지 마.
- BubbleLauncherController.cs, BubbleSwapController.cs, BubbleCurrentController.cs는 새로 만들지 마.
```

## 사용 방법

1. 위 `text` 박스 안의 내용을 전부 복사합니다.
2. 다음 작업 요청으로 그대로 붙여넣습니다.
3. 기능 53번이 성공하면 다음에는 기능 54번 프롬프트를 만들면 됩니다.
