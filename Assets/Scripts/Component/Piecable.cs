using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface Piecable {

    Piece GetPiece();

    Vector3 GetLogicalPosition();
    Quaternion GetLogicalRotation();

    void SetLogicalPosition(Vector3 position);
    void SetLogicalRotation(Quaternion rotation);
}
