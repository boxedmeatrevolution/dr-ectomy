using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputComponent : MonoBehaviour
{
    
    public bool clickedPlaceNow = false;
    public bool clickedRotateForwardNow = false;
    public bool clickedRotateReverseNow = false;
    public bool clickedPlace = false;
    public bool clickedRotateForward = false;
    public bool clickedRotateReverse = false;

    void Start() {
        
    }

    void Update() {
        // Handle clicks.
        if (Input.GetAxisRaw("Place") == 1) {
            this.clickedPlaceNow = false;
            if (!this.clickedPlace) {
                this.clickedPlace = true;
                this.clickedPlaceNow = true;
            }
        }
        if (Input.GetAxisRaw("Place") == 0) {
            this.clickedPlace = false;
            this.clickedPlaceNow = false;
        }

        if (Input.GetAxisRaw("Rotate Forward") == 1) {
            this.clickedRotateForwardNow = false;
            if (!this.clickedRotateForward) {
                this.clickedRotateForward = true;
                this.clickedRotateForwardNow = true;
            }
        }
        if (Input.GetAxisRaw("Rotate Forward") == 0) {
            this.clickedRotateForward = false;
            this.clickedRotateForwardNow = false;
        }

        if (Input.GetAxisRaw("Rotate Reverse") == 1) {
            this.clickedRotateReverseNow = false;
            if (!this.clickedRotateReverse) {
                this.clickedRotateReverse = true;
                this.clickedRotateReverseNow = true;
            }
        }
        if (Input.GetAxisRaw("Rotate Reverse") == 0) {
            this.clickedRotateReverse = false;
            this.clickedRotateReverseNow = false;
        }
    }

}
