using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP;
using DragonU3DSDK;
using Tool;
using UnityEngine;
using UnityEngine.Networking;

namespace GameModule
{

    public class FacebookPicMgr : Singleton<FacebookPicMgr>
    {
        private Dictionary<string, Texture2D> mFacebookPicCacheDict = new Dictionary<string, Texture2D>();
        private string fbAvatarUrl = "https://graph.facebook.com/{0}/picture?width=84&height=84&type=normal";
        //private string fbAvatarUrl = "https://www.facebook.com/photo?fbid={0}";
        //private string fbAvatarUrl = "https://graph.facebook.com/{0}/picture?type=normal";

        
        public void GetFacebookPic(string fbId, Action<string, Texture2D> callback)
        {
            if (string.IsNullOrEmpty(fbId))
                return;

            if (mFacebookPicCacheDict.ContainsKey(fbId))
            {
                if (callback != null)
                    callback(fbId, mFacebookPicCacheDict[fbId]);
            }
            else
            {
                IEnumeratorTool.instance.StartCoroutine(GetPicture(fbId, callback));
            }
        }

        IEnumerator GetPicture(string fbId, Action<string, Texture2D> callback)
        {
            string uri = string.Format(fbAvatarUrl, fbId);
            using (HTTPRequest request = new HTTPRequest(new Uri(uri), HTTPMethods.Get, (req, rep) =>
              {
                  if (rep == null)
                  {
                      DebugUtil.LogError("GetPicture() Response null {0}", uri);
                      callback("GetPicture() Response null {0}", null);
                  }
                  else if (rep.StatusCode >= 200 && rep.StatusCode < 300)
                  {
                      //DebugUtil.LogError("GetPicture() Success. StatusCode={0}, DataAsTexture2D={1}, DataAsText={2}, Data={3} ", rep.StatusCode, rep.DataAsTexture2D, rep.DataAsText, rep.Data);

                      if (rep.DataAsTexture2D != null)
                      {
                          Texture2D picTexture = rep.DataAsTexture2D;
                          if (mFacebookPicCacheDict.ContainsKey(fbId))
                              mFacebookPicCacheDict[fbId] = picTexture;
                          else
                              mFacebookPicCacheDict.Add(fbId, picTexture);

                          try
                          {
                              callback?.Invoke(fbId, mFacebookPicCacheDict[fbId]);
                          }
                          catch (Exception e)
                          {
                              throw e;
                          }
                      }
                  }
                  else
                  {
                      DebugUtil.LogError("GetPicture() Unexpected response from server, status = {0}", rep.StatusCode);
                  }
              })
            {
                DisableCache = true,
                IsCookiesEnabled = false,
                ConnectTimeout = TimeSpan.FromSeconds(5),
                Timeout = TimeSpan.FromSeconds(20)
            })
            {
                request.Send();
                yield return IEnumeratorTool.instance.StartCoroutine(request);
            }
        }
    }
}
