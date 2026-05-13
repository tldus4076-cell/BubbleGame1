# 기능 프롬프트: Stage 1 버블 초기 배치하기

아래 프롬프트를 그대로 복사해서 사용하세요.

```text
나는 초보자이고, Unity 6으로 2D 버블슈터 게임을 만들고 있어.

이번에는 **"Stage 1 버블 초기 배치하기"** 기능을 만들고 싶어.
(기능 목록 44번에 해당하는 내용이지만, 테스트를 위해 먼저 구현하고 싶어.)

참고 문서 위치:
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_Planning.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_StagePlan.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_FeatureList.md

작업 폴더:
- C:\Users\admin\Documents\BubbleGame

Unity 프로젝트 폴더:
- C:\Users\admin\Documents\BubbleGame\BubbleGame

현재 상태:
- 기능 1번~17번 (배경, 타이머, 점수, 슈터, 조준, 조준선 반사) 은 완료했어.
- 기능 18번 (현재 버블 표시) 은 완료했어.
- WallsRoot, LeftWall, RightWall, Ceiling 벽 오브젝트가 준비되어 있어.
- 버블 이미지 (빨강, 파랑, 노랑) 가 준비되어 있어.
- 기존 배경, 타이머, 게이지, 점수, 슈터 조준 기능은 절대 망가뜨리지 말아줘.
- EventSystem은 삭제된 상태야. 이번 기능에는 필요 없으면 만들지 마.
- 앞으로 자동 세팅 메뉴는 만들지 말고, 수동 세팅 방식으로 알려줘.

목표:
게임 시작 시 화면 위쪽 (천장 아래) 에 버블들이 격자 모양으로 배치되게 만들고 싶어.
Stage 1 이므로 빨강, 파랑, 노랑 색 버블만 사용해서 배치하고 싶어.

조건:
- Unity 6 2D 프로젝트 기준으로 설명해줘.
- 이번에는 "버블 배치"만 만들어줘.
- 버블 발사, 벽 반사, 매칭 제거 로직은 아직 만들지 마.
- 버블은 천장 (Ceiling) 아래쪽에 여러 줄로 배치되어야 해.
- 버블은 왼쪽 벽 (LeftWall) 과 오른쪽 벽 (RightWall) 사이에 있어야 해.
- 기존 StageBackgroundController.cs 등은 건드리지 마.
- 초보자도 이해할 수 있게 쉬운 말로 설명해줘.
- 어려운 영어 용어가 나오면 한국말 뜻도 같이 설명해줘.
- 전체 코드를 빠짐없이 작성해줘.
- 코드마다 주석을 초보자도 알아보기 쉽게 달아줘.
- 어느 GameObject에 스크립트를 붙이는지 알려줘.
- Inspector에서 뭘 설정해야 하는지 알려줘.

버블 배치 방식:
- StageBubbleLayout.cs 같은 새 스크립트를 만들어줘.
- StageBubbleLayout.cs는 WallsRoot 에 붙이는 방식으로 해줘. (벽 범위 안에 배치해야 하니까)
- Inspector에서 줄 수 (rows), 열 수 (cols), 버블 크기 (bubbleSize) 를 조절하게 해줘.
- Inspector에서 사용할 버블 Sprite 목록 (빨강, 파랑, 노랑) 을 연결하게 해줘.
- Start() 함수에서 버블들을 생성 (Instantiate) 하게 해줘.
- 버블 위치 계산:
  - 천장 바로 아래부터 시작해서 아래쪽으로 내려오게 해줘.
  - 왼쪽 벽과 오른쪽 벽 사이 간격에 맞게 가로로 배치해줘.
  - 각 줄마다 버블이 서로 겹치지 않게 간격을 둬줘.
- 색 지정:
  - 배치할 때 빨강, 파랑, 노랑 중 하나를 랜덤으로 고르게 해줘.

코드 작성 조건:
- StageBubbleLayout.cs를 새로 만들어줘.
- 기존 크립트는 건드리지 마.
- [Header("한글 설명")]과 [Tooltip("초보자용 설명")]을 사용해줘.
- 변수 이름은 영어로 유지해줘.
- 코드 주석은 한글로 작성해줘.
- 실행 흐름을 순서대로 알려줘.

수동 세팅 조건:
- 자동 세팅 메뉴를 만들지 마.
- Inspector에서 직접 값을 넣는 방법을 알려줘.

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

주의:
- 이번에는 버블 배치만 만들어줘.
- 버블 발사, 매칭, 제거 기능은 만들지 마.
- EventSystem은 다시 만들지 마.
```

## 사용 방법

1. 위 `text` 박스 안의 내용을 전부 복사합니다.
2. 다음 작업 요청으로 그대로 붙여넣습니다.
3. 기능이 성공하면 다음에는 버블 발사 기능을 진행하면 됩니다.
