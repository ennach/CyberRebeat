using Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.States
{
    public class MovieState : BaseState
    {
        public MovieModel Vo { get; set; }

        public MovieState(GameStateModel targetState)
            : base(targetState)
        {
            this.Vo = new MovieModel();
        }

        //ムービー実行
        public bool Play()
        {
            //ムービー準備
            if (this.Vo.MoviePlayFlag == CodeMoviePlayFlag.START)
            {
                //メモリ解放
                Resources.UnloadUnusedAssets();
                System.GC.Collect(2);

                //ムービーファイル読込、音量セット
                this.Vo.Movie = Resources.Load<MovieTexture>(string.Format("movie/{0}", this.Vo.MovieName));
                this.Vo.MovieView.texture = this.Vo.Movie;
                this.Vo.Audio.clip = this.Vo.Movie.audioClip;
                this.Vo.Audio.volume = MainState.Config.Vo.ConfigVolume * 0.25f;

                //開始
                this.Vo.Movie.Play();
                this.Vo.Audio.Play();
                this.Vo.MoviePlayFlag = CodeMoviePlayFlag.PLAYING;
                return true;

                /*
                //ムービーファイル読込、音量セット
                //終了時にオブジェクトを破壊するので、毎回取得する必要あり
                this.Vo.Player = GameObject.Find("MovieControl").AddComponent<MoviePlayer>();
                var movie = this.Vo.Player.GetComponent<MoviePlayer>();
                movie.Load(Resources.Load<TextAsset>("movie/" + this.Vo.MovieName));
                movie.drawToScreen = true;
                movie.GetComponent<AudioSource>().GetComponent<AudioSource>().volume = MainState.Config.Vo.ConfigVolume * 0.25f;

                //開始
                movie.play = true;
                this.Vo.MoviePlayFlag = CodeMoviePlayFlag.PLAYING;
                return true;
                 */
            }
            //ムービー中
            else if (this.Vo.MoviePlayFlag == CodeMoviePlayFlag.PLAYING)
            {
                //クリックでムービー強制終了
                if (Input.GetMouseButtonDown(1))
                {
                    this.Vo.Movie.Stop();
                    this.Vo.Audio.Stop();
                }

                if (!this.Vo.Movie.isPlaying)
                {
                    //終了
                    this.Vo.Movie.Stop();
                    this.Vo.Audio.Stop();
                    this.Vo.MoviePlayFlag = CodeMoviePlayFlag.NONE;
                    this.Vo.MovieName = null;
                    this.Vo.MovieView.texture = null;
                    this.Vo.Audio.clip = null;
                    Resources.UnloadUnusedAssets();
                    System.GC.Collect(2);
                }

                return true;

                /*
                //クリックでムービー強制終了
                if (Input.GetMouseButtonDown(1))
                {
                    this.Vo.Player.play = false;
                    //終了
                    this.Vo.MoviePlayFlag = CodeMoviePlayFlag.NONE;
                    this.Vo.MovieName = null;
                    Resources.UnloadUnusedAssets();
                    System.GC.Collect(2);
                }

                //ムービー終了
                //クリック、またはムービーが最後まで終わった
                if (this.Vo.Player.play == false)
                {
                    this.Vo.Player.drawToScreen = false;
                    //キャッシュクリア、メモリ解放など
                    this.Vo.Player.source = null;
                    TextAsset.DestroyImmediate(this.Vo.Player.source, true);
                    AudioClip.DestroyImmediate(this.Vo.Player.audioSource);
                    MoviePlayer.DestroyImmediate(GameObject.Find("MovieControl").GetComponent<MoviePlayer>());
                    AudioSource.DestroyImmediate(GameObject.Find("MovieControl").GetComponent<AudioSource>());

                    //終了
                    this.Vo.MoviePlayFlag = CodeMoviePlayFlag.NONE;
                    this.Vo.MovieName = null;
                    Resources.UnloadUnusedAssets();
                    System.GC.Collect(2);
                }
                return true;
                 */
            }
            else
            {
                //ムービー停止中
                return false;
            }
        }
    }
}
