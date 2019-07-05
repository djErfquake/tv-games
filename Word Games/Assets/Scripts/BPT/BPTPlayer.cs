using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BPTPlayer : MonoBehaviour
{
    public PlayerColor playerColor;
    public Image timerImage;



    public void Setup(string name, PlayerColor playerColor)
    {
        this.name = name;
        this.playerColor = playerColor;

        this.timerImage.color = this.playerColor.mainColor;
        this.timerImage.rectTransform.sizeDelta.With(y: 200);
    }


    public void Draw(List<List<DrawingPoint>> drawingData)
    {
        Debug.Log("Drawing Data:");
        for (int i = 0; i < drawingData.Count; i++)
        {
            for (int j = 0; j < drawingData[i].Count; j++)
            {
                Debug.Log(drawingData[i][j].x + "," + drawingData[i][j].y + ": " + drawingData[i][j].dragging);
            }
        }
    }
}
