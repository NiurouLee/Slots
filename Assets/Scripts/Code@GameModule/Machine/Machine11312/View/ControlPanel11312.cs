using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using static System.Net.Mime.MediaTypeNames;

namespace GameModule{
    public class ControlPanel11312 : ControlPanel
    {
        public GameObject ScoreBox;
        public Transform winTexts;
        public ControlPanel11312(Transform transform) : base(transform)
        {
            winTexts = transform.Find("WinText");
            
        }
        /// <summary>
        /// totalWin 框播放
        /// </summary>
        /// <returns></returns>
        public async void ShowScoreBox(){
            if(ScoreBox == null){
                ScoreBox = context.assetProvider.InstantiateGameObject("Score");
                ScoreBox.transform.SetParent(transform);
                ScoreBox.transform.localPosition = new Vector3(0,70,0);
                ScoreBox.transform.localScale = new Vector3(100,100,0);
                var sort = ScoreBox.AddComponent<SortingGroup>();
                sort.sortingLayerName = "FullScreenFx";
            }
            ScoreBox.gameObject.SetActive(true);
            ScoreBox.GetComponent<Animator>().Play("Open");
            await context.WaitSeconds(0.73f);
            ScoreBox.gameObject.SetActive(false);
        }
    }
}

