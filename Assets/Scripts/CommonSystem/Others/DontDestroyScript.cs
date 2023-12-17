using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyScript : MonoBehaviour {
    protected void Awake() {
        DontDestroyOnLoad(gameObject);
    }
}
