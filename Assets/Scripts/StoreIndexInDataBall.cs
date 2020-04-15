using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreIndexInDataBall : MonoBehaviour
{
    int index;

    public void SetIndex(int index)
    {
        this.index = index;
    }

    public int GetIndex() { return this.index; }
}
