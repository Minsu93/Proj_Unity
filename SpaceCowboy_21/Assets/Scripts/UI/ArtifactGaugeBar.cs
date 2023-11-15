using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactGaugeBar : MonoBehaviour
{
    public Image[] gunGaugeImgs;
    PlayerWeapon pWeapon;
    
    // Start is called before the first frame update
    void Start()
    {
        pWeapon = GameManager.Instance.player.GetComponent<PlayerWeapon>();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        //float[] gaugeMax = pWeapon.gunGaugesMax;
        //float[] gaugeCur = pWeapon.gunGauges;

        //for(int i = 0; i < gunGaugeImgs.Length; i++)
        //{
        //    gunGaugeImgs[i].fillAmount = gaugeCur[i] / gaugeMax[i];
        //}
    }
  
}
