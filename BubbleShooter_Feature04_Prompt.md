# 기능 4 프롬프트: 스테이지가 바뀔 때 배경 이미지 바꾸기

아래 프롬프트를 그대로 복사해서 사용하세요.

```text
나는 초보자이고, Unity 6으로 2D 버블슈터 게임을 만들고 있어.

이번에는 기능 목록 4번인 "스테이지가 바뀔 때 배경 이미지도 바뀌게 하기"만 만들고 싶어.

참고 문서 위치:
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_Planning.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_StagePlan.md
- C:\Users\admin\Documents\BubbleGame\BubbleShooter_FeatureList.md

작업 폴더:
- C:\Users\admin\Documents\BubbleGame

Unity 프로젝트 폴더:
- C:\Users\admin\Documents\BubbleGame\BubbleGame

앞으로 만드는 코드와 프롬프트는 가능하면 C:\Users\admin\Documents\BubbleGame 폴더 안에 저장해줘.

현재 상태:
- 기능 1번 "Stage 1 배경 이미지 넣기"는 이미 만들었어.
- 기능 2번 "Stage 2 배경 이미지 넣기"도 이미 만들었어.
- 기능 3번 "Stage 3 배경 이미지 넣기"도 이미 만들었어.
- StageBackgroundController.cs에 Stage 1, Stage 2, Stage 3 배경 Sprite 변수가 있을 수 있어.
- 기존 Stage 1 배경 표시 기능과 Stage 2/Stage 3 배경 연결 기능은 절대 망가뜨리지 말아줘.

목표:
Stage 번호를 바꾸면 배경 이미지도 Stage 1, Stage 2, Stage 3에 맞게 바뀌게 만들고 싶어.

조건:
- Unity 6 2D 프로젝트 기준으로 설명해줘.
- 배경 이미지는 Sprite로 사용할 예정이야.
- Stage 1, Stage 2, Stage 3 배경 Sprite를 각각 사용할 수 있어야 해.
- Stage 번호가 1이면 Stage 1 배경이 보여야 해.
- Stage 번호가 2이면 Stage 2 배경이 보여야 해.
- Stage 번호가 3이면 Stage 3 배경이 보여야 해.
- 배경은 게임 화면 전체에 꽉 차게 보여야 해.
- 배경은 슈터, 버블, UI보다 뒤에 있어야 해.
- 배경은 카메라가 보는 화면 뒤에 항상 고정되어 보여야 해.
- 아직 실제 클리어 후 다음 스테이지 이동 기능은 만들지 마. 그건 나중에 기능 67번에서 만들 예정이야.
- 이번에는 Inspector에서 현재 Stage 번호를 바꾸면 배경이 바뀌는 테스트용 방식으로 만들어줘.
- 나중에 GameManager나 StageManager가 생기면 코드에서 SetStageBackground(스테이지 번호) 같은 함수로 배경을 바꿀 수 있게 준비해줘.
- 초보자도 이해할 수 있게 쉬운 말로 설명해줘.
- 어려운 영어 용어가 나오면 한국말 뜻도 같이 설명해줘.
- 전체 코드를 빠짐없이 작성해줘.
- 코드마다 주석을 초보자도 알아보기 쉽게 달아줘.
- 어느 GameObject(게임 오브젝트)에 스크립트를 붙이는지 알려줘.
- Inspector(인스펙터)에서 뭘 설정해야 하는지 알려줘.
- 자주 틀리는 부분도 알려줘.
- 너무 복잡한 방식보다 쉬운 방식으로 알려줘.
- 기존 기능을 망가뜨리지 않는 방향으로 설명해줘.

코드 작성 조건:
- 기존 StageBackgroundController.cs를 수정하는 방식으로 해줘.
- 기존 Stage 1, Stage 2, Stage 3 배경 Sprite 변수는 유지해줘.
- 현재 선택된 스테이지 번호를 저장하는 int currentStageNumber 변수를 추가해줘.
- currentStageNumber는 Inspector에서 조절할 수 있게 해줘.
- currentStageNumber 값은 1~3까지만 사용하게 해줘.
- SetStageBackground(int stageNumber) 함수를 만들어줘.
- Stage 번호가 잘못 들어오면 Stage 1 배경으로 안전하게 돌아가거나 경고를 보여줘.
- Stage 2나 Stage 3 Sprite가 비어 있으면 오류로 멈추지 말고 경고를 보여주고 Stage 1 배경을 유지해줘.
- 배경이 바뀐 뒤에도 화면 크기에 꽉 차게 다시 맞춰줘.
- [Header("한글 설명")]을 사용해줘.
- 필요하면 [Tooltip("초보자용 설명")]도 한글로 추가해줘.
- 변수 이름은 Unity/C# 규칙 때문에 영어로 유지해줘.
- 인스펙터에 보이는 설명은 초보자가 이해하기 쉽게 한글로 작성해줘.
- 코드 주석도 계속 한글로 작성해줘.
- 어려운 문법보다 쉬운 문법으로 작성해줘.
- 이 코드를 함수별로 나눠줘.
- 이 코드를 주석 많이 넣어서 다시 써줘.
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
- 가능하면 Project Settings(프로젝트 설정)나 Scene(씬) 저장이 필요한 부분을 체크리스트에 넣어줘.

이번 기능에서 원하는 쉬운 구현 방향:
- StageBackgroundController.cs 안에서 currentStageNumber 값을 보고 배경을 선택하게 해줘.
- Inspector에서 currentStageNumber를 1, 2, 3으로 바꿔서 테스트할 수 있게 해줘.
- OnValidate를 사용해서 Inspector에서 숫자를 바꾸면 Scene 창에서도 바로 배경이 바뀌게 해줘.
- Play 모드에서도 currentStageNumber에 맞는 배경이 보여야 해.
- 나중에 다른 스크립트에서 SetStageBackground(2), SetStageBackground(3)처럼 호출할 수 있게 public 함수로 만들어줘.
- 자동 세팅 스크립트가 있다면 Assets/Image/1.png, Assets/Image/2.png, Assets/Image/3.png를 각각 자동 연결할 수 있게 유지해줘.
- 단, 2.png나 3.png가 아직 없어도 오류가 나지 않게 해줘.

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
- 이번에는 기능 4번만 만들어줘.
- 실제 클리어 후 다음 스테이지로 이동하는 기능은 만들지 마.
- Stage 선택 UI도 아직 만들지 마.
- 지금은 Inspector에서 Stage 번호를 바꿔 테스트하는 방식이면 충분해.
- 아직 슈터, 버블, UI 기능은 없을 수 있으니 배경 기능만 독립적으로 유지해줘.
```

## 사용 방법

1. 위 `text` 박스 안의 내용을 전부 복사합니다.
2. 다음 작업 요청으로 그대로 붙여넣습니다.
3. 답변을 받은 뒤 Unity 6 프로젝트에 적용합니다.
4. 기능 4번이 성공하면 다음에는 기능 5번 프롬프트를 만들면 됩니다.
