using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace GameModule
{
    public class JackpotPanel11029 : JackPotPanel
    {


        public JackpotPanel11029(Transform inTransform) : base(inTransform)
        {


        }
        
        public void ShowWinFrame(int jackpotid)
        {
            transform.Find($"Level{jackpotid}Group/MiniWinFrame").transform.gameObject.SetActive(true);
        }
        
        public void HideWinFrame(int jackpotid)
        {
            transform.Find($"Level{jackpotid}Group/MiniWinFrame").transform.gameObject.SetActive(false);
        }
    }
}
        
                
        
