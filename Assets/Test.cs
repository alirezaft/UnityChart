using System;
using Unity.VisualScripting;
using UnityChart.Runtime;
using UnityEngine;
using Random = UnityEngine.Random;

public class Test : MonoBehaviour
{
    private DataProvider providerv;
    private int vnum = 0;
    private DataProvider providerh;
    private int hnum = 0;

    private void Start()
    {
        providerv = new DataProvider(Color.blue, "Vertical Buttons", "vbutton");
        providerv.AddDataPoint(-2);
        providerv.AddDataPoint(-1);
        providerv.AddDataPoint(-3);
        providerv.AddDataPoint(-2);
        providerv.AddDataPoint(-5);
        providerv.AddDataPoint(-6);
        providerh = new DataProvider(Color.red, "Horizontal Buttons", "hbutton");
        // providerh.AddDataPoint(5);
        // providerh.AddDataPoint(2);
    }

    private void Update()
    {
        // providerv.AddDataPoint(Random.Range(0f, 23f));
        // if(Input.GetKeyDown(KeyCode.UpArrow))
        // {
        //     vnum++;
        //     providerv.AddDataPoint(vnum);
        // }else if (Input.GetKeyDown(KeyCode.DownArrow))
        // {
        //     vnum--;
        //     providerv.AddDataPoint(vnum);
        // }else if (Input.GetKeyDown(KeyCode.LeftArrow))
        // {
        //     hnum++;
        //     providerh.AddDataPoint(hnum);
        // }else if (Input.GetKeyDown(KeyCode.RightArrow))
        // {
        //     hnum--;
        //     providerh.AddDataPoint(hnum);
        // }
    }

    private void OnDisable()
    {
        // providerv.Dispose();
        // providerh.Dispose();
    }
}
