using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAmmoDrop : Collectable
{
    SkillArtifactSlot skillSlot;

    protected override void ConsumeEffect()
    {
        skillSlot.AddSkillAmmo(1);
    }

    void Start()
    {
        skillSlot = GameManager.Instance.skillSlot;
    }


}
