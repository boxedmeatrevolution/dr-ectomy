using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleComponent : MonoBehaviour
{

    private float timer = 2.0f;
    private float fadeTimer = 0.0f;
    private TextMeshPro textMesh;

    void Start() {
        this.textMesh = this.GetComponentInChildren<TextMeshPro>();
        this.textMesh.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    }
    
    void Update() {
        this.timer -= Time.deltaTime;
        if (this.timer <= 0.0f) {
            this.fadeTimer += Time.deltaTime;
            this.textMesh.color = new Color(1.0f, 1.0f, 1.0f, Mathf.Lerp(0.0f, 1.0f, this.fadeTimer));
            if (Input.GetAxisRaw("Place") == 1) {
                int buildIndex = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(buildIndex + 1);
            }
        }
    }
}
