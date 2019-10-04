using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectorComponent : MonoBehaviour
{

    public Piece.Connection connection;

    public GameObject closedConnectorPrefab;
    public GameObject splatterPrefab;
    public GameObject splatterBlobPrefab;

    private PieceComponent parentPiece;
    private ClosedConnectorComponent closedConnectorComponent = null;
    private ConnectorComponent otherConnectorComponent = null;

    private GameObject emitter;

    private new SpriteRenderer renderer;

    public PieceComponent GetPieceComponent() {
        return this.parentPiece;
    }

    void Start() {
        this.parentPiece = this.GetComponentInParent<PieceComponent>();
        this.renderer = this.GetComponentInChildren<SpriteRenderer>();
        this.emitter = this.transform.Find("Emitter").gameObject;
    }
    
    void Update() {
        this.renderer.color = this.parentPiece.GetSpriteRenderer().color;

        // Emit particles.
        if (this.closedConnectorComponent == null) {
            if (this.parentPiece.GetPiece().health.alive) {
                if (Random.Range(0.0f, 1.0f) < 0.2f) {
                    GameObject splatterObj = GameObject.Instantiate(Random.Range(0.0f, 1.0f) < 0.75f ? this.splatterPrefab : this.splatterBlobPrefab);
                    SplatterComponent splatter = splatterObj.GetComponent<SplatterComponent>();
                    splatter.direction = this.transform.TransformDirection(new Vector3(1.0f, 0.0f, 0.0f));
                    splatter.transform.localPosition = this.emitter.transform.position + new Vector3(0.0f, 0.0f, -0.2f);
                    SpriteRenderer spriteRenderer = splatter.GetComponentInChildren<SpriteRenderer>();
                    spriteRenderer.color = this.renderer.color;
                }
            }
        }
    }

    public void Sever() {
        if (this.otherConnectorComponent != null) {
            this.otherConnectorComponent.renderer.enabled = true;
            this.otherConnectorComponent.closedConnectorComponent = null;
            this.otherConnectorComponent.otherConnectorComponent = null;
        }
        if (this.closedConnectorComponent != null) {
            GameObject.Destroy(this.closedConnectorComponent.gameObject);
        }
        this.renderer.enabled = true;
        this.closedConnectorComponent = null;
        this.otherConnectorComponent = null;
    }

    public void ConnectToPiece(ConnectorComponent otherConnectorComponent) {
        if (this.otherConnectorComponent == otherConnectorComponent) {
            // Already connected.
            return;
        }

        this.Sever();

        GameObject closedConnectorObject = GameObject.Instantiate(this.closedConnectorPrefab);

        this.renderer.enabled = false;
        this.closedConnectorComponent = closedConnectorObject.GetComponent<ClosedConnectorComponent>();
        this.otherConnectorComponent = otherConnectorComponent;

        this.otherConnectorComponent.renderer.enabled = false;
        this.otherConnectorComponent.closedConnectorComponent = this.closedConnectorComponent;
        this.otherConnectorComponent.otherConnectorComponent = this;

        this.closedConnectorComponent.GetComponentInChildren<SpriteRenderer>().enabled = false;
        this.closedConnectorComponent.startConnector = this;
        this.closedConnectorComponent.endConnector = this.otherConnectorComponent;
        this.closedConnectorComponent.PositionForPieces();
    }

    public void ConnectToBoard(BoardComponent boardComponent) {
        this.Sever();

        GameObject closedConnectorObject = GameObject.Instantiate(this.closedConnectorPrefab);

        this.renderer.enabled = false;
        this.closedConnectorComponent = closedConnectorObject.GetComponent<ClosedConnectorComponent>();
        this.closedConnectorComponent.GetComponentInChildren<SpriteRenderer>().enabled = false;
        this.closedConnectorComponent.startConnector = this;
        this.closedConnectorComponent.boardComponent = boardComponent;
        Point dir = this.connection.direction.GetUnitVector();
        this.closedConnectorComponent.direction = new Vector3(dir.x, dir.y, dir.z);
        this.closedConnectorComponent.PositionForBoard();
    }
}
