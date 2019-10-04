using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockComponent : MonoBehaviour, Piecable
{

    public int width;
    public int height;

    private Piece piece;

    // Start is called before the first frame update
    void Start() {
        bool[,] shape = new bool[height, width];
        for (int i = 0; i < width; ++i) {
            for (int j = 0; j < height; ++j) {
                shape[j, i] = true;
            }
        }
        Piece.WinCondition winCondition = (Piece piece, Placement placement) => true;
        Piece.LoseCondition loseCondition = (Piece piece, Placement placement) => false;
        this.piece = new Piece(shape, Enumerable.Empty<Piece.Connection>(), new Health(0), winCondition, loseCondition);
    }

    // Update is called once per frame
    void Update() {
        
    }

    public Piece GetPiece() {
        return this.piece;
    }

    public Vector3 GetLogicalPosition() {
        return this.transform.localPosition;
    }

    public Quaternion GetLogicalRotation() {
        return this.transform.localRotation;
    }

    public void SetLogicalPosition(Vector3 position) {
        this.transform.localPosition = position;
    }

    public void SetLogicalRotation(Quaternion rotation) {
        this.transform.localRotation = rotation;
    }
}
