using System.Collections;
using System.Collections.Generic;
using Tool;
using UnityEngine;

namespace GameModule
{
    public class JackPotSmallPanel11024:TransformHolder
    {
        private JackpotInfoState11024 _jackpotInfoState;

        public JackpotInfoState11024 jackpotInfoState
        {
            get
            {
                if (_jackpotInfoState == null)
                {
                    _jackpotInfoState = context.state.Get<JackpotInfoState11024>();
                }
                return _jackpotInfoState;
            }
        }
        private JackPotPanel11024 _jpPanel;

        public JackPotPanel11024 jpPanel
        {
            get
            {
                if (_jpPanel == null)
                {
                    _jpPanel = context.view.Get<JackPotPanel11024>();
                }

                return _jpPanel;
            }
        }

        private int forceShowJpType = 0;
        private int nowShowJpType;
        private Coroutine rollJpCoroutine;
        private List<Transform> jpList = new List<Transform>();
        public JackPotSmallPanel11024(Transform inTransform):base(inTransform)
        {
            for (var i = 1; i <= 3; i++)
            {
                jpList.Add(transform.Find("Level"+i+"Group"));
            }
        }

        public void InitAfterBindingContext()
        {
            nowShowJpType = 1;
            ShowNextJp();
        }

        public void StartRollJP()
        {
            if (rollJpCoroutine != null)
            {
                StopRollJP();
            }
            rollJpCoroutine = context.StartCoroutine(RollJp());
            context.AddCoroutine(rollJpCoroutine);
        }

        public void StopRollJP()
        {
            if (rollJpCoroutine != null)
            {
                context.StopCoroutine(rollJpCoroutine);
                context.RemoveCoroutine(rollJpCoroutine);
            }
        }
        public IEnumerator RollJp () {
            while (true)
            {
                ShowNextJp();
                yield return new WaitForSeconds(2);
            }
        }
        public void ShowNextJp()
        {
            if (forceShowJpType > 0)
            {
                return;
            }
            nowShowJpType++;
            while (nowShowJpType > 3)
                nowShowJpType -= 3;
            ShowJp(nowShowJpType);
        }

        public void ShowJp(int jpType)
        {
            for (var i = 1; i <= 3; i++)
            {
                if (i == jpType)
                {
                    ulong numJackpot = jackpotInfoState.GetJackpotValue((uint)(4-i));
                    jpList[i-1].gameObject.SetActive(true);
                    jpList[i - 1].Find("Num").GetComponent<TextMesh>().SetText("");
                    jpList[i-1].Find("Num").GetComponent<TextMesh>().SetText(numJackpot.GetCommaFormat());
                }
                else
                {
                    jpList[i-1].gameObject.SetActive(false);
                }
            }
        }
        public void SetForceShowJpType(int jpType)
        {
            forceShowJpType = jpType;
            if (jpType > 0)
            {
                ShowJp(jpType);
            }
            else
            {
                ShowJp(nowShowJpType);
            }
        }
    }
}