using UnityEngine;

public class DefaultWinCondition : WinCondition {

    public bool Invoke(Piece piece, Placement placement) {
        return piece.health.wounds == 0 && piece.health.alive && placement.board.type == Board.Type.Organic;
    }
}
