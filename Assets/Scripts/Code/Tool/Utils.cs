using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
public static class Utils
{
	public static void SetLocalPosition(Transform transform, float x = 0, float y = 0, float z = 0)
	{
		transform.localPosition = new Vector3(x, y, z);
	}
	
	public static void SetLocalScale(Transform transform, float x = 1, float y = 1, float z = 1)
	{
		transform.localScale = new Vector3(x, y, z);
	}
	  
	public static void SetLocalPosition(Transform[] transforms, Vector3[] positions, int count)
	{
		for (var i = 0; i < count; i++)
		{
			transforms[i].localPosition = positions[i];
		}
	}
	
	public static void SetLocalPosition(Transform[] transforms, float[] posX,  float[] posY,  float[] posZ, int count)
	{
		for (var i = 0; i < count; i++)
		{
			transforms[i].localPosition = new Vector3(posX[i], posY[i], posZ[i]);
		}
	}
	
    public static long ToUnixTime(DateTime date)
    {
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return Convert.ToInt64((date - epoch).TotalSeconds);
    }

	public static string ThoundsSeparate(this long num)
	{
		String result = String.Format("{0:###,###,###}", num);
		if (num == 0)
			return "0";
		return result;
	}
	
	public static string ThoundsSeparate(this int num)
	{
		String result = String.Format("{0:###,###,###}", num);
		if (num == 0)
			return "0";
		return result;
	}

	public static string SecToTime(int secs)
	{
		TimeSpan t = TimeSpan.FromSeconds(secs);
		if(t.Days > 0)
        {
			if(t.Days > 1)
            {
				return t.Days + " days";
            }
			return t.Days + " day";
        }
		return t.ToString();
		/*
		int d = Mathf.FloorToInt(secs / (3600 * 24));
		int h = 0;
		int m = 0;
		string s = "00";
		if(d > 0)
        {
			h = Mathf.FloorToInt((secs - d * (3600 * 24))/3600);
			m = Mathf.FloorToInt((secs - d * (3600 * 24) - h * 3600) / 60);
			s = ((int)(secs - d * (3600 * 24) - h * 3600 - m * 60)).ToString();
		}
		else
        {
			h = Mathf.FloorToInt(secs / 3600);
			m = Mathf.FloorToInt((secs - h * 3600) / 60);
			s = ((int)(secs - h * 3600 - m * 60)).ToString();
		}

		if (Convert.ToInt32(s) < 10)
		{
			s = "0" + s;
		}
		if(d > 0)
        {
			return d + ":" + h + ":" + m + ":" + s;
		}
		else
        {
			if (h == 0)
			{
				return m + ":" + s;
			}
			else
            {
				return h + ":" + m + ":" + s;
			}
		}
		*/
	} 
	
	public static string CalcMd5Hash(string input)
    {
        // Convert the input string to a byte array and compute the hash.
        byte[] data = Encoding.UTF8.GetBytes(input);
        return CalcMd5Hash(data);
    }

    public static string CalcMd5Hash(byte[] data)
    {
        // Create a new md5
        System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
        byte[] bytes = md5.ComputeHash(data);

        // Create a new Stringbuilder to collect the bytes
        // and create a string.
        StringBuilder sBuilder = new StringBuilder();

        // Loop through each byte of the hashed data
        // and format each one as a hexadecimal string.
        for (int i = 0; i < bytes.Length; i++)
        {
            sBuilder.Append(bytes[i].ToString("x2").ToLower());
        }

        // Return the hexadecimal string.
        return sBuilder.ToString();
    }
    
    private static int GetDirection(Vector3 currentDirection, Vector3 nextDirection)
    {
        Vector3 cross = Vector3.Cross(currentDirection, nextDirection);

        if (Vector3.Dot(cross, Vector3.forward) > 0.0)
        {
            //Debug.Log("Right turn");
            return 1;
        }
        else
        {
            //Debug.Log("Left turn");
            return 0;
        }
    }
    
    public static int safeAreaBottom
    {
        get
        {
            return 34;
        }
    }

    public static int safeAreaTop
    {
        get
        {
            return 44;
        }
    }

    public const float edgeScale = 1.9f;
    public static bool isLongScreen
    {
        get
        {
            float scale = Screen.width / (float)Screen.height;
            
            if (scale > edgeScale)
            {
                return true;
            }
            return false;
        }
    }

	public const float smallScale = 1.7f;
	public static bool isSmallScreen
	{
		get
		{
			float scale = Screen.width / (float)Screen.height;

			if (scale < smallScale)
			{
				return true;
			}
			return false;
		}
	}

	public static bool isIphoneX
    {
        get
        {
#if UNITY_EDITOR
            if (Screen.width == 1125 && Screen.height == 2436)
            {
                return true;
            }
            return false;
#else
        if(SystemInfo.deviceModel.Contains("iPhone10.3") || SystemInfo.deviceModel.Contains("iPhone10.6"))
        {
            return true;
        }
        return false;
#endif
        }
    }
}
