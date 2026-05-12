# 기능 11 프롬프트: 배경 이미지와 어울리는 위치에 점수 배치하기

아래 프롬프트를 그대로 복사해서 사용하세요.

```text
나는 초보자이고, Unity 6으로 2D 버블슈터 게임을 만들고 있어.

이번에는 기능 목록 11번인 "배경 이미지와 어울리는 위치에 점수 배치하기"만 만들고 싶어.

참고 문서 위치:
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_Planning.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_StagePlan.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_FeatureList.md

작업 폴더:
- C:\Users\admin\Documents\BubbleGame

Unity 프로젝트 폴더:
- C:\Users\admin\Documents\BubbleGame\BubbleGame

현재 상태:
- 기능 1번~4번 배경 기능은 완료했어.
- 기능 5번~8번 타이머 숫자/게이지 기능은 완료했어.
- 기능 9번 점수 숫자 넣기는 완료했어.
- 기능 10번 점수 증가 테스트도 완료했어.
- T 키를 누르면 점수가 30씩 올라가는 것을 확인했어.
- ScoreController.cs가 있고 ScoreText에 점수를 표시하고 있어.
- BubbleScoreManager.cs가 있고 AddBubbleScore(int removedBubbleCount)로 점수를 올릴 수 있어.
- EventSystem은 삭제했지만, 이번 기능에는 필요 없어. 다시 만들지 마.
- 기존 배경, 타이머, 게이지, 점수 증가 기능은 절대 망가뜨리지 말아줘.

목표:
점수 숫자가 배경에 묻히지 않고 잘 보이게 위치와 스타일을 정리하고 싶어.
특히 배경 이미지에 이미 SCORE 표시 칸이 있으니, 그 칸 근처에 점수 숫자를 내가 직접 배치할 수 있게 하고 싶어.

조건:
- Unity 6 2D 프로젝트 기준으로 설명해줘.
- 이번에는 기능 11번 "점수 숫자 위치 배치"만 만들어줘.
- 점수 증가 로직은 건드리지 마.
- T 키 테스트 점수 증가 기능은 유지해줘.
- ScoreText 위치는 내가 직접 Scene 창에서 조절할 수 있어야 해.
- Play를 눌러도 ScoreText 위치가 자동으로 중앙이나 기본 위치로 돌아가면 안 돼.
- 기존 ScoreController.cs의 점수 표시 기능은 유지해줘.
- 기존 BubbleScoreManager.cs의 점수 증가 기능도 유지해줘.
- 기존 StageBackgroundController.cs, TimerController.cs, TimerGaugeController.cs는 건드리지 마.
- TextMeshPro 오류가 있었으므로 Unity 기본 UI Text를 계속 사용해줘.
- ScoreText는 배경보다 앞에 보여야 해.
- ScoreText는 밝은 배경에서도 잘 보이게 Shadow(그림자) 또는 Outline(외곽선)을 사용할 수 있게 해줘.
- 점수 숫자의 색, 글자 크기, 그림자 색, 그림자 거리 등을 Inspector에서 조절할 수 있게 해줘.
- 점수 UI 위치와 크기는 자동으로 계속 고정하지 마. 처음 만들 때만 기본 위치를 잡고 이후에는 사용자가 직접 옮긴 위치를 유지해줘.
- 초보자도 이해할 수 있게 쉬운 말로 설명해줘.
- 어려운 영어 용어가 나오면 한국말 뜻도 같이 설명해줘.
- 전체 코드를 빠짐없이 작성해줘.
- 코드마다 주석을 초보자도 알아보기 쉽게 달아줘.
- 어느 GameObject(게임 오브젝트)에 스크립트를 붙이는지 알려줘.
- Inspector(인스펙터)에서 뭘 설정해야 하는지 알려줘.
- 자주 틀리는 부분도 알려줘.

UI 조건:
- ScoreText에 스타일을 적용해줘.
- ScoreText의 위치와 크기는 자동으로 바꾸지 마.
- ScoreText 글자 색은 기본 흰색으로 해줘.
- ScoreText에 Shadow를 붙여줘.
- 원하면 Outline도 사용할 수 있게 해줘.
- ScoreText의 Font Size는 Inspector에서 조절할 수 있게 해줘.
- ScoreText의 Text 값은 ScoreController가 계속 관리하게 해줘.
- 스타일 스크립트가 Text 값을 0으로 계속 덮어쓰면 안 돼.

코드 작성 조건:
- ScoreUIStyleController.cs 같은 새 스크립트를 만들어줘.
- ScoreUIStyleController.cs는 ScoreText 오브젝트에 붙이는 방식으로 해줘.
- 이 스크립트는 Text 색, Font Size, Shadow, Outline만 관리하게 해줘.
- 이 스크립트는 RectTransform 위치와 크기를 자동으로 변경하지 말아줘.
- 이 스크립트는 점수 값 text를 직접 바꾸지 말아줘. 점수 숫자 변경은 ScoreController가 담당해야 해.
- ScoreController.cs의 점수 표시 로직은 가능하면 건드리지 마.
- BubbleScoreManager.cs도 건드리지 마.
- 자동 세팅 메뉴가 있다면 Bubble Shooter > Setup Score Text를 다시 눌렀을 때 ScoreText에 ScoreUIStyleController도 붙게 해줘.
- 단, Setup Score Text를 다시 눌러도 기존 ScoreText 위치와 크기는 유지되게 해줘.
- EventSystem은 만들지 마.
- [Header("한글 설명")]을 사용해줘.
- 필요하면 [Tooltip("초보자용 설명")]도 한글로 추가해줘.
- 변수 이름은 Unity/C# 규칙 때문에 영어로 유지해줘.
- 인스펙터에 보이는 설명은 초보자가 이해하기 쉽게 한글로 작성해줘.
- 코드 주석도 계속 한글로 작성해줘.
- 어려운 문법보다 쉬운 문법으로 작성해줘.
- 이 코드를 함수별로 나눠줘.
- 이 코드가 왜 필요한지 설명해줘.
- 이 함수는 언제 호출되는지 설명해줘.
- 이 변수가 어떤 역할인지 설명해줘.
- 이 코드가 실행되는 흐름을 순서대로 알려줘.
- 초보자가 실수하기 쉬운 부분도 알려줘.
- 실행 순서를 번호로 설명해줘.
- 이 코드에서 변수, 함수, if문이 각각 무슨 역할인지 알려줘.
- 내가 외워야 할 핵심만 뽑아줘.

자동저장 조건:
- 기능 하나가 완성되면 Unity에서 Scene(씬)을 저장하라고 알려줘.
- Ctrl + S로 저장하는 방법도 알려줘.
- 가능하면 Scene 저장이 필요한 부분을 체크리스트에 넣어줘.

이번 기능에서 원하는 쉬운 구현 방향:
- ScoreUIStyleController.cs를 새로 만들어줘.
- ScoreText 오브젝트에 붙여줘.
- Text 컴포넌트를 자동으로 찾아서 글자 색과 크기를 적용하게 해줘.
- Shadow 컴포넌트를 자동으로 붙이고 색과 거리를 조절하게 해줘.
- Outline 컴포넌트도 선택적으로 사용할 수 있게 해줘.
- Inspector에서 useShadow, useOutline을 켜고 끌 수 있게 해줘.
- 기본 글자 색은 흰색으로 해줘.
- 기본 Shadow 색은 검은색 반투명으로 해줘.
- 기본 Outline 색은 어두운 갈색 또는 검은색으로 해줘.
- ScoreText 위치와 크기는 절대 자동으로 바꾸지 말아줘.
- Unity 메뉴 Bubble Shooter > Setup Score Text를 다시 누르면 ScoreUIStyleController도 붙게 해줘.

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

설명 스타일:
- 초등학생도 이해할 수 있게 차근차근 설명해줘.
- 코드만 알려주지 말고 코드 설명도 해줘.
- 한 줄씩 설명하고 설치 순서도 알려줘.
- 오브젝트 세팅 순서도 알려줘.
- 오류날 수 있는 부분도 알려줘.
- 함수별 설명을 해줘.
- 이 코드가 왜 이렇게 동작하는지 초보자도 이해하게 설명해줘.
- 실행 순서를 번호로 설명해줘.
- 변수, 함수, if문이 각각 무슨 역할인지 알려줘.
- 그림처럼 비유해서 설명해줘.
- 마지막에 내가 외워야 할 핵심만 짧게 뽑아줘.

주의:
- 이번에는 기능 11번만 만들어줘.
- 점수 증가 로직은 변경하지 마.
- ScoreText 위치와 크기는 자동으로 변경하지 마.
- 기존 배경, 타이머, 게이지 코드는 건드리지 마.
- EventSystem은 다시 만들지 마.
```

## 사용 방법

1. 위 `text` 박스 안의 내용을 전부 복사합니다.
2. 다음 작업 요청으로 그대로 붙여넣습니다.
3. 기능 11번이 성공하면 다음에는 기능 12번 프롬프트를 만들면 됩니다.
