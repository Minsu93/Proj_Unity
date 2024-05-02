using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillArtifact : MonoBehaviour
{
    /// 아티팩트 별 다른 스킬입니다. 
    /// 사용 시 효과가 모두 다릅니다. 
    /// 

    //스킬이 어떤 식으로 작동하는지
    public abstract void SkillOperation(Vector3 pos, Vector2 dir);

}
