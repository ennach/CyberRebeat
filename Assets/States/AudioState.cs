using Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.States
{
    public class AudioState : BaseState
    {
        public AudioModel Vo { get; set; }

        public AudioState(GameStateModel targetState)
            : base(targetState)
        {
            this.Vo = new AudioModel();
        }

        public void SetBGM(string audioFilePath, float vol)
        {
            this.Vo.AudioSource.SetBGM(audioFilePath, vol);
        }

        public void AudioPlay()
        {
            if (this.Vo.BGM_STATUS == CodeBgmStatus.Done)
            {
                if (!this.Vo.AudioSource.GetComponent<AudioSource>().isPlaying && this.Vo.AudioSource.GetComponent<AudioSource>().clip != null)
                {
                    this.Vo.AudioSource.GetComponent<AudioSource>().Play();
                }
                else if (this.Vo.AudioSource.GetComponent<AudioSource>().clip != null)
                {
                    this.Vo.AudioSource.GetComponent<AudioSource>().volume = MainState.Config.Vo.ConfigVolume * 0.01f;
                }
            }
        }

        public void FadeInOut()
        {
            if (this.Vo.ToAudioVolume == CodeToAudioVolume.NoChange) return;

            //ctlBackを上げ、ctlを下げていく
            if (this.Vo.BGM_STATUS == CodeBgmStatus.Start)
            {
                this.Vo.BGM_STATUS = CodeBgmStatus.FadeInOut;
            }
            else if (this.Vo.BGM_STATUS == CodeBgmStatus.FadeInOut)
            {
                // 音量の変化は6Fに1回
                if (MainState.Camera.Vo.Count % 6 != 0) { return; }

                var transVolume = 0.05f;
                var currentVolume = this.Vo.AudioSource.GetVolume();
                bool audioTrans = (0 < currentVolume && this.Vo.ToAudioVolume == CodeToAudioVolume.OFF)
                               || (currentVolume < 1 && this.Vo.ToAudioVolume == CodeToAudioVolume.ON);

                if (audioTrans)
                {
                    this.Vo.AudioSource.ChangeVolume(this.Vo.ToAudioVolume * transVolume);
                }
                else
                {
                    this.Vo.BGM_STATUS = CodeBgmStatus.LastOne;
                }
            }
            else if (this.Vo.BGM_STATUS == CodeBgmStatus.LastOne)
            {
                if (this.Vo.ToAudioVolume == CodeToAudioVolume.ON)
                {
                    this.Vo.BGM_STATUS = CodeBgmStatus.Done;
                }
                else if (this.Vo.ToAudioVolume == CodeToAudioVolume.OFF)
                {
                    this.Vo.BGM_STATUS = CodeBgmStatus.NoExec;
                }
                this.Vo.ToAudioVolume = CodeToAudioVolume.NoChange;
            }
        }
    }
}
