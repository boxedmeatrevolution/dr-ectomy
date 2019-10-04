using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipboardComponent : MonoBehaviour
{

    public bool zoomed = false;
    private Vector2 restPosition;
    private float restRotation;
    private float restScale = 0.5f;

    public float zoomedScale = 1.0f;
    public float zoomedZ = -5.0f;

    void Start() {
        this.restPosition = this.transform.localPosition;
        this.restRotation = this.transform.localRotation.eulerAngles.z;
        this.restScale = this.transform.localScale.x;
    }
    
    void Update() {
        Vector3 targetPosition = new Vector3(0.0f, 0.0f, zoomedZ);
        float targetRotation = 0.0f;
        float targetScale = this.zoomedScale;

        if (!zoomed) {
            targetPosition = this.restPosition;
            targetRotation = this.restRotation;
            targetScale = this.restScale;
        }

        this.transform.localPosition = Vector3.Slerp(this.transform.localPosition, targetPosition, 20.0f * Time.deltaTime);
        this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, Quaternion.Euler(0.0f, 0.0f, targetRotation), 20.0f * Time.deltaTime);
        this.transform.localScale = Vector3.Slerp(this.transform.localScale, new Vector3(targetScale, targetScale, 1.0f), 20.0f * Time.deltaTime);
    }
}
