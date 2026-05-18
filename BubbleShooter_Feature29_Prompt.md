# 기능 29 프롬프트: 버블에 붙기

아래 프롬프트를 그대로 복사해서 사용하세요.

```text
나는 초보자이고, Unity 6으로 2D 버블슈터 게임을 만들고 있어.

이번에는 기능 목록 29번인 "버블에 붙기"만 만들고 싶어.

참고 문서 위치:
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_Planning.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_StagePlan.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_FeatureList.md

작업 폴더:
- C:\Users\admin\Documents\BubbleGame

Unity 프로젝트 폴더:
- C:\Users\admin\Documents\BubbleGame\BubbleGame

현재 상태:
- 기능 1번~28번은 완료했어. (22번 모바일 터치는 패스)
- BubbleLauncherController.cs가 있고 ShooterRoot에 붙어 있어.
- 마우스 클릭으로 버블을 발사할 수 있어.
- 발사된 버블은 벽에 반사되고, 천장에 닿으면 멈춰.
- 한 번에 하나만 발사돼.
- WallsRoot, LeftWall, RightWall, Ceiling 벽 오브젝트가 준비되어 있어.
- StageBubbleLayout이 만든 스테이지 버블이 화면 위에 배치되어 있어.
- 기존 배경, 타이머, 게이지, 점수, 슈터, 조준선, 현재 버블, 다음 버블, 바꾸기, 발사 기능은 절대 망가뜨리지 말아줘.
- EventSystem은 삭제된 상태야.

목표:
발사된 버블이 스테이지 버블에 닿으면 그 자리에 멈추고 붙게 하고 싶어.
버블끼리 닿으면 새 버블이 붙어야 해.

조건:
- Unity 6 2D 프로젝트 기준으로 설명해줘.
- 이번에는 기능 29번 "버블에 붙기"만 만들어줘.
- BubbleLauncherController.cs의 BubbleCollisionHandler에 이미 스테이지 버블 충돌 처리가 있는지 확인해줘.
- 이미 구현되어 있으면 확인만 해줘.
- 구현되어 있지 않으면 스테이지 버블 충돌 처리를 구현해줘.
- 스테이지 버블에 닿으면 Rigidbody2D를 Static으로 바꿔서 고정시켜줘.
- 스테이지 버블의 이름은 "Bubble_"으로 시작합니다.
- 기존 발사/조준/조준선/배경/타이머/점수 기능은 유지해줘.
- ShooterRoot 위치는 자동으로 바꾸지 마.
- 초보자도 이해할 수 있게 쉬운 말로 설명해줘.

코드 작성 조건:
- BubbleLauncherController.cs의 버블 충돌 관련 설정만 확인/수정해줘.
- 새 스크립트를 만들 필요가 없으면 만들지 마.
- Inspector에서 뭘 설정해야 하는지 알려줘.

자동저장 조건:
- 기능 하나가 완성되면 Unity에서 Scene(씬)을 저장하라고 알려줘.
- Ctrl + S로 저장하는 방법도 알려줘.

반드시 아래 형식으로 답해줘:

1. 기능 설명
2. 전체 코드 (수정된 부분만 또는 확인 완료)
3. 코드 설명
4. 유니티 적용 방법
5. 오류 체크 포인트
6. Inspector에서 조절할 변수
7. 테스트 성공 기준
8. 다음 기능으로 넘어가기 전 체크리스트

주의:
- 이번에는 기능 29번만 만들어줘.
- 기존 발사 기능을 망가뜨리지 마.
- EventSystem은 다시 만들지 마.
```

## 사용 방법

1. 위 `text` 박스 안의 내용을 전부 복사합니다.
2. 다음 작업 요청으로 그대로 붙여넣습니다.
3. 기능 29번이 성공하면 다음에는 기능 30번 프롬프트를 만들면 됩니다.
