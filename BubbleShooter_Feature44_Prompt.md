# 기능 44 프롬프트: Stage별 시작 버블 배치 저장

아래 프롬프트를 그대로 복사해서 사용하세요.

```text
나는 초보자이고, Unity 6으로 2D 버블슈터 게임을 만들고 있어.

이번에는 기능 목록 44번인 "Stage별 시작 버블 배치 저장"만 만들고 싶어.

참고 문서 위치:
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_Planning.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_StagePlan.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_FeatureList.md

작업 폴더:
- C:\Users\admin\Documents\BubbleGame

Unity 프로젝트 폴더:
- C:\Users\admin\Documents\BubbleGame\BubbleGame

현재 상태:
- 기능 1번~43번은 완료했어. (22번 모바일 터치는 패스)
- 기능 43번 "Stage별 색 종류 저장" 테스트 완료했어.
- StageDataController.cs가 이미 있어서 Inspector에서 스테이지 이름, 번호, 배경 이미지, StageBackgroundController, 버블 색 Sprite[], colorCount를 저장할 수 있어.
- StageBubbleLayout.cs가 이미 있어서 Inspector에서 버블 배치, 크기, 간격, 벽 bounds를 설정할 수 있어.
- StageBubbleLayout.cs에는 rows, cols, bubbleSpacing, useStaggeredRows, bubbleSprites[] 변수가 있어.
- StageBubbleLayout.cs에는 Stage1Pattern[][]이 하드코딩되어 있어서 시작 배치가 고정되어 있어.
- BubbleGridManager.cs가 이미 있어서 격자 칸(BubbleSlot) 관리, 버블 등록을 담당해.
- 아직 StageDataController에서 스테이지별 시작 버블 배치를 저장하는 기능이 없어.
- 기존 배경, 타이머, 게이지, 점수, 슈터, 조준선, 현재 버블, 다음 버블, 발사, 격자, 같은 색 찾기, 버블 제거, 점수 증가, 제거 효과, 천장 연결 찾기, 떠 있는 버블 찾기, 떠 있는 버블 떨어뜨리기, 떨어진 버블 점수, 스테이지 이름, 배경 이미지, 색 종류 기능은 절대 망가뜨리지 말아줘.
- EventSystem은 삭제된 상태야. 이번 기능에는 필요 없으면 만들지 마.

목표:
스테이지별로 시작 버블 배치를 저장하고 싶어.
Stage 1은 단순한 4줄 배치, Stage 2는 조금 복잡한 5줄 배치, Stage 3은 복잡한 6줄 배치를 Inspector에서 설정할 수 있게 하고 싶어.

조건:
- Unity 6 2D 프로젝트 기준으로 설명해줘.
- 이번에는 기능 44번 "Stage별 시작 버블 배치 저장"만 만들어줘.
- StageDataController.cs를 수정해줘.
- StageBubbleLayout.cs는 수정하지 마.
- BubbleGridManager.cs는 수정하지 마.
- StageDataController.cs에 시작 배치 설정 변수를 추가해줘:
  - [SerializeField] private int startRows = 4; (시작 시 채울 줄 수)
  - [SerializeField] private int startCols = 6; (한 줄의 칸 수)
- StageDataController.cs에 배치 패턴을 저장하는 변수를 추가해줘:
  - [SerializeField] private int[] startBubblePattern; (시작 버블 배치를 숫자로 저장)
- startBubblePattern은 "0=빨강, 1=파랑, 2=노랑" 같은 숫자 배열입니다.
- Inspector에서 startBubblePattern에 숫자를 입력하면 그 모양대로 버블이 배치됩니다.
- startBubblePattern이 비어 있으면 기본 배치를 사용합니다.
- 나중에 스테이지가 바뀌면 Inspector 값만 바꾸면 되게 해줘.
- 기존 StageBubbleLayout.cs는 수정하지 마.
- 기존 BubbleGridManager.cs는 수정하지 마.
- 기존 ShooterRoot 위치는 자동으로 바꾸지 마.
- 기존 벽 오브젝트 위치도 자동으로 바꾸지 마.
- 자동 세팅 메뉴는 만들지 마.
- 초보자도 이해할 수 있게 쉬운 말로 설명해줘.
- 어려운 영어 용어가 나오면 한국말 뜻도 같이 설명해줘.

코드 작성 조건:
- StageDataController.cs만 수정해줘.
- 새 스크립트는 만들지 마.
- [Header("한글 설명")]을 사용할 수 있는 새 Inspector 변수에는 한글 설명을 넣어줘.
- [Tooltip("초보자용 설명")]도 한글로 추가해줘.
- 변수 이름은 Unity/C# 규칙 때문에 영어로 유지해줘.
- 코드 주석은 한글로 많이 작성해줘.
- 어려운 문법보다 쉬운 문법으로 작성해줘.
- GameObject.Find() 사용 금지.
- 하드코딩된 Tag 사용 금지.
- 외부 객체 참조는 반드시 [SerializeField] 또는 Interface 사용.
- SRP(단일 책임 원칙)를 지켜줘. StageDataController는 스테이지 데이터만 담당해.
- 다른 스크립트에서 startBubblePattern을 가져갈 수 있는 public 함수를 만들어줘:
  - GetStartBubblePattern() 함수: 현재 startBubblePattern을 return 해줘.
  - GetStartRows() 함수: 현재 startRows를 return 해줘.
  - GetStartCols() 함수: 현재 startCols를 return 해줘.
- ShowStageInfo()에서 배치 설정 로그도 출력해줘:
  - [기능 44] Stage {stageNumber} 배치 설정: {startRows}줄 x {startCols}칸, 패턴 {startBubblePattern.Length}개

중요한 테스트 상황:
- Inspector에서 startRows를 4로, startCols를 6으로 설정.
- Inspector에서 startBubblePattern에 숫자를 입력 (예: 0,0,1,1,2,2, 0,0,1,1,2 등).
- Play하면 Console에 "[기능 44]" 로그가 출력되는지 확인.
- GetStartBubblePattern() 함수가 올바른 배열을 반환하는지 확인.
- GetStartRows() 함수가 올바른 숫자를 반환하는지 확인.
- GetStartCols() 함수가 올바른 숫자를 반환하는지 확인.
- 기존 발사/정렬/같은 색 찾기/버블 제거/점수 증가/제거 효과/천장 연결 찾기/떠 있는 버블 찾기/떠 있는 버블 떨어뜨리기/떨어진 버블 점수/스테이지 이름/배경 이미지/색 종류 기능이 정상 작동하는지 확인.

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
- 왜 스테이지별로 배치를 다르게 해야 하는지 알려줘.
- Inspector에서 int 배열을 입력하는 것이 왜 편한지 쉽게 설명해줘.
- startBubblePattern과 startRows, startCols가 어떻게 연결되는지 쉽게 설명해줘.
- 함수별 설명을 해줘.
- 이 코드가 왜 이렇게 동작하는지 초보자도 이해하게 설명해줘.
- 실행 순서를 번호로 설명해줘.
- 변수, 함수, if문이 각각 무슨 역할인지 알려줘.
- 마지막에 내가 외워야 할 핵심만 짧게 뽑아줘.

주의:
- 이번에는 기능 44번만 만들어줘.
- StageBubbleLayout.cs는 수정하지 마.
- BubbleGridManager.cs는 수정하지 마.
- 기존 발사 기능과 정렬 기능을 망가뜨리지 마.
- ShooterRoot, WallsRoot, LeftWall, RightWall, Ceiling 위치를 자동으로 바꾸지 마.
- 자동 세팅 메뉴는 만들지 마.
- EventSystem은 다시 만들지 마.
- BubbleLauncherController.cs, BubbleSwapController.cs, BubbleCurrentController.cs는 새로 만들지 마.
```

## 사용 방법

1. 위 `text` 박스 안의 내용을 전부 복사합니다.
2. 다음 작업 요청으로 그대로 붙여넣습니다.
3. 기능 44번이 성공하면 다음에는 기능 45번 프롬프트를 만들면 됩니다.
