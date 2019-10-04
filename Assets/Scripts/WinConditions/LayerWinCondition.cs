using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerWinCondition : MonoBehaviour, WinCondition {

    public int layer = 0;

    private DefaultWinCondition cond = new DefaultWinCondition();
    public bool Invoke(Piece piece, Placement placement) {
        return this.cond.Invoke(piece, placement) && placement.transform.offset.z == this.layer;
    }
}
