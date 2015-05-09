using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.States
{
    public static class GameState
    {
        private static GameStateModel Vo { get; set; }

        private static GUITexture MovieTest { get; set; }
        private static AudioSource MovieAudio { get; set; }
        private static MovieTexture MTex { get; set; }

        public static void initialize()
        {
            Vo = new GameStateModel();
        }

        public static void Start()
        {
            Vo.Camera.Start();
        }

        public static void Update()
        {
            Vo.Camera.Update();
        }

        public static void LateUpdate()
        {
            Vo.Camera.LateUpdate();
        }

        // Canvasのボタンクリック時用
        public static GameStateModel GetState()
        {
            return Vo;
        }
    }
}
