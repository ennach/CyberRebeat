using Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.States
{
    [Serializable]
    public class ScenarioState : BaseState
    {
        public ScenarioModel Vo { get; set; }

        public ScenarioState(GameStateModel targetState)
            : base(targetState)
        {
            this.Vo = new ScenarioModel();
        }

        public string GetSpecificLine(int i)
        {
            if (this.Vo.ScenarioData.Count > i)
            {
                return this.Vo.ScenarioData[i];
            }
            else
            {
                return null;
            }
        }

        public bool IsCharEnd()
        {
            if (this.Vo.CharCount > this.Vo.CurrentLine.Count() - 1)
            {
                return true;
            }

            return false;
        }

        public void FileLoad(string fileName)
        {
            var tx = Resources.Load<TextAsset>(fileName).text;
            this.Vo.ScenarioData = tx.Split(new string[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
            this.Vo.CurrentFileName = fileName;
        }

        public bool NextLine()
        {
            this.Vo.LineCount++;
            this.Vo.MacroQueue.Clear();
            this.Vo.CharCount = 0;

            if (this.Vo.LineCount == this.Vo.ScenarioData.Count - 1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// シナリオジャンプ
        /// Storageファイルを読み込んで、markで始まる行までジャンプ
        /// markが空欄なら１行目
        /// </summary>
        /// <param name="storage">Storage.</param>
        /// <param name="mark">Mark.</param>
        public void Jump(string storage, string mark)
        {
            this.Vo.LineCount = 0;
            this.Vo.CharCount = 0;
            this.Vo.MacroQueue.Clear();
            this.FileLoad(storage.Replace(".ks", ""));
            if (mark != null && mark != string.Empty)
            {
                var index = this.Vo.ScenarioData.Select((n, i) => new { n, i }).Where(n => n.n.Contains(mark)).First().i;
                this.Vo.LineCount = index;

                /*
                while (!Regex.IsMatch(mark, string.Format("^{0}",CurrentLine)))
                {
                    NextLine();
                }
                */
            }
        }

        /// <summary>
        /// true:結論が出た　false:まだ遡り途中
        /// </summary>
        /// <returns><c>true</c>, if queue recursive was nexted, <c>false</c> otherwise.</returns>
        public bool NextQueueRecursive()
        {
            if (this.Vo.MacroQueue.Count > 0 && this.Vo.MacroQueue.Last().Queue >= this.Vo.MacroQueue.Last().Values.Count - 1)
            {
                this.Vo.MacroQueue.RemoveAt(this.Vo.MacroQueue.Count - 1);
                return this.NextQueueRecursive();
            }
            //最後のQueueが終了
            else if (this.Vo.MacroQueue.Count == 0)
            {
                //次のScenarioLineへの移動が必要
                return false;
            }
            else
            {
                this.Vo.MacroQueue.Last().Queue++;
                //普通に処理終了
                return true;
            }
        }

        public bool NextChar()
        {
            this.Vo.CharCount++;

            if (this.Vo.CharCount == this.Vo.CurrentLine.Count() - 1)
            {
                return false;
            }

            return true;
        }

        public void CharEnd()
        {
            this.Vo.CharCount = this.Vo.CurrentLine.Count();
        }

        public int GetMacroLastQueue()
        {
            return this.Vo.MacroQueue[this.Vo.MacroQueue.Count - 1].Queue;
        }

        public List<string> GetMacroLastValues()
        {
            return this.Vo.MacroQueue[this.Vo.MacroQueue.Count - 1].Values;
        }

        public void RemoveLastQueue()
        {
            this.Vo.MacroQueue.RemoveAt(this.Vo.MacroQueue.Count - 1);
        }
    }
}
