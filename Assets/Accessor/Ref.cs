using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Accessor
{
    public static class Ref
    {
        // 表側コントロール
        public static MainAccessor ctl { get; set; }
        // 裏側(Transition用)コントロール
        public static MainAccessor ctlBack { get; set; }
        // UI
        public static CanvasAccessor Canvas { get; set; }

        public static void init()
        {
            Ref.ctl = new MainAccessor();
            Ref.ctl.GetControls();
            Ref.ctlBack = new MainAccessor().Clone(Ref.ctl);
            Ref.Canvas = new CanvasAccessor();
        }
    }
}
