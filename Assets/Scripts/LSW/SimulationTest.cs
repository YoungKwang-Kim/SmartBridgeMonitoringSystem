using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SimulationTest : MonoBehaviour
{
    public GameObject beforeCellFraction;
    public GameObject afterCellFraction;
    private string afterCellFractionName;

    private void Start()
    {
        afterCellFractionName = afterCellFraction.name;
        beforeCellFraction.SetActive(true);
    }

    public void SimulationButton()
    {
        beforeCellFraction.SetActive(false);
        Instantiate(afterCellFraction);
    }

    public void ResetButton()
    {
        // "AfterCellFractionTest" �̸��� ���� ��� ���� ������Ʈ�� ã�Ƽ� ����
        GameObject[] afterCellFractions = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in afterCellFractions)
        {
            if (obj.name == afterCellFractionName+"(Clone)")
            {
                Destroy(obj);
            }
        }
        beforeCellFraction.SetActive(true);
    }
}