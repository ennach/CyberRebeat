using Assets;
using Assets.Accessor;
using Assets.States;
using Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Controller
{
    public class CanvasController : MonoBehaviour 
    {
        public EnumScrollBarType SideBarStatus;

        public GameStateModel MainState { get { return GameState.GetState(); } }

        #region クリックイベント

        //タイトル画面　スタートClick
        public void BtnStart_OnClick()
        {
            Ref.Canvas.CvMenu.SetActive(false);
            MainState.Scenario.Jump("prologue.ks", "*prologue");
        }

        //タイトル画面　ロードClick
        public void BtnStartLoad_OnClick()
        {
            Ref.Canvas.CvSideBar.SetActive(true);
            changeSideBarStatus(EnumScrollBarType.NONE);
        }

        //サイドバー　セーブClick
        public void BtnSideSave_OnClick()
        {
            //サブスクロールを出す/隠す
            //スクロールが出た場合、各ボタンにセーブ設定を付与
            changeSideBarStatus(EnumScrollBarType.SAVE);
        }

        //サイドバー　ロードClick
        public void BtnSideLoad_OnClick()
        {
            //サブスクロールを出す/隠す
            //スクロールが出た場合、各ボタンにロード設定を付与
            changeSideBarStatus(EnumScrollBarType.LOAD);
        }

        //サイドバー　コンフィグClick
        public void BtnSideConfig_OnClick()
        {
            //サブスクロールを出す/隠す
            //スクロールが出た場合、コンフィグ設定を表示
            changeSideBarStatus(EnumScrollBarType.CONFIG);
        }

        //サイドバー　クローズClick
        public void BtnSideClose_OnClick()
        {
            //サブスクロールを出す/隠す
            //スクロールが出た場合、コンフィグ設定を表示
            changeSideBarStatus(EnumScrollBarType.NONE);
            Ref.Canvas.CvSideBar.SetActive(false);
        }

        #endregion

        #region スライダイベント

        //音量が変化したら、設定をセーブ
        //Runtime Onlyでないと、ゲーム終了時にOnValueChangedメソッドが走るので注意
        public void SdVolume_OnValueChanged()
        {
            MainState.Config.Vo.ConfigVolume = (int)Ref.Canvas.SdVolume.GetComponent<Slider>().value;
            MainState.Config.Save(); 
        }

        //テキスト速度が変化したら、設定をセーブ
        public void SdTextSpeed_OnValueChanged()
        {
            MainState.Config.Vo.ConfigSpeead = (int)Ref.Canvas.SdTextSpeed.GetComponent<Slider>().value;
            MainState.Config.Save();
        }

        #endregion

        //押したサイドバーボタンのタイプを引数に渡す
        //現在のサブスクロールステータスと勘案し、表示制御を行う
        private void changeSideBarStatus(EnumScrollBarType nextType)
        {
            //変更前
            var prevType = this.SideBarStatus;

            //サブスクロールを消す
            if (nextType == EnumScrollBarType.NONE || prevType == nextType)
            {
                Ref.Canvas.PnlSideScroll.SetActive(false);
                this.SideBarStatus = EnumScrollBarType.NONE;
                return;
            }

            //先にステータスを更新しちゃう
            this.SideBarStatus = nextType;

            // SAVE以外 -> SAVE
            if (nextType == EnumScrollBarType.SAVE)
            {
                Ref.Canvas.PnlSideScroll.SetActive(true);
                Ref.Canvas.PnlSideScrollCont.SetActive(true);
                Ref.Canvas.PnlSideScrollConfig.SetActive(false);
                this.setBtnsSaveSetting(Ref.Canvas.ScrollBtnList);
                Ref.Canvas.BtnContName.transform.FindChild("Text").GetComponent<Text>().text = "SAVE";
                return;
            }
            // LOAD以外 -> LOAD
            else if (nextType == EnumScrollBarType.LOAD)
            {
                Ref.Canvas.PnlSideScroll.SetActive(true);
                Ref.Canvas.PnlSideScrollCont.SetActive(true);
                Ref.Canvas.PnlSideScrollConfig.SetActive(false);
                this.setBtnsLoadSetting(Ref.Canvas.ScrollBtnList);
                Ref.Canvas.BtnContName.transform.FindChild("Text").GetComponent<Text>().text = "LOAD";
                return;
            }
            // CONFIG以外 -> CONFIG
            else if (nextType == EnumScrollBarType.CONFIG)
            {
                Ref.Canvas.PnlSideScroll.SetActive(true);
                Ref.Canvas.PnlSideScrollCont.SetActive(false);
                Ref.Canvas.PnlSideScrollConfig.SetActive(true);
                var config = MainState.Config.Vo;
                Ref.Canvas.SdVolume.GetComponent<Slider>().value = config.ConfigVolume;
                Ref.Canvas.SdTextSpeed.GetComponent<Slider>().value = config.ConfigSpeead;
                return;
            }
        }

        //セーブボタン設定
        public void setBtnsSaveSetting(List<GameObject> btns)
        {
            //ボタンにセーブ用の設定を入れていく
            foreach (var btn in btns.Select((n, i) => new { n, i }))
            {
                var ini = btn.n;
                var r = btn.i;

                //セーブデータ取得　主にボタンテキスト用
                var saveData = CommonLogic.FileAccess.GetSaveData(r, MainState);
                //ボタンテキスト設定
                if (saveData != null)
                {
                    ini.transform.FindChild("Button").FindChild("Text").GetComponent<Text>().text = string.Format("SAVE[{0}]\n{1}\n{2}", r, saveData.UpdateDate.ToShortDateString(), saveData.UpdateDate.ToShortTimeString());
                }
                else
                {
                    ini.transform.FindChild("Button").FindChild("Text").GetComponent<Text>().text = string.Format("SAVE[{0}]\n{1}", r, "-");
                }

                //ボタン　クリックイベント設定
                //セーブデータが既に存在するかどうかで、確認メッセージが変わる
                string popupTxt = string.Empty;
                if (saveData != null)
                {
                    var scenarioLines = Regex.Split(CommonLogic.createTextAreaText(saveData.Scenario.CurrentLine), "\r\n");
                    var etc = scenarioLines.Length > 1 ? "..." : string.Empty;
                    var scenarioLine = scenarioLines[0] + etc;
                    popupTxt = string.Format("{0} {1}\n{2}\n\nすでにデータがあります。上書きしますか？", saveData.UpdateDate.ToShortDateString(), saveData.UpdateDate.ToShortTimeString(), scenarioLine);
                }
                else
                {
                    popupTxt = "セーブしますか？";
                }
                ini.transform.FindChild("Button").GetComponent<Button>().onClick.RemoveAllListeners();
                ini.transform.FindChild("Button").GetComponent<Button>().onClick.AddListener(showPopupSave(popupTxt, r));
            }
        }

        //各セーブデータをクリック時、確認ポップアップを出す
        private UnityAction showPopupSave(string txt, int num)
        {
            //ポップアップのＯＫボタン押下時処理
            UnityAction onClickOK = () =>
            {
                //Save 暫定
                //ここのメソッドでnumを使うのが要点 そのため、Action<int>ではなくActionで済む
                MainState.SaveLoad.Save(num.ToString("00"));
                //ポップアップを閉じる
                MainState.Popup.Close();
                //ポップアップを再度開く　「セーブしました」
                var confirm = new PopupModel()
                {
                    IsQuestion = false,
                    Text = "セーブしました。",
                    OnClickOK = () => 
                    { 
                        MainState.Popup.Close();
                    }
                };
                MainState.Popup.Vo = confirm;
                MainState.Popup.Show();
            };

            //ポップアップのＮＧ押下時処理
            UnityAction onClickNG = () =>
            {
                // ポップアップを閉じる
                MainState.Popup.Close();
            };

            var popupModel = new PopupModel()
            {
                Text = txt,
                OnClickOK = onClickOK,
                OnClickNG = onClickNG,
                IsQuestion = true
            };

            return () =>
            {
                MainState.Popup.Vo = popupModel;
                MainState.Popup.Show();
            };
        }

        //ロードボタン設定
        public void setBtnsLoadSetting(List<GameObject> btns)
        {
            //ボタンにセーブ用の設定を入れていく
            foreach (var btn in btns.Select((n, i) => new { n, i }))
            {
                var ini = btn.n;
                var r = btn.i;

                //セーブデータ取得　主にボタンテキスト用
                var saveData = CommonLogic.FileAccess.GetSaveData(r, MainState);
                //ボタンテキスト設定
                if (saveData != null)
                {
                    ini.transform.FindChild("Button").FindChild("Text").GetComponent<Text>().text = string.Format("LOAD[{0}]\n{1}\n{2}", r, saveData.UpdateDate.ToShortDateString(), saveData.UpdateDate.ToShortTimeString());
                }
                else
                {
                    ini.transform.FindChild("Button").FindChild("Text").GetComponent<Text>().text = string.Format("LOAD[{0}]\n{1}", r, "-");
                }

                //ボタン　クリックイベント設定
                //セーブデータが既に存在するかどうかで、挙動が変わる
                if (saveData != null)
                {
                    var scenarioLines = Regex.Split(CommonLogic.createTextAreaText(saveData.Scenario.CurrentLine),"\r\n");
                    var etc = scenarioLines.Length > 1 ? "..." : string.Empty;
                    var scenarioLine = scenarioLines[0] + etc;
                    var popupTxt = string.Format("{0} {1}\n{2}\n\nロードしますか？", saveData.UpdateDate.ToShortDateString(), saveData.UpdateDate.ToShortTimeString(), scenarioLine);
                    ini.transform.FindChild("Button").GetComponent<Button>().onClick.RemoveAllListeners();
                    ini.transform.FindChild("Button").GetComponent<Button>().onClick.AddListener(showPopupLoad(popupTxt, r));
                }
                else
                {
                    //セーブデータがない場合、クリックしても反応しない
                    ini.transform.FindChild("Button").GetComponent<Button>().onClick.RemoveAllListeners();
                }
            }
        }

        //各セーブデータをクリック時、確認ポップアップを出す
        private UnityAction showPopupLoad(string txt, int num)
        {
            UnityAction onClickOK = () =>
            {
                //ポップアップを閉じる
                MainState.Popup.Close();
                //Load処理
                //暫定
                MainState.SaveLoad.Load(num.ToString("00"));
                //スクロールを閉じる
                Ref.Canvas.CvSideBar.SetActive(false);
                //スタート画面からのロードの場合、START/LAODのメニューも消す
                Ref.Canvas.CvMenu.SetActive(false);

            };

            UnityAction onClickNG = () =>
            {
                // ポップアップを閉じる
                MainState.Popup.Close();
            };

            var popupModel = new PopupModel()
            {
                Text = txt,
                OnClickOK = onClickOK,
                OnClickNG = onClickNG,
                IsQuestion = true
            };

            return () =>
            {
                MainState.Popup.Vo = popupModel;
                MainState.Popup.Show();
            };
        }
    }
}
