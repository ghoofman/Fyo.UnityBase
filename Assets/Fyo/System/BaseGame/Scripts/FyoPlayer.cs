﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FyoPlayer : MonoBehaviour {
    public SocketGamepad Gamepad;
    public int PlayerId;
    public bool Ready = false;
    
    public virtual void SetInput(JSONObject InputMsg) {        
    }

    protected virtual void CheckInput(JSONObject InputMsg) {
    }
}