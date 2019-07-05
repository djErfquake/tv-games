using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawingManager : MonoBehaviour
{
    public Transform linesContainer;
    public GameObject linePrefab;
    private List<LineRenderer> lines = new List<LineRenderer>();

    private int lineIndex = 0;
    private Vector2 lastPoint;


    private void Start()
    {
        Clear();
    }

    public void Clear()
    {
        while (lines.Count > 0)
        {
            LineRenderer line = lines[0];
            lines.RemoveAt(0);
            Destroy(line);
        }
        lineIndex = 0;
    }

    public void AddPoint(DrawingPoint newDrawPoint)
    {
        Vector2 newPoint = new Vector2(newDrawPoint.x, newDrawPoint.y);

        // add line if necessary
        if (!newDrawPoint.dragging)
        {
            GameObject lineGo = Instantiate(linePrefab);
            lineGo.transform.SetParent(linesContainer, false);
            LineRenderer line = lineGo.GetComponent<LineRenderer>();
            lines.Add(line);
            lineIndex = lines.Count - 1;

            lines[lineIndex].positionCount = 2;
            lines[lineIndex].SetPosition(0, newPoint);
            lines[lineIndex].SetPosition(1, newPoint);
        }
        else
        {
            int newPointIndex = lines[lineIndex].positionCount;
            lines[lineIndex].positionCount++;
            lines[lineIndex].SetPosition(newPointIndex, newPoint);
        }

        lastPoint = newPoint;
    }
}

public class DrawingPoint
{
    public float x, y;
    public bool dragging = false;

    public DrawingPoint(float x, float y, bool dragging)
    {
        this.x = x;
        this.y = y;
        this.dragging = dragging;
    }
}