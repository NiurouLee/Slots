using System;
using System.Globalization;
using UnityEngine;

namespace GameModule
{
    public class Tools
    {
        public static string GetLeastDigits(object inNum,int leastDigits = 0,bool needZeroPlaceHolder = false)
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
                
                double f = Math.Floor((double)(tempNum * Math.Pow(10, decimalsNum)));
                tempNum = f / Math.Pow(10, decimalsNum);

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
        public static float GetTextWidth(TextMesh mesh)
        {
            float width = 0;
            foreach (char symbol in mesh.text)
            {
                CharacterInfo info;
                if (mesh.font.GetCharacterInfo(symbol, out info, mesh.fontSize, mesh.fontStyle))
                {
                    width += info.advance;
                }
            }
            return width * 0.1f;
        }
    }
}