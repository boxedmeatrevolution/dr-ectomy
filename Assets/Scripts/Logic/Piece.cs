using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

[Serializable]
public class Piece {

    private readonly bool[,] shape;
    private readonly Connection[] connections;
    public Health health;
    public readonly WinCondition winCondition;
    public readonly LoseCondition loseCondition;

    public Piece(bool[,] shape, IEnumerable<Connection> connections, Health health) :
        this(shape, connections, health,
        (Piece piece, Placement placement) => piece.health.wounds == 0 && piece.health.alive && placement.board.type == Board.Type.Organic,
        (Piece piece, Placement placement) => !piece.health.alive) {
    }

    public Piece(bool[,] shape, IEnumerable<Connection> connections, Health health, WinCondition winCondition, LoseCondition loseCondition) {
        this.shape = (bool[,]) shape.Clone();
        this.connections = connections.Select((Connection connection) => {
            connection.position.z = 0;
            return connection;
        }).ToArray();
        this.health = health;
        this.winCondition = winCondition;
        this.loseCondition = loseCondition;

        foreach (Connection connection in this.connections) {
            // Check that the connection is valid.
            Point connectionStart = connection.position;
            Point connectionEnd = connection.position + connection.direction.GetUnitVector();
            if (!this.IsFilled(connectionStart) || this.IsFilled(connectionEnd)) {
                throw new ArgumentException("Connection must be on edge of piece");
            }
        }
    }

    public int GetWidth() {
        return shape.GetLength(1);
    }

    public int GetHeight() {
        return shape.GetLength(0);
    }

    public Rectangle GetBoundingBox() {
        return new Rectangle(new Point(), new Point(this.GetWidth(), this.GetHeight(), 1));
    }

    public bool IsFilled(Point point) {
        if (point.x >= 0 && point.x < this.GetWidth() && point.y >= 0 && point.y < this.GetHeight() && point.z == 0) {
            return this.shape[point.y, point.x];
        }
        else {
            return false;
        }
    }

    public ReadOnlyCollection<Connection> GetConnections() {
        return Array.AsReadOnly(this.connections);
    }

    [Serializable]
    public struct Connection {

        public Connection(Point position, Direction direction) {
            this.position = position;
            this.direction = direction;
        }

        public Point position;
        public Direction direction;

    }

    public delegate bool WinCondition(Piece piece, Placement placement);
    public delegate bool LoseCondition(Piece piece, Placement placement);

}
