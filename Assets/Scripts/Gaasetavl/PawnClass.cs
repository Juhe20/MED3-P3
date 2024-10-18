using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnClass : MonoBehaviour
{
    public int x;
    public int y;

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
