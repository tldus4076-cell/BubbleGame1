# 기능 30 프롬프트: 버블 위치 정렬

아래 프롬프트를 그대로 복사해서 사용하세요.

```text
나는 초보자이고, Unity 6으로 2D 버블슈터 게임을 만들고 있어.

이번에는 기능 목록 30번인 "버블 위치 정렬"만 만들고 싶어.

참고 문서 위치:
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_Planning.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_StagePlan.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_FeatureList.md

작업 폴더:
- C:\Users\admin\Documents\BubbleGame

Unity 프로젝트 폴더:
- C:\Users\admin\Documents\BubbleGame\BubbleGame

현재 상태:
- 기능 1번~29번은 완료했어. (22번 모바일 터치는 패스)
- 기능 29번 "버블에 붙기" 테스트는 완료했어.
- BubbleLauncherController.cs가 있고 ShooterRoot에 붙어 있어.
- BubbleLauncherController.cs 안에 BubbleCollisionHandler가 있어.
- 발사된 버블은 벽에 반사되고, 천장이나 스테이지 버블 근처에서 멈춰.
- 발사된 버블이 멈추면 StageBubbleLayout 자식으로 들어가고 이름이 Bubble_로 시작하게 되어 있어.
- StageBubbleLayout.cs가 Stage 1 버블을 배치하고 있어.
- StageBubbleLayout.cs에는 GetBubbleDiameter(), GetBubbleSpacing(), TryGetPlayAreaWorldBounds() 같은 함수가 있어.
- WallsRoot, LeftWall, RightWall, Ceiling 벽 오브젝트가 준비되어 있어.
- StageBubbleLayout이 만든 스테이지 버블 이름은 Bubble_0_0 같은 형태야.
- 기존 배경, 타이머, 게이지, 점수, 슈터, 조준선, 현재 버블, 다음 버블, 바꾸기, 발사, 벽 반사, 천장에 붙기, 버블에 붙기 기능은 절대 망가뜨리지 말아줘.
- EventSystem은 삭제된 상태야. 이번 기능에는 필요 없으면 만들지 마.

목표:
발사된 버블이 다른 버블에 붙은 뒤, 버블슈터처럼 예쁜 격자 위치에 딱 맞게 정렬되게 하고 싶어.
버블이 기존 버블과 겹치거나, LeftWall/RightWall/Ceiling 밖으로 나가면 안 돼.

조건:
- Unity 6 2D 프로젝트 기준으로 설명해줘.
- 이번에는 기능 30번 "버블 위치 정렬"만 만들어줘.
- 같은 색 찾기, 같은 색 3개 제거, 점수 증가는 아직 만들지 마. 그건 다음 기능에서 할 거야.
- 발사된 버블이 멈출 때 반드시 격자 위치에 맞게 정렬해줘.
- 격자는 버블슈터처럼 벌집 모양 6방향 기준이면 좋아.
- 기존 Stage 1 시작 버블 배치와 같은 간격을 사용해줘.
- StageBubbleLayout.GetBubbleSpacing() 값을 기준으로 격자 간격을 계산해줘.
- 세로 간격은 bubbleSpacing * Mathf.Sqrt(3f) / 2f 방식으로 계산해줘.
- 붙을 위치 후보는 기존 버블 주변 6방향 빈칸으로 계산해줘.
- 이미 버블이 있는 위치에는 절대 붙지 않게 해줘.
- LeftWall, RightWall, Ceiling 안쪽 공간을 벗어나는 후보는 제외해줘.
- 조준선이 가리키는 방향과 최대한 가까운 빈칸에 붙게 해줘.
- 벽 반사 후에는 꺾인 조준선 경로와 가까운 빈칸에 붙게 해줘.
- 천장 쪽으로 쏜 경우에도 천장 위에 새 줄처럼 붙지 말고, 기존 스테이지 버블의 빈 격자 칸에 붙게 해줘.
- 단, 기능 28 "천장에 붙기" 자체가 완전히 망가지면 안 돼. 스테이지 버블이 없는 특수 상황에서는 천장에 붙을 수 있어도 돼.
- 아래쪽으로 길게 쌓이는 현상이 생기지 않게, 위쪽 줄부터 정렬되게 해줘.
- 기존 ShooterRoot 위치는 자동으로 바꾸지 마.
- 기존 벽 오브젝트 위치도 자동으로 바꾸지 마.
- 자동 세팅 메뉴는 만들지 마.
- 초보자도 이해할 수 있게 쉬운 말로 설명해줘.
- 어려운 영어 용어가 나오면 한국말 뜻도 같이 설명해줘.

코드 작성 조건:
- 가능하면 BubbleLauncherController.cs와 StageBubbleLayout.cs만 수정해줘.
- 새 스크립트가 꼭 필요하지 않으면 만들지 마.
- StageBubbleLayout.cs에는 격자 간격과 플레이 영역 안쪽 범위를 알려주는 함수가 이미 있는지 확인해줘.
- 없으면 필요한 함수만 최소로 추가해줘.
- BubbleLauncherController.cs에서는 발사 버블이 멈출 때 정렬 위치를 계산하는 함수를 정리해줘.
- 함수 이름은 초보자가 읽기 쉬운 이름으로 해줘. 예: FindBestEmptyNeighborPosition, IsBubblePositionOccupied, IsBubblePositionInsidePlayArea.
- [Header("한글 설명")]을 사용할 수 있는 새 Inspector 변수에는 한글 설명을 넣어줘.
- [Tooltip("초보자용 설명")]도 한글로 추가해줘.
- 변수 이름은 Unity/C# 규칙 때문에 영어로 유지해줘.
- 코드 주석은 한글로 작성해줘.
- 어려운 문법보다 쉬운 문법으로 작성해줘.
- 기존 기능을 크게 갈아엎지 말고, 필요한 부분만 최소 수정해줘.
- 기존 배경, 타이머, 게이지, 점수, 슈터 조준, 조준선, 현재/다음 버블 기능 파일은 가능하면 건드리지 마.

중요한 테스트 상황:
- 스테이지 버블 바로 아래 빈칸에 붙는지 확인.
- 왼쪽 끝을 조준하면 왼쪽 끝 가까운 빈칸에 붙는지 확인.
- 오른쪽 끝을 조준하면 오른쪽 끝 가까운 빈칸에 붙는지 확인.
- 천장 쪽으로 조준해도 천장 위로 붙지 않고 기존 스테이지의 빈 격자에 붙는지 확인.
- 왼쪽 벽 반사 조준선으로 쏘면 꺾인 경로 근처 빈칸에 붙는지 확인.
- 오른쪽 벽 반사 조준선으로 쏘면 꺾인 경로 근처 빈칸에 붙는지 확인.
- 이미 버블이 있는 위치에 겹쳐 붙지 않는지 확인.
- LeftWall, RightWall, Ceiling 밖으로 버블이 나가지 않는지 확인.
- 한 번 붙은 버블에도 다음 버블이 다시 붙을 수 있는지 확인.

자동저장 조건:
- 기능 하나가 완성되면 Unity에서 Scene(씬)을 저장하라고 알려줘.
- Ctrl + S로 저장하는 방법도 알려줘.

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
- 격자, 후보 위치, 빈칸 검사 같은 말을 쉽게 풀어서 설명해줘.
- 함수별 설명을 해줘.
- 이 코드가 왜 이렇게 동작하는지 초보자도 이해하게 설명해줘.
- 실행 순서를 번호로 설명해줘.
- 마지막에 내가 외워야 할 핵심만 짧게 뽑아줘.

주의:
- 이번에는 기능 30번만 만들어줘.
- 같은 색 찾기나 버블 제거는 만들지 마.
- 기존 발사 기능과 조준선 기능을 망가뜨리지 마.
- ShooterRoot, WallsRoot, LeftWall, RightWall, Ceiling 위치를 자동으로 바꾸지 마.
- 자동 세팅 메뉴는 만들지 마.
- EventSystem은 다시 만들지 마.
```

## 사용 방법

1. 위 `text` 박스 안의 내용을 전부 복사합니다.
2. 다음 작업 요청으로 그대로 붙여넣습니다.
3. 기능 30번이 성공하면 다음에는 기능 31번 프롬프트를 만들면 됩니다.
