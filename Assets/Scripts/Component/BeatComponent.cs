using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatComponent : MonoBehaviour
{

    private float beat = 0.0f;

    void Start() {
        
    }

    void Update() {

        // BEAT THE HEART
        float timeConstant = 1.2f;
        this.beat *= (1.0f - Time.deltaTime / (0.5f * timeConstant));
        if (this.beat < Mathf.Exp(-2.0f)) {
            this.beat = 1.0f;
        }

    }

    public float GetBeat() {
        return this.beat;
    }
}
