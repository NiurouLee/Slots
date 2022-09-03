using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class PigGroupView11024:TransformHolder
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
        public List<PigView11024> pigList = new List<PigView11024>();
        public List<Transform> boardList = new List<Transform>();
        public PigGroupView11024(Transform inTransform):base(inTransform)
        {
            pigList.Clear();
            for (var i = 1; i <= 3; i++)
            {
                var pigInstance = new PigView11024(transform.Find("Pig" + i),i-1);
                pigList.Add(pigInstance);
                boardList.Add(transform.Find("Pig"+i+"Bg"));
            }
            
        }

        public void InitAfterBindingContext()
        {
            
        }

        public void InitView()
        {
            for (var i = 0; i < 3; i++)
            {
                pigList[i].InitState((int) extraState.GetPigCollectLevel(i));
            }
        }

        public async Task CollectPigCoin(int pigType)
        {
            var targetType = (int) extraState.GetPigCollectLevel(pigType);
            if (targetType != pigList[pigType].nowLevel)
            {
                await pigList[pigType].CollectCoin();
                await pigList[pigType].Glow(targetType);   
            }
            else
            {
                await pigList[pigType].CollectCoin();
            }
        }

        public bool NeedGlow(int pigType)
        {
            var targetType = (int) extraState.GetPigCollectLevel(pigType);
            if (targetType != pigList[pigType].nowLevel)
            {
                return true;
            }
            return false;
        }

        public async Task PerformExplode()
        {
            var taskList = new List<Task>();
            for (var i = 0; i < 3; i++)
            {
                if (extraState.HasReSpinType(i))
                {
                    taskList.Add(pigList[i].Explode());
                }
            }
            AudioUtil.Instance.PlayAudioFx("J0123_Trigger");
            await Task.WhenAll(taskList);
        }

        public Vector3 GetCollectPosition(int pigType)
        {
            return pigList[pigType].transform.position;
        }
    }
}