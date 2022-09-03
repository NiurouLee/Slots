// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/20/17:52
// Ver : 1.0.0
// Description : ValueUpdater.cs
// ChangeLog :
// **********************************************

using System;
using TMPro;
using UnityEngine.UI;

namespace GameModule
{
   public abstract class ValueUpdater<T, V>
   {
      public abstract void UpdateValue(T c, V v);
   }
   
   class  TextUpdater : ValueUpdater<TextMeshProUGUI, string>
   {
      public override void UpdateValue(TextMeshProUGUI textMeshProUGUI, string v)
      {
         textMeshProUGUI.text = v;
      }
   }
   
   class CommaNumberUpdater : ValueUpdater<TextMeshProUGUI, long>
   {
      public override void UpdateValue(TextMeshProUGUI textMeshProUGUI, long v)
      {
         textMeshProUGUI.text = v.GetCommaFormat();
      }
   }

   class AbbreviationNumUpdater:ValueUpdater<TextMeshProUGUI, long>
   {
      public override void UpdateValue(TextMeshProUGUI textMeshProUGUI, long v)
      {
         textMeshProUGUI.text = v.GetAbbreviationFormat();
      }
   }

   class ButtonActionUpdater:ValueUpdater<Button, Action>
   {
      public override void UpdateValue(Button button, Action click)
      {
         button.onClick.RemoveAllListeners();
         
         button.onClick.AddListener(() =>
         {
            click?.Invoke();
         });
      }
   }
}