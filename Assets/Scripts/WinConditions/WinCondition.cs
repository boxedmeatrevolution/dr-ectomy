public interface WinCondition {
    bool Invoke(Piece piece, Placement placement);
}

public interface LoseCondition {
    bool Invoke(Piece piece, Placement placement);
}
