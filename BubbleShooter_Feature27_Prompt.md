# 기능 27 프롬프트: 벽 반사 후 속도 유지하기

아래 프롬프트를 그대로 복사해서 사용하세요.

```text
나는 초보자이고, Unity 6으로 2D 버블슈터 게임을 만들고 있어.

이번에는 기능 목록 27번인 "벽 반사 후 속도 유지"만 만들고 싶어.

참고 문서 위치:
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_Planning.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_StagePlan.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_FeatureList.md

작업 폴더:
- C:\Users\admin\Documents\BubbleGame

Unity 프로젝트 폴더:
- C:\Users\admin\Documents\BubbleGame\BubbleGame

현재 상태:
- 기능 1번~26번은 완료했어. (22번 모바일 터치는 패스)
- BubbleLauncherController.cs가 있고 ShooterRoot에 붙어 있어.
- 마우스 클릭으로 버블을 발사할 수 있어.
- 발사된 버블은 왼쪽/오른쪽 벽에 반사되고, 천장이나 스테이지 버블에 닿으면 멈춰.
- 한 번에 하나만 발사돼.
- WallsRoot, LeftWall, RightWall, Ceiling 벽 오브젝트가 준비되어 있고 BoxCollider2D가 붙어 있어.
- 기존 배경, 타이머, 게이지, 점수, 슈터, 조준선, 현재 버블, 다음 버블, 바꾸기, 발사 기능은 절대 망가뜨리지 말아줘.
- EventSystem은 삭제된 상태야.

목표:
버블이 벽에 부딪힌 뒤에도 속도가 유지되어야 해.
벽에 닿아서 느려지거나 멈추면 안 돼.

조건:
- Unity 6 2D 프로젝트 기준으로 설명해줘.
- 이번에는 기능 27번 "벽 반사 후 속도 유지"만 만들어줘.
- BubbleLauncherController.cs에 이미 PhysicsMaterial2D와 Rigidbody2D 설정이 있는지 확인해줘.
- 이미 구현되어 있으면 확인만 해줘.
- 구현되어 있지 않으면 설정을 수정해줘.
- PhysicsMaterial2D의 friction이 0인지 확인해줘.
- Rigidbody2D의 linearDamping이 0인지 확인해줘.
- 기존 발사/조준/조준선/배경/타이머/점수 기능은 유지해줘.
- ShooterRoot 위치는 자동으로 바꾸지 마.
- 초보자도 이해할 수 있게 쉬운 말로 설명해줘.

코드 작성 조건:
- BubbleLauncherController.cs의 속도 유지 관련 설정만 확인/수정해줘.
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
- 이번에는 기능 27번만 만들어줘.
- 기존 발사 기능을 망가뜨리지 마.
- EventSystem은 다시 만들지 마.
```

## 사용 방법

1. 위 `text` 박스 안의 내용을 전부 복사합니다.
2. 다음 작업 요청으로 그대로 붙여넣습니다.
3. 기능 27번이 성공하면 다음에는 기능 28번 프롬프트를 만들면 됩니다.
