using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using System.IO;
using Base;
using System.Xml.Serialization;
using Assets.Accessor;

namespace Assets.States
{
    public class CameraState : BaseState
    {
        public CameraModel Vo { get; set; }

        public CameraState(GameStateModel targetState)
            : base(targetState)
        {
            this.Vo = new CameraModel();
        }


        public void Start()
        {
            //コンフィグ読込
            MainState.Config.Vo = CommonLogic.FileAccess.GetConfigFile(MainState);

            //Canvasのコントロールへの参照を取得
            Ref.init();

            //サイドバー非表示
            Ref.Canvas.CvSideBar.SetActive(false);
            //ポップアップ非表示
            Ref.Canvas.CvPopup.SetActive(false);

            MainState.Camera.Vo.Count = 0;

            MainState.Scenario.FileLoad(SettingClass.START_SCENARIO_FILE);

            //マクロ読込
            this.Vo.MacroDictionary = createMacroDictionary();

            MainState.Camera.Vo.COMMAND_STATUS = CodeCommandStatus.Start;
            MainState.Camera.Vo.BackMessageAppear = CodeBackMessageAppear.NoChange;

            Screen.SetResolution(SettingClass.SCREEN_WIDTH, SettingClass.SCREEN_HEIGHT, false);
            MainState.Camera.Vo.ScreenSize.Width = SettingClass.SCREEN_WIDTH;
            MainState.Camera.Vo.ScreenSize.Height = SettingClass.SCREEN_HEIGHT;


            MainState.Camera.Vo.clickeSpantime = 5;

            Resources.UnloadUnusedAssets();
            System.GC.Collect(2);
        }

        public void Update()
        {
            //trueは動画再生中
            if (MainState.Movie.Play()) { return; }

            if (!MainState.BackLog.Execute()) { return; }

            //ロードフラグが立ってたら、ロードする
            if (MainState.SaveLoad.LoadFlag == CodeLoadFlag.ON)
            {
                MainState.Camera.Load_Run();
                MainState.SaveLoad.LoadFlag = CodeLoadFlag.OFF;
                return;
            }

            //コンフィグ中は下に行かない
            if (Ref.Canvas.CvSideBar.activeSelf)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    Ref.Canvas.CvPopup.SetActive(false);
                    Ref.Canvas.CvSideBar.SetActive(false);
                }

                //コンフィグ上での音量変更を直に反映させるため
                //これを外すと、SideBarを消すまで反映されない
                MainState.Audio.FadeInOut();
                MainState.Audio.AudioPlay();
                return;
            }

            // スキップ中処理
            if (CommonLogic.DuringSkip())
            {
                MainState.Camera.Vo.Count = 1000;
            }
            //Common.WriteConsole(count.ToString());

            //シナリオファイルの現在行を取得
            MainState.Camera.Vo.Count++;
            var currentLine = MainState.Scenario.Vo.CurrentLine;

            //命令行かシナリオ行かを取得
            var scenarioSwitch = CommonLogic.SwitchReadLine(currentLine);

            switch (scenarioSwitch)
            {
                //読み込まない行の場合、次の行へ
                case CodeScenarioSwitch.None:
                    MainState.Scenario.NextLine();
                    break;
                //コマンドの場合、解析して処理
                case CodeScenarioSwitch.Command:
                    var commandArgs = CommandAnalyse(new CommandArgs(currentLine, currentLine, MainState.Scenario.Vo.MacroQueue));
                    if (MainState.Camera.DoCommand(commandArgs))
                    {
                        MainState.Camera.Vo.Count = 0;

                        //最後のQueueが終わったら、次の行へ
                        if (!MainState.Scenario.NextQueueRecursive())
                        {
                            MainState.Scenario.NextLine();
                            break;
                        }
                    };
                    break;
                //シナリオの場合、内容を表示
                case CodeScenarioSwitch.Scenario:
                    if (DrawText(currentLine))
                    {
                        MainState.Scenario.NextLine();
                    };
                    break;
            }

            MainState.Audio.FadeInOut();
            MainState.Audio.AudioPlay();

            //シナリオ文章表示が終わっている場合
            //右クリックでメニューを表示
            if (scenarioSwitch == CodeScenarioSwitch.Scenario && MainState.Scenario.IsCharEnd())
            {
                if (Input.GetMouseButtonDown(1))
                {
                    Ref.Canvas.CvSideBar.SetActive(true);
                    Ref.Canvas.PnlSideScroll.SetActive(false);
                    Ref.Canvas.CvSideBar.transform.SetAsLastSibling();
                }

                if (CommonLogic.MouseWheelUp())
                {
                    MainState.BackLog.Vo.ctl = Ref.ctl;
                    MainState.BackLog.BackLogBegin();
                }
            }
        }

        public void LateUpdate()
        {
            // Updateの最初だけだと、Transactionが終わった最後のフレームに対応できない
            resize();
        }

        /// <summary>
        /// ウィンドウサイズの変更にあわせて、GUI位置を調整する
        /// </summary>
        private void resize()
        {
            // 前フレームと変更がなければ処理終了
            var noChanged = (MainState.Camera.Vo.ScreenSize.Width ==  Screen.width
                && MainState.Camera.Vo.ScreenSize.Height ==  Screen.height);
            if (noChanged) { return; }

            float xDiff = (float)(Screen.width - SettingClass.SCREEN_WIDTH) / 2;
            float yDiff = (float)(Screen.height - SettingClass.SCREEN_HEIGHT) / 2;

            Action<GUITexture, int, int> changeTexturePos = (n, baseX, baseY) =>
            {
                var newRect = new Rect(n.pixelInset);
                newRect.x = baseX + xDiff;
                newRect.y = baseY + yDiff;
                n.pixelInset = newRect;
            };

            Action<GUIText, int, int> changeTextPos = (n, baseX, baseY) =>
            {
                var newOffset = new Vector2(n.pixelOffset.x, n.pixelOffset.y);
                newOffset.x = baseX + xDiff;
                newOffset.y = baseY + yDiff;
                n.pixelOffset = newOffset;
            };

            Action<MainAccessor> resizeAccessor = (n) =>
            {
                changeTexturePos(n.messageNameWindow.GetComponent<GUITexture>(), 137, 30);
                changeTexturePos(n.messageWindow.GetComponent<GUITexture>(), 137, 30);
                changeTextPos(n.textArea.GetComponent<GUIText>(), 172, 175);
                changeTextPos(n.textAreaShadow.GetComponent<GUIText>(), 173, 174);
                changeTextPos(n.textName.GetComponent<GUIText>(), 172, 222);
                changeTextPos(n.textNameShadow.GetComponent<GUIText>(), 173, 221);

                changeTexturePos(n.guiStand.GetComponent<GUITexture>(), SettingClass.CharaCenter / 2, 0);
                changeTexturePos(n.guiStand1.GetComponent<GUITexture>(), SettingClass.CharaLeft / 2, 0);
                changeTexturePos(n.guiStand2.GetComponent<GUITexture>(), SettingClass.CharaRight / 2, 0);
                changeTexturePos(n.guiStand3.GetComponent<GUITexture>(), SettingClass.CharaIcon / 2, 0);
            };

            resizeAccessor(Ref.ctl);
            resizeAccessor(Ref.ctlBack);

            MainState.Camera.Vo.ScreenSize.Width = Screen.width;
            MainState.Camera.Vo.ScreenSize.Height = Screen.height;
        }

        #region Load_Run

        // Voにセットしたロード番号をロードし、画面をその場面まで進める
        public void Load_Run()
        {
            MainState.SaveLoad.Load(MainState.SaveLoad.LoadNumber);
            MainState.Camera.Vo.Count = 0;
            MainState.Scenario.FileLoad(MainState.SaveLoad.Vo.Scenario.CurrentFileName);
            MainState.Scenario.Vo.LineCount = 0;

            while (MainState.Scenario.Vo.LineCount != MainState.SaveLoad.Vo.Scenario.LineCount)
            {
                MainState.Camera.Vo.Count++;
                //Common.WriteConsole(count.ToString());
                var currentLine = MainState.Scenario.Vo.CurrentLine;

                //命令行かシナリオ行かを取得
                var scenarioSwitch = CommonLogic.SwitchReadLine(currentLine);

                switch (scenarioSwitch)
                {
                    //読み込まない行の場合、次の行へ
                    case CodeScenarioSwitch.None:
                        MainState.Scenario.NextLine();
                        break;
                    //コマンドの場合、解析して処理
                    case CodeScenarioSwitch.Command:
                        var commandArgs = CommandAnalyse(new CommandArgs(currentLine, currentLine, MainState.Scenario.Vo.MacroQueue));
                        if (DoCommand(commandArgs, true))
                        {
                            MainState.Camera.Vo.Count = 0;

                            //最後のQueueが終わったら、次の行へ
                            if (!MainState.Scenario.NextQueueRecursive())
                            {
                                MainState.Scenario.NextLine();
                                break;
                            }
                        };
                        break;
                    case CodeScenarioSwitch.Scenario:
                        if (true)
                        {
                            MainState.Scenario.NextLine();
                        };
                        break;
                }
            }
        }

        #endregion

        #region コマンド解析

        private class CommandArgs
        {
            // 処理中の行（マクロ内含む）
            public string CurrentLine { get; set; }
            // 処理中の行（マクロ外）　マクロに渡す引数取得のため
            public string OriginalLine { get; set; }
            // マクロ実行の状態保持
            public List<MacroClass> MacroQueue { get; set; }

            public CommandArgs(string currentLine, string originalLine, List<MacroClass> macroQueue)
            {
                this.CurrentLine = currentLine;
                this.OriginalLine = originalLine;
                this.MacroQueue = macroQueue;
            }
        }

        /// <summary>
        /// コマンド解析
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private CommandArgs CommandAnalyse(CommandArgs args)
        {
            // すでに解析済みの行であれば、処理対象の行をCurrentにセットし、返す
            var isFirstLoop = args.CurrentLine == args.OriginalLine;
            if (isFirstLoop && args.MacroQueue.Count != 0)
            {
                args.CurrentLine = args.MacroQueue.Last().Values[args.MacroQueue.Last().Queue];
                // ここでreturnさせると、スキップ時にメッセージウィンドウが消える
                //return args;
            }

            //コマンドを取得
            var commandWord = CommonLogic.GetCommandWord(args.CurrentLine);

            //マクロである限り、再帰
            if (this.Vo.MacroDictionary.ContainsKey(commandWord))
            {
                args.CurrentLine = this.Vo.MacroDictionary[commandWord][0];
                args.MacroQueue.Add(new MacroClass() { Values = this.Vo.MacroDictionary[commandWord], Queue = 0 });
                return CommandAnalyse(args);
            }

            return args;
        }

        #endregion

        /// <summary>
        /// コマンド実行
        /// 処理中はfalse、処理が終わればtrueで次の行へ
        /// </summary>
        /// <returns><c>true</c>, if command was done, <c>false</c> otherwise.</returns>
        /// <param name="currentLine">Current line.</param>
        private bool DoCommand(CommandArgs args, bool loadFlag = false)
        {
            var currentLine = args.CurrentLine;
            var originalLine = args.OriginalLine;

            //コマンド実行前に初期化
            Ref.ctl.textArea.GetComponent<GUIText>().text = string.Empty;
            Ref.ctl.textAreaShadow.GetComponent<GUIText>().text = string.Empty;

            //コマンドを取得
            var commandWord = CommonLogic.GetCommandWord(currentLine);

            //想定したコマンドであれば処理開始
            //トランジション系コマンド
            if (commandWord == CodeCommandWord.Jump)
            {
                var param = CommonLogic.CommandParams(currentLine, currentLine);
                MainState.Scenario.Jump(param.Get("storage"), param.Get("target"));
            }
            else if (CodeCommandWord.ToTransitionList().Contains(commandWord))
            {
                //処理
                CommonLogic.Command.Transition(currentLine, MainState);

                //テキストの透過度はメッセージウィンドウに依存
                Ref.ctl.textArea.SetTextAlpha(Ref.ctl.messageWindow.GetAlpha() * 2);
                Ref.ctl.textAreaShadow.SetTextAlpha(Ref.ctl.messageWindow.GetAlpha() * 2);

                //コマンド　トランジションが終わったら裏レイヤを削除
                //trueを返して処理終了
                if (MainState.Camera.Vo.COMMAND_STATUS == CodeCommandStatus.Done)
                {
                    // 裏側の立ち絵と背景を透明化
                    Ref.ctlBack.Reset();

                    //この処理を入れないと、コピーするたびに背景が下がり、そのうち-10（カメラ)を突破してしまい、表示されなくなる
                    var outPosition = Ref.ctl.bgGround.transform.position;
                    Ref.ctl.bgGround.transform.position = new Vector3(outPosition.x, outPosition.y, 0);

                    MainState.Camera.Vo.COMMAND_STATUS = CodeCommandStatus.Start;
                    MainState.Camera.Vo.BackMessageAppear = CodeBackMessageAppear.NoChange;
                    return true;
                };
            }
            //音楽セット、フェード開始フラグON
            else if (CodeCommandWord.ToAudioList().Contains(commandWord))
            {
                processingBgmCommand(currentLine, originalLine, commandWord);
                return true;
            }
            //@s 停止タグ
            else if (commandWord == CodeCommandWord.S)
            {
                return false;
            }
            else if (commandWord == CodeCommandWord.Video)
            {
                // ロードによる読み飛ばし中は、動画は読み込まない
                if (loadFlag)
                {
                    return true;
                }
                //既に同じMovieNameが入っている＝動画が終了した次のフレームで、またここに来た
                var param = CommonLogic.CommandParams(currentLine, originalLine);
                MainState.Movie.Vo.MoviePlayFlag = CodeMoviePlayFlag.START;
                MainState.Movie.Vo.MovieName = (param.Get("storage") ?? "");
                Resources.UnloadUnusedAssets();
                System.GC.Collect(2);
                return true;
            }
            else if (commandWord == CodeCommandWord.GameClear)
            {
                MainState.Config.GameClear();
                return true;
            }
            //非トランジション系コマンド
            else if (CodeCommandWord.ToList().Contains(commandWord))
            {
                //処理
                CommonLogic.Command.SetLayers(currentLine, originalLine, MainState);

                //テキストの透過度はメッセージウィンドウに依存
                Ref.ctl.textArea.SetTextAlpha(Ref.ctl.messageWindow.GetAlpha() * 2);
                Ref.ctl.textAreaShadow.SetTextAlpha(Ref.ctl.messageWindow.GetAlpha() * 2);
                return true;
            }
            //それ以外はエラー
            else
            {
                return true;
            }

            return false;
        }

        #region BGM

        private void processingBgmCommand(string currentLine, string originalLine, string commandWord)
        {
            var param = CommonLogic.CommandParams(currentLine, originalLine);
            if (commandWord == CodeCommandWord.FadeInBGM)
            {
                if (param.Get("storage") != null)
                {
                    MainState.Audio.SetBGM(param.Get("storage") ?? "none", 0);
                    MainState.Audio.Vo.ToAudioVolume = CodeToAudioVolume.ON;
                }
                else
                {
                    MainState.Audio.Vo.ToAudioVolume = CodeToAudioVolume.NoChange;
                }
            }
            else if (commandWord == CodeCommandWord.FadeOutBGM)
            {
                MainState.Audio.Vo.ToAudioVolume = CodeToAudioVolume.OFF;
            }
            MainState.Audio.Vo.BGM_STATUS = CodeBgmStatus.Start;
        }

        #endregion

        #region シナリオ

        /// <summary>
        /// シナリオを書きだし
        /// 処理中はfalse、処理が終わればtrueで次の行へ
        /// </summary>
        /// <param name="currentLine">Current line.</param>
        private bool DrawText(string currentLine)
        {
            if (MainState.Camera.Vo.clickeSpantime > 0)
            {
                MainState.Camera.Vo.clickeSpantime -= 1;
            }

            var speed = (int)((CodeConfigSpeed.Max - MainState.Config.Vo.ConfigSpeead) / 25);
            if (MainState.Config.Vo.ConfigSpeead == CodeConfigSpeed.Max)
            {
                MainState.Scenario.CharEnd();
            }
            else if (MainState.Config.Vo.ConfigSpeead > CodeConfigSpeed.High)
            {
                MainState.Scenario.NextChar();
                MainState.Scenario.NextChar();
            }
            else if (MainState.Camera.Vo.Count % speed == 0)
            {
                MainState.Scenario.NextChar();
            }

            var charCount = MainState.Scenario.Vo.CharCount;

            string visualText = string.Empty;
            if (currentLine.Count() < charCount)
            {
                visualText = currentLine;
            }
            else
            {
                visualText = currentLine.Substring(0, charCount);
            }

            //折り返し処理
            Ref.ctl.textAreaShadow.GetComponent<GUIText>().text = CommonLogic.createTextAreaText(visualText);
            Ref.ctl.textArea.GetComponent<GUIText>().text = CommonLogic.createTextAreaText(visualText);

            if (CommonLogic.DuringSkip())
            {
                ClearName();
                return true;
            }

            if (Input.GetMouseButtonDown(0) && MainState.Camera.Vo.clickeSpantime == 0)
            {
                MainState.Camera.Vo.clickeSpantime = 5;

                if (MainState.Scenario.IsCharEnd())
                {
                    MainState.Camera.Vo.Count = 0;
                    ClearName();
                    return true;
                }
                else
                {
                    MainState.Scenario.CharEnd();
                }
            }

            DrawName();

            return false;
        }

        public void DrawName()
        {
            if (CommonLogic.SwitchReadLine(MainState.Scenario.Vo.CurrentLine) == CodeScenarioSwitch.Scenario
               && CommonLogic.IsSpeakLine(MainState.Scenario.Vo.CurrentLine)
               && Ref.ctl.speakerName != string.Empty)
            {
                Ref.ctl.messageNameWindow.SetAlpha(0.5f);
                Ref.ctl.textName.GetComponent<GUIText>().text = Ref.ctl.speakerName;
                Ref.ctl.textNameShadow.GetComponent<GUIText>().text = Ref.ctl.speakerName;
            }
        }

        public void ClearName()
        {
            Ref.ctl.textName.GetComponent<GUIText>().text = string.Empty;
            Ref.ctl.textNameShadow.GetComponent<GUIText>().text = string.Empty;

            //Scenario.CurrentFileNameによって分ける
            Ref.ctl.speakerName = "ヒロ";
            //暫定
            if (MainState.Scenario.Vo.CurrentFileName == "c3-1"
               || MainState.Scenario.Vo.CurrentFileName == "c3-2"
               || MainState.Scenario.Vo.CurrentFileName == "c3-3"
               || MainState.Scenario.Vo.CurrentFileName == "c3-4"
               || MainState.Scenario.Vo.CurrentFileName == "ed"
               )
            {
                Ref.ctl.speakerName = "舞";
            }

            //次の行もセリフなら、消さないで継続
            //ただし、画面遷移は消す（例外的にマクロ名で指定）
            var sCount = MainState.Scenario.Vo.LineCount + 1;
            string nextLine = MainState.Scenario.GetSpecificLine(sCount);
            while (nextLine != null
                  && (CommonLogic.SwitchReadLine(nextLine) != CodeScenarioSwitch.Scenario
                && !nextLine.Contains("@black_out")
                && !nextLine.Contains("@message_out")
                && nextLine != "@name")
                  )
            {
                nextLine = MainState.Scenario.GetSpecificLine(sCount);
                sCount++;
            }

            bool nextScenarioIsSpeak = nextLine != null
                && CommonLogic.SwitchReadLine(nextLine) == CodeScenarioSwitch.Scenario
                    && CommonLogic.IsSpeakLine(nextLine);

            if (!nextScenarioIsSpeak)
            {
                Ref.ctl.messageNameWindow.SetAlpha(0);
            }
        }

        #endregion

        #region マクロ読込

        /// <summary>
        /// Macro.txtからマクロ名のみ抜き出す
        /// </summary>
        /// <returns>The names.</returns>
        public IEnumerable<string> getMacroNames()
        {
            var tx = Resources.Load<TextAsset>("Macro").text;
            var macroData = tx.Split(new string[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries).ToList();

            return macroData.Where(n => Regex.IsMatch(n, "^[macro name=")).Select(n => n.Replace("[macro name=", "").Replace("]", ""));
        }

        /// <summary>
        /// Macro名と中身のコードの辞書
        /// </summary>
        /// <returns>The dictionary.</returns>
        public Dictionary<string, List<string>> createMacroDictionary()
        {
            var tx = Resources.Load<TextAsset>("Macro").text;
            var macroData = tx.Split(new string[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries).ToList();

            //ここがおかしい
            //もしかしたら取れてないかも
            var macroNames = macroData.Where(n => n.Contains("[macro name=")).Select(n => n.Replace("[macro name=", "").Replace("]", ""));

            var dics = new Dictionary<string, List<string>>();
            foreach (var dt in macroNames)
            {
                var values = macroData.SkipWhile(n => n != string.Format("[macro name={0}]", dt)).Skip(1).TakeWhile(n => n != "[endmacro]");
                dics.Add(dt, values.ToList());
            }

            return dics;
        }

        #endregion

    }
}
