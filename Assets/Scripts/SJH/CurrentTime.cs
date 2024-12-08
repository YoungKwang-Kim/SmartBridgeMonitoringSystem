using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CurrentTime : MonoBehaviour
{
    public Text currentDate;
    public Text currentTime;
    public DateTime currentDateTime;

    void Start()
    {
        InvokeRepeating("UpdateCurrent", 1f, 1f);
    }

    // Update is called once per frame
    void UpdateCurrent()
    {
        currentDateTime = DateTime.Now;
        currentDate.text = currentDateTime.ToString("yyyy��  MM��  dd��");

        string timeFormat = "tt h:mm";
        string currentTimeString = currentDateTime.ToString(timeFormat);

        //���� ���� �ؽ�Ʈ
        currentTimeString = currentTimeString.Replace("AM", "����").Replace("PM", "����");

        currentTime.text = currentTimeString;
    }
}