using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameModule
{
    public class LinkBigPigGroupView11024:TransformHolder
    {
        private ExtraState11024 _extraState;
        public ExtraState11024 extraState
        {
            get
            {
                if (_extraState == null)
                {
                    _extraState =  context.state.Get<ExtraState11024>();
                }
                return _extraState;
            }
        }
        public Dictionary<string, Transform> pigGroupList = new Dictionary<string, Transform>();
        public LinkBigPigGroupView11024(Transform inTransform):base(inTransform)
        {
        }

        public virtual void InitPigGroupList()
        {
            {
                string pigGroupType = "1";
                pigGroupList[pigGroupType] = transform.Find("Pig" + pigGroupType + "Group");
            }
            {
                string pigGroupType = "3";
                pigGroupList[pigGroupType] = transform.Find("Pig" + pigGroupType + "Group");
            }
            {
                string pigGroupType = "13";
                pigGroupList[pigGroupType] = transform.Find("Pig" + pigGroupType + "Group");
            }
        }
        public void InitAfterBindingContext()
        {
            InitPigGroupList();
        }

        public string GetNowPigType()
        {
            var nowPigType = "";
            for (var i = 0; i < 3; i++)
            {
                if (extraState.HasReSpinType(i))
                {
                    nowPigType += i+1;
                }
            }
            return nowPigType;
        }
        public void RefreshView()
        {
            var nowPigType = GetNowPigType();
            var keyList = new List<string>(pigGroupList.Keys);
            for (var i = 0; i < keyList.Count; i++)
            {
                var key = keyList[i];
                if (nowPigType == key)
                {
                    var pigGroup = pigGroupList[key];
                    pigGroup.gameObject.SetActive(true);
                    if (!extraState.HasReSpinType(1))
                    {
                        var pigTypeList = key.ToCharArray();
                        for (var i1 = 0; i1 < pigTypeList.Length; i1++)
                        {
                            var pigAnimator = pigGroup.Find("Pig" + pigTypeList[i1]).GetComponent<Animator>();
                            XUtility.PlayAnimation(pigAnimator,"IdleBig");
                            var pigBoard = pigGroup.Find("Pig" + pigTypeList[i1] + "Bg");
                            pigBoard.gameObject.SetActive(true);
                        }   
                    }
                }
                else
                {
                    pigGroupList[key].gameObject.SetActive(false);
                }
            }
        }

        public void HidePigBoard()
        {
            var nowPigType = GetNowPigType();
            var keyList = new List<string>(pigGroupList.Keys);
            for (var i = 0; i < keyList.Count; i++)
            {
                var key = keyList[i];
                if (nowPigType == key)
                {
                    var pigGroup = pigGroupList[key];
                    var pigTypeList = key.ToCharArray();
                    for (var i1 = 0; i1 < pigTypeList.Length; i1++)
                    {
                        var pigBoard = pigGroup.Find("Pig" + pigTypeList[i1] + "Bg");
                        pigBoard.gameObject.SetActive(false);
                    }
                }
            }
        }

        public Vector3 GetPigBoardPosition(int inPigType)
        {
            var pigType = inPigType + 1;
            var nowPigType = GetNowPigType();
            var pigGroup = pigGroupList[nowPigType];
            var pigBoard = pigGroup.Find("Pig" + pigType + "Bg");
            return pigBoard.position;
        }
    }
}