using Assets.States;
using Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Assets
{
    public static partial class CommonLogic
    {
        public static class FileAccess
        {
            //コンフィグファイルを取得する
            public static ConfigModel GetConfigFile(GameStateModel MainState)
            {
                try
                {
                    var loadData = File.ReadAllBytes(MainState.Config.Vo.SaveDir + "Config.dat");
                    return CommonLogic.Serializer.BytesToClass<ConfigModel>(loadData);
                }
                catch
                {
                    MainState.Config.Vo.ConfigSpeead = CodeConfigSpeed.High;
                    MainState.Config.Vo.ConfigVolume = CodeConfigVolume.DEFAULT;
                    MainState.Config.Vo.GameClearFlag = CodeGameClearFlag.START;
                    MainState.Config.Vo.UpdateDate = DateTime.Now;
                    MainState.Config.Save();
                    return MainState.Config.Vo;
                }
            }

            //セーブデータを取得する
            public static SaveLoadModel GetSaveData(int dataNum, GameStateModel MainState)
            {
                try
                {
                    var str = dataNum.ToString("00");
                    var loadData = File.ReadAllBytes(MainState.SaveLoad.Vo.SaveDir + str + ".dat");
                    return CommonLogic.Serializer.BytesToClass<SaveLoadModel>(loadData);
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
