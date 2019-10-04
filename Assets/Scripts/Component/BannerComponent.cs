using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerComponent : MonoBehaviour
{

    private new SpriteRenderer renderer;
    private float alpha = 0.0f;

    void Start() {
        this.renderer = this.GetComponentInChildren<SpriteRenderer>();
        this.renderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    }
    
    void Update() {
        this.alpha += 2.0f * Time.deltaTime;
        this.alpha = Mathf.Clamp(this.alpha, 0.0f, 1.0f);
        this.renderer.color = new Color(1.0f, 1.0f, 1.0f, this.alpha);
    }

}
