using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardComponent : MonoBehaviour {

    public int width;
    public int height;
    public int depth;
    public Board.Type type;

    private Board board;

    void Start () {
        this.board = new Board(this.width, this.height, this.depth, 0, this.type);
	}

    void Update() {
    }

    // Estimate the placement of a piece within the board.
    public Placement GetPlacement<T>(T pieceComponent) where T: MonoBehaviour, Piecable {
        // The (x, y) of both the board and piece are measured relative to their center.
        // We need to change it to lower left (taking into account piece rotation).
        Piece piece = pieceComponent.GetPiece();
        Vector3 position = this.transform.position;
        position -= Constant.TILE_WIDTH * new Vector3(
            this.board.boundary.GetDimensions().x / 2.0f,
            this.board.boundary.GetDimensions().y / 2.0f);
        Vector3 piecePosition = pieceComponent.GetLogicalPosition();
        piecePosition -= pieceComponent.GetLogicalRotation() * (Constant.TILE_WIDTH * new Vector3(
            piece.GetWidth() / 2.0f,
            piece.GetHeight() / 2.0f));
        // Calculate relative difference between the two, in tiles, and the angle, in quarters.
        Vector3 displacement = (piecePosition - position) / Constant.TILE_WIDTH;
        Vector2Int boardPosition = new Vector2Int(Mathf.RoundToInt(displacement.x), Mathf.RoundToInt(displacement.y));
        int boardAngle = (Mathf.RoundToInt(pieceComponent.GetLogicalRotation().eulerAngles.z / 90) + 4) % 4;

        // Finally, give the placement.
        Transform transform = new Transform(new Point(boardPosition.x, boardPosition.y, this.board.boundary.lower.z), (Direction)boardAngle);
        return new Placement(this.board, transform);
    }

    public Board GetBoard() {
        return this.board;
    }
}
