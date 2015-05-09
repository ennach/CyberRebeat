using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Base;
using System;
using System.Diagnostics;
using Assets.States;
using Assets.Accessor;
using Assets;

public partial class MainCamera : MonoBehaviour {

    void Start () 
    {
        //画面状態　初期化
        GameState.initialize();

        GameState.Start();
    }

    void Update ()
    {
        GameState.Update();
 
    }

    void LateUpdate()
    {
        GameState.LateUpdate();
    }

}
