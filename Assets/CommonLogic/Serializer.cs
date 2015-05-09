using Assets.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    public static partial class CommonLogic
    {
        public static class Serializer
        {
            //byteデータをクラスに変換する Load時に使用
            public static T BytesToClass<T>(byte[] byteArray) where T : class, new()
            {
                if (byteArray == null) { return null; }
               
                T resulClass;

                // BinaryFormatterオブジェクトの生成
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bForma = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                // メモリストリームをオープン
                using (System.IO.MemoryStream mStr = new System.IO.MemoryStream(byteArray))
                {
                    // バイナリの読み込みと、シリアル化された物の逆変換
                    resulClass = bForma.Deserialize(mStr) as T;
                }

                return resulClass;
            }

            //クラスをbyteデータにする　Save時に使用
            public static byte[] ClassToBytes<T>(T saveObject) where T : class, new()
            {

                byte[] resultBytes;
                // BinaryFormatterオブジェクト生成
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bForma = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                using (System.IO.MemoryStream mStr = new System.IO.MemoryStream())
                {
                    // シリアル化し、そのバイナリをメモリストリームへ保存
                    bForma.Serialize(mStr, saveObject);

                    // メモリストリームに格納されているデーターをバイト型配列に設定
                    resultBytes = mStr.ToArray();
                }

                return resultBytes;
            }
        }
    }
}
