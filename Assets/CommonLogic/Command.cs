using Assets.Accessor;
using Assets.States;
using Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;

namespace Assets
{
    public static partial class CommonLogic
    {
        public static class Command
        {
            public static void SetLayers(string currentLine, string originalLine, GameStateModel MainState)
            {
                //var outVo = inVo;

                var commandWord = CommonLogic.GetCommandWord(currentLine);
                var param = CommonLogic.CommandParams(currentLine, originalLine);

                if (commandWord == CodeCommandWord.Backlay)
                {
                    SetBg(null);
                    CharaLoadAll(null);
                    Ref.ctlBack.messageWindow.SetAlpha(0);
                    MainState.Camera.Vo.BackMessageAppear = CodeBackMessageAppear.NoChange;
                }
                else if (commandWord == CodeCommandWord.Layopt)
                {
                    if (param.Get("layer") == "message")
                    {
                        MainState.Camera.Vo.BackMessageAppear = param.Get("visible") == "true" ? CodeBackMessageAppear.Appear : CodeBackMessageAppear.DisAppear;
                    }
                    else if (param.Get("layer") != null)
                    {
                        SetBg(null);
                        CharaLoad(param.Get("storage") ?? "fgNone", param.Get("layer"));
                    }
                }
                else if (commandWord == CodeCommandWord.Image)
                {
                    //背景　キャラもImageコマンドがあるため
                    if (param.Get("layer") == CodeLayer.Base)
                    {
                        SetBg(param.Get("storage") ?? "black");
                        CharaLoadAll("fgNone");
                    }
                    else if (param.Get("layer") != null)
                    {
                        SetBg(null);
                        CharaLoad(param.Get("storage") ?? "fgNone", param.Get("layer"));
                    }

                    SetSpeakerName(param.Get("storage"), Ref.ctl);
                }
                else if (commandWord == CodeCommandWord.Name)
                {
                    //発言者を直接指定
                    Ref.ctl.speakerName = param.Get("ch") ?? string.Empty;
                }
            }

            private static void SetSpeakerName(string speakerFileName, MainAccessor ctl)
            {
                var speakerDics = new Dictionary<string, string>();
                speakerDics.Add("m", "ミサ");
                speakerDics.Add("k", "香奈");
                speakerDics.Add("l", "リリー");
                speakerDics.Add("y", "キョーコ");
                speakerDics.Add("s", "サラ");
                speakerDics.Add("o", "綾");
                speakerDics.Add("i", "葵");
                speakerDics.Add("a", "ミサ");
                speakerDics.Add("h", "舞");
                speakerDics.Add("n", "まひろ");
                speakerDics.Add("e", "香奈");

                var iconDics = new Dictionary<string, string>();
                iconDics.Add("a", "alpha");
                iconDics.Add("m", "MES");
                iconDics.Add("l", "LOB");
                iconDics.Add("n", "NESS");
                iconDics.Add("p", "Phantom");
                iconDics.Add("s", "Subs");
                iconDics.Add("v", "Vulture");
                iconDics.Add("r", "Prey'r");
                iconDics.Add("w", "WARLOCK");

                if (speakerFileName != null && speakerFileName.Length > 2)
                {
                    ctl.speakerName = speakerDics.Get(speakerFileName.Substring(0, 1));
                }
                else if (speakerFileName != null
                        && (speakerFileName.Length == 1
                            || speakerFileName.Substring(0, 1) == "w"))
                {
                    ctl.speakerName = iconDics.Get(speakerFileName.Substring(0, 1));
                }
            }

            /// <summary>
            /// トランジション
            /// 表レイヤを消していき、裏レイヤを浮かび上がらせる
            /// 最後に表と裏を入れ替えて終了
            /// </summary>
            /// <param name="inVo">In vo.</param>
            public static void Transition(string currentLine, GameStateModel MainState)
            {
                //var outVo = inVo;

                if (MainState.Camera.Vo.COMMAND_STATUS == CodeCommandStatus.Start)
                {
                    //最初は裏が空なので、生成する
                    //表のコピーか、何か追加するかはメソッドによる
                    //Spriteの情報はコピーして使い回すため、引数に表のctlは必要
                    MainState.Camera.Vo.COMMAND_STATUS = CodeCommandStatus.DoCommand;
                }
                else if (MainState.Camera.Vo.COMMAND_STATUS == CodeCommandStatus.DoCommand)
                {
                    var param = CommonLogic.CommandParams(currentLine);
                    //alpha値が1Fごとに切り替わるスピード
                    float ttt = int.Parse(param.Get("time") ?? "1000");
                    var transSpeed = 1000 / (float)(ttt * 60);

                    if (CommonLogic.DuringSkip())
                    {
                        transSpeed = 1;
                    }

                    //メッセージウィンドウ有無
                    var messageAppear = MainState.Camera.Vo.BackMessageAppear;

                    //変更後のalphaが100%になるまで変更前と変更後のalphaを変化
                    //alphaが１を超えた場合の処理も必要？
                    if (Ref.ctl.bgGround.GetAlpha() > 0)
                    {
                        Ref.ctl.bgGround.ChangeAlpha(-transSpeed);
                        Ref.ctl.guiStand.ChangeAlpha(-transSpeed);
                        Ref.ctlBack.guiStand.ChangeAlpha(transSpeed);
                        Ref.ctl.guiStand1.ChangeAlpha(-transSpeed);
                        Ref.ctlBack.guiStand1.ChangeAlpha(transSpeed);
                        Ref.ctl.guiStand2.ChangeAlpha(-transSpeed);
                        Ref.ctlBack.guiStand2.ChangeAlpha(transSpeed);
                        Ref.ctl.guiStand3.ChangeAlpha(-transSpeed);
                        Ref.ctlBack.guiStand3.ChangeAlpha(transSpeed);


                        if (messageAppear == CodeBackMessageAppear.Appear)
                        {
                            Ref.ctl.messageWindow.ChangeAlpha(transSpeed / 2);
                        }
                        else if (messageAppear == CodeBackMessageAppear.DisAppear)
                        {
                            Ref.ctl.messageWindow.ChangeAlpha(-transSpeed);
                        }
                    }
                    else
                    {
                        MainState.Camera.Vo.COMMAND_STATUS = CodeCommandStatus.LastOne;
                    }

                }
                else if (MainState.Camera.Vo.COMMAND_STATUS == CodeCommandStatus.LastOne)
                {
                    //変更前と変更後のオブジェクトを入れ替える
                    Ref.ctl.guiStand = exchangeBackToFace(Ref.ctl.guiStand, Ref.ctlBack.guiStand);
                    Ref.ctl.guiStand1 = exchangeBackToFace(Ref.ctl.guiStand1, Ref.ctlBack.guiStand1);
                    Ref.ctl.guiStand2 = exchangeBackToFace(Ref.ctl.guiStand2, Ref.ctlBack.guiStand2);
                    Ref.ctl.guiStand3 = exchangeBackToFace(Ref.ctl.guiStand3, Ref.ctlBack.guiStand3);

                    Ref.ctl.bgGround.GetComponent<SpriteRenderer>().sprite = Ref.ctlBack.bgGround.GetComponent<SpriteRenderer>().sprite;
                    Ref.ctl.bgGround.GetComponent<SpriteRenderer>().color = Ref.ctlBack.bgGround.GetComponent<SpriteRenderer>().color;

                    var outPosition = Ref.ctl.bgGround.transform.position;
                    Ref.ctl.bgGround.transform.position = new Vector3(outPosition.x, outPosition.y, outPosition.z - 0.1f);

                    MainState.Camera.Vo.COMMAND_STATUS = CodeCommandStatus.Done;
                }
            }

            /// <summary>
            /// 裏のGuiStandを表へ
            /// </summary>
            /// <returns>The back to face.</returns>
            /// <param name="ctlGuiStand">Ctl GUI stand.</param>
            /// <param name="ctlBackGuiStand">Ctl back GUI stand.</param>
            private static GameObject exchangeBackToFace(GameObject ctlGuiStand, GameObject ctlBackGuiStand)
            {
                ctlGuiStand.GetComponent<GUITexture>().texture = ctlBackGuiStand.GetComponent<GUITexture>().texture;
                ctlGuiStand.GetComponent<GUITexture>().pixelInset = ctlBackGuiStand.GetComponent<GUITexture>().pixelInset;
                ctlGuiStand.GetComponent<GUITexture>().color = ctlBackGuiStand.GetComponent<GUITexture>().color;
                ctlGuiStand.transform.position = new Vector3(0, 0, 0);
                return ctlGuiStand;
            }

            /// <summary>
            /// 背景
            /// まず表のSpriteをコピーした後、切り替え先の画像を読み込む
            /// bgName=nullのとき、背景は引き継ぐ
            /// </summary>
            /// <returns>The on.</returns>
            /// <param name="ctl">Ctl.</param>
            public static void SetBg(string bgName)
            {
                Ref.ctlBack.bgGround.GetComponent<SpriteRenderer>().sprite = Ref.ctl.bgGround.GetComponent<SpriteRenderer>().sprite;
                Ref.ctlBack.bgGround.GetComponent<SpriteRenderer>().color = Ref.ctl.bgGround.GetComponent<SpriteRenderer>().color;

                var outPosition = Ref.ctl.bgGround.transform.position;
                Ref.ctlBack.bgGround.transform.position = new Vector3(outPosition.x, outPosition.y, outPosition.z + 0.1f);

                //背景は、変更後側のアルファ値をあらかじめ100%にしておく
                Ref.ctlBack.bgGround.SetAlpha(1);

                //変更後側の画像を差し替え
                if (bgName != null)
                {
                    Texture2D tex = new Texture2D(0, 0);
                    tex = Resources.Load<Texture2D>(string.Format("bgimage/{0}", bgName)) ?? Resources.Load<Texture2D>(string.Format("image/{0}", bgName));

                    var stnsp = Ref.ctlBack.bgGround.GetComponent<SpriteRenderer>().sprite;
                    Ref.ctlBack.bgGround.GetComponent<SpriteRenderer>().sprite = Sprite.Create(tex, stnsp.textureRect, new Vector2(0.5f, 0.5f));
                }
            }

            /// <summary>
            /// キャラ立ち絵　裏側に読込
            /// </summary>
            /// <param name="ctl">Ctl.</param>
            /// <param name="chara">Chara.</param>
            public static void CharaLoadAll(string cName)
            {
                CharaLoad(cName, CodeLayer.Center);
                CharaLoad(cName, CodeLayer.Left);
                CharaLoad(cName, CodeLayer.Right);
                CharaLoad(cName, CodeLayer.Icon);
            }

            public static void CharaLoad(string cName, string layer)
            {
                var ctlGuiStand = Ref.ctl.GetStand(layer);
                var ctlBackGuiStand = Ref.ctlBack.GetStand(layer);

                ctlBackGuiStand.GetComponent<GUITexture>().texture = ctlGuiStand.GetComponent<GUITexture>().texture;
                ctlBackGuiStand.GetComponent<GUITexture>().pixelInset = ctlGuiStand.GetComponent<GUITexture>().pixelInset;
                ctlBackGuiStand.GetComponent<GUITexture>().color = ctlGuiStand.GetComponent<GUITexture>().color;
                ctlBackGuiStand.transform.localScale = new Vector3(0, 0, 1);
                ctlBackGuiStand.transform.position = new Vector3(0, 0, 1);

                if (cName != null)
                {
                    Texture2D guiTex = new Texture2D(0, 0);
                    guiTex = Resources.Load<Texture2D>(string.Format("fgimage/{0}", cName ?? "fgNone"));

                    ctlBackGuiStand.GetComponent<GUITexture>().texture = guiTex;
                    ctlBackGuiStand.SetAlpha(0);

                    int standard = 0;
                    if (layer == CodeLayer.Center)     { standard = SettingClass.CharaCenter; }
                    else if (layer == CodeLayer.Left)  { standard = SettingClass.CharaLeft; }
                    else if (layer == CodeLayer.Right) { standard = SettingClass.CharaRight; }
                    else if (layer == CodeLayer.Icon)  { standard = SettingClass.CharaIcon; }

                    ctlBackGuiStand.GetComponent<GUITexture>().pixelInset = new Rect(standard - guiTex.width / 2, 0, (float)guiTex.width, (float)guiTex.height);
                    ctlBackGuiStand.transform.localScale = new Vector3(0, 0, 1);
                    ctlBackGuiStand.transform.position = new Vector3(0, 0, 1);
                }
            }
        }
    }
}
