using UnityEngine;

namespace GameModule
{
    public class MultiplierNoticeView11006 : TransformHolder
    {

        private TextMesh txtMultiplier;


        private Animator animatorCollectNotice;

        public MultiplierNoticeView11006(Transform inTransform) : base(inTransform)
        {

            txtMultiplier = transform.Find("Text_Total/Text")
                .GetComponent<TextMesh>();
            animatorCollectNotice = transform.GetComponent<Animator>();
            transform.gameObject.SetActive(false);
        }


        public void RefreshMultiplier(int multiplier)
        {
            txtMultiplier.text = multiplier.ToString();
        }


        public void Open()
        {
            
            this.transform.gameObject.SetActive(true);
            XUtility.PlayAnimation(animatorCollectNotice, "Appear", () =>
            {
                //animatorCollectNotice.Play("Idle");
            });

        }

        public void Close()
        {
            XUtility.PlayAnimation(animatorCollectNotice, "Close",
                () =>
                {
                    this.transform.gameObject.SetActive(false);
                });

        }
    }
}