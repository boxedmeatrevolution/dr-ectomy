using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationWinCondition : MonoBehaviour, WinCondition {

    public Direction orientation = Direction.Right;

    private DefaultWinCondition cond = new DefaultWinCondition();
    public bool Invoke(Piece piece, Placement placement) {
        return this.cond.Invoke(piece, placement) && placement.transform.rotation == this.orientation;
    }
}
