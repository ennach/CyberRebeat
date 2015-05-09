using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Accessor
{
    public class CanvasAccessor
    {
        public GameObject CvMenu { get; set; }
        public GameObject CvSideBar { get; set; }
        public GameObject PnlSide { get; set; }
        public GameObject PnlSideScroll { get; set; }
        public GameObject PnlSideScrollCont { get; set; }

        //Config
        public GameObject PnlSideScrollConfig { get; set; }
        public GameObject SdVolume { get; set; }
        public GameObject SdTextSpeed { get; set; }

        //ポップアップCanvas
        public GameObject CvPopup { get; set; }
        public GameObject BtnPopupOK { get; set; }
        public GameObject BtnPopupNG { get; set; }

        //スクロールメニューボタン（固定　SAVE/LOAD）
        public GameObject BtnContName { get; set; }

        //スクロール中のボタン(20個)
        public List<GameObject> ScrollBtnList { get; set; }

        public CanvasAccessor()
        {
            CvMenu = GameObject.Find("CvMenu");

            CvSideBar = GameObject.Find("CvSideBar");
            PnlSide = GameObject.Find("PnlSide");
            PnlSideScroll = GameObject.Find("PnlSideScroll");
            PnlSideScrollCont = GameObject.Find("PnlSideScrollCont");

            PnlSideScrollConfig = GameObject.Find("PnlSideScrollConfig");
            SdVolume = GameObject.Find("SdVolume");
            SdTextSpeed = GameObject.Find("SdTextSpeed");

            CvPopup = GameObject.Find("CvPopup");
            BtnPopupOK = GameObject.Find("BtnPopupOK");
            BtnPopupNG = GameObject.Find("BtnPopupNG");

            BtnContName = GameObject.Find("BtnContName");


            ScrollBtnList = scrollBtnsCreate(this.PnlSideScrollCont).ToList();
        }


        //スクロールボタン生成
        private IEnumerable<GameObject> scrollBtnsCreate(GameObject pnlSideScrollCont)
        {
            //ボタン生成位置
            Func<int, Vector3> createLocation = (n) => { return new Vector3() { x = 0, y = 930 - 100 * n, z = 0 }; };
            //プレハブ
            var prefab = (GameObject)Resources.Load("prefabs/BtnPnl");
            var range = Enumerable.Range(0, 20);
            foreach (var r in range)
            {
                //ボタン生成
                var btn = UnityEngine.GameObject.Instantiate(prefab) as GameObject;

                //スクロールにセット
                var rect = btn.GetComponent<RectTransform>();
                btn.transform.SetParent(pnlSideScrollCont.transform);

                //20個並べる
                rect.localPosition = createLocation(r);
                rect.anchoredPosition = createLocation(r);

                yield return btn;
            }
        }
    }
}
