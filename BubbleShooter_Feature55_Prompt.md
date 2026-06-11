# 기능 55 프롬프트: Stage 2 난이도 - 빈칸을 조금 넣기

아래 프롬프트를 그대로 복사해서 사용하세요.

```text
나는 초보자이고, Unity 6으로 2D 버블슈터 게임을 만들고 있어.

이번에는 기능 목록 55번인 "Stage 2 난이도 - 빈칸을 조금 넣기"만 만들고 싶어.

참고 문서 위치:
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_Planning.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_StagePlan.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_FeatureList.md

작업 폴더:
- C:\Users\admin\Documents\BubbleGame

Unity 프로젝트 폴더:
- C:\Users\admin\Documents\BubbleGame\BubbleGame

현재 상태:
- 기능 1번~54번은 완료했어. (22번 모바일 터치는 패스)
- 기능 54번 "Stage 2 난이도 - 제한 샷 수를 Stage 1보다 줄이기" 테스트 완료했어.
- StageDataController.cs가 이미 있어서 Inspector에서 startBubblePattern을 저장할 수 있어.
- 기능 44에서 startBubblePattern 변수와 GetStartBubblePattern() 함수를 만들었어.
- 기능 53에서 StageBubbleLayout.cs가 StageDataController의 startBubblePattern을 읽어서 배치에 적용하도록 수정했어.
- Stage 2는 Stage 1보다 조금 복잡한 배치가 필요해.
- 아직 Stage 2에서 빈칸이 포함된 배치가 적용되어 있지 않어.
- 기존 배경, 타이머, 게이지, 점수, 슈터, 조준선, 현재 버블, 다음 버블, 발사, 격자, 같은 색 찾기, 버블 제거, 점수 증가, 제거 효과, 천장 연결 찾기, 떠 있는 버블 찾기, 떠 있는 버블 떨어뜨리기, 떨어진 버블 점수, 스테이지 이름, 배경 이미지, 색 종류, 배치, 제한 샷 수, 제한 시간, 장애물, Stage 1 난이도, Stage 2 복잡 배치, Stage 2 제한 샷 수 기능은 절대 망가뜨리지 말아줘.
- EventSystem은 삭제된 상태야. 이번 기능에는 필요 없으면 만들지 마.

목표:
Stage 2의 시작 배치에 빈칸을 조금 넣고 싶어.
버블 사이에 빈 공간이 있어서 조준이 필요한 배치를 만들고 싶어.

조건:
- Unity 6 2D 프로젝트 기준으로 설명해줘.
- 이번에는 기능 55번 "Stage 2 난이도 - 빈칸을 조금 넣기"만 만들어줘.
- StageDataController.cs를 수정하지 마. (이미 startBubblePattern이 있음)
- StageBubbleLayout.cs를 수정하지 마.
- 기존 코드는 건드리지 마.
- Stage 2 씬의 Inspector 설정만 확인하고 안내해줘.
- StageDataController의 Inspector에서:
  - Start Bubble Pattern에 빈칸이 포함된 패턴 입력
- 빈칸은 -1로 표시합니다. (예: -1=빈칸, 0=빨강, 1=파랑, 2=노랑, 3=초록)
- StageBubbleLayout에서 -1이면 버블을 만들지 않고 건너뜁니다.
- 추천 Stage 2 배치 패턴 (빈칸 포함):
  - 1번째 줄 6개: 0, 1, -1, 3, 1, 0 (가운데 빈칸)
  - 2번째 줄 5개: 3, 2, 1, 0, 3
  - 3번째 줄 5개: 0, 3, -1, 1, 0 (가운데 빈칸)
  - 4번째 줄 4개: 1, 2, 3, 1
  - 5번째 줄 4개: 2, 0, 3, 2
- 이 패턴은 버블 사이에 빈 공간이 있어서 조준이 필요합니다.
- 초보자도 이해할 수 있게 쉬운 말로 설명해줘.
- 어려운 영어 용어가 나오면 한국말 뜻도 같이 설명해줘.

코드 작성 조건:
- 새 스크립트를 만들지 마.
- 기존 스크립트를 수정하지 마.
- Inspector 설정 방법만 안내해줘.

중요한 테스트 상황:
- Inspector에서 Start Bubble Pattern에 빈칸(-1)이 포함된 패턴을 입력.
- Play하면 빈칸이 있는 배치가 나오는지 확인.
- 빨강/파랑/노랑/초록 4색이 모두 보이는지 확인.
- 버블 사이에 빈 공간이 있는지 확인.
- 기존 발사/정렬/같은 색 찾기/버블 제거/점수 증가/제거 효과/천장 연결 찾기/떠 있는 버블 찾기/떠 있는 버블 떨어뜨리기/떨어진 버블 점수/스테이지 이름/배경 이미지/색 종류/배치/제한 샷 수/제한 시간/장애물/Stage 1 난이도/Stage 2 복잡 배치/Stage 2 제한 샷 수 기능이 정상 작동하는지 확인.

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
- 왜 Stage 2에서 빈칸이 필요한지 알려줘.
- 빈칸이 있으면 왜 조준이 필요한지 알려줘.
- Inspector에서 int 배열을 입력하는 것이 왜 편한지 쉽게 설명해줘.
- 함수별 설명을 해줘.
- 이 코드가 왜 이렇게 동작하는지 초보자도 이해하게 설명해줘.
- 실행 순서를 번호로 설명해줘.
- 변수, 함수, if문이 각각 무슨 역할인지 알려줘.
- 마지막에 내가 외워야 할 핵심만 짧게 뽑아줘.

주의:
- 이번에는 기능 55번만 만들어줘.
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
3. 기능 55번이 성공하면 다음에는 기능 56번 프롬프트를 만들면 됩니다.
