using DamageNumbersPro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAction : MonoBehaviour, ITarget, IHitable, IKickable
{

    //1. ü���� BossUI���� �о ȭ�鿡 �����. 
    //2. �÷��̾� ��ġ�� (�ʿ��ϴٸ�)������ �� �ִ� EnemyBrain
    //3. Update���� �������� ���Ͽ� ���� �ൿ.
    /// A - �÷��̾� �ֺ��� ���� ��ҷ� �̵��Ѵ�. �̵��� ��ġ�� �÷��̾� ���� ���
    /// B - �÷��̾� �ֺ��� ���� ��ҷ� �̵��Ѵ�. ���� �ð��� ���� ������. ������ ���.
    /// C - �÷��̾� �ֺ��� ���� ��ҷ� �̵��Ѵ�. ������ ������� �Ѹ���. ������ �÷��̾ ������ ���ظ� �ְ� �������. ������ ���缭 ������ �� �ִ�. 
    /// D - ��ȭ�� �Ǵ�. �Ϲ� ������ 2���� ��ȯ�Ѵ�.  -> �������� Ȯ������ ����ϴ� ����.
    /// E - (�ʻ��, ���� : Phase 2) ���ڸ����� �⸦ ������. �÷��̾ �����ϴ� ������ �߻��Ѵ�. 
    /// ������ ������ ��Ÿ���� �ִ�. 
    //4. Die�� ���̺갡 Ŭ����ȴ�. 

    bool bossActivate = false;
    Health bossHealth;
    Collider2D coll;
    [SerializeField] GameObject projHitObj;
    [SerializeField] DamageNumber dmgNum;
    [SerializeField] ParticleSystem hitEffect;    //�¾��� �� ȿ��

    public event Action<float> BossUIUpdateEvent;   //���� HP UI ������Ʈ�� �̺�Ʈ -> BossSpawner �Ҵ�
    public event Action BossDieEvent;   //���� ����� �� �������� Ŭ���� �̺�Ʈ -> BossSpawner �Ҵ�

    //������ ���ϵ�.
    [Header("Patterns")]
    [SerializeField] AttackPatternSet[] patternPresets;

    [ContextMenu("GetPatterns")]
    void GetChildPatterns()
    {
        AttackPattern[] atps = GetComponentsInChildren<AttackPattern>();
        patternPresets = new AttackPatternSet[atps.Length];
        for(int i = 0; i < atps.Length; i++)
        {
            AttackPatternSet preset = new AttackPatternSet();
            preset.pattern = atps[i];
            patternPresets[i] = preset;
        }
    }

    private void Awake()
    {
        bossHealth = GetComponent<Health>();
        coll = GetComponent<Collider2D>();
        projHitObj.SetActive(false);
    }

    //���� ���� ����(���� �ִϸ��̼�)����
    public void BossAwake()
    {
        InitializeBoss();
    }

    //���� ���� Ȱ��ȭ
    public void InitializeBoss()
    {
        //��μ� Ȱ��ȭ ����. 
        projHitObj.SetActive(true);
        bossHealth.ResetHealth();
        
        for (int i = 0; i < patternPresets.Length; i++)
        {
            patternPresets[i].InitializePreset();
        }

        //bossActivate = true;
        StartCoroutine(updateRoutine());
    }

    IEnumerator updateRoutine()
    {
        while (true)
        {
            for(int i = 0; i < patternPresets.Length; i++)
            {
                if (patternPresets[i].pattern.IsCoolTimeOver())
                {
                    if (patternPresets[i].pattern.CheckCondition())
                    {
                        Debug.LogFormat("���� {0} : {1} ����", i, patternPresets[i].pattern.name);
                        yield return StartCoroutine(patternPresets[i].pattern.StartPattern());
                        break;
                    }
                }
                yield return null;
            }
            yield return new WaitForSeconds(0.25f);
        }
    }




    #region ������ ����, Die Event
    public void DamageEvent(float damage, Vector2 hitPoint)
    {
        if (bossHealth.AnyDamage(damage))
        {
            //������ num ����.
            dmgNum.Spawn(hitPoint, damage);
            //UI������Ʈ
            if (BossUIUpdateEvent != null) BossUIUpdateEvent(bossHealth.HealthPercent);

            if (!bossHealth.IsDead())
            {
                //�´� ȿ�� 
                if (hitEffect != null) GameManager.Instance.particleManager.GetParticle(hitEffect, transform.position, transform.rotation);
            }
            else
            {
                WhenDieEvent();
            }
        }
    }

    public void Kicked(Vector2 hitPos)
    {
        throw new System.NotImplementedException();
    }

    public virtual void WhenDieEvent()
    {
        //�浹 ó�� ��Ȱ��ȭ
        projHitObj.SetActive(false);
        bossActivate = false;
        StopAllCoroutines();

        if (BossDieEvent != null) BossDieEvent();
        //���� �ִϸ��̼� 
        //�� ������Ʈ ����?
        //WaveManager�� ���� Ŭ���� ����
        WaveManager.instance.MonsterDisapper();
    }


    #endregion

    public Collider2D GetCollider()
    {
        return coll;
    }

}

[Serializable]
public class AttackPatternSet
{
    public AttackPattern pattern;
    public float coolTime;

    public void InitializePreset()
    {
        pattern.coolTime = this.coolTime;
    }
}

