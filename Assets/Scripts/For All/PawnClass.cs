using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class PawnClass : MonoBehaviour
{
    public int x;
    public int y;
    public bool isDam = false;
    public bool animate = false;
    public Animator animator;

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
