using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Base
{
    public static class Extension
    {
        /// <summary>
        /// ハッシュを取得
        /// </summary>
        /// <param name="dics">Dics.</param>
        /// <param name="key">Key.</param>
        public static string Get(this Dictionary<string,string> dics, string key)
        {
            string val;
            if (dics.TryGetValue(key, out val))
            {
                return val;
            }
            else
            {
                return null;
            }
        }

        //GUITextのアルファ値設定
        public static void SetTextAlpha(this GameObject obj, float alpha)
        {
            //var gui = obj.guiText;
            alpha = alpha > 1 ? 1 : alpha;
            alpha = alpha < 0 ? 0 : alpha;
            var guiColor = obj.GetComponent<GUIText>().color;
            obj.GetComponent<GUIText>().color = new Color(guiColor.r, guiColor.g, guiColor.b, alpha);
        }

        //GUITextureのアルファ値設定
        public static void SetAlpha(this GameObject obj, float alpha)
        {
            var gui = obj.GetComponent<GUITexture>();
            alpha = alpha > 1 ? 1 : alpha;
            alpha = alpha < 0 ? 0 : alpha;
            if (gui == null)
            {
                var spr = obj.GetComponent<SpriteRenderer>();
                obj.GetComponent<SpriteRenderer>().color = new Color(spr.color.r, spr.color.g, spr.color.b, alpha);
            }
            else
            {
                var guiColor = obj.GetComponent<GUITexture>().color;
                obj.GetComponent<GUITexture>().color = new Color(guiColor.r, guiColor.g, guiColor.b, alpha);
            }
        }

        //GUITextureのアルファ値取得
        public static float GetAlpha(this GameObject obj)
        {
            var gui = obj.GetComponent<GUITexture>();
            if (gui == null)
            {
                var spr = obj.GetComponent<SpriteRenderer>();
                return spr.color.a;
            }
            else
            {
                return obj.GetComponent<GUITexture>().color.a;
            }
        }

        //アルファ値加算
        public static void ChangeAlpha(this GameObject obj, float alpha)
        {
            obj.SetAlpha(obj.GetAlpha() + alpha);
        }

        /// <summary>
        /// BGMを鳴らす 基本的にDoCommandでの初期値設定用
        /// SetとPlayは分ける
        /// </summary>
        /// <param name="audioFilePath">Audio file path.</param>
        public static void SetBGM(this GameObject obj ,string audioFilePath, float vol)
        {
            var audio = obj.GetComponent<AudioSource>();


            if (audioFilePath != "none")
            {
                audio.clip = Resources.Load<AudioClip>(string.Format("bgm/{0}",audioFilePath));
                audio.loop = true;
                audio.volume = vol;
            }
            else
            {
                audio.clip = new AudioClip();
                audio.loop = true;
                audio.volume = vol;

            }
        }

        public static void SetVolume(this GameObject obj, float vol)
        {
            vol = vol > 1 ? 1 : vol;
            vol = vol < 0 ? 0 : vol;
            obj.GetComponent<AudioSource>().volume = vol;
        }

        public static float GetVolume(this GameObject obj)
        {
            return obj.GetComponent<AudioSource>().volume;
        }

        public static void ChangeVolume(this GameObject obj, float vol)
        {
            var currentVolume = obj.GetVolume();
            obj.SetVolume(currentVolume + vol);
        }
    }
}