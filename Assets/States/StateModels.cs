using Assets.Accessor;
using Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.States
{
    //メイン
    public class GameStateModel
    {
        public ConfigState Config { get; set; }
        public SaveLoadState SaveLoad { get; set; }
        public ScenarioState Scenario { get; set; }
        public AudioState Audio { get; set; }
        public MovieState Movie { get; set; }
        public PopupState Popup { get; set; }
        public CameraState Camera { get; set; }
        public BackLogState BackLog { get; set; }

        public GameStateModel()
        {
            Config = new ConfigState(this);
            SaveLoad = new SaveLoadState(this);
            Scenario = new ScenarioState(this);
            Audio = new AudioState(this);
            Movie = new MovieState(this);
            Popup = new PopupState(this);
            Camera = new CameraState(this);
            BackLog = new BackLogState(this);
        }
    }


    //コンフィグ関連
    [Serializable]
    public class ConfigModel
    {
        public string SaveDir { get { return Application.dataPath + SettingClass.SAVE_DIR; } }
        public DateTime UpdateDate { get; set; }

        public int ConfigSpeead { get; set; }
        public int ConfigVolume { get; set; }

        public int GameClearFlag { get; set; }
    }

    //セーブ・ロード関連
    [Serializable]
    public class SaveLoadModel
    {
        //保存ディレクトリ
        public string SaveDir { get { return Application.dataPath + SettingClass.SAVE_DIR; } }
        //シナリオ状況
        public ScenarioModel Scenario { get; set; }
        //更新日時
        public DateTime UpdateDate { get; set; }
    }

    //シナリオ関連
    [Serializable]
    public class ScenarioModel
    {
        //シナリオファイルの中身
        public List<string> ScenarioData { get; set; }
        //現在行数
        public int LineCount { get; set; }
        //現在文字位置
        public int CharCount { get; set; }
        //現在シナリオファイル
        public string CurrentFileName { get; set; }
        //現在行
        public string CurrentLine{ get{ return ScenarioData[LineCount]; }}
        //再帰　コマンド実行状況
        public List<MacroClass> MacroQueue { get; set; }

        public ScenarioModel()
        {
            this.LineCount = 0;
            this.CharCount = 0;
            this.MacroQueue = new List<MacroClass>();
        }
    }

    //マクロ　シナリオ実行用
    [Serializable]
    public class MacroClass
    {
        public List<string> Values { get; set; }
        public int Queue { get; set; }

        public void QueueNext()
        {
            Queue++;
        }
    }

    //BGM関連
    [Serializable]
    public class AudioModel
    {
        public GameObject AudioSource { get; set; }
        public int BGM_STATUS { get; set; }
        public int ToAudioVolume { get; set; }

        public AudioModel()
        {
            AudioSource = GameObject.Find("AudioSource");
            BGM_STATUS = CodeBgmStatus.NoExec;
            ToAudioVolume = CodeToAudioVolume.NoChange;
        }
    }

    //ムービー関連
    public class MovieModel
    {
        public int MoviePlayFlag {get;set;}
        public string MovieName {get;set;}
        //public MoviePlayer Player{get;set;}

        public GUITexture MovieView { get; set; }
        public AudioSource Audio { get; set; }
        public MovieTexture Movie { get; set; }

        public MovieModel()
        {
            var control = GameObject.Find("MovieView");
            this.MovieView = control.GetComponent<GUITexture>();
            this.Audio = control.GetComponent<AudioSource>();
            this.MoviePlayFlag = CodeMoviePlayFlag.NONE;
        }
    }

    //ポップアップ
    public class PopupModel
    {
        public string Text { get; set; }
        public bool IsQuestion { get; set; }
        public UnityAction OnClickOK { get; set; }
        public UnityAction OnClickNG { get; set; }

        public PopupModel()
        {
            this.Text = string.Empty;
            this.IsQuestion = true;
            this.OnClickOK = () => { };
            this.OnClickNG = () => { };
        }
    }

    public class BackLogModel
    {
        public int Count { get; set; }

        public int BackLogCount { get; set; }
        public int BackLogStatus { get; set; }

        public GameObject MainView { get; set; }
        public MainAccessor ctl { get; set; }


        public BackLogModel()
        {
            this.Count = 0;
            this.BackLogCount = 0;
            this.BackLogStatus = CodeBackLogStatus.None;
            this.MainView = GameObject.Find("backLogCover");
        }
    }

    public class CameraModel 
    {
        public int Count { get; set; }
        public int COMMAND_STATUS { get; set; }
        public int BackMessageAppear { get; set; }
        public int  clickeSpantime { get; set; }
        public Dictionary<string, List<string>> MacroDictionary { get; set; }
        public ScreenSizeModel ScreenSize { get; set; }

        public class ScreenSizeModel
        {
            public int Height { get; set; }
            public int Width { get; set; }
        }

        public CameraModel()
        {
            ScreenSize = new ScreenSizeModel();
        }
    }
}
