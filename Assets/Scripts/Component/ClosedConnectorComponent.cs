using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosedConnectorComponent : MonoBehaviour
{

    public ConnectorComponent startConnector;
    public ConnectorComponent endConnector;

    public BoardComponent boardComponent;
    public Vector3 direction;

    private new SpriteRenderer renderer;

    private BeatComponent beat;

    public void PositionForPieces() {
        Vector3 startPos = this.startConnector.transform.position;
        Vector3 endPos = this.endConnector.transform.position;
        Vector3 delta = endPos - startPos;

        float distance = delta.magnitude;
        float angle = Mathf.Rad2Deg * Mathf.Atan2(delta.y, delta.x);

        this.renderer = this.GetComponentInChildren<SpriteRenderer>();

        float beatFactor = 1.0f;
        if (this.beat != null) {
            beatFactor = (1 + 0.05f * this.beat.GetBeat());
        }

        this.transform.position = this.startConnector.transform.position + new Vector3(0.0f, 0.0f, -0.2f);
        this.transform.localScale = new Vector3(beatFactor * distance / 2.0f, beatFactor, 1.0f);
        this.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, angle);
    }

    public void PositionForBoard() {
        Quaternion rotation = this.startConnector.GetPieceComponent().transform.rotation;
        Vector3 delta = rotation * this.direction * Constant.TILE_WIDTH;

        float distance = delta.magnitude;
        float angle = Mathf.Rad2Deg * Mathf.Atan2(delta.y, delta.x);

        float beatFactor = 1.0f;
        if (this.beat != null) {
            beatFactor = (1 + 0.05f * this.beat.GetBeat());
        }

        this.transform.position = this.startConnector.transform.position + new Vector3(0.0f, 0.0f, -0.2f);
        this.transform.localScale = new Vector3(beatFactor * distance / 2.0f, beatFactor, 1.0f);
        this.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, angle);
    }

    void Start() {
        this.renderer = this.GetComponentInChildren<SpriteRenderer>();
        this.beat = GameObject.FindObjectOfType<BeatComponent>();
    }
    
    void Update() {
        if (!this.renderer.enabled) {
            this.renderer.enabled = true;
        }
        if (this.startConnector != null) {
            this.renderer.color = this.startConnector.GetPieceComponent().GetSpriteRenderer().color;
        }
        if (this.endConnector != null) {
            this.PositionForPieces();
        }
        else if (this.boardComponent != null) {
            this.PositionForBoard();
        }
    }
}
