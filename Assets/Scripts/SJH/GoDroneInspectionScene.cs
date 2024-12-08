using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoDroneInspectionScene : MonoBehaviour
{
    public GameObject DamageChartScreen;
   public void GoDroneScene()
    {
        DamageChartScreen.SetActive(false);
    }
}
