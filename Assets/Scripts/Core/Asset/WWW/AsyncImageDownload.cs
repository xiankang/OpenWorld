using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using Core.Utils.Log;
using System.Collections.Generic;
using System.Net;

namespace Core.Asset
{
    public class AsyncImageDownload
    {
        private static Dictionary<string, Sprite> spriteCacheDic = new Dictionary<string, Sprite>();

        public static bool Init()
        {
            if (!Directory.Exists(Application.persistentDataPath + "/ImageCache/"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/ImageCache/");
            }
            return true;
        }

        public static void SetAsyncImage(string url, Image image, bool mipmap = false)
        {
            if (string.IsNullOrEmpty(url))
            {
                LogHelper.ERROR("AsyncImageDownload", "SetAsyncImage error url is null");
                return;
            }

            //判断是否是第一次加载这张图片  
            if (!File.Exists(ImageCachePath + url.GetHashCode()))
            {
                //如果之前不存在缓存文件  
                ResourceManager._Instance.StartCoroutine(DownloadImage(url, image, mipmap));
            }
            else
            {
                ResourceManager._Instance.StartCoroutine(LoadLocalImage(url, image, mipmap));
            }
        }

        private static IEnumerator DownloadImage(string url, Image image, bool mipmap, bool retryed = false)
        {
            if (string.IsNullOrEmpty(url))
            {
                LogHelper.ERROR("AsyncImageDownload", "DownloadImage error url is null");
                yield break;
            }

            if (spriteCacheDic.ContainsKey(url))
            {
                if (image != null)
                    image.overrideSprite = spriteCacheDic[url];

                yield break;
            }

            LogHelper.DEBUG("AsyncImageDownload", "Downloading new image:{0} url:{1}", (ImageCachePath + url.GetHashCode()), url);//url转换HD5作为名字
            WWW www = new WWW(url);
            yield return www;
            try
            {
                if (!string.IsNullOrEmpty(www.error) && !retryed)
                {
                    ResourceManager._Instance.StartCoroutine(DownloadImage(url, image, mipmap, true));
                }
                else
                {
                    Texture2D tex2d;
                    if (mipmap)
                    {
                        tex2d = www.texture;
                    }
                    else
                    {
                        var tex = www.texture;
                        tex2d = new Texture2D(tex.width, tex.height, tex.format, false);
                        www.LoadImageIntoTexture(tex2d);
                        //tex2d.SetPixels(tex.GetPixels(0), 0);
                        //tex2d.Apply();

                        //将图片保存至缓存路径  
                        byte[] pngData = tex2d.EncodeToPNG();
                        File.WriteAllBytes(ImageCachePath + url.GetHashCode(), pngData);
                    }
                    Sprite m_sprite = Sprite.Create(tex2d, new Rect(0, 0, tex2d.width, tex2d.height), Vector2.zero);
                    if (!spriteCacheDic.ContainsKey(url))
                    {
                        spriteCacheDic.Add(url, m_sprite);
                    }

                    //image.sprite = m_sprite;
                    if (image != null)
                        image.overrideSprite = m_sprite;
                }
            }
            catch (System.Exception ex)
            {
                LogHelper.ERROR("AsyncImageDownload", "Downloading new image:{0} url:{1} error:{2}", (ImageCachePath + url.GetHashCode()), url, ex.Message);
            }
        }

        private static IEnumerator LoadLocalImage(string url, Image image, bool mipmap)
        {
            if (string.IsNullOrEmpty(url))
            {
                LogHelper.ERROR("AsyncImageDownload", "LoadLocalImage error url is null");
                yield break;
            }

            if (spriteCacheDic.ContainsKey(url))
            {
                if (image != null)
                    image.overrideSprite = spriteCacheDic[url];

                yield break;
            }

            string filePath = "file:///" + ImageCachePath + url.GetHashCode();

            LogHelper.DEBUG("AsyncImageDownload", "Getting local image:{0}", filePath);
            WWW www = new WWW(filePath);
            yield return www;

            try
            {
                Texture2D texture;
                if (mipmap)
                {
                    texture = www.texture;
                }
                else
                {
                    var tex = www.texture;
                    texture = new Texture2D(tex.width, tex.height, tex.format, false);
                    www.LoadImageIntoTexture(texture);
                    //texture.SetPixels(tex.GetPixels(0), 0);
                    //texture.Apply();
                }

                Sprite m_sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                if (!spriteCacheDic.ContainsKey(url))
                {
                    spriteCacheDic.Add(url, m_sprite);
                }
                //image.sprite = m_sprite;
                if (image != null)
                    image.overrideSprite = m_sprite;
            }
            catch (System.Exception ex)
            {
                LogHelper.DEBUG("AsyncImageDownload", "Getting local image:{0} error{1}", filePath, ex.Message);
            }
        }

        public static string ImageCachePath
        {
            get
            {
                //pc,ios //android :jar:file//  
                return Application.persistentDataPath + "/ImageCache/";
            }
        }
    }
}