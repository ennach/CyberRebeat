using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using System.IO;
using Base;
using UnityEngine.UI;
using Assets.Accessor;

namespace Assets.States
{
    [Serializable]
    public class PopupState : BaseState
    {
        public PopupModel Vo { get; set; }

        public PopupState(GameStateModel targetState)
            : base(targetState)
        {
            this.Vo = new PopupModel();
        }

        //ポップアップ開始
        public void Show()
        {
            Ref.Canvas.CvPopup.SetActive(true);

            var txt = GameObject.Find("TxPopupQuestion");
            txt.GetComponent<Text>().text = this.Vo.Text;

            this.createShowBtns();
        }

        //OK、NGボタンにイベント追加 isQuestionによってOKのみ or OK/NG を出し分け
        private void createShowBtns()
        {
            //OKボタン　クリックイベント
            var btnOK = Ref.Canvas.BtnPopupOK.transform.FindChild("Button").GetComponent<Button>();
            btnOK.onClick.RemoveAllListeners();
            btnOK.onClick.AddListener(this.Vo.OnClickOK);

            //NGボタン　クリックイベント
            var btnNG = Ref.Canvas.BtnPopupNG.transform.FindChild("Button").GetComponent<Button>();
            btnNG.onClick.RemoveAllListeners();
            btnNG.onClick.AddListener(this.Vo.OnClickNG);

            //OKのみの場合、OKボタンを中央寄せ
            //そうでない場合、OKボタンは左側
            var rectOK = Ref.Canvas.BtnPopupOK.GetComponent<RectTransform>();
            rectOK.localPosition = new Vector3()
            {
                x = this.Vo.IsQuestion ? -100 : 0,
                y = rectOK.localPosition.y,
                z = rectOK.localPosition.z
            };
            rectOK.anchoredPosition = new Vector2()
            {
                x = this.Vo.IsQuestion ? -100 : 0,
                y = rectOK.anchoredPosition.y
            };

            //OKのみの場合、NGボタンを消す
            //var pnlNG = GameObject.Find("BtnPopupNG");
            Ref.Canvas.BtnPopupNG.SetActive(this.Vo.IsQuestion);
        }

        //閉じる
        //基本的に単独では呼ばず、this.Vo.OnClickOK か this.Vo.OnClickNG にActionを入れるとき、そこで使われる
        public void Close()
        {
            Ref.Canvas.CvPopup.SetActive(false);
        }
    }
}
