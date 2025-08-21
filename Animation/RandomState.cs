using UnityEngine;

public class RandomState : StateMachineBehaviour
{
    // 파라미터를 참조하기 위한 해시값 저장
    //readonly int _hashRandomIdle = Animator.StringToHash("RandomIdle");
     
    //상태 전환에 사용될 파라미터 이름
    [SerializeField] private string stateParameterName;
    public int numberOfStates = 3;  //상태 개수
    public float minNormTime = 0f;
    public float maxNormTime = 5f;
    
    // 애니메이션 상태가 끝나기 전에, 전환할 시간을  min~max 범위 내에서 랜덤하게 결정합니다. 설정된 시간 이후에 랜덤 상태로 전환됩니다.
    protected float _randomNormTime;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _randomNormTime = Random.Range(minNormTime, maxNormTime);  //랜덤하게 선택된 대기시간
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 이 상태를 벗어나면(상태가 전환중이면) 파라미터를 다시 -1로 재설정합니다.
        if (animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).fullPathHash == stateInfo.fullPathHash)
        {
            //animator.SetInteger(_hashRandomIdle, -1);
            animator.SetInteger(stateParameterName, -1);
        }
        //상태가 randomNormTime을 초과하고 아직 전환되지 않은 경우 파라미터를 랜덤값으로 설정합니다.
        if (stateInfo.normalizedTime > _randomNormTime && !animator.IsInTransition(0))
        {
            //animator.SetInteger(_hashRandomIdle, Random.Range(0, numberOfStates));
            animator.SetInteger(stateParameterName, Random.Range(0, numberOfStates));
        }
    }
} 
/*
~~사용방법~~
1. 스크립트 추가:
RandomState 스크립트를 Unity 프로젝트에 추가합니다.
애니메이터 상태에서 랜덤한 전환을 관리하려는 애니메이션 클립에 이 StateMachineBehaviour를 추가해야 합니다.
2. 애니메이터에서 설정:
Unity 에디터에서 애니메이터 상태 머신을 열고, 전환하려는 애니메이션 상태를 선택합니다.
RandomState 스크립트를 해당 상태의 StateMachineBehaviour에 추가합니다.
numberOfStates, minNormTime, maxNormTime을 적절히 설정하여 원하는 랜덤 대기 시간 및 상태 전환 범위를 조정합니다.
3. Animator 파라미터 설정:
Animator에서 "RandomIdle"이라는 Integer 파라미터를 추가해야 합니다. 이 파라미터는 스크립트에서 설정한 값에 따라 애니메이션 상태를 전환하는 데 사용됩니다.
예를 들어, "RandomIdle" 값이 0, 1, 2일 때 각각 다른 "Idle" 애니메이션 상태로 전환될 수 있도록 애니메이터 상태 머신을 설정합니다.
4. 상태 전환 조건 설정:
애니메이터에서 "RandomIdle" 값이 변경될 때, 이를 기반으로 상태 전환 조건을 설정합니다. 예를 들어, "RandomIdle"이 0이면 "Idle_0"으로 전환, 1이면 "Idle_1"으로 전환 등으로 설정할 수 있습니다.

~~예시~~
애니메이터 파라미터 설정:

"RandomIdle"이라는 Integer 파라미터를 추가하고, 이를 기준으로 애니메이션 상태를 전환합니다.
애니메이터 상태 설정:

"Idle_0", "Idle_1", "Idle_2" 등의 상태를 만들고, 전환 조건으로 "RandomIdle" 파라미터 값을 설정합니다.
상태 전환 조건 예시:

"RandomIdle"이 0일 때: Idle_0으로 전환
"RandomIdle"이 1일 때: Idle_1으로 전환
"RandomIdle"이 2일 때: Idle_2으로 전환
이렇게 하면 애니메이션이 일정 시간이 지난 후, 랜덤하게 "Idle" 상태로 전환되며 다양한 "Idle" 애니메이션을 순차적으로 사용하게 됩니다.*/