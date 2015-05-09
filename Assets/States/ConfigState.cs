using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using System.IO;
using Base;
using System.Xml.Serialization;

namespace Assets.States
{
    [Serializable]
    public class ConfigState : BaseState
    {
        public ConfigModel Vo { get; set; }

        public ConfigState(GameStateModel targetModel)
            : base(targetModel)
        {
            this.Vo = new ConfigModel();
        }

        public void GameClear()
        {
            if(this.Vo.GameClearFlag != CodeGameClearFlag.CLEAR)
            {
                this.Vo.GameClearFlag = CodeGameClearFlag.CLEAR;
            }
            this.Save();
        }

        public bool Save()
        {
            this.Vo.UpdateDate = DateTime.Now;
            var saveVo = new ConfigModel()
            {
                ConfigSpeead = this.Vo.ConfigSpeead,
                ConfigVolume = this.Vo.ConfigVolume,
                GameClearFlag = this.Vo.GameClearFlag,
                UpdateDate = DateTime.Now
            };
            var data = CommonLogic.Serializer.ClassToBytes<ConfigModel>(saveVo);
            File.WriteAllBytes(this.Vo.SaveDir + "Config.dat", data);
            return true;
        }
    }
}

