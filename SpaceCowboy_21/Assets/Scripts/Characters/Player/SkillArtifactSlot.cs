using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillArtifactSlot : MonoBehaviour
{
    public int maxSkillAmmo = 3;
    public int currSkillAmmo;

    [SerializeField] private SkillArtifact skillArtifact;

    //SkillAmmo를 충전한다.
    public void AddSkillAmmo(int amount)
    {
        currSkillAmmo += amount;
        if(currSkillAmmo > maxSkillAmmo)
        {
            currSkillAmmo = maxSkillAmmo;
        }
    }

    //스킬을 사용한다
    public void UseSkillArtifact(Vector3 pos, Vector2 dir)
    {
        if (currSkillAmmo <= 0) return;
        currSkillAmmo -= 1;

        skillArtifact.SkillOperation(pos, dir);
    }

    
}
