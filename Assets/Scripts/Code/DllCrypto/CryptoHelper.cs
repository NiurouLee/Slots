using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Debug = UnityEngine.Debug;

public class CryptoHelper
    { 
        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string EncryptStr(string value, string key)
        {
            try
            {
                Byte[] keyArray = System.Text.Encoding.UTF8.GetBytes(key);
                Byte[] toEncryptArray = System.Text.Encoding.UTF8.GetBytes(value);
                var rijndael = new System.Security.Cryptography.RijndaelManaged();
                rijndael.Key = keyArray;
                rijndael.Mode = System.Security.Cryptography.CipherMode.ECB;
                rijndael.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
                System.Security.Cryptography.ICryptoTransform cTransform = rijndael.CreateEncryptor();
                Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return null;
            }
        }
        
        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string DecryptStr(string value, string key)
        {
            try
            {
                Byte[] keyArray = System.Text.Encoding.UTF8.GetBytes(key);
                Byte[] toEncryptArray = Convert.FromBase64String(value);
                var rijndael = new System.Security.Cryptography.RijndaelManaged();
                rijndael.Key = keyArray;
                rijndael.Mode = System.Security.Cryptography.CipherMode.ECB;
                rijndael.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
                System.Security.Cryptography.ICryptoTransform cTransform = rijndael.CreateDecryptor();
                Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return System.Text.Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return null;
            }
        }
        
        /// <summary>
        /// AES 算法加密(ECB模式) 将明文加密
        /// </summary>
        /// <param name="toEncryptArray,">明文</param>
        /// <param name="Key">密钥</param>
        /// <returns>加密后base64编码的密文</returns>
        public static byte[] AesEncrypt(byte[] toEncryptArray, string Key)
        {
            try
            {
                byte[] keyArray = Encoding.UTF8.GetBytes(Key);

                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = rDel.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return resultArray;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return null;
            }
        }
        
        /// <summary>
        /// AES 算法解密(ECB模式) 将密文base64解码进行解密，返回明文
        /// </summary>
        /// <param name="toDecryptArray">密文</param>
        /// <param name="Key">密钥</param>
        /// <returns>明文</returns>
        public static byte[] AesDecrypt(byte[] toDecryptArray, string Key)
        {
            try
            {
                byte[] keyArray = Encoding.UTF8.GetBytes(Key);

                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = rDel.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toDecryptArray, 0, toDecryptArray.Length);
                return resultArray;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return null;
            }
        }
        
        /// <summary>
        /// AES 算法加密(ECB模式) 无padding填充，用于分块解密
        /// </summary>
        /// <param name="toEncryptArray,">明文</param>
        /// <param name="Key">密钥</param>
        /// <returns>加密后base64编码的密文</returns>
        public static byte[] AesEncryptWithNoPadding(byte[] toEncryptArray, string Key)
        {
            try
            {
                //byte[] keyArray = Encoding.UTF8.GetBytes(Key);
                byte[] keyArray = GetKey(Key);

                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.None;

                ICryptoTransform cTransform = rDel.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                
                return resultArray;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return null;
            }
        }
        
        /// <summary>
        /// AES 算法解密(ECB模式) 无padding填充，用于分块解密
        /// </summary>
        /// <param name="toDecryptArray">密文</param>
        /// <param name="Key">密钥</param>
        /// <returns>明文</returns>
        public static byte[] AesDecryptWithNoPadding(byte[] toDecryptArray, string Key)
        {
            try
            {
                //byte[] keyArray = Encoding.UTF8.GetBytes(Key);
                //UnityEngine.Profiling.Profiler.BeginSample($"==========GetKey");
                
                //Stopwatch stopwatch = Stopwatch.StartNew();
                //stopwatch.Restart();
                byte[] keyArray = GetKey(Key);
                
                //Debug.LogError($"==========GetKey:{stopwatch.ElapsedMilliseconds}");
               
                //stopwatch.Restart();
                //UnityEngine.Profiling.Profiler.EndSample();

                //UnityEngine.Profiling.Profiler.BeginSample($"==========AesDecryptWithNoPadding");

                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.None;

                ICryptoTransform cTransform = rDel.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toDecryptArray, 0, toDecryptArray.Length);
                
                //Debug.LogError($"==========AesDecryptWithNoPadding:{stopwatch.ElapsedMilliseconds}");
                //stopwatch.Stop();
                //UnityEngine.Profiling.Profiler.EndSample();
                return resultArray;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return null;
            }
        }

        private unsafe static byte[] GetKey(string key)
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(key);

            fixed (byte* b = keyArray)
            {
                var listB = b;
                int count = keyArray.Length;
                for (int i = 0; i < count; i++)
                {
                    *listB = (byte)(*listB << 1);
                    listB++;
                }
            }

            return keyArray;
        }
    }
