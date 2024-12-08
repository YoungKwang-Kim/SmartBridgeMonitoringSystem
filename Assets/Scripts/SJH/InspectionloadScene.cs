using UnityEngine;
using UnityEngine.SceneManagement;

public class InspectionloadScene : MonoBehaviour
{
    public void GoInspectionScene()
    {
        SceneManager.LoadScene("Drone Inspection Scene", LoadSceneMode.Single);
    }
}