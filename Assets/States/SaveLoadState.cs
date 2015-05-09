using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using System.IO;
using Base;

namespace Assets.States
{
    [Serializable]
    public class SaveLoadState : BaseState
    {
        public SaveLoadModel Vo { get; set; }

        //これを立てておくと、MainCameraでLoad_Runが走る
        public string LoadFlag { get; set; }
        //Load対象
        public string LoadNumber { get; set; }

        public SaveLoadState(GameStateModel targetState)
            : base(targetState)
        {
            this.Vo = new SaveLoadModel();
        }

        public bool Save(string dataNum)
        {
            //現在のシナリオ状況を取得
            this.Vo.Scenario = MainState.Scenario.Vo;
            this.Vo.UpdateDate = DateTime.Now;
            File.WriteAllBytes(this.Vo.SaveDir + dataNum + ".dat", CommonLogic.Serializer.ClassToBytes<SaveLoadModel>(this.Vo));
            return true;
        }

        public bool Load(string dataNum)
        {
            var loadData = File.ReadAllBytes(this.Vo.SaveDir + dataNum + ".dat");
            var loadClass = CommonLogic.Serializer.BytesToClass<SaveLoadModel>(loadData);

            this.LoadNumber = dataNum;
            this.Vo.Scenario = loadClass.Scenario;
            this.LoadFlag = CodeLoadFlag.ON;

            return true;
        }
    }
}
