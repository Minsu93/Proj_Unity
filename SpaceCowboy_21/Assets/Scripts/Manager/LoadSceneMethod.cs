using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneMethod : MonoBehaviour
{
    public void Loadscene(string sceneName)
    {
        GameManager.Instance.LoadsceneByName(sceneName);
    }

}
