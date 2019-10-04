using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRayButtonComponent : MonoBehaviour
{
    public bool pressed = false;
    private SpriteRenderer buttonRenderer;
    private SpriteRenderer pressedButtonRenderer;

    void Start() {
        SpriteRenderer[] renderers = this.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer renderer in renderers) {
            if (renderer.gameObject.name.Equals("XRayButtonSprite")) {
                this.buttonRenderer = renderer;
            }
            else if (renderer.gameObject.name.Equals("XRayButtonPressedSprite")) {
                this.pressedButtonRenderer = renderer;
            }
        }
    }
    
    void Update() {
        this.buttonRenderer.enabled = !this.pressed;
        this.pressedButtonRenderer.enabled = this.pressed;
    }

}
