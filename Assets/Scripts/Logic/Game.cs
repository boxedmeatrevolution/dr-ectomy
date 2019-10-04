using System;
using System.Collections.Generic;
using System.Linq;

public class Game {
    
    IList<Piece> pieces = new List<Piece>();
    IList<Placement> placements = new List<Placement>();

    public Game() {
    }

    // Ticks the game forward by one turn. This checks the health of pieces and returns if the game is lost.
    public State Tick() {
        this.TickPieceWounds();
        // Update the health of each piece.
        foreach (Piece piece in this.pieces) {
            this.TickPieceHealth(piece);
        }

        // Check win conditions of each piece. If all pieces agree on a win, then we've won.
        bool won = true;
        bool lost = false;
        for (int pieceIdx = 0; pieceIdx < this.pieces.Count; ++pieceIdx) {
            Piece piece = this.pieces[pieceIdx];
            Placement placement = this.placements[pieceIdx];

            // If any piece doesn't agree on a win, we haven't won yet.
            if (!piece.winCondition(piece, placement)) {
                won = false;
            }
            // If any piece says we lost, then we have lost.
            if (piece.loseCondition(piece, placement)) {
                lost = true;
            }
        }

        // Losing takes priority over winning.
        if (lost) {
            return State.Lost;
        }
        else if (won) {
            return State.Won;
        }
        else {
            return State.InProgress;
        }
    }

    public Placement GetPlacement(Piece piece) {
        int pieceIdx = this.pieces.IndexOf(piece);
        if (pieceIdx == -1) {
            throw new ArgumentOutOfRangeException("Piece is not contained within game");
        }
        return this.placements[pieceIdx];
    }

    // Try to place the piece at a location. Returns whether it was successful.
    public bool Place(Piece piece, Placement placement) {
        if (piece == null) {
            return false;
        }
        // Make sure that the placement and all places above it are free (so that it can be placed).
        for (int z = placement.transform.offset.z; z >= placement.board.boundary.lower.z; --z) {
            Placement overPlacement = placement;
            overPlacement.transform.offset.z = z;
            if (!this.CheckPlacementFree(piece, overPlacement)) {
                return false;
            }
        }
        // And also that the place right under it is occupied (so that it doesn't fall).
        Placement underPlacement = placement;
        underPlacement.transform.offset.z += 1;
        if (this.CheckPlacementFree(piece, underPlacement)) {
            return false;
        }
        
        this.ForcePlace(piece, placement);
        return true;
    }

    // Try to place a piece at a location. Gradually drop the piece until it is successful (if ever).
    // Return the resulting placement.
    public Placement? PlaceSoft(Piece piece, Placement placement) {
        if (piece == null) {
            return null;
        }
        // If the z=0 placement is full, then there is no way. Return failure.
        placement.transform.offset.z = placement.board.boundary.lower.z;
        if (!this.CheckPlacementFree(piece, placement)) {
            return null;
        }
        // Otherwise, keep falling down to find the right placement location.
        while (true) {
            placement.transform.offset.z += 1;
            if (!this.CheckPlacementFree(piece, placement)) {
                break;
            }
        }
        placement.transform.offset.z -= 1;
        this.ForcePlace(piece, placement);
        return placement;
    }

    // Try to remove a piece. Returns false if removal is not allowed.
    public bool Take(Piece piece) {
        int pieceIdx = this.pieces.IndexOf(piece);
        if (pieceIdx == -1) {
            return false;
        }
        // Check that every place above the piece is free, so that it can be lifted out.
        Placement placement = this.placements[pieceIdx];
        for (int z = placement.transform.offset.z - 1; z >= placement.board.boundary.lower.z; --z) {
            Placement overPlacement = placement;
            overPlacement.transform.offset.z = z;
            if (!this.CheckPlacementFree(piece, overPlacement)) {
                return false;
            }
        }

        this.ForceTakeAt(pieceIdx);
        return true;
    }
    
    public void ForcePlace(Piece piece, Placement placement) {
        this.pieces.Add(piece);
        this.placements.Add(placement);
    }

    public void ForceTake(Piece piece) {
        int pieceIdx = this.pieces.IndexOf(piece);
        if (pieceIdx != -1) {
            this.ForceTakeAt(pieceIdx);
        }
    }

    public void ForceTakeAt(int pieceIdx) {
        this.pieces.RemoveAt(pieceIdx);
        this.placements.RemoveAt(pieceIdx);
    }

    // Makes sure that a piece can fit into a certain placement without collision.
    public bool CheckPlacementFree(Piece piece, Placement placement) {
        return GetPlacementCollisionBoard(piece, placement) == null && GetPlacementCollisionPiece(piece, placement) == null;
    }

    // Returns whether a piece will collide with the board at a placement.
    public Board GetPlacementCollisionBoard(Piece piece, Placement placement) {
        // Check if the bounding box of the piece is within the board it will be placed on.
        Board board = placement.board;
        Rectangle boundingBox = placement.transform.ApplyInverse(piece.GetBoundingBox());
        return Intersection.Contains(board.boundary, boundingBox) ? null : board;
    }

    // Returns a piece (if any) that the piece will collide with at a placement.
    public Piece GetPlacementCollisionPiece(Piece piece, Placement placement) {
        Rectangle boundingBox = placement.transform.ApplyInverse(piece.GetBoundingBox());
        // Then loop through every piece to make sure there is no collision.
        for (int pieceIdx = 0; pieceIdx < this.pieces.Count; ++pieceIdx) {
            // Get a bounding box for each piece, and use it to do a broad check.
            Piece otherPiece = this.pieces[pieceIdx];
            Placement otherPlacement = this.placements[pieceIdx];
            if (otherPlacement.board == placement.board) {
                Rectangle otherBoundingBox = otherPlacement.transform.ApplyInverse(otherPiece.GetBoundingBox());
                Rectangle? checkIntersection = Intersection.Intersect(boundingBox, otherBoundingBox);
                if (checkIntersection != null) {
                    Rectangle intersection = checkIntersection.Value;
                    // There is an intersection of bounding boxes, so we must loop over every tile of each shape.
                    for (int x = intersection.lower.x; x < intersection.upper.x; ++x) {
                        for (int y = intersection.lower.y; y < intersection.upper.y; ++y) {
                            for (int z = intersection.lower.z; z < intersection.upper.z; ++z) {
                                Point test = new Point(x, y, z);
                                Point testLocal = placement.transform.Apply(test);
                                Point otherTestLocal = otherPlacement.transform.Apply(test);
                                // If the two pieces are filled at the same location, then this isn't a valid location to place.
                                if (piece.IsFilled(testLocal) && otherPiece.IsFilled(otherTestLocal)) {
                                    return otherPiece;
                                }
                            }
                        }
                    }
                }
            }
        }

        return null;
    }

    // Returns the board connected to a piece by the connection (in local coords).
    public Board GetPieceConnectionBoard(Piece piece, Placement placement, Piece.Connection localConnection) {
        Piece.Connection connection = new Piece.Connection(
            placement.transform.ApplyInverse(localConnection.position),
            placement.transform.ApplyInverse(localConnection.direction));
        Point connectionPoint = connection.position + connection.direction.GetUnitVector();
        bool connected = placement.board.type == Board.Type.Organic && !Intersection.Contains(placement.board.boundary, connectionPoint);
        if (piece.health.alive && connected) {
            return placement.board;
        }
        else {
            return null;
        }
    }

    // Return the piece connected to our piece by a connection (in local coords).
    public Pair<Piece, Piece.Connection>? GetPieceConnectionPiece(Piece piece, Placement placement, Piece.Connection localConnection) {
        Piece.Connection connection = new Piece.Connection(
            placement.transform.ApplyInverse(localConnection.position),
            placement.transform.ApplyInverse(localConnection.direction));
        Point connectionPoint = connection.position + connection.direction.GetUnitVector();
        for (int otherPieceIdx = 0; otherPieceIdx < this.pieces.Count; ++otherPieceIdx) {
            Piece otherPiece = this.pieces[otherPieceIdx];
            Placement otherPlacement = this.placements[otherPieceIdx];
            if (piece.health.alive && otherPiece.health.alive && placement.board == otherPlacement.board) {
                foreach (Piece.Connection otherLocalConnection in otherPiece.GetConnections()) {
                    Piece.Connection otherConnection = new Piece.Connection(
                        otherPlacement.transform.ApplyInverse(otherLocalConnection.position),
                        otherPlacement.transform.ApplyInverse(otherLocalConnection.direction));
                    // If it lines up with our connection:
                    if (otherConnection.position.Equals(connectionPoint) && otherConnection.direction == connection.direction.Opposite()) {
                        return new Pair<Piece, Piece.Connection>(otherPiece, otherLocalConnection);
                    }
                }
            }
        }
        return null;
    }

    public void TickPieceWounds() {
        for (int pieceIdx = 0; pieceIdx < this.pieces.Count; ++pieceIdx) {
            Piece piece = this.pieces[pieceIdx];
            Placement placement = this.placements[pieceIdx];
            uint wounds = 0;

            // If the board is inorganic, there is always a wound by default.
            if (placement.board.type != Board.Type.Organic) {
                wounds += 1;
            }

            foreach (Piece.Connection localConnection in piece.GetConnections()) {
                bool boardConnection = this.GetPieceConnectionBoard(piece, placement, localConnection) != null;
                if (!boardConnection) {
                    bool pieceConnection = this.GetPieceConnectionPiece(piece, placement, localConnection) != null;
                    if (!pieceConnection) {
                        wounds += 1;
                    }
                }
            }
            piece.health.wounds = wounds;
        }
    }

    // Updates the health of a single piece.
    void TickPieceHealth(Piece piece) {
        // Update the health based on number of good connections.
        if (piece.health.wounds != 0 && piece.health.alive) {
            piece.health.health -= 1;
            if (piece.health.health < 0) {
                piece.health.health = 0;
                piece.health.alive = false;
            }
        }
    }

    public enum State {
        InProgress,
        Won,
        Lost,
    }

}
