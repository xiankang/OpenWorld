using Core.Utils.Log;
using System;
using System.IO;
using System.Text;

namespace EnAndDecode
{
    public class EncodeAndDecode
    {
        public static bool EncodeAssetBoundleDir(string dir)
        {
            //// 对创建好的文件进行加密处理  
            ///**
            // *   打包完成，就可以对打包文件进行加密了
            // *   需要打包文件的名称，完整路劲等 ,现在由于有多个AB包文件 
            // */
            //// 搜索 AssetBundle 包存放的目录 
            DirectoryInfo encodedir = new DirectoryInfo(dir);

            EncodeDirFile(encodedir);
            return true;
        }

        public static void EncodeDirFile(DirectoryInfo dirinfo)
        {
            foreach (var abfile in dirinfo.GetFiles("*.unity3d"))
            {
                string assetFile = abfile.FullName;
                string assetCodeFile = abfile.FullName;

                byte[] data = File.ReadAllBytes(assetFile);

                // FileStream encodeFile = File.Create(assetCodeFile);
                FileStream encodeFile = File.OpenWrite(assetCodeFile);
                byte[] result = EnAndDecode.EncodeAndDecode.Encode(data);
                encodeFile.Write(result, 0, result.Length);
                encodeFile.Close();
            }

            foreach (var dirvar in dirinfo.GetDirectories())
            {
                EncodeDirFile(dirvar);
            }
        }

        public static byte[] Encode(byte[] text)
        {
            if (text == null)
            {
                return null;
            }
            /*
             *  加密算法 (下面的方法不用占内存空间，不增加数据大小） 
             */

            int dataLength = text.Length;
            for (int i = 0; i < dataLength; i += 1)
            {
                byte tmp = text[i];
                text[i] = (byte)(tmp ^ '&');
            }

            return text;

        }

        public static byte[] Decode(byte[] text)
        {
            if (text == null)
                return null;
            /**
             *   解密算法 （这里设计和加密是相同的） 
             */


            int dataLength = text.Length;
            for (int i = 0; i < dataLength; i += 1)
            {
                byte tmp = text[i];
                text[i] = (byte)(tmp ^ '&');
            }

            return text;
        }

    }
}