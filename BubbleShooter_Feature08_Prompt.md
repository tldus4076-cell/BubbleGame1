# 기능 8 프롬프트: 배경 색과 겹치지 않게 타이머 게이지 색 정하기

아래 프롬프트를 그대로 복사해서 사용하세요.

```text
나는 초보자이고, Unity 6으로 2D 버블슈터 게임을 만들고 있어.

이번에는 기능 목록 8번인 "배경 색과 겹치지 않게 타이머 게이지 색 정하기"만 만들고 싶어.

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
- 기능 1번~4번 배경 기능은 완료했어.
- 기능 5번 타이머 숫자 기능도 완료했어.
- 기능 6번 타이머 숫자 위치 배치도 완료했어.
- 기능 7번 타이머 게이지 기능도 완료했어.
- TimerController.cs가 있고 타이머가 4:00, 3:59처럼 분:초 형식으로 줄어들어.
- TimerGaugeController.cs가 있고 게이지가 시간에 맞춰 줄어들어.
- 지금은 게이지 밖 배경 막대는 사용하지 않고, 노란색 TimerGaugeFill만 사용할 예정이야.
- TimerGaugeBackground는 삭제했거나 사용하지 않을 수 있어.
- EventSystem은 이미 삭제했어. 지금 기능에는 필요 없지만, 나중에 버튼 기능에서 필요하면 다시 만들어줘.
- 기존 타이머 숫자, 게이지 감소 기능, 배경 기능은 절대 망가뜨리지 말아줘.

목표:
타이머 게이지 색이 배경 이미지와 겹쳐서 안 보이지 않게 하고 싶어.
특히 Stage 1 숲 배경, Stage 2 바다 배경, Stage 3 동굴 배경에서도 게이지가 잘 보여야 해.

조건:
- Unity 6 2D 프로젝트 기준으로 설명해줘.
- 이번에는 기능 8번 "게이지 색 정하기"만 만들어줘.
- 타이머가 줄어드는 로직은 건드리지 마.
- TimerText 위치는 자동으로 바꾸지 마.
- 기존 StageBackgroundController.cs는 가능하면 건드리지 마.
- 기존 TimerController.cs의 시간 감소 로직도 건드리지 마.
- 기존 TimerGaugeController.cs의 fillAmount 감소 로직도 유지해줘.
- 게이지는 노란색/주황색 계열을 기본으로 사용하고 싶어.
- 게이지가 밝은 배경에서도 보이도록 색을 더 진하게 하거나 테두리/그림자/외곽선을 넣어줘.
- 게이지 밖 배경 막대는 사용하지 않을 수 있어. 전체 배경 이미지 안의 타이머 칸 위에 노란색 게이지만 올릴 거야.
- 따라서 TimerGaugeFill 하나만으로도 잘 보이게 만들어줘.
- 배경별로 색을 자동으로 바꾸는 복잡한 기능은 아직 필요 없어.
- 대신 Inspector에서 게이지 색, 그림자 색, 테두리 색을 쉽게 바꿀 수 있게 해줘.
- 나중에 Stage별로 다른 색을 쓰고 싶을 때 확장하기 쉽게 준비해줘.
- 초보자도 이해할 수 있게 쉬운 말로 설명해줘.
- 어려운 영어 용어가 나오면 한국말 뜻도 같이 설명해줘.
- 전체 코드를 빠짐없이 작성해줘.
- 코드마다 주석을 초보자도 알아보기 쉽게 달아줘.
- 어느 GameObject(게임 오브젝트)에 스크립트를 붙이는지 알려줘.
- Inspector(인스펙터)에서 뭘 설정해야 하는지 알려줘.
- 자주 틀리는 부분도 알려줘.

UI 조건:
- TimerGaugeFill에 색을 적용해줘.
- TimerGaugeFill에 Outline(외곽선) 또는 Shadow(그림자)를 붙여서 잘 보이게 해줘.
- Unity 기본 UI Image에서 쓸 수 있는 Outline, Shadow 컴포넌트를 사용해줘.
- 색은 기본값으로 노란색/주황색 계열을 추천해줘.
- 예: 게이지 색은 진한 노랑, 외곽선은 어두운 갈색 또는 진한 회색, 그림자는 검은색 반투명.
- 현재 둥근 노란색 Sprite를 쓰고 있다면 그 Sprite의 색이 더 잘 보이도록 Image color나 보조 효과를 조정해줘.
- TimerGaugeFill의 위치나 크기는 자동으로 바꾸지 마. 내가 직접 맞춘 위치를 유지해야 해.

코드 작성 조건:
- TimerGaugeStyleController.cs 같은 새 스크립트를 만들어줘.
- TimerGaugeStyleController.cs는 TimerGaugeFill 오브젝트에 붙이는 방식으로 해줘.
- 이 스크립트는 Image 색, Outline 색, Shadow 색, Shadow 거리 등을 Inspector에서 조절할 수 있게 해줘.
- TimerGaugeController.cs의 게이지 감소 로직은 건드리지 마.
- TimerController.cs도 건드리지 마.
- 자동 세팅 메뉴가 있다면 TimerGaugeFill에 TimerGaugeStyleController를 자동으로 붙이게 해줘.
- EventSystem은 만들지 마. 이 기능에는 필요 없어.
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
- 가능하면 Project Settings(프로젝트 설정)나 Scene(씬) 저장이 필요한 부분을 체크리스트에 넣어줘.

이번 기능에서 원하는 쉬운 구현 방향:
- TimerGaugeStyleController.cs를 새로 만들어줘.
- TimerGaugeFill 오브젝트에 붙여줘.
- Image 컴포넌트를 자동으로 찾아서 색을 적용하게 해줘.
- Outline 컴포넌트를 자동으로 붙이고 색과 두께를 조절하게 해줘.
- Shadow 컴포넌트를 자동으로 붙이고 색과 거리를 조절하게 해줘.
- Inspector에서 useOutline, useShadow를 켜고 끌 수 있게 해줘.
- 기본 게이지 색은 노란색/주황색으로 해줘.
- 기본 Outline 색은 어두운 갈색으로 해줘.
- 기본 Shadow 색은 검은색 반투명으로 해줘.
- Unity 메뉴 Bubble Shooter > Setup Timer Gauge를 다시 누르면 TimerGaugeFill에 스타일 스크립트도 붙게 해줘.
- 하지만 TimerGaugeFill의 위치와 크기는 자동으로 바꾸지 말아줘.

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
- 이번에는 기능 8번만 만들어줘.
- 타이머 감소 로직은 변경하지 마.
- 게이지 감소 로직도 변경하지 마.
- TimerGaugeFill 위치와 크기는 자동으로 변경하지 마.
- 점수 UI는 만들지 마.
- 기존 배경 관련 코드는 건드리지 마.
- EventSystem은 다시 만들지 마.
```

## 사용 방법

1. 위 `text` 박스 안의 내용을 전부 복사합니다.
2. 다음 작업 요청으로 그대로 붙여넣습니다.
3. 답변을 받은 뒤 Unity 6 프로젝트에 적용합니다.
4. 기능 8번이 성공하면 다음에는 기능 9번 프롬프트를 만들면 됩니다.
