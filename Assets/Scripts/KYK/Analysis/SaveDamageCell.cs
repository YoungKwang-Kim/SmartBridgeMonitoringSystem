using PolyAndCode.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveDamageCell : MonoBehaviour
{
    // DamageCell�� �Ӽ���
    public TextMeshProUGUI _number;
    public TextMeshProUGUI _damaged;
    public TextMeshProUGUI _value;
    // Cell ������ ����
    public float space = 30f;

    private GameObject newCell;
    // DamageCell���� ����Ʈ
    private List<GameObject> newCells = new List<GameObject>();
    public float yPosition;

    private void Awake()
    {
        _number = GetComponentInChildren<TextMeshProUGUI>();
        _damaged = GetComponentInChildren<TextMeshProUGUI>();
        _value = GetComponentInChildren<TextMeshProUGUI>();
    }

    // DamageCell�� ���� �־��ִ� �޼���
    public void SetDamageCell(int number, string damaged, float value, RectTransform parent)
    {
        _number.text = number.ToString();
        _damaged.text = damaged;
        _value.text = value.ToString("F1") + "cm";
        newCell = Instantiate(gameObject, parent);
        newCells.Add(newCell);
        RectTransform newCellTransform = newCell.GetComponent<RectTransform>();
        // Anchor�� �г��� ��ܿ� �ξ��� ������ ���̳ʽ��� �ٴ´�.
        yPosition = -(newCellTransform.sizeDelta.y + space) * (number - 1);
        // ����� yPosition���� �г��� ���̰��� �����ش�. �׷����� ������ �������� �����˴ϴ�.
        newCellTransform.localPosition = new Vector3(0, parent.sizeDelta.y + yPosition, 0);
    }

    // ��� newCells ����Ʈ�� �ִ� ������Ʈ�� �����մϴ�.
    public void DeleteAllDamageCells()
    {
        foreach (GameObject cell in newCells)
        {
            Destroy(cell);
        }
        newCells.Clear(); // ����Ʈ �ʱ�ȭ
    }
}