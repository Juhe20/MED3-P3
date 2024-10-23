using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosClass : MonoBehaviour
{
    public int x;
    public int y;
    public bool occupied = false;
    public string occupiedBy = string.Empty;

    public int getListX()
    {
        return x;
    }
    public int getListY()
    {
        return y;
    }

    public void setListX(int x)
    {
        this.x = x;
    }

    public void setListY(int y)
    {
        this.y = y;
    }

}
