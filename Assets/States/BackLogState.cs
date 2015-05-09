using Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.States
{
    public class BackLogState : BaseState
    {
        public BackLogModel Vo { get; set; }

        public BackLogState(GameStateModel targetState)
            : base(targetState)
        {
            this.Vo = new BackLogModel();
        }
        
        //メニュースタート
        public void BackLogBegin()
        {
            this.Vo.BackLogCount = 0;
            this.Vo.MainView.SetAlpha(0.3f);
            this.Vo.BackLogStatus = CodeBackLogStatus.BackLog;
            var nowCount = this.Vo.BackLogCount;
            backLogBack();
            while (CommonLogic.SwitchReadLine(backLogCurrentLine()) != CodeScenarioSwitch.Scenario)
            {
                if (MainState.Scenario.Vo.LineCount - this.Vo.BackLogCount <= 1)
                {
                    this.Vo.BackLogCount = nowCount;
                    break;
                }
                backLogBack();
            }
        }
        
        //メニュー終了
        public void BackLogEnd()
        {
            this.Vo.BackLogCount = 0;
            this.Vo.BackLogStatus = CodeBackLogStatus.None;
            this.Vo.MainView.SetAlpha(0);
        }

        public void backLogBack()
        {
            if (MainState.Scenario.Vo.LineCount > this.Vo.BackLogCount)
            {
                this.Vo.BackLogCount++;
            }
        }

        public void backLogReverse()
        {
            if (this.Vo.BackLogCount > 1)
            {
                this.Vo.BackLogCount--;
            }
            else
            {
                BackLogEnd();
            }
        }

        public string backLogCurrentLine()
        {
            return MainState.Scenario.Vo.ScenarioData[MainState.Scenario.Vo.LineCount - this.Vo.BackLogCount];
        }
        
        /// <summary>
        /// trueで終了、falseで継続
        /// </summary>
        public bool Execute()
        {
            this.Vo.Count++;

            if (this.Vo.BackLogStatus == CodeMenuStatus.None)
            {
                return true;
            }

            switch (this.Vo.BackLogStatus)
            {
                case CodeBackLogStatus.BackLog:
                    DoBackLog();
                    break;
                    
            }
            
            return false;
        }

        public void DoBackLog()
        {
            /*
            if(ctl.textName != null)
            {
                ctl.textName.SetAlpha(0);
                ctl.textAreaShadow.SetAlpha(0);
            }
            */
            this.Vo.ctl.textArea.GetComponent<GUIText>().text = CommonLogic.createTextAreaText(backLogCurrentLine());
            this.Vo.ctl.textAreaShadow.GetComponent<GUIText>().text = CommonLogic.createTextAreaText(backLogCurrentLine());


            if (Input.GetMouseButtonDown(1))
            {
                this.BackLogEnd();
            }

            if (CommonLogic.MouseWheelUp())
            {
                var nowCount = this.Vo.BackLogCount;
                backLogBack();
                while (CommonLogic.SwitchReadLine(backLogCurrentLine()) != CodeScenarioSwitch.Scenario)
                {
                    if (MainState.Scenario.Vo.LineCount - this.Vo.BackLogCount <= 1)
                    {
                        this.Vo.BackLogCount = nowCount;
                        break;
                    }
                    backLogBack();
                }
            }

            if (CommonLogic.MouseWheelDown())
            {
                var nowCount = this.Vo.BackLogCount;
                backLogReverse();
                while (CommonLogic.SwitchReadLine(backLogCurrentLine()) != CodeScenarioSwitch.Scenario)
                {
                    if (this.Vo.BackLogCount <= 0)
                    {
                        this.Vo.BackLogCount = nowCount;
                        break;
                    }
                    backLogReverse();
                }

            }
        }
    }
}
