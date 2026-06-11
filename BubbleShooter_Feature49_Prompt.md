# 기능 49 프롬프트: Stage 1 난이도 - 시작 배치를 단순하게 만들기

아래 프롬프트를 그대로 복사해서 사용하세요.

```text
나는 초보자이고, Unity 6으로 2D 버블슈터 게임을 만들고 있어.

이번에는 기능 목록 49번인 "Stage 1 난이도 - 시작 배치를 단순하게 만들기"만 만들고 싶어.

참고 문서 위치:
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_Planning.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_StagePlan.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_FeatureList.md

작업 폴더:
- C:\Users\admin\Documents\BubbleGame

Unity 프로젝트 폴더:
- C:\Users\admin\Documents\BubbleGame\BubbleGame

현재 상태:
- 기능 1번~48번은 완료했어. (22번 모바일 터치는 패스)
- 기능 48번 "Stage 1 난이도 - 색을 3종류만 사용" 테스트 완료했어.
- StageDataController.cs가 이미 있어서 Inspector에서 startBubblePattern을 저장할 수 있어.
- 기능 44에서 startBubblePattern 변수와 GetStartBubblePattern() 함수를 만들었어.
- StageBubbleLayout.cs가 이미 있어서 Inspector에서 버블 배치를 설정할 수 있어.
- 아직 Stage 1에서 실제로 단순한 배치가 적용되어 있지 않어.
- 기존 배경, 타이머, 게이지, 점수, 슈터, 조준선, 현재 버블, 다음 버블, 발사, 격자, 같은 색 찾기, 버블 제거, 점수 증가, 제거 효과, 천장 연결 찾기, 떠 있는 버블 찾기, 떠 있는 버블 떨어뜨리기, 떨어진 버블 점수, 스테이지 이름, 배경 이미지, 색 종류, 배치, 제한 샷 수, 제한 시간, 장애물, 3색 기능은 절대 망가뜨리지 말아줘.
- EventSystem은 삭제된 상태야. 이번 기능에는 필요 없으면 만들지 마.

목표:
Stage 1의 시작 배치를 단순하게 만들고 싶어.
같은 색이 가까이 있어서 초보자가 쉽게 제거할 수 있는 배치를 만들고 싶어.

조건:
- Unity 6 2D 프로젝트 기준으로 설명해줘.
- 이번에는 기능 49번 "Stage 1 난이도 - 시작 배치를 단순하게 만들기"만 만들어줘.
- StageDataController.cs를 수정하지 마. (이미 startBubblePattern이 있음)
- StageBubbleLayout.cs를 수정하지 마.
- 기존 코드는 건드리지 마.
- Stage 1 씬의 Inspector 설정만 확인하고 안내해줘.
- StageDataController의 Inspector에서:
  - Start Rows: 4 설정
  - Start Cols: 6 설정
  - Start Bubble Pattern에 단순한 배치 패턴 입력
- StageBubbleLayout의 Inspector에서:
  - rows: 4 설정
  - cols: 6 설정
- 단순한 배치 패턴이란:
  - 같은 색이 옆에 붙어 있어서 3개 이상 쉽게 제거할 수 있는 배치
  - 예: 빨강 3개가 연속으로 붙어 있으면 쉽게 제거 가능
- 추천 배치 패턴 (0=빨강, 1=파랑, 2=노랑):
  - 1번째 줄: 0, 0, 1, 1, 2, 2 (빨빨파파노노)
  - 2번째 줄: 0, 0, 1, 1, 2 (빨빨파파노)
  - 3번째 줄: 0, 1, 2, 0, 1 (빨파노빨파)
  - 4번째 줄: 2, 2, 1, 0 (노노파빨)
- 이 배치는 같은 색이 가까이 있어서 초보자가 쉽게 제거할 수 있습니다.
- 나중에 Stage 2, Stage 3을 만들면 더 복잡한 패턴을 넣으면 됨.
- 초보자도 이해할 수 있게 쉬운 말로 설명해줘.
- 어려운 영어 용어가 나오면 한국말 뜻도 같이 설명해줘.

코드 작성 조건:
- 새 스크립트를 만들지 마.
- 기존 스크립트를 수정하지 마.
- Inspector 설정 방법만 안내해줘.
- 코드가 필요하면 기존 코드를 어떻게 설정하는지 설명해줘.

중요한 테스트 상황:
- Stage 1 씬에서 Play하면 같은 색이 가까이 있는 배치가 나오는지 확인.
- 빨강 2개가 옆에 붙어 있는지 확인.
- 파랑 2개가 옆에 붙어 있는지 확인.
- 노랑 2개가 옆에 붙어 있는지 확인.
- 기존 발사/정렬/같은 색 찾기/버블 제거/점수 증가/제거 효과/천장 연결 찾기/떠 있는 버블 찾기/떠 있는 버블 떨어뜨리기/떨어진 버블 점수/스테이지 이름/배경 이미지/색 종류/배치/제한 샷 수/제한 시간/장애물/3색 기능이 정상 작동하는지 확인.

자동저장 조건:
- 기능 하나가 완성되면 Unity에서 Scene(씬)을 저장하라고 알려줘.
- Ctrl + S로 저장하는 방법도 알려줘.
- Unity 상단 메뉴에 자동 저장 메뉴를 새로 만들지는 마.

반드시 아래 형식으로 답해줘:

1. 기능 설명
2. 수정한 파일 (없으면 "수정한 파일 없음 - Inspector 설정만 안내")
3. Inspector 설정 방법
4. 코드 설명
5. 유니티 적용 방법
6. 오류 체크 포인트
7. Inspector에서 조절할 변수
8. 테스트 성공 기준
9. 다음 기능으로 넘어가기 전 체크리스트

설명 스타일:
- 초등학생도 이해할 수 있게 차근차근 설명해줘.
- 코드만 알려주지 말고 코드 설명도 해줘.
- 왜 Stage 1에서 단순한 배치가 필요한지 알려줘.
- 같은 색이 가까이 있으면 왜 쉬운지 알려줘.
- Inspector에서 int 배열을 입력하는 것이 왜 편한지 쉽게 설명해줘.
- startBubblePattern과 startRows, startCols가 어떻게 연결되는지 쉽게 설명해줘.
- 함수별 설명을 해줘.
- 이 코드가 왜 이렇게 동작하는지 초보자도 이해하게 설명해줘.
- 실행 순서를 번호로 설명해줘.
- 변수, 함수, if문이 각각 무슨 역할인지 알려줘.
- 마지막에 내가 외워야 할 핵심만 짧게 뽑아줘.

주의:
- 이번에는 기능 49번만 만들어줘.
- 기존 스크립트를 수정하지 마.
- 기존 발사 기능과 정렬 기능을 망가뜨리지 마.
- ShooterRoot, WallsRoot, LeftWall, RightWall, Ceiling 위치를 자동으로 바꾸지 마.
- 자동 세팅 메뉴는 만들지 마.
- EventSystem은 다시 만들지 마.
- BubbleLauncherController.cs, BubbleSwapController.cs, BubbleCurrentController.cs는 새로 만들지 마.
```

## 사용 방법

1. 위 `text` 박스 안의 내용을 전부 복사합니다.
2. 다음 작업 요청으로 그대로 붙여넣습니다.
3. 기능 49번이 성공하면 다음에는 기능 50번 프롬프트를 만들면 됩니다.
