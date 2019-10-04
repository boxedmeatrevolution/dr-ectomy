using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveWinCondition : MonoBehaviour, WinCondition {
    public bool Invoke(Piece piece, Placement placement) {
        return placement.board.type == Board.Type.Metal;
    }
}
