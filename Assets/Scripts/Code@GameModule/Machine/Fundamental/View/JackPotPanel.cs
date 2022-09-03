using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameModule
{

    public class JackPotPanel: TransformHolder
    {
        protected JackpotInfoState jackpotInfoState;
        
        protected List<TextMesh> listTextJackpot = new List<TextMesh>();
        protected List<TextMeshPro> listTextJackpotPro = new List<TextMeshPro>();

        private bool firstExecuteUpdate = true;
        
        private bool isLocked = false;

        public JackPotPanel(Transform inTransform) : base(inTransform)
        {
            int index = 1;
            while (transform.Find($"Level{index}Group/Text"))
            {
                var transformTxt = transform.Find($"Level{index}Group/Text");
                if (transformTxt && transformTxt.GetComponent<TextMesh>())
                {
                    listTextJackpot.Add(transformTxt.GetComponent<TextMesh>());   
                }
                if (transformTxt && transformTxt.GetComponent<TextMeshPro>())
                {
                    listTextJackpotPro.Add(transformTxt.GetComponent<TextMeshPro>());   
                }
                index++;
            }
        }
         
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            jackpotInfoState = context.state.Get<JackpotInfoState>();
            
            UpdateJackpotValue();
        }


        public override void Update()
        {
            base.Update();

            if (isLocked)
                return;
          
            if (Time.frameCount % 5 == 0 || firstExecuteUpdate)
            {
                if (transform.gameObject.activeSelf || firstExecuteUpdate)
                {
                    UpdateJackpotValue();
                    firstExecuteUpdate = false;
                }
            }
        }

        public void UpdateJackpotAndLockJackpotPanel()
        {
            UpdateJackpotValue();
            isLocked = true;
        }
        
        public void UnlockJackpotPanel()
        {
            isLocked = false;
        }
        
        public virtual void UpdateJackpotValue()
        {
            for (int i = 0; i < listTextJackpot.Count; i++)
            {
                ulong numJackpot = jackpotInfoState.GetJackpotValue((uint)(i+1));
                //listTextJackpot[i].text = numJackpot.GetCommaFormat();
                if (listTextJackpot[i])
                {
                    listTextJackpot[i].SetText(numJackpot.GetCommaFormat());   
                }
            }
            for (int i = 0; i < listTextJackpotPro.Count; i++)
            {
                ulong numJackpot = jackpotInfoState.GetJackpotValue((uint)(i+1));
                //listTextJackpot[i].text = numJackpot.GetCommaFormat();
                if (listTextJackpotPro[i])
                {
                    listTextJackpotPro[i].SetText(numJackpot.GetCommaFormat());   
                }
            }
        }
    }
}
