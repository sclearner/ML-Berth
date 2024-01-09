using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManagerUI : MonoBehaviour
{
    public TMP_Text ui;
    private float time;
    private Manager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponent<Manager>();
        time = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (manager.TaskCount > 0) time += Time.deltaTime;
        ui.text = $"Tasks left: {manager.TaskCount} - {manager.label}\nTime run: {Mathf.FloorToInt(time) / 60:0#}:{Mathf.FloorToInt(time) % 60:0#}";
    }
}
