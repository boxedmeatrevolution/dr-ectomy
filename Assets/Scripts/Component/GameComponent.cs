using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameComponent : MonoBehaviour {

    private Game game;
    private PieceComponent[] pieceComponents;
    private BoardComponent[] boardComponents;

    private new Camera camera;
    private InputComponent input;
    private ClipboardComponent clipboard;
    private RestartButtonComponent restartButton;
    private NextLevelComponent nextLevel;
    private PreviousLevelComponent previousLevel;
    private XRayButtonComponent xray;
    private LineRenderer lineRenderer;

    public PieceComponent highlightedPiece;

    private Mode mode = Mode.Setup;

    public bool NO_LOSING = false;

    public int moveCount = 0;

    // The piece currently being moved by the player and the placement it had before it started moving.
    private PieceComponent activePieceComponent;
    private Placement activePieceOldPlacement;
    private float clipboardTimer;
    private float gameOverTimer;
    private float placementTimer;
    
    public GameObject winBanner;
    public GameObject loseBanner;
    public GameObject healthPrefab;

    void Start () {
        // Check for each object with a Board or Piece and use them to create the initial state.
        this.boardComponents = GameObject.FindObjectsOfType<BoardComponent>();
        this.pieceComponents = GameObject.FindObjectsOfType<PieceComponent>();
        this.camera = GameObject.FindObjectOfType<Camera>();
        this.input = GameObject.FindObjectOfType<InputComponent>();
        this.clipboard = GameObject.FindObjectOfType<ClipboardComponent>();
        this.xray = GameObject.FindObjectOfType<XRayButtonComponent>();
        this.lineRenderer = this.GetComponent<LineRenderer>();

        this.game = new Game();
	}
	
	void Update () {

        bool rehighlight = false;
        bool highlight = false;

        if (this.mode == Mode.Setup) {
            this.restartButton = GameObject.FindObjectOfType<RestartButtonComponent>();
            this.nextLevel = GameObject.FindObjectOfType<NextLevelComponent>();
            this.previousLevel = GameObject.FindObjectOfType<PreviousLevelComponent>();
            // First place the bones.
            BlockComponent[] blockComponents = GameObject.FindObjectsOfType<BlockComponent>();
            foreach (BlockComponent blockComponent in blockComponents) {
                Piece piece = blockComponent.GetPiece();
                bool placed = false;
                foreach (BoardComponent boardComponent in this.boardComponents) {
                    Placement placement = boardComponent.GetPlacement(blockComponent);
                    if (this.game.CheckPlacementFree(piece, placement)) {
                        this.game.ForcePlace(piece, placement);
                        placed = true;
                        break;
                    }
                }
                if (!placed) {
                    throw new Exception("Couldn't place block");
                }
                this.SnapPieceToBoard(blockComponent);
            }

            foreach (PieceComponent pieceComponent in this.pieceComponents) {
                Piece piece = pieceComponent.GetPiece();
                bool placed = false;
                foreach (BoardComponent boardComponent in this.boardComponents) {
                    Placement placement = boardComponent.GetPlacement(pieceComponent);
                    placement.transform.offset.z = Mathf.RoundToInt(pieceComponent.logicalPosition.z / Constant.TILE_WIDTH);
                    if (this.game.CheckPlacementFree(piece, placement)) {
                        this.game.ForcePlace(piece, placement);
                        placed = true;
                        break;
                    }
                }
                if (!placed) {
                    throw new Exception("Couldn't place piece");
                }
                this.SnapPieceToBoard(pieceComponent);
            }

            // Update connections.
            foreach (PieceComponent pieceComponent in this.pieceComponents) {
                this.UpdateConnectors(pieceComponent);
            }

            // Give everyone a health indicator.
            foreach (PieceComponent pieceComponent in this.pieceComponents) {
                GameObject healthObject = GameObject.Instantiate(this.healthPrefab);
                HealthComponent healthComponent = healthObject.GetComponent<HealthComponent>();
                Vector3 newPosition = pieceComponent.transform.localPosition;
                newPosition.z = healthObject.transform.localPosition.z;
                healthObject.transform.localPosition = newPosition;
                healthComponent.pieceComponent = pieceComponent;
            }

            this.game.TickPieceWounds();

            this.clipboardTimer = 0.5f;
            this.clipboard.zoomed = true;
            this.clipboard.transform.localPosition = new Vector3(0.0f, 0.0f, this.clipboard.zoomedZ);
            this.clipboard.transform.localRotation = Quaternion.identity;
            this.clipboard.transform.localScale = new Vector3(this.clipboard.zoomedScale, this.clipboard.zoomedScale, 1.0f);
            this.mode = Mode.Clipboard;
        }
        else if (this.mode == Mode.Clipboard) {
            if (this.clipboardTimer > 0.0f) {
                this.clipboardTimer -= Time.deltaTime;
            }
            else if (this.input.clickedPlaceNow) {
                this.clipboard.zoomed = false;
                this.mode = Mode.Selecting;
            }
        }
        else if (this.mode == Mode.Selecting) {
            // Look for mouse clicks to indicate starting to drag a piece.
            Collider2D collider = Physics2D.OverlapPoint(this.camera.ScreenToWorldPoint(Input.mousePosition), LayerMask.GetMask("Pieces"));
            if (collider != null) {
                PieceComponent pieceComponent = collider.gameObject.GetComponent<PieceComponent>();
                if (this.input.clickedPlaceNow) {
                    Piece piece = pieceComponent.GetPiece();
                    this.activePieceComponent = pieceComponent;
                    this.activePieceOldPlacement = this.game.GetPlacement(piece);

                    bool canTake = this.game.Take(piece);
                    if (canTake) {
                        this.SeverConnectors(this.activePieceComponent);
                        this.mode = Mode.Moving;
                        // Disable x-ray if it is on.
                        if (this.xray.pressed) {
                            this.xray.pressed = false;
                            foreach (PieceComponent pieceyComponent in this.pieceComponents) {
                                Collider2D[] colliders = pieceyComponent.GetComponentsInChildren<Collider2D>();
                                foreach (Collider2D locCollider in colliders) {
                                    locCollider.enabled = true;
                                }
                            }
                        }
                    }
                    else {
                        this.activePieceComponent = null;
                        pieceComponent.Jiggle(0.2f);
                    }
                }
                else {
                    if (pieceComponent == this.highlightedPiece) {
                        rehighlight = true;
                    }
                    else if (this.highlightedPiece != null) {
                        this.highlightedPiece.highlight = false;
                    }
                    this.highlightedPiece = pieceComponent;
                    this.highlightedPiece.highlight = true;
                    highlight = true;
                }
            }

            if (this.input.clickedPlaceNow) {
                collider = Physics2D.OverlapPoint(this.camera.ScreenToWorldPoint(Input.mousePosition), LayerMask.GetMask("Buttons"));
                if (collider != null) {
                    // Clicking the clipboard.
                    if (collider.gameObject == this.clipboard.gameObject) {
                        this.clipboardTimer = 0.25f;
                        this.clipboard.zoomed = true;
                        this.mode = Mode.Clipboard;
                    }
                    // Clicking the x-ray button.
                    if (collider.gameObject == this.xray.gameObject) {
                        this.xray.pressed = !this.xray.pressed;
                        if (this.xray.pressed) {
                            // If pressed, disable collisions for pieces on upper level.
                            foreach (PieceComponent pieceComponent in this.pieceComponents) {
                                if (this.game.GetPlacement(pieceComponent.GetPiece()).transform.offset.z == 0) {
                                    Collider2D[] colliders = pieceComponent.GetComponentsInChildren<Collider2D>();
                                    foreach (Collider2D locCollider in colliders) {
                                        locCollider.enabled = false;
                                    }
                                }
                            }
                        }
                        else {
                            // Otherwise, enable all collisions.
                            foreach (PieceComponent pieceComponent in this.pieceComponents) {
                                Collider2D[] colliders = pieceComponent.GetComponentsInChildren<Collider2D>();
                                foreach (Collider2D locCollider in colliders) {
                                    locCollider.enabled = true;
                                }
                            }
                        }
                    }
                    //Clicking restart button.
                    if (collider.gameObject == this.restartButton.gameObject) {
                        int buildIndex = SceneManager.GetActiveScene().buildIndex;
                        SceneManager.LoadScene(buildIndex);
                    }
                    if (collider.gameObject == this.nextLevel.gameObject) {
                        int buildIndex = SceneManager.GetActiveScene().buildIndex + 1;
                        if (buildIndex >= SceneManager.sceneCountInBuildSettings) {
                            buildIndex = 0;
                        }
                        SceneManager.LoadScene(buildIndex);
                    }
                    if (collider.gameObject == this.previousLevel.gameObject) {
                        int buildIndex = SceneManager.GetActiveScene().buildIndex - 1;
                        if (buildIndex <= 0) {
                            buildIndex = 0;
                        }
                        SceneManager.LoadScene(buildIndex);

                    }
                }
            }
        }
        else if (this.mode == Mode.Moving) {
            Vector3 newPosition = this.camera.ScreenToWorldPoint(Input.mousePosition);
            newPosition.z = -1.0f;
            this.activePieceComponent.logicalPosition = newPosition;

            // Rotate the piece if asked.
            int currentAngle = Mathf.RoundToInt(this.activePieceComponent.logicalRotation.eulerAngles.z / 90.0f);
            if (this.input.clickedRotateForwardNow) {
                currentAngle += 1;
                this.activePieceComponent.logicalRotation = Quaternion.Euler(0.0f, 0.0f, currentAngle * 90.0f);
            }
            if (this.input.clickedRotateReverseNow) {
                currentAngle -= 1;
                this.activePieceComponent.logicalRotation = Quaternion.Euler(0.0f, 0.0f, currentAngle * 90.0f);
            }

            if (this.input.clickedPlaceNow) {
                // Try to place the active piece. Keep track of the piece it collides with (if any).
                Piece piece = this.activePieceComponent.GetPiece();
                Piece collisionPiece = null;
                Placement? placement = null;
                foreach (BoardComponent boardComponent in this.boardComponents) {
                    Placement placementTest = boardComponent.GetPlacement(this.activePieceComponent);
                    placement = this.game.PlaceSoft(piece, placementTest);
                    if (placement == null) {
                        Board nextCollisionBoard = this.game.GetPlacementCollisionBoard(piece, placementTest);
                        Piece nextCollisionPiece = this.game.GetPlacementCollisionPiece(piece, placementTest);
                        if (nextCollisionBoard == null && nextCollisionPiece != null) {
                            collisionPiece = nextCollisionPiece;
                        }
                    }
                    if (placement != null) {
                        break;
                    }
                }

                // See if we succeeded in placement.
                if (placement == null) {
                    // We didn't. Try again.
                    this.game.ForcePlace(piece, this.activePieceOldPlacement);
                    if (collisionPiece != null) {
                        PieceComponent otherPieceComponent = this.GetPieceComponentFromPiece(collisionPiece);
                        if (otherPieceComponent != null) {
                            otherPieceComponent.Jiggle(1.0f);
                        }
                    }
                    this.activePieceComponent.Jiggle(1.0f);
                    this.SnapPieceToBoard(this.activePieceComponent);
                    this.activePieceComponent = null;
                    this.mode = Mode.Selecting;
                }
                else if (placement.Equals(this.activePieceOldPlacement)) {
                    // We did, but it's in the spot we took it in so it didn't do anything.
                    this.SnapPieceToBoard(this.activePieceComponent);
                    this.UpdateConnectors(this.activePieceComponent);
                    this.activePieceComponent = null;
                    this.mode = Mode.Selecting;
                }
                else {
                    // We succeeded!
                    this.moveCount += 1;
                    this.SnapPieceToBoard(this.activePieceComponent);
                    this.UpdateConnectors(this.activePieceComponent);
                    this.game.TickPieceWounds();
                    this.activePieceComponent = null;
                    this.mode = Mode.Placing;
                    this.placementTimer = 0.5f;
                }
            }
        }
        else if (this.mode == Mode.Placing) {
            // When we are placing, basically that means waiting on a short timer for some animations, and
            // for the HP of things to tick down.
            this.placementTimer -= Time.deltaTime;
            if (this.placementTimer < 0.0f) {
                Game.State gameState = this.game.Tick();
                if (gameState == Game.State.InProgress || this.NO_LOSING) {
                    this.mode = Mode.Selecting;
                }
                else if (gameState == Game.State.Lost) {
                    this.gameOverTimer = 2.0f;
                    GameObject.Instantiate(this.loseBanner);
                    this.mode = Mode.Lost;
                }
                else if (gameState == Game.State.Won) {
                    this.gameOverTimer = 2.0f;
                    GameObject.Instantiate(this.winBanner);
                    this.mode = Mode.Won;
                }
            }
        }
        else if (this.mode == Mode.Lost) {
            if (this.gameOverTimer > 0.0f) {
                this.gameOverTimer -= Time.deltaTime;
            }
            else if (this.input.clickedPlaceNow) {
                int buildIndex = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(buildIndex);
            }
        }
        else if (this.mode == Mode.Won) {
            if (this.gameOverTimer > 0.0f) {
                this.gameOverTimer -= Time.deltaTime;
            }
            else if (this.input.clickedPlaceNow) {
                int buildIndex = SceneManager.GetActiveScene().buildIndex + 1;
                if (buildIndex >= SceneManager.sceneCountInBuildSettings) {
                    buildIndex = 0;
                }
                SceneManager.LoadScene(buildIndex);
            }
        }

        // Draw the selection outline.
        if (!highlight) {
            if (this.highlightedPiece != null) {
                this.highlightedPiece.highlight = false;
                this.highlightedPiece = null;
            }
            this.lineRenderer.positionCount = 0;
        }
        // Not rehighlighting, so change stuff up.
        if (!rehighlight && highlight) {
            // First find the path in the game space.
            Piece piece = this.highlightedPiece.GetPiece();
            List<Pair<Point, Point>> segments = new List<Pair<Point, Point>>();
            for (int i = 0; i < piece.GetWidth(); ++i) {
                for (int j = 0; j < piece.GetHeight(); ++j) {
                    Point point = new Point(i, j, 0);
                    if (piece.IsFilled(point)) {
                        Point left = new Point(i - 1, j, 0);
                        Point right = new Point(i + 1, j, 0);
                        Point up = new Point(i, j + 1, 0);
                        Point down = new Point(i, j - 1, 0);
                        Point up_right = new Point(i + 1, j + 1, 0);
                        if (!piece.IsFilled(left)) {
                            segments.Add(new Pair<Point, Point>(point, up));
                        }
                        if (!piece.IsFilled(right)) {
                            segments.Add(new Pair<Point, Point>(right, up_right));
                        }
                        if (!piece.IsFilled(up)) {
                            segments.Add(new Pair<Point, Point>(up, up_right));
                        }
                        if (!piece.IsFilled(down)) {
                            segments.Add(new Pair<Point, Point>(point, right));
                        }
                    }
                }
            }
            // Now that we have all segments, condense them into a point list.
            List<Point> loop = new List<Point>();
            loop.Add(segments.First().a);
            while (segments.Count != 0) {
                Point current = loop.Last();
                for (int i = 0; i < segments.Count; ++i) {
                    Pair<Point, Point> segment = segments[i];
                    if (segment.a.Equals(current)) {
                        loop.Add(segment.b);
                        segments.RemoveAt(i);
                        break;
                    }
                    if (segment.b.Equals(current)) {
                        loop.Add(segment.a);
                        segments.RemoveAt(i);
                        break;
                    }
                }
            }

            // Now transform each position into the world space.
            List<Vector3> looop = new List<Vector3>();
            for (int i = 0; i < loop.Count; ++i) {
                Point point = loop[i];
                Vector3 version = Constant.TILE_WIDTH * new Vector3(point.x, point.y, 0.0f);
                Vector3 dimensions = Constant.TILE_WIDTH * new Vector3(piece.GetWidth(), piece.GetHeight(), 0.0f);
                version -= 0.5f * dimensions;
                version = this.highlightedPiece.GetLogicalRotation() * version;
                version += this.highlightedPiece.GetLogicalPosition();
                looop.Add(version);
            }
            looop.RemoveAt(looop.Count - 1);

            this.lineRenderer.positionCount = looop.Count;
            this.lineRenderer.SetPositions(looop.ToArray());
        }
	}

    // Severs all connectors attached to a piece.
    public void SeverConnectors(PieceComponent pieceComponent) {
        ConnectorComponent[] connectorComponents = pieceComponent.GetConnectorComponents();
        foreach (ConnectorComponent connectorComponent in connectorComponents) {
            connectorComponent.Sever();
        }
    }

    // Goes through all connectors for a piece and checks whether they are connected.
    public void UpdateConnectors(PieceComponent pieceComponent) {
        Piece piece = pieceComponent.GetPiece();
        Placement placement = this.game.GetPlacement(piece);
        ConnectorComponent[] connectorComponents = pieceComponent.GetConnectorComponents();
        foreach (ConnectorComponent connectorComponent in connectorComponents) {
            connectorComponent.Sever();
            // Check whether a connection exists, and if so, to what.
            Piece.Connection connection = connectorComponent.connection;
            Board boardConnection = this.game.GetPieceConnectionBoard(piece, placement, connection);
            Pair<Piece, Piece.Connection>? pair = this.game.GetPieceConnectionPiece(piece, placement, connection);
            if (boardConnection != null) {
                BoardComponent boardComponent = this.GetBoardComponentFromBoard(boardConnection);
                connectorComponent.ConnectToBoard(boardComponent);
            }
            if (pair != null) {
                PieceComponent otherPieceComponent = this.GetPieceComponentFromPiece(pair.Value.a);
                Piece.Connection otherConnection = pair.Value.b;
                int otherConnectionIdx = otherPieceComponent.GetPiece().GetConnections().IndexOf(otherConnection);
                ConnectorComponent otherConnectorComponent = otherPieceComponent.GetConnectorComponents()[otherConnectionIdx];
                otherConnectorComponent.ConnectToPiece(connectorComponent);
            }
        }
    }

    PieceComponent GetPieceComponentFromPiece(Piece piece) {
        foreach (PieceComponent pieceComponent in this.pieceComponents) {
            if (pieceComponent.GetPiece() == piece) {
                return pieceComponent;
            }
        }
        return null;
    }

    BoardComponent GetBoardComponentFromBoard(Board board) {
        foreach (BoardComponent boardComponent in this.boardComponents) {
            if (boardComponent.GetBoard() == board) {
                return boardComponent;
            }
        }
        return null;
    }

    // Forces a piece component to be in the correct position as it has been placed within the board.
    void SnapPieceToBoard<T>(T pieceComponent) where T: MonoBehaviour, Piecable {
        Piece piece = pieceComponent.GetPiece();
        Placement placement = this.game.GetPlacement(pieceComponent.GetPiece());
        Board board = placement.board;
        BoardComponent boardComponent = this.GetBoardComponentFromBoard(board);
        float angle = ((float)((int)placement.transform.rotation) * 90);

        Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, angle);
        Vector3 relativePosition = Constant.TILE_WIDTH * new Vector3(
            placement.transform.offset.x,
            placement.transform.offset.y,
            placement.transform.offset.z);
        relativePosition += rotation * (Constant.TILE_WIDTH * new Vector3(
            piece.GetWidth() / 2.0f,
            piece.GetHeight() / 2.0f));

        Vector3 boardPosition = boardComponent.transform.position;
        boardPosition -= Constant.TILE_WIDTH * new Vector3(
            board.boundary.GetDimensions().x / 2.0f,
            board.boundary.GetDimensions().y / 2.0f);
        boardPosition.z = 0.0f;

        pieceComponent.SetLogicalRotation(rotation);
        pieceComponent.SetLogicalPosition(relativePosition + boardPosition);
    }

    public Game GetGame() {
        return this.game;
    }

    private enum Mode {
        // Just starting.
        Setup,
        // Clipboard mode--show the goals.
        Clipboard,
        // Player is current dragging a piece around.
        Moving,
        // Player has just released a piece and the board is updating.
        Placing,
        // Board has finished updating and waiting for next piece to be clicked.
        Selecting,
        // Level has been lost.
        Lost,
        // Level has been won.
        Won,
    }

}
