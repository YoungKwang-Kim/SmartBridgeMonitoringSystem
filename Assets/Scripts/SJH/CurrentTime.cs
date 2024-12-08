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
        currentDate.text = currentDateTime.ToString("yyyy년  MM월  dd일");

        string timeFormat = "tt h:mm";
        string currentTimeString = currentDateTime.ToString(timeFormat);

        //오전 오후 텍스트
        currentTimeString = currentTimeString.Replace("AM", "오전").Replace("PM", "오후");

        currentTime.text = currentTimeString;
    }
}