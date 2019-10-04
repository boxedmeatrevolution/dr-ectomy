using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplatterComponent : MonoBehaviour
{

    public Vector3 direction;

    public float vel;

    private float timer;
    private float maxTimer;
    private new SpriteRenderer renderer;

    private Vector3 velocity;
    private float rotationVelocity;
    private float initialScale;
    private Color initialColor;

    void Start() {
        this.maxTimer = 0.6f;
        this.timer = this.maxTimer;
        this.renderer = this.GetComponent<SpriteRenderer>();
        if (this.direction.magnitude > 0.01) {
            this.direction.Normalize();
            this.velocity = this.vel * this.direction + this.vel * new Vector3(
                Random.Range(-0.75f, 0.75f),
                Random.Range(-0.75f, 0.75f));
        }
        else {
            float mag = this.vel + this.vel * Random.Range(-0.75f, 0.75f);
            float dir = Random.Range(0.0f, 2.0f * Mathf.PI);
            this.velocity = mag * new Vector3(Mathf.Cos(dir), Mathf.Sin(dir), 0.0f);
        }
        this.rotationVelocity = Random.Range(-90.0f, 90.0f);
        this.initialScale = this.transform.localScale.x;
        this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f));
        this.initialColor = this.renderer.color;
    }
    
    void Update() {
        this.timer -= Time.deltaTime;
        if (this.timer < 0.0f) {
            GameObject.Destroy(this.gameObject);
        }

        this.transform.localPosition += Time.deltaTime * this.velocity;
        float angle = this.transform.localRotation.eulerAngles.z;
        angle += Time.deltaTime * rotationVelocity;
        this.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, angle);

        this.velocity -= 0.5f * Time.deltaTime * this.velocity;
        
        float alpha = this.timer / this.maxTimer;
        float scale = 2.0f / (1.0f + alpha);
        this.transform.localScale = new Vector3(scale * this.initialScale, scale * this.initialScale, 1.0f);
        Color newColor = this.initialColor;
        newColor.a *= alpha;
        this.renderer.color = newColor;
    }
}
