using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{

    public PieceComponent pieceComponent;

    public Material sickMaterial;

    private SpriteRenderer topRenderer;
    private SpriteRenderer bottomRenderer;
    private TextMeshPro textMesh;

    private Vector2 initialSize;

    private float level = 1.0f;
    private float highlightZ;

    void Start() {
        SpriteRenderer[] renderers = this.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer renderer in renderers) {
            if (renderer.name.Equals("HealthSpriteTop")) {
                this.topRenderer = renderer;
            }
            else if (renderer.name.Equals("HealthSpriteBottom")) {
                this.bottomRenderer = renderer;
            }
        }
        this.highlightZ = this.transform.localPosition.z;
        this.textMesh = this.GetComponentInChildren<TextMeshPro>();
        this.initialSize = this.topRenderer.size;
    }
    
    void Update() {

        Vector3 position = this.transform.localPosition;
        Vector3 nextPosition = this.pieceComponent.transform.localPosition;
        if (this.pieceComponent.highlight) {
            nextPosition.z = highlightZ;
        }
        else {
            nextPosition.z -= 0.2f;
        }
        this.transform.localPosition = nextPosition;

        int health = this.pieceComponent.GetPiece().health.health;
        int maxHealth = this.pieceComponent.GetPiece().health.maxHealth;
        bool wounded = this.pieceComponent.GetPiece().health.wounds != 0;
        bool alive = this.pieceComponent.GetPiece().health.alive;

        if (health > 50) {
            this.pieceComponent.GetPiece().health.health = 100;
            this.topRenderer.enabled = false;
            this.bottomRenderer.enabled = false;
            this.textMesh.enabled = false;
        }

        float fraction = ((float)health + 1) / ((float)maxHealth + 1);
        float scaleFactor = 1.0f;

        Color textMeshColor;

        if (alive) {
            this.textMesh.text = (health + 1).ToString();
            textMeshColor = new Color(1.0f, 1.0f, 1.0f);
            if (wounded) {
                textMesh.fontStyle = FontStyles.Bold;
            }
            else {
                textMesh.fontStyle = FontStyles.Normal;
            }

            if (level != fraction) {
                level += 10.0f * Time.deltaTime * (fraction - level);
                scaleFactor = 1.0f + 0.8f * (level - fraction) * maxHealth;
            }
            this.topRenderer.material = this.pieceComponent.GetSpriteRenderer().material;
        }
        else {
            this.textMesh.text = ":(";
            textMeshColor = new Color(0.0f, 1.0f, 0.0f);
            level = 1.0f;
            this.topRenderer.material = this.sickMaterial;
        }

        this.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1.0f);

        Vector2 newSize = this.initialSize;
        newSize.y *= level;
        this.topRenderer.size = newSize;
        Vector3 newPosition = this.topRenderer.transform.localPosition;
        newPosition.y = -0.5f * this.topRenderer.transform.localScale.y * (this.initialSize.y - newSize.y);
        this.topRenderer.transform.localPosition = newPosition;

        
        float h, s, v;
        float rh, rs, rv;
        Color rendererColor = this.pieceComponent.GetSpriteRenderer().color;
        Color.RGBToHSV(textMeshColor, out h, out s, out v);
        Color.RGBToHSV(rendererColor, out rh, out rs, out rv);

        float alpha;
        if (!this.pieceComponent.highlight) {
            v = rv;
            alpha = rendererColor.a;
        }
        else {
            v = 1.0f;
            alpha = 1.0f;
        }
        textMeshColor = Color.HSVToRGB(h, s, v);
        rendererColor = Color.HSVToRGB(rh, rs, v);

        textMeshColor.a = alpha;
        rendererColor.a = alpha;

        this.textMesh.color = textMeshColor;
        this.topRenderer.color = rendererColor;
        this.bottomRenderer.color = rendererColor;
    }
}
