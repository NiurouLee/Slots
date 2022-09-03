using System;
using System.Globalization;
using ILRuntime.Runtime;
using UnityEngine;
using UnityEngine.UI;

public static class Extension
{
    /// <summary>
    /// 按照3位一组用逗号分割数字
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static string GetCommaFormat(this long num)
    {
        return num.ToString("N0", CultureInfo.InvariantCulture);
    }
    
    /// <summary>
    /// 按照3位一组用逗号分割数字
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static string GetCommaFormat(this ulong num)
    {
        return num.ToString("N0", CultureInfo.InvariantCulture);
    }
    
    /// <summary>
    /// 如果小于10位按照3位一组用逗号分割数字，如果大于等于length位缩写
    /// </summary>
    /// <param name="num"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string GetCommaOrSimplify(this long num, int length = 10)
    {
        if(num >= Math.Pow(10, length - 1))
        {
            return num.GetAbbreviationFormat();
        }
        else
        {
            return num.ToString("N0", CultureInfo.InvariantCulture);
        }
    }
    /// <summary>
    /// 按照3位一组用逗号分割数字
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static string GetCommaFormat(this double num)
    {
        return num.ToInt64().ToString("N0", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// 按照3位一组用逗号分割数字
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static string GetCommaFormat(this int num)
    {
        return num.ToString("N0", CultureInfo.InvariantCulture);
    }
    
    /// <summary>
    /// 按照3位一组用逗号分割数字
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static string GetCommaFormat(this uint num)
    {
        return num.ToString("N0", CultureInfo.InvariantCulture);
    }

    public static bool IsNumeric(this object obj)
    {
        switch(Type.GetTypeCode(obj.GetType()))
        {
            case TypeCode.Byte:
            case TypeCode.SByte:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.Decimal:
            case TypeCode.Double:
            case TypeCode.Single:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    ///  格式化一个字符串为K/M/B/T/Q的缩写形式
    /// </summary>
    /// <param name="inNum">要格式化的数字</param>
    /// <param name="decimalsNum">保留小数位位数</param>
    /// <param name="roundingFunction">取整函数（Floor，Round, Celling等）</param>
    /// <param name="needZeroPlaceHolder">小数部分如果位数不够是否需要0占位</param>
    /// <returns></returns>
    public static string GetAbbreviationFormat(this object inNum, int decimalsNum = 2, bool needZeroPlaceHolder = false,
        Func<double, double> roundingFunction = null)
    {
        if (inNum.IsNumeric())
        {
            double num = Convert.ToDouble(inNum);
            
            roundingFunction = roundingFunction ?? Math.Floor;
            double toFormatValue = Math.Abs(num);
            var index = 0;
            string[] postfix = {"", "K", "M", "B", "T", "Q"};
            while (toFormatValue >= 1000)
            {
                toFormatValue /= 1000;
                index++;
            }

            double f = roundingFunction((double)(toFormatValue * Math.Pow(10, decimalsNum)));
            toFormatValue = f / Math.Pow(10, decimalsNum);

            //四舍五入后可能会多一位数字，再次做一次检查
            if (toFormatValue >= 1000)
            {
                toFormatValue /= 1000;
                index++;
            }

            index = Math.Min(index, postfix.Length - 1);

            toFormatValue = num > 0 ? toFormatValue : -toFormatValue;

            if (needZeroPlaceHolder)
            {
                return toFormatValue.ToString("N" + decimalsNum, CultureInfo.InvariantCulture) + postfix[index];
            }
            else
            {
                string decimalsFormat = ".##";
                if (decimalsNum != 2)
                {
                    decimalsFormat = ".";
                    for (var i = 0; i < decimalsNum; i++)
                        decimalsFormat += "#";
                }

                return toFormatValue.ToString(
                    "#,#" + decimalsFormat + postfix[index] + ";-#,#" + decimalsFormat + postfix[index] + ";0",
                    CultureInfo.InvariantCulture);
            }
        }
        return "";
    }

    public static string GetLeastDigits(this object inNum,int leastDigits = 0,bool needZeroPlaceHolder = false)
    {
        if (inNum.IsNumeric())
        {
            double num = Convert.ToDouble(inNum);
            var thresholdNum = Math.Pow(10, leastDigits);
            var tempNum = num;
            var index = 0;
            string[] postfix = {"", "K", "M", "B", "T", "Q"};
            string GetPostFix(int postfixIndex)
            {
                string totalPostfix = "";
                var postfixCount = postfix.Length - 1;
                while (postfixIndex > postfixCount)
                {
                    totalPostfix = postfix[postfixCount] + totalPostfix;
                    postfixIndex = postfixIndex - postfixCount;
                }
                totalPostfix = postfix[postfixIndex] + totalPostfix;
                return totalPostfix;
            }
            while (tempNum >= thresholdNum)
            {
                tempNum *= 0.001;
                index++;
            }

            var tempTempNum = tempNum;
            var tempNumDigits = 0;
            while (tempTempNum >= 1)
            {
                tempTempNum *= 0.1;
                tempNumDigits++;
            }

            var decimalsNum = leastDigits - tempNumDigits;
            if (needZeroPlaceHolder)
            {
                return tempNum.ToString("N" + decimalsNum, CultureInfo.InvariantCulture) + GetPostFix(index);
            }
            else
            {
                string decimalsFormat = ".##";
                if (decimalsNum != 2)
                {
                    decimalsFormat = ".";
                    for (var i = 0; i < decimalsNum; i++)
                        decimalsFormat += "#";
                }

                return tempNum.ToString(
                    "#,#" + decimalsFormat + postfix[index] + ";-#,#" + decimalsFormat + GetPostFix(index) + ";0",
                    CultureInfo.InvariantCulture);
            }
        }
        return "";
    }
    public static void SetText(this TextMesh textMesh, string text)
    {
        textMesh.text = text;
        textMesh.gameObject.SendMessage("UpdateTextScale", SendMessageOptions.DontRequireReceiver);
    }
    public static void SetText(this Text tex, string text)
    {
        tex.text = text;
        tex.gameObject.SendMessage("UpdateTextScale", SendMessageOptions.DontRequireReceiver);
    }
    
    public static bool HasState(this Animator anim, string stateName, int layer = 0) {
 
        int stateID = Animator.StringToHash(stateName);
        return anim.HasState(layer, stateID);
 
    }
}