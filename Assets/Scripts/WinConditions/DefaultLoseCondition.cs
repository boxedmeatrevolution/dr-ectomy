using UnityEngine;

public class DefaultLoseCondition : LoseCondition {

    public bool Invoke(Piece piece, Placement placement) {
        return !piece.health.alive;
    }
}
