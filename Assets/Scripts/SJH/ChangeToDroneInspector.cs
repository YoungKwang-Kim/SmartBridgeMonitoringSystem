using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeToDroneInspector : MonoBehaviour
{
    public void ChangeScene()
    {
        SceneManager.LoadScene("Drone Inspection Scene");
    }
}
