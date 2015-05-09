using Assets.States;
using Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets
{
    public static partial class CommonLogic
    {
        /// <summary>
        /// デバッグ用　画面に任意の文字を表示
        /// </summary>
        /// <param name="str">String.</param>
        public static void WriteConsole(string str)
        {
            var csl = GameObject.Find("z_DbgConsole");
            csl.GetComponent<GUIText>().text = str;
        }

        /// <summary>
        /// 現在行のコマンドを取得
        /// </summary>
        /// <returns>The command word.</returns>
        /// <param name="currentLine">Current line.</param>
        public static string GetCommandWord(string currentLine)
        {
            var currentSplits = Regex.Split(currentLine, " ");
            return currentSplits[0].Substring(1);
        }

        /// <summary>
        /// コマンド行のパラメーターをハッシュで返す
        /// </summary>
        /// <returns>The analys.</returns>
        /// <param name="currentLine">Current line.</param>
        public static Dictionary<string, string> CommandParams(string currentLine)
        {
            var currentSplits = Regex.Split(currentLine, " ");
            var dics = new Dictionary<string, string>();

            foreach (var sp in currentSplits.Where(n => n.Contains("=")))
            {
                var param = Regex.Split(sp, "=");
                dics.Add(param[0].Replace("\"", ""), param[1].Replace("\"", ""));
            }

            return dics;
        }

        /// <summary>
        /// コマンド行のパラメーターをハッシュで返す
        /// マクロの%引数込み変換
        /// </summary>
        /// <returns>The analys.</returns>
        /// <param name="currentLine">Current line.</param>
        public static Dictionary<string, string> CommandParams(string currentLine, string originalLine)
        {
            var currentSplits = Regex.Split(currentLine, " ");
            var dics = new Dictionary<string, string>();

            var originalParam = CommandParams(originalLine);

            foreach (var sp in currentSplits.Where(n => n.Contains("=")))
            {
                var param = Regex.Split(sp, "=");
                var key = param[0].Replace("\"", "");
                var value = param[1].Replace("\"", "");

                if (value.Substring(0, 1) == "%")
                {
                    value = originalParam.Get(value.Substring(1));
                }

                dics.Add(key, value);
            }

            return dics;
        }

        //会話行か否か
        //名前欄表示判定で使う
        public static bool IsSpeakLine(string currentLine)
        {
            if (currentLine != null
               && currentLine.Length > 0
               && (currentLine.Substring(0, 1) == "「" || currentLine.Substring(0, 1) == "『"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //スキップ中かどうか
        public static bool DuringSkip()
        {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                return true;
            }
            return false;
        }

        //メッセージテキスト　改行処理
        public static string createTextAreaText(string visualText)
        {
            var lines = new List<string>();
            string str = string.Empty;
            string processChars = "『』「」。！？、";
            for (int i = 0; i < visualText.Length; i++)
            {
                var c = visualText[i];
                str += c;
                if (str.LengthByte() >= SettingClass.LINE_LENGTH
                   && i + 1 != visualText.Length
                   && !processChars.Contains(visualText[i + 1]))
                {
                    lines.Add(str + "\r\n");
                    str = string.Empty;
                }
            }

            lines.Add(str);
            return string.Join("", lines.ToArray());
        }

        //マウスホイール（上）取得
        public static bool MouseWheelUp()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            return scroll > 0;
        }

        //マウスホイール（下）取得
        public static bool MouseWheelDown()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            return scroll < 0;
        }

        /// <summary>
        /// シナリオ振り分け
        /// </summary>
        /// <returns>The read line.</returns>
        /// <param name="lineData">Line data.</param>
        public static int SwitchReadLine(string currentLine)
        {
            if (string.IsNullOrEmpty(currentLine) || currentLine.Substring(0, 1) == "*" || currentLine.Substring(0, 1) == ";")
            {
                return CodeScenarioSwitch.None;
            }
            else if (currentLine.Substring(0, 1) == "@")
            {
                return CodeScenarioSwitch.Command;
            }
            else
            {
                return CodeScenarioSwitch.Scenario;
            }
        }

        public static int LengthByte(this string str)
        {
            var byte_1 = str.Where(n => Regex.IsMatch(n.ToString(), @"[0-9A-Za-z\*\[\]\/\.\-]")).Count();
            var byte_2 = str.Where(n => !Regex.IsMatch(n.ToString(), @"[0-9A-Za-z\*\[\]\/\.\-]")).Count() * 2;

            return byte_1 + byte_2;
            //return Encoding.Unicode.GetByteCount(str);
        }




    }
}
