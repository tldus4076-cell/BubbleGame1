# 기능 13 프롬프트: 마우스 위치를 따라 슈터 조준하기

아래 프롬프트를 그대로 복사해서 사용하세요.

```text
나는 초보자이고, Unity 6으로 2D 버블슈터 게임을 만들고 있어.

이번에는 기능 목록 13번인 "마우스 위치를 따라 슈터가 회전하게 하기"만 만들고 싶어.

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
- 기능 9번~11번 점수 기능은 완료했어.
- 기능 12번 슈터 배치 기능은 완료했어.
- ShooterRoot와 ShooterVisual이 있어.
- ShooterController.cs는 슈터 이미지와 정렬만 담당하고, 위치는 내가 직접 Scene 창에서 조절하는 방식이야.
- Play를 눌러도 ShooterRoot 위치가 자동으로 바뀌면 안 돼.
- 기존 배경, 타이머, 게이지, 점수, 슈터 배치 기능은 절대 망가뜨리지 말아줘.
- EventSystem은 삭제된 상태야. 이번 기능에는 필요 없으면 만들지 마.
- Player Settings의 Active Input Handling은 Both로 바꿔둔 상태야.

목표:
마우스 위치를 따라 슈터가 회전하게 만들고 싶어.
마우스를 왼쪽 위로 옮기면 슈터가 왼쪽 위를 보고, 오른쪽 위로 옮기면 오른쪽 위를 보게 하고 싶어.

조건:
- Unity 6 2D 프로젝트 기준으로 설명해줘.
- 이번에는 기능 13번 "마우스 위치를 따라 슈터 회전"만 만들어줘.
- 터치 조준은 아직 만들지 마. 그건 기능 14번에서 만들 거야.
- 아래쪽 조준 제한은 아직 만들지 마. 그건 기능 15번에서 만들 거야.
- 조준선 표시는 아직 만들지 마. 그건 기능 16번에서 만들 거야.
- 버블 발사는 아직 만들지 마. 그건 기능 21번에서 만들 거야.
- 이번에는 마우스 위치를 바라보도록 슈터가 회전하는 것만 만들면 돼.
- ShooterRoot 위치는 자동으로 바꾸지 마.
- 기존 ShooterController.cs의 위치 관련 동작은 다시 추가하지 마.
- 마우스 위치는 카메라를 기준으로 월드 좌표로 바꿔서 계산해줘.
- 슈터가 어느 축을 기준으로 앞을 보는지 Inspector에서 조절할 수 있게 해줘.
- 내 슈터 이미지가 기본적으로 위쪽을 바라보는 이미지일 수도 있고, 오른쪽을 바라보는 이미지일 수도 있으니 angleOffset 같은 보정값을 넣어줘.
- 기본적으로 2D에서 Z축 회전만 사용해줘.
- 슈터는 배경보다 앞에 계속 보여야 해.
- UI 위치와 점수/타이머는 건드리지 마.
- 초보자도 이해할 수 있게 쉬운 말로 설명해줘.
- 어려운 영어 용어가 나오면 한국말 뜻도 같이 설명해줘.
- 전체 코드를 빠짐없이 작성해줘.
- 코드마다 주석을 초보자도 알아보기 쉽게 달아줘.
- 어느 GameObject(게임 오브젝트)에 스크립트를 붙이는지 알려줘.
- Inspector(인스펙터)에서 뭘 설정해야 하는지 알려줘.
- 자주 틀리는 부분도 알려줘.

입력 조건:
- 현재 프로젝트는 Active Input Handling을 Both로 설정했어.
- 이번에는 마우스 조준만 만들면 되니까 예전 Input.mousePosition을 사용해도 돼.
- 가능하면 UnityEngine.Input을 사용하되, 새 Input System 프로젝트에서 에러가 나지 않게 주의사항도 알려줘.
- 만약 코드에서 새 Input System까지 대응하는 게 더 안전하면 그 방식으로 해줘.
- 단, 너무 복잡한 방식은 피하고 초보자가 이해하기 쉽게 해줘.

오브젝트 조건:
- ShooterRoot 또는 ShooterVisual 중 어느 오브젝트를 회전할지 명확히 정해줘.
- 추천은 ShooterRoot를 회전시키는 방식이야.
- ShooterVisual은 ShooterRoot의 자식으로 두고, 이미지 위치만 담당하게 해줘.
- ShooterRoot의 위치는 내가 직접 맞춘 위치 그대로 유지되어야 해.
- 회전만 바뀌어야 해.

코드 작성 조건:
- ShooterAimController.cs 같은 새 스크립트를 만들어줘.
- ShooterAimController.cs는 ShooterRoot에 붙이는 방식으로 해줘.
- 기존 ShooterController.cs는 가능하면 건드리지 마.
- targetCamera, rotationTarget, angleOffset, rotateSpeed 같은 변수를 만들어줘.
- targetCamera는 비워두면 Main Camera를 자동으로 찾게 해줘.
- rotationTarget은 비워두면 자기 자신 transform을 회전하게 해줘.
- angleOffset은 슈터 이미지 방향 보정용으로 만들어줘.
- rotateSpeed는 0이면 즉시 회전, 0보다 크면 부드럽게 회전하게 해줘.
- 마우스 위치와 슈터 위치의 방향 벡터를 구해줘.
- Mathf.Atan2를 사용해서 각도를 구해줘.
- Quaternion.Euler(0, 0, angle)를 사용해서 Z축 회전을 적용해줘.
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
- ShooterAimController.cs를 새로 만들어줘.
- ShooterRoot 오브젝트에 ShooterAimController.cs를 붙여줘.
- 자동 세팅 메뉴가 필요하다면 Bubble Shooter > Setup Shooter Aim 메뉴를 만들어줘.
- 자동 세팅 메뉴를 누르면 ShooterRoot에 ShooterAimController가 붙게 해줘.
- 기존 ShooterRoot 위치는 절대 바꾸지 마.
- Play를 누르고 마우스를 움직였을 때 슈터가 마우스를 바라보게 해줘.
- 슈터 이미지 방향이 맞지 않으면 Inspector의 angleOffset 값으로 보정하는 방법을 알려줘.
- EventSystem은 만들지 마.

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
- 이번에는 기능 13번만 만들어줘.
- 터치 조준은 만들지 마.
- 아래쪽 조준 제한은 만들지 마.
- 조준선은 만들지 마.
- 버블 발사는 만들지 마.
- ShooterRoot 위치를 자동으로 바꾸지 마.
- 기존 배경, 타이머, 게이지, 점수 코드는 건드리지 마.
- EventSystem은 다시 만들지 마.
```

## 사용 방법

1. 위 `text` 박스 안의 내용을 전부 복사합니다.
2. 다음 작업 요청으로 그대로 붙여넣습니다.
3. 기능 13번이 성공하면 다음에는 기능 14번 프롬프트를 만들면 됩니다.
