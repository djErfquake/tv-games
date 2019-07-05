using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    






}

[System.Serializable]
public class PlayerColor
{
    public Color mainColor;
    public Color secondaryColor;

    public PlayerColor(Color mainColor, Color secondaryColor)
    {
        this.mainColor = mainColor;
        this.secondaryColor = secondaryColor;
    }
}
