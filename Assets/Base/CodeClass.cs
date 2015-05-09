using System;
using System.Collections;
using System.Collections.Generic;

namespace Base
{
    public enum EnumScrollBarType
    {
        NONE,
        SAVE,
        LOAD,
        CONFIG
    }

    public static class CodeGameClearFlag
    {
        public const int START = 0;
        public const int CLEAR = 9;
    }

    public static class CodeMoviePlayFlag
    {
        public const int NONE = 0;
        public const int START = 1;
        public const int PLAYING = 2;
    }

    public static class CodeConfigVolume
    {
        public const int DEFAULT = 80;
    }

    public static class CodeConfigSpeed
    {
        //1frame 2char
        public const int High = 75;
        //NoWait
        public const int Max = 100;
    }

    public static class CodeMenuStatus
    {
        public const int None = 0;
        public const int MainMenu = 1;
        public const int Save = 2;
        public const int Load = 3;
        public const int Config = 4;
    }

    public static class CodeConfirmStatus
    {
        public const int None = 0;
        public const int Confirm = 1;
    }

    public static class CodeConfirmResult
    {
        public const int OK = 1;
        public const int NoAnswer = 0;
        public const int NG = -1;
    }

    public static class CodeBackLogStatus
    {
        public const int None = 0;
        public const int BackLog = 1;
    }

    public static class CodeScenarioSwitch
    {
        public const int None = 0;
        public const int Command = 1;
        public const int Scenario = 2;
    }

    public static class CodeCommandStatus
    {
        public const int Start = 0;
        public const int DoCommand = 1;
        public const int LastOne = 2;
        public const int Done = 3;
    }

    public static class CodeBgmStatus
    {
        public const int NoExec = 0;
        public const int Start = 1;
        public const int FadeInOut = 2;
        public const int LastOne = 3;
        public const int Done = 4;
    }

    public static class CodeBackMessageAppear
    {
        //メッセージウィンドウのTransフラグ
        public const int Appear = 1;
        public const int NoChange = 0;
        public const int DisAppear = -1;
    }

    public static class CodeToAudioVolume
    {
        //BGMのTransフラグ
        public const int ON = 1;
        public const int NoChange = 0;
        public const int OFF = -1;
    }

    public static class CodeLoadFlag
    {
        //Load_Runを走らせる
        public const string ON = "1"; 
        public const string OFF = "0";
    }

    public static class CodeLayer
    {
        //BGMのTransフラグ
        public const string Base = "base"; 
        public const string Center = "0";
        public const string Left = "1";
        public const string Right = "2";
        public const string Icon = "3";
    }

    public static class CodeCommandWord
    {
        public const string Layopt = "layopt";
        public const string Backlay = "backlay";
        public const string Image = "image";
        public const string cm = "cm";
        public const string Trans = "trans";
        public const string Wait = "wait";
        public const string FadeInBGM = "fadeinbgm";
        public const string FadeOutBGM = "fadeoutbgm";
        public const string S = "s";
        public const string Name = "name";
        public const string Jump = "jump";
        public const string Video = "video";
        public const string GameClear = "gameClear";
        
        
        public static List<string> ToList()
        {
            return new List<string>()
            {
                CodeCommandWord.Layopt,
                CodeCommandWord.Backlay,
                CodeCommandWord.Image,
                CodeCommandWord.cm,
                CodeCommandWord.Trans,
                CodeCommandWord.Wait,   
                CodeCommandWord.FadeInBGM,
                CodeCommandWord.FadeOutBGM,
                CodeCommandWord.S,
                CodeCommandWord.Name,
                CodeCommandWord.Jump,
                CodeCommandWord.Video,
                CodeCommandWord.GameClear
            };
        }
        
        public static List<string> ToTransitionList()
        {
            return new List<string>()
            {
                CodeCommandWord.Trans
            };
        }

        public static List<string> ToAudioList()
        {
            return new List<string>()
            {
                CodeCommandWord.FadeInBGM,
                CodeCommandWord.FadeOutBGM
            };
        }
    }

    /*
    public static class CodeCommandWord
    {
        public const string New = "new";
        public const string BlackOut = "black_out";
        public const string MessageOn = "message_on";
        public const string cm = "cm";
        public const string Image = "image";
        public const string Wait = "wait";
        public const string BgmOn = "bgm_on";
        public const string BgmOff = "bgm_off";
        public const string Chara = "chara";
        public const string Chara1 = "chara1";
        public const string Chara2 = "chara2";
        public const string Chara12 = "chara12";


        public static List<string> ToList()
        {
            return new List<string>()
            {
                CodeCommandWord.New,
                CodeCommandWord.BlackOut,
                CodeCommandWord.MessageOn,
                CodeCommandWord.cm,
                CodeCommandWord.Image,
                CodeCommandWord.Wait,
                CodeCommandWord.BgmOn,
                CodeCommandWord.BgmOff,
                CodeCommandWord.Chara,
                CodeCommandWord.Chara1,
                CodeCommandWord.Chara2,
                CodeCommandWord.Chara12,

            };
        }

        public static List<string> ToTransitionList()
        {
            return new List<string>()
            {
                CodeCommandWord.New,
                CodeCommandWord.BlackOut,
                CodeCommandWord.MessageOn,
                CodeCommandWord.BgmOn,
                CodeCommandWord.BgmOff,
                CodeCommandWord.Chara,
                CodeCommandWord.Chara1,
                CodeCommandWord.Chara2,
                CodeCommandWord.Chara12,
            };
        }
    }
    */

}

