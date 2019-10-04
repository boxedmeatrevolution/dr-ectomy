using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoLoseCondition : MonoBehaviour, LoseCondition {
    public bool Invoke(Piece piece, Placement placement) {
        return false;
    }
}
