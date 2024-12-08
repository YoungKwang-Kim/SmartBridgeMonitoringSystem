using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneLoad : MonoBehaviour
{
    public void GoToDroneInspectionScene()
    {
        SceneManager.LoadScene("DashBoard Scene", LoadSceneMode.Single);
    }
}