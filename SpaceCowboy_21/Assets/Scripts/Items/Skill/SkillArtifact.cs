using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillArtifact : MonoBehaviour
{
    /// ��Ƽ��Ʈ �� �ٸ� ��ų�Դϴ�. 
    /// ��� �� ȿ���� ��� �ٸ��ϴ�. 
    /// 

    //��ų�� � ������ �۵��ϴ���
    public abstract void SkillOperation(Vector3 pos, Vector2 dir);

}
