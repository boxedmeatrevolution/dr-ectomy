using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceComponent : MonoBehaviour, Piecable {

    // Used for construction only.
    public bool[] shape = new bool[1] { true };
    public int width = 1;
    public Health health = new Health(3);

    public GameObject bloodSplatterPrefab;
    public GameObject miasmaSplatterPrefab;

    public WinCondition winConditionScript;
    public LoseCondition loseConditionScript;

    private float splatterTiming = 0.0f;

    private Piece piece;
    private Piece.Connection[] connections;
    private ConnectorComponent[] connectorComponents;

    private BeatComponent beat;

    public ConnectorComponent[] GetConnectorComponents() {
        return this.connectorComponents;
    }

    private XRayButtonComponent xrayButton;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer shadowRenderer;

    public bool highlight = false;

    // The transform used for game logic.
    public Vector3 logicalPosition;
    public Quaternion logicalRotation;

    // Jiggly effect.
    private float jigglePos = 0.0f;
    private float jiggleTime = 10.0f;

	void Start () {
        this.xrayButton = GameObject.FindObjectOfType<XRayButtonComponent>();
        this.logicalPosition = this.transform.localPosition;
        this.logicalRotation = this.transform.localRotation;
        this.beat = GameObject.FindObjectOfType<BeatComponent>();

        this.health.maxHealth = this.health.health;

        Component[] components = this.GetComponents<Component>();
        foreach (Component component in components) {
            if (component is WinCondition) {
                this.winConditionScript = (WinCondition)component;
            }
            if (component is LoseCondition) {
                this.loseConditionScript = (LoseCondition)component;
            }
        }

        if (this.winConditionScript == null) {
            this.winConditionScript = new DefaultWinCondition();
        }
        if (this.loseConditionScript == null) {
            this.loseConditionScript = new DefaultLoseCondition();
        }
        
        SpriteRenderer[] renderers = this.gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer renderer in renderers) {
            if (renderer.name.Contains(this.name)) {
                if (renderer.name.Contains("Sprite")) {
                    this.spriteRenderer = renderer;
                }
                if (renderer.name.Contains("Shadow")) {
                    this.shadowRenderer = renderer;
                }
            }
        }

        if (this.winConditionScript == null) {
            this.winConditionScript = new DefaultWinCondition();
        }
        if (this.loseConditionScript == null) {
            this.loseConditionScript = new DefaultLoseCondition();
        }

        int height = this.shape.Length / this.width;
        bool[,] trueShape = new bool[height, this.width];
        for (int i = 0; i < height; ++i) {
            for (int j = 0; j < this.width; ++j) {
                trueShape[i, j] = this.shape[i * this.width + j];
            }
        }

        this.connectorComponents = this.GetComponentsInChildren<ConnectorComponent>();
        this.connections = new Piece.Connection[connectorComponents.Length];
        for (int i = 0; i < connectorComponents.Length; ++i) {
            ConnectorComponent connectorComponent = this.connectorComponents[i];
            this.connections[i] = connectorComponent.connection;
        }
        this.piece = new Piece(trueShape, this.connections, health, winConditionScript.Invoke, loseConditionScript.Invoke);
    }

    void Update() {
        this.transform.localPosition = Vector3.Slerp(this.transform.localPosition, this.logicalPosition, 20.0f * Time.deltaTime);
        this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, this.logicalRotation, 20.0f * Time.deltaTime);

        // Update the jiggle!
        float omega = 50.0f;
        float decayTime = 0.2f;
        this.jiggleTime += Time.deltaTime;
        if (this.jiggleTime < 5.0f) {
            this.jigglePos = 0.1f * Mathf.Exp(-this.jiggleTime / decayTime) * Mathf.Sin(omega * this.jiggleTime);
        }
        else {
            this.jigglePos = 0.0f;
        }

        // Update brightness of the sprite.
        float t = 1.0f - (this.transform.position.z - (-1.0f)) / 2.0f;
        float a = 1.0f;
        float v = t < 0.5f ? Mathf.Lerp(0.30f, 0.80f, 2.0f * t) : Mathf.Lerp(0.80f, 1.0f, 2.0f * (t - 0.5f));
        if (this.xrayButton.pressed) {
            a = Mathf.Lerp(1.0f, 0.10f, 2.0f * t);
        }
        if (this.highlight) {
            v = 1.0f;
        }
        float scaleFactor = t > 0.5f ? Mathf.Lerp(1.0f, 1.1f, 2.0f * (t - 0.5f)) : 1.0f;

        float beatFactor = (this.piece.health.wounds == 0 && this.piece.health.alive) ? (1 + 0.05f * this.beat.GetBeat()) : 1.0f;

        float scaleX = scaleFactor * (1 + this.jigglePos) * beatFactor;
        float scaleY = scaleFactor * (1 - this.jigglePos) * beatFactor;

        this.transform.localScale = new Vector3(scaleX, scaleY, 1.0f);
        this.spriteRenderer.color = new Color(v, v, v, a);

        // Update the shadow brightness.
        Color shadowColor = new Color(1.0f, 1.0f, 1.0f, Mathf.Lerp(0.25f, 0.0f, 2.0f * t));
        this.shadowRenderer.color = shadowColor;

        bool miasma = !this.piece.health.alive;
        bool wound = !miasma && (this.piece.health.wounds != 0);

        if (miasma || wound) {
            splatterTiming -= Time.deltaTime;
            if (splatterTiming < 0.0f) {
                splatterTiming = 0.4f;
                Vector3 position = new Vector3(
                    this.piece.GetWidth() * Constant.TILE_WIDTH * UnityEngine.Random.Range(-0.5f, 0.5f),
                    this.piece.GetHeight() * Constant.TILE_WIDTH * UnityEngine.Random.Range(-0.5f, 0.5f),
                    -0.1f
                );
                position = this.transform.TransformPoint(position);
                // If dead, make some miasma
                if (miasma) {
                    GameObject miasmaObj = GameObject.Instantiate(this.miasmaSplatterPrefab);
                    miasmaObj.transform.localPosition = position;
                    SpriteRenderer spriteRenderer = miasmaObj.GetComponentInChildren<SpriteRenderer>();
                    spriteRenderer.color = new Color(v, v, v, a);
                }
                // If wounded, make some blood.
                else if (wound) {
                    GameObject bloodObj = GameObject.Instantiate(this.bloodSplatterPrefab);
                    bloodObj.transform.localPosition = position;
                    SpriteRenderer spriteRenderer = bloodObj.GetComponentInChildren<SpriteRenderer>();
                    spriteRenderer.color = new Color(v, v, v, 0.5f * a);
                }
            }
        }
    }

    public SpriteRenderer GetSpriteRenderer() {
        return this.spriteRenderer;
    }

    public void Jiggle(float amount) {
        this.jiggleTime = 0.0f;
    }

    public Piece GetPiece() {
        return this.piece;
    }

    public Vector3 GetLogicalPosition() {
        return this.logicalPosition;
    }

    public Quaternion GetLogicalRotation() {
        return this.logicalRotation;
    }

    public void SetLogicalPosition(Vector3 position) {
        this.logicalPosition = position;
    }

    public void SetLogicalRotation(Quaternion rotation) {
        this.logicalRotation = rotation;
    }
}
