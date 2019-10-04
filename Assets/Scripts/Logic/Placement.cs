using System;
using System.Collections.Generic;

[Serializable]
public struct Placement : IEquatable<Placement> {

    public Board board;
    public Transform transform;

    public Placement(Board board, Transform transform) {
        this.board = board;
        this.transform = transform;
    }

    public override bool Equals(object obj) {
        if (obj is Placement) {
            return this.Equals((Placement)obj);
        }
        else {
            return false;
        }
    }
    public bool Equals(Placement other) {
        return this.board == other.board && this.transform.Equals(other.transform);
    }

    public override int GetHashCode() {
        var hashCode = 702779134;
        hashCode = hashCode * -1521134295 + EqualityComparer<Board>.Default.GetHashCode(board);
        hashCode = hashCode * -1521134295 + EqualityComparer<Transform>.Default.GetHashCode(transform);
        return hashCode;
    }
}
