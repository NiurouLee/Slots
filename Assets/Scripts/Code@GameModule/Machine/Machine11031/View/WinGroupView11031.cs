using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;

namespace GameModule
{
    public class WinGroupView11031 : TransformHolder
    {
        [ComponentBinder("WinText1")] protected Transform WinGroup1;

        [ComponentBinder("WinText2")] protected Transform WinGroup2;

        [ComponentBinder("WinText3")] protected Transform WinGroup3;

        [ComponentBinder("WinText4")] protected Transform WinGroup4;

        [ComponentBinder("WinText5")] protected Transform WinGroup5;

        [ComponentBinder("WinText6")] protected Transform WinGroup6;

        [ComponentBinder("WinText7")] protected Transform WinGroup7;

        [ComponentBinder("WinText8")] protected Transform WinGroup8;

        [ComponentBinder("WinText9")] protected Transform WinGroup9;

        [ComponentBinder("WinText10")] protected Transform WinGroup10;

        [ComponentBinder("WinHigh")] protected Transform WinGroupHigh;

        protected Transform[] winGroupsNodes;

        private List<Transform> lightGroupsNodes;

        public WinGroupView11031(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            winGroupsNodes = new[]
            {
                WinGroup1, WinGroup2, WinGroup3, WinGroup4, WinGroup5, WinGroup6, WinGroup7, WinGroup8, WinGroup9,
                WinGroup10, WinGroupHigh
            };
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            var highLightGp = transform.Find("HighLightGP");
            var childCount = 10;
            lightGroupsNodes = new List<Transform>(childCount);
            for (var i = 0; i < childCount; i++)
            {
                var lightGroup = highLightGp.GetChild(i);
                lightGroupsNodes.Add(lightGroup);
            }
        }

        public void ShowHighLight()
        {
            var chilliCount = (int) context.state.Get<ExtraState11031>().GetLinkPepperCount();
            if (chilliCount >= 6 && chilliCount < 40)
            {
                var index = Constant11031.GetFinalPepperIndex((int) context.state.Get<ExtraState11031>()
                    .GetLinkPepperCount());
                lightGroupsNodes[index].transform.gameObject.SetActive(true);
                XUtility.PlayAnimation(lightGroupsNodes[index].transform.gameObject.GetComponent<Animator>(),
                    "Highlight");
            }
        }

        public void ShowLinkHighLight()
        {
            var chilliCount = (int) context.state.Get<ExtraState11031>().GetLinkPepperCount();
            if (chilliCount >= 6 && chilliCount < 40)
            {
                var index = Constant11031.GetFinalPepperIndex((int) context.state.Get<ExtraState11031>()
                    .GetLinkPepperCount());
                lightGroupsNodes[index].transform.gameObject.SetActive(true);
                XUtility.PlayAnimation(lightGroupsNodes[index].transform.gameObject.GetComponent<Animator>(),
                    "Highlight");
            }
        }

        public void PlayBaseBiggerNum()
        {
            var chilliCount = (int) context.state.Get<ExtraState11031>().GetLinkPepperCount();
            if (chilliCount >= 6 && chilliCount < 40)
            {
                var index = Constant11031.GetFinalPepperIndex((int) context.state.Get<ExtraState11031>()
                    .GetLinkPepperCount());
                XUtility.PlayAnimation(winGroupsNodes[index].transform.Find("Num").gameObject.GetComponent<Animator>(),
                    "NumActive");
            }
        }

        public void PlayBaseBiggestNum()
        {
            XUtility.PlayAnimation(winGroupsNodes[10].transform.Find("Num").gameObject.GetComponent<Animator>(),
                "NumIncrease");
        }

        public void PlayLinkBiggerNum()
        {
            var chilliCount = (int) context.state.Get<ExtraState11031>().GetLinkPepperCount();
            if (chilliCount >= 6)
            {
                var index = Constant11031.GetFinalPepperIndex((int) context.state.Get<ExtraState11031>()
                    .GetLinkPepperCount());
                XUtility.PlayAnimation(winGroupsNodes[index].transform.Find("Num").gameObject.GetComponent<Animator>(),
                    "NumIncrease");
            }
        }

        public void PlayBiggerNumIdle()
        {
            for (var i = 0; i < winGroupsNodes.Length; i++)
            {
                winGroupsNodes[i].transform.Find("Num").gameObject.GetComponent<Animator>().Play("NumIdle");
            }
        }

        public void HideHighLight(int pay)
        {
            int index = context.state.Get<ExtraState11031>().GetPayIndex(pay);
            lightGroupsNodes[index].transform.gameObject.SetActive(false);
        }

        public void HideAllHighLight()
        {
            for (var i = 0; i < lightGroupsNodes.Count; i++)
            {
                lightGroupsNodes[i].transform.gameObject.SetActive(false);
            }
        }

        public Vector3 GetIntegralPos(int index)
        {
            return winGroupsNodes[index].transform.position;
        }

        public Vector3 GetLowIntegralPos()
        {
            return winGroupsNodes[0].transform.position;
        }

        public void ShowWinGroup()
        {
            for (var i = 0; i < winGroupsNodes.Length - 1; i++)
            {
                int winRate = Constant11031.WinRateLists[i];
                var chips = context.state.Get<BetState>().GetPayWinChips(winRate);
                winGroupsNodes[i].transform.Find("Text").gameObject.GetComponent<TextMesh>()
                    .SetText(chips.GetCommaFormat());
            }
        }
    }
}