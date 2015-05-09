using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Base;

namespace Assets.Accessor
{
    public class MainAccessor
    {
        public GameObject messageWindow { get; set; }
        public GameObject textArea { get; set; }
        public GameObject textAreaShadow { get; set; }

        public GameObject messageNameWindow { get; set; }
        public GameObject textName { get; set; }
        public GameObject textNameShadow { get; set; }
        public string speakerName { get; set; }

        public GameObject guiStand { get; set; }
        public GameObject guiStand1 { get; set; }
        public GameObject guiStand2 { get; set; }
        public GameObject guiStand3 { get; set; }
        public GameObject bgGround { get; set; }

        /// <summary>
        /// 参照取得
        /// 裏側のコントロール参照はCloneで行うため、コンストラクタでは取得を行わない
        /// </summary>
        public void GetControls()
        {
            messageWindow = GameObject.Find("MessageWindow");
            textArea = GameObject.Find("TextArea");
            textAreaShadow = GameObject.Find("TextAreaShadow");

            messageNameWindow = GameObject.Find("MessageNameWindow");
            textName = GameObject.Find("TextName");
            textNameShadow = GameObject.Find("TextNameShadow");

            guiStand = GameObject.Find("GuiStand");
            guiStand1 = GameObject.Find("GuiStand1");
            guiStand2 = GameObject.Find("GuiStand2");
            guiStand3 = GameObject.Find("GuiStand3");
            bgGround = GameObject.Find("BgGround");
        }

        /// <summary>
        /// 裏側用
        /// </summary>
        /// <param name="main"></param>
        /// <returns></returns>
        public MainAccessor Clone(MainAccessor main)
        {
            var clone = new MainAccessor()
            {
                messageWindow = GameObject.Instantiate(main.messageWindow),
                textArea = GameObject.Instantiate(main.textArea),
                textAreaShadow = GameObject.Instantiate(main.textAreaShadow),
                messageNameWindow = GameObject.Instantiate(main.messageNameWindow),
                textName = GameObject.Instantiate(main.textName),
                textNameShadow = GameObject.Instantiate(main.textNameShadow),
                guiStand = GameObject.Instantiate(main.guiStand),
                guiStand1 = GameObject.Instantiate(main.guiStand1),
                guiStand2 = GameObject.Instantiate(main.guiStand2),
                guiStand3 = GameObject.Instantiate(main.guiStand3),
                bgGround = GameObject.Instantiate(main.bgGround)
            };

            clone.bgGround.SetAlpha(0);

            return clone;
        }

        /// <summary>
        /// 裏側用　Transition終了時に実行
        /// </summary>
        public void Reset()
        {
            messageWindow.SetAlpha(0);
            guiStand.SetAlpha(0);
            guiStand1.SetAlpha(0);
            guiStand2.SetAlpha(0);
            guiStand3.SetAlpha(0);
            bgGround.SetAlpha(0);
        }

        /// <summary>
        /// 立ち絵番号で対象のコントロールを返す
        /// </summary>
        /// <param name="codeLayer"></param>
        /// <returns></returns>
        public GameObject GetStand(string codeLayer)
        {
            switch (codeLayer)
            {
                case CodeLayer.Center: return guiStand;
                case CodeLayer.Left: return guiStand1;
                case CodeLayer.Right: return guiStand2;
                case CodeLayer.Icon:  return guiStand3;
            }

            return null;
        }
    }
}
