using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TestCube", menuName = "Weapon/TechCube/OnImpact", order = int.MaxValue)]
public class OnImpactCube : AlienTechCube
{
    //ť���AlienTechCube�� ��ӹް�,
    //AssetMenu(����)
    //ID, �ɷ�ġ
    //Ÿ��,�̵�,��� �� �̺�Ʈ�� �Ű澲�� �ȴ�. 

    //�������� ���� �̺�Ʈ�� �߰����� �ʴ´�. 

    //Ÿ�� ��
    public override void OnImpactEvent()
    {
        Debug.Log("C");
    }
}
