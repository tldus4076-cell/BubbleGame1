# 기능 41 프롬프트: Stage 이름 저장 + 잠깐 표시 후 자동 숨김

아래 프롬프트를 그대로 복사해서 사용하세요.

```text
나는 초보자이고, Unity 6으로 2D 버블슈터 게임을 만들고 있어.

이번에는 기능 목록 41번인 "Stage 이름 저장 + 잠깐 표시 후 자동 숨김"만 만들고 싶어.

참고 문서 위치:
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_Planning.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_StagePlan.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_FeatureList.md

작업 폴더:
- C:\Users\admin\Documents\BubbleGame

Unity 프로젝트 폴더:
- C:\Users\admin\Documents\BubbleGame\BubbleGame

현재 상태:
- 기능 1번~40번은 완료했어. (22번 모바일 터치는 패스)
- 기능 40번 "떨어진 버블에도 점수 주기" 테스트 완료했어.
- 현재 발사 시스템은 Grid 기반으로 동작해:
  - ShooterController.cs가 발사를 담당해.
  - BubbleGridManager.cs가 격자 칸(BubbleSlot) 관리, 버블 등록, 같은 색 찾기, 매칭 규칙 확인, 버블 제거, 제거 효과, 천장 연결 버블 찾기, 떠 있는 버블 찾기, 떠 있는 버블 떨어뜨리기, 떨어진 버블 점수 이벤트를 담당해.
- StageBackgroundController.cs가 이미 있어서 Inspector에서 배경 Sprite를 연결할 수 있어.
- StageBubbleLayout.cs가 이미 있어서 Inspector에서 버블 배치, 크기, 간격, 벽 bounds를 설정할 수 있어.
- 아직 스테이지 이름을 저장하고, 게임 시작 시 잠깐 보여준 뒤 자동으로 숨기는 기능이 없어.
- 기존 배경, 타이머, 게이지, 점수, 슈터, 조준선, 현재 버블, 다음 버블, 발사, 격자, 같은 색 찾기, 버블 제거, 점수 증가, 제거 효과, 천장 연결 찾기, 떠 있는 버블 찾기, 떠 있는 버블 떨어뜨리기, 떨어진 버블 점수 기능은 절대 망가뜨리지 말아줘.
- EventSystem은 삭제된 상태야. 이번 기능에는 필요 없으면 만들지 마.

목표:
스테이지 이름을 저장하고, 게임 시작 시 화면에 잠깐 보여준 뒤 자동으로 사라지게 하고 싶어.
Stage 1을 플레이하면 "Stage 1"이 1~2초 정도 보였다가 사라지고, Stage 2를 플레이하면 "Stage 2"가 잠깐 보였다가 사라지게 하고 싶어.

조건:
- Unity 6 2D 프로젝트 기준으로 설명해줘.
- 이번에는 기능 41번 "Stage 이름 저장 + 잠깐 표시 후 자동 숨김"만 만들어줘.
- 새 스크립트 StageDataController.cs를 만들어줘.
- 만약 StageDataController.cs가 이미 있으면 새로 만들지 말고 기존 파일만 수정해줘.
- StageDataController.cs는 Inspector에서 스테이지 이름을 직접 입력할 수 있게 해줘.
- [SerializeField] private string stageName = "Stage 1"; 같은 변수를 만들어줘.
- [SerializeField] private int stageNumber = 1; 같은 변수를 만들어줘.
- 스테이지 이름을 화면에 표시하는 TextMeshPro 텍스트를 연결할 수 있게 해줘.
- [SerializeField] private TMPro.TextMeshProUGUI stageNameText; 같은 변수를 만들어줘.
- 게임 시작 시 스테이지 이름을 자동으로 보여줄지 정하는 변수를 만들어줘.
- [SerializeField] private bool showOnStart = true; 같은 변수를 만들어줘.
- 스테이지 이름이 몇 초 뒤 사라질지 정하는 변수를 만들어줘.
- [SerializeField] private float hideDelay = 1.5f; 같은 변수를 만들어줘.
- TextMeshPro가 설치되어 있지 않으면 안내해줘.
- stageNameText가 연결되어 있으면 Start()에서 stageName을 표시해줘.
- stageNameText가 연결되어 있으면 hideDelay초 뒤 자동으로 글자를 숨겨줘.
- stageNameText가 비어 있으면 Debug.Log로만 스테이지 이름을 출력해줘.
- Stage 1 씬에는 "Stage 1", Stage 2 씬에는 "Stage 2"를 Inspector에서 입력하면 돼.
- 나중에 스테이지가 바뀌면 Inspector 값만 바꾸면 되게 해줘.
- 기존 StageBackgroundController.cs는 수정하지 마.
- 기존 StageBubbleLayout.cs는 수정하지 마.
- 기존 ShooterRoot 위치는 자동으로 바꾸지 마.
- 기존 벽 오브젝트 위치도 자동으로 바꾸지 마.
- 자동 세팅 메뉴는 만들지 마.
- 초보자도 이해할 수 있게 쉬운 말로 설명해줘.
- 어려운 영어 용어가 나오면 한국말 뜻도 같이 설명해줘.

코드 작성 조건:
- StageDataController.cs를 만들어줘.
- 이미 StageDataController.cs가 있다면 그 파일만 수정해줘.
- 기존 다른 스크립트는 수정하지 마.
- Inspector에서 스테이지 이름, 스테이지 번호, 표시 시간, TextMeshPro 텍스트를 직접 설정할 수 있게 해줘.
- [Header("한글 설명")]을 사용할 수 있는 새 Inspector 변수에는 한글 설명을 넣어줘.
- [Tooltip("초보자용 설명")]도 한글로 추가해줘.
- 변수 이름은 Unity/C# 규칙 때문에 영어로 유지해줘.
- 코드 주석은 한글로 많이 작성해줘.
- 어려운 문법보다 쉬운 문법으로 작성해줘.
- GameObject.Find() 사용 금지.
- 하드코딩된 Tag 사용 금지.
- 외부 객체 참조는 반드시 [SerializeField] 또는 Interface 사용.
- C# event/Action/UnityEvent를 사용할 수 있는 구조는 열어두되, 이번 기능에서 꼭 필요하지 않으면 억지로 만들지 마.
- SRP(단일 책임 원칙)를 지켜줘. StageDataController는 스테이지 데이터와 스테이지 이름 표시만 담당해.
- 자동 세팅 메뉴는 만들지 마.
- EventSystem은 만들지 마.

필수 함수 조건:
- ShowStageName() 함수를 만들어줘.
  - stageNameText가 연결되어 있으면 stageNameText.gameObject.SetActive(true)를 먼저 실행해줘.
  - stageNameText.text = stageName; 으로 이름을 넣어줘.
  - 이전 숨김 Coroutine이 돌고 있으면 멈추고 새로 시작해줘.
  - hideDelay초 뒤 HideStageName()이 실행되게 해줘.
- HideStageName() 함수를 만들어줘.
  - stageNameText가 연결되어 있으면 stageNameText.gameObject.SetActive(false)로 숨겨줘.
- HideStageNameAfterDelay() Coroutine을 만들어줘.
  - yield return new WaitForSeconds(hideDelay); 뒤에 HideStageName()을 호출해줘.
- SetStageName(string newName) 함수를 만들어줘.
  - stageName을 새 이름으로 바꾸고 ShowStageName()을 호출해줘.
- GetStageName() 함수를 만들어줘.
  - 현재 stageName을 return 해줘.
- GetStageNumber() 함수를 만들어줘.
  - 현재 stageNumber를 return 해줘.
- SetStageNumber(int newNumber) 함수를 만들어줘.
  - 1보다 작으면 1로 고쳐줘.

중요한 테스트 상황:
- Stage 1 씬에서 stageName을 "Stage 1"로 설정하고 Play하면 화면에 "Stage 1"이 보이는지 확인.
- "Stage 1" 글자가 hideDelay초 뒤 자동으로 사라지는지 확인.
- 사라진 뒤에도 게임 플레이 화면을 가리지 않는지 확인.
- stageNameText를 연결하면 텍스트로 표시되는지 확인.
- stageNameText를 연결하지 않으면 Debug.Log로만 출력되는지 확인.
- Inspector에서 stageName을 "Stage 2"로 바꾸면 표시도 "Stage 2"로 바뀌는지 확인.
- Inspector에서 hideDelay를 1.5, 2.0 등으로 바꾸면 사라지는 시간이 달라지는지 확인.
- 기존 발사/정렬/같은 색 찾기/버블 제거/점수 증가/제거 효과/천장 연결 찾기/떠 있는 버블 찾기/떠 있는 버블 떨어뜨리기/떨어진 버블 점수 기능이 정상 작동하는지 확인.

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
- 왜 스테이지 이름을 따로 저장하는지 알려줘.
- 왜 글자가 계속 떠 있으면 방해되는지 알려줘.
- 왜 잠깐 보였다가 사라지는 방식이 좋은지 알려줘.
- Inspector에서 값을 바꾸는 것이 왜 편한지 쉽게 설명해줘.
- Coroutine은 "시간을 기다렸다가 실행하는 기능"이라고 쉽게 설명해줘.
- ScriptableObject와 [SerializeField]의 차이를 쉽게 설명해줘. (이번에는 [SerializeField]를 사용해)
- 함수별 설명을 해줘.
- 이 코드가 왜 이렇게 동작하는지 초보자도 이해하게 설명해줘.
- 실행 순서를 번호로 설명해줘.
- 변수, 함수, if문이 각각 무슨 역할인지 알려줘.
- 마지막에 내가 외워야 할 핵심만 짧게 뽑아줘.

주의:
- 이번에는 기능 41번만 만들어줘.
- 기존 다른 스크립트는 수정하지 마.
- 기존 발사 기능과 정렬 기능을 망가뜨리지 마.
- ShooterRoot, WallsRoot, LeftWall, RightWall, Ceiling 위치를 자동으로 바꾸지 마.
- 자동 세팅 메뉴는 만들지 마.
- EventSystem은 다시 만들지 마.
- BubbleLauncherController.cs, BubbleSwapController.cs, BubbleCurrentController.cs는 새로 만들지 마.
```

## 사용 방법

1. 위 `text` 박스 안의 내용을 전부 복사합니다.
2. 다음 작업 요청으로 그대로 붙여넣습니다.
3. 기능 41번이 성공하면 다음에는 기능 42번 프롬프트를 만들면 됩니다.
