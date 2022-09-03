using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;

namespace GameModule
{
    public class MiniGameView11029 : TransformHolder
    {
        [ComponentBinder("WinGroup1")] protected Transform WinGroup1;

       [ComponentBinder("WinGroup2")] protected Transform WinGroup2;

       [ComponentBinder("WinGroup3")] protected Transform WinGroup3;

       [ComponentBinder("WinGroup4")] protected Transform WinGroup4;
       [ComponentBinder("WinGroup5")] protected Transform WinGroup5;

       [ComponentBinder("WinGroup6")] protected Transform WinGroup6;
       [ComponentBinder("WinGroup7")] protected Transform WinGroup7;

       [ComponentBinder("WinGroup8")] protected Transform WinGroup8;
       
        protected Transform[] winGroupsNodes;

        public MiniGameView11029(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            winGroupsNodes = new[]
                {WinGroup1, WinGroup2, WinGroup3, WinGroup4, WinGroup5, WinGroup6, WinGroup7, WinGroup8};
        }

        public void ShowWinGroup(ulong freeSpinBet)
        {
            ulong pay = 1;
            for (var i = 0; i < winGroupsNodes.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        pay = 125000;
                        break;
                    case 1:
                        pay = 1000;
                        break;
                    case 2:
                        pay = 625;
                        break;
                    case 3:
                        pay = 375;
                        break;
                    case 4:
                        pay = 300;
                        break;
                    case 5:
                        pay = 300;
                        break;
                    case 6:
                        pay = 200;
                        break;
                    case 7:
                        pay = 100;
                        break; } 
                ulong chips =  (pay * freeSpinBet / 100);
                winGroupsNodes[i].transform.Find("IntegralText").gameObject.GetComponent<TextMesh>().text =
                    chips.GetAbbreviationFormat(0);
                winGroupsNodes[i].transform.Find("MiniWinFrame").gameObject.SetActive(false);

            }
        }
        
        public void PlayWinFrame(int index)
        {
            winGroupsNodes[index-1].transform.Find("MiniWinFrame").gameObject.SetActive(true);
            winGroupsNodes[index-1].transform.Find("MiniWinFrame").gameObject.GetComponent<Animator>().Play("MiniWinFrame");
        }
    }
}