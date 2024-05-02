using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillArtifactSlot : MonoBehaviour
{
    public int maxSkillAmmo = 3;
    public int currSkillAmmo;

    [SerializeField] private SkillArtifact skillArtifact;

    //SkillAmmo�� �����Ѵ�.
    public void AddSkillAmmo(int amount)
    {
        currSkillAmmo += amount;
        if(currSkillAmmo > maxSkillAmmo)
        {
            currSkillAmmo = maxSkillAmmo;
        }
    }

    //��ų�� ����Ѵ�
    public void UseSkillArtifact(Vector3 pos, Vector2 dir)
    {
        if (currSkillAmmo <= 0) return;
        currSkillAmmo -= 1;

        skillArtifact.SkillOperation(pos, dir);
    }

    
}
