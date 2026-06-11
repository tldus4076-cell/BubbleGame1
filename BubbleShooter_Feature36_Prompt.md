# 기능 36 프롬프트: 버블 제거 효과 넣기

아래 프롬프트를 그대로 복사해서 사용하세요.

```text
나는 초보자이고, Unity 6으로 2D 버블슈터 게임을 만들고 있어.

이번에는 기능 목록 36번인 "버블 제거 효과"만 만들고 싶어.

참고 문서 위치:
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_Planning.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_StagePlan.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_FeatureList.md

작업 폴더:
- C:\Users\admin\Documents\BubbleGame

Unity 프로젝트 폴더:
- C:\Users\admin\Documents\BubbleGame\BubbleGame

현재 상태:
- 기능 1번~35번은 완료했어. (22번 모바일 터치는 패스)
- 기능 35번 "버블 제거 시 점수 증가 + 증가 규칙" 테스트는 완료했어.
- 현재 발사 시스템은 Grid 기반으로 동작해:
  - ShooterController.cs가 발사를 담당해.
  - BubbleGridManager.cs가 격자 칸(BubbleSlot) 관리, 버블 등록, 같은 색 찾기, 매칭 규칙 확인, 버블 제거를 담당해.
  - BubbleGridManager.cs의 RemoveMatchedBubbles() 함수에서 같은 색 버블 + 옆 버블을 ClearBubbleSlot()으로 제거해.
  - ClearBubbleSlot()은 slot.occupied = false, slot.bubbleObject = null, Destroy(bubbleObject)를 실행해.
- 아직 버블이 제거될 때 시각적 효과(터지는 효과)가 없어.
- 점수는 이미 기능 35에서 정상 작동해.
- 기존 배경, 타이머, 게이지, 점수, 슈터, 조준선, 현재 버블, 다음 버블, 발사, 격자, 같은 색 찾기, 버블 제거, 점수 증가 기능은 절대 망가뜨리지 말아줘.
- EventSystem은 삭제된 상태야. 이번 기능에는 필요 없으면 만들지 마.

목표:
버블이 제거될 때 터지는 효과나 시각적 효과를 보이고 싶어.
플레이어가 "버블이 방금 사라졌구나"라고 한눈에 알 수 있게 해줘.

조건:
- Unity 6 2D 프로젝트 기준으로 설명해줘.
- 이번에는 기능 36번 "버블 제거 효과"만 만들어줘.
- 점수 증가는 이미 기능 35에서 했어. 이번에 점수 로직은 건드리지 마.
- 연결 끊긴 버블 찾기/떨어뜨리기는 아직 만들지 마. 그건 기능 37번~40번에서 할 거야.
- 효과는 가벼운 것으로 시작해줘. 예:
  - 옵션 A: 버블이 살짝 커졌다가 작아지면서 사라짐 (스케일 애니메이션)
  - 옵션 B: SpriteRenderer를 잠깐 숨겼다가 Destroy
  - 옵션 C: 색이 살짝 밝아졌다가 사라짐
- 파티클 시스템은 초보자에게 어려우니 이번 기능에서는 사용하지 마.
- 가능하면 새 스크립트 1개(BubbleRemovalEffectController.cs)를 만들어줘.
- BubbleGridManager.cs는 ClearBubbleSlot() 호출 직전에 효과를 발생시키도록 약간만 수정해줘.
- 기존 ShooterRoot 위치는 자동으로 바꾸지 마.
- 기존 벽 오브젝트 위치도 자동으로 바꾸지 마.
- 자동 세팅 메뉴는 만들지 마.
- 초보자도 이해할 수 있게 쉬운 말로 설명해줘.
- 어려운 영어 용어가 나오면 한국말 뜻도 같이 설명해줘.

코드 작성 조건:
- 새 스크립트 BubbleRemovalEffectController.cs를 만들어줘.
- 이 스크립트는 ShooterRoot 또는 같은 위치의 오브젝트에 붙여줘.
- [SerializeField] private BubbleGridManager gridManager; 로 GridManager를 연결해줘.
- BubbleGridManager.cs의 RemoveMatchedBubbles() 안에서, ClearBubbleSlot()을 호출하기 전에 효과 함수를 호출해줘.
- 효과 함수는 새 스크립트에 만들어줘. 예: PlayRemovalEffect(Vector3 worldPosition, Color bubbleColor)
- 효과는 버블이 있던 월드 위치에서 보여야 해.
- 효과는 0.3~0.5초 정도 보이고 나서 사라져야 해.
- 효과는 SpriteRenderer로 간단한 동그라미 Sprite를 만들어서 보여줘.
- 임시 동그라미 Sprite는 코드로 생성할 수 있어. 기존 CreateTemporaryShooterSprite() 함수를 참고해줘.
- [Header("한글 설명")]을 사용할 수 있는 새 Inspector 변수에는 한글 설명을 넣어줘.
- [Tooltip("초보자용 설명")]도 한글로 추가해줘.
- 변수 이름은 Unity/C# 규칙 때문에 영어로 유지해줘.
- 코드 주석은 한글로 많이 작성해줘.
- 어려운 문법보다 쉬운 문법으로 작성해줘.
- 기존 기능을 크게 갈아엎지 말고, 필요한 부분만 최소 수정해줘.
- 기존 배경, 타이머, 게이지, 슈터 조준, 조준선, 현재/다음 버블 기능 파일은 가능하면 건드리지 마.
- GameObject.Find() 사용 금지.
- 하드코딩된 Tag 사용 금지.
- 외부 객체 참조는 반드시 [SerializeField] 또는 Interface 사용.
- Coroutine을 사용해서 효과 애니메이션을 구현해줘.

C# 이벤트 조건:
- 효과를 발동시킬 때 이벤트를 사용해도 되고, 직접 함수를 호출해도 돼.
- 이번에는 직접 함수 호출이 더 쉬우니까 직접 호출로 해줘.
- 점수 기능과 마찬가지로 이벤트를 쓸 필요는 없어.

중요한 테스트 상황:
- 같은 색 3개를 제거하면 3개 모두에서 효과가 보여야 해.
- 같은 색 4개를 제거하면 4개 모두에서 효과가 보여야 해.
- 같은 색 5개 이상을 제거하면 모두에서 효과가 보여야 해.
- 옆에 붙은 다른 색 버블도 같이 제거될 때 그 버블에서도 효과가 보여야 해.
- 효과는 0.5초 이내에 사라져야 해.
- 효과 후에 버블이 실제로 사라져야 해.
- 점수는 이번 기능에서 안 올라가도 돼. (이미 기능 35에서 처리됨)
- 기존 발사/정렬/같은 색 찾기/버블 제거/점수 증가 기능이 정상 작동하는지 확인.

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
- 왜 효과가 필요한지 쉽게 설명해줘.
- Coroutine이란 무엇인지 쉽게 설명해줘.
- 효과 함수가 언제 호출되는지 알려줘.
- 함수별 설명을 해줘.
- 이 코드가 왜 이렇게 동작하는지 초보자도 이해하게 설명해줘.
- 실행 순서를 번호로 설명해줘.
- 변수, 함수, if문이 각각 무슨 역할인지 알려줘.
- 마지막에 내가 외워야 할 핵심만 짧게 뽑아줘.

주의:
- 이번에는 기능 36번만 만들어줘.
- 파티클 시스템은 사용하지 마.
- 점수 로직은 건드리지 마.
- 연결 끊긴 버블 떨어뜨리기는 만들지 마.
- 기존 발사 기능과 정렬 기능을 망가뜨리지 마.
- ShooterRoot, WallsRoot, LeftWall, RightWall, Ceiling 위치를 자동으로 바꾸지 마.
- 자동 세팅 메뉴는 만들지 마.
- EventSystem은 다시 만들지 마.
- BubbleLauncherController.cs, BubbleSwapController.cs, BubbleCurrentController.cs는 새로 만들지 마.
- 새 스크립트를 만들 때 BubbleRemovalEffectController.cs로 만들어줘.
```

## 사용 방법

1. 위 `text` 박스 안의 내용을 전부 복사합니다.
2. 다음 작업 요청으로 그대로 붙여넣습니다.
3. 기능 36번이 성공하면 다음에는 기능 37번 프롬프트를 만들면 됩니다.
