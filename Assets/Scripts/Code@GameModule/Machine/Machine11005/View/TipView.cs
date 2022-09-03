using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Tool;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameModule
{
    public class TipView: TransformHolder
    {

        protected TextMesh[] listTextMesh;
        protected SpriteRenderer[] listSpriteRenderer;
        
        public TipView(Transform inTransform,bool isAlphaAnim) : base(inTransform)
        {
            this.isAlphaAnim = isAlphaAnim;

            if (isAlphaAnim)
            {
                listTextMesh = transform.GetComponentsInChildren<TextMesh>(true);
                listSpriteRenderer = transform.GetComponentsInChildren<SpriteRenderer>(true);
            }

            var selectEventCustomHandler = transform.gameObject.AddComponent<SelectEventCustomHandler>();
            selectEventCustomHandler.BindingDeselectedAction(async (BaseEventData baseEventData) =>
            {
                   
                Close();
            });
        }


        protected Tweener tweener;
        protected Tweener tweenerAlpha;
        protected Coroutine coroAutoClose;

        public bool isAutoClose = false;

        public float timeAutoClose = 3;

        private bool isAlphaAnim = true;

        public bool isOpen
        {
            get;
            protected set;
        }

        protected bool isPlaying = false;

        public float timeAnim = 0.3f;
        public virtual void Open()
        {
            if (!isOpen && !isPlaying)
            {
                isPlaying = true;
                isOpen = true;
                EventSystem.current.SetSelectedGameObject(transform.gameObject);
                tweener?.Kill();
                tweenerAlpha?.Kill();
                if (coroAutoClose!=null)
                {
                    IEnumeratorTool.instance.StopCoroutine(coroAutoClose);
                }

                transform.gameObject.SetActive(true);
                transform.localScale = Vector3.zero;
                tweener = transform.DOScale(Vector3.one, timeAnim);
                tweener.onComplete += () =>
                {
                    isPlaying = false;
                };

                tweenerAlpha = DOTween.To(()=>0, (alpha) =>
                {
                    for (int i = 0; i < listSpriteRenderer.Length; i++)
                    {
                        Color c = listSpriteRenderer[i].color;
                        c.a = alpha;
                        listSpriteRenderer[i].color = c;
                    }
                    
                    for (int i = 0; i < listTextMesh.Length; i++)
                    {
                        Color c = listTextMesh[i].color;
                        c.a = alpha;
                        listTextMesh[i].color = c;
                    }
                }, 1f, timeAnimAlpha);

                if (isAutoClose)
                {
                    coroAutoClose = IEnumeratorTool.instance.StartCoroutine(AutoClose());
                }

                OnOpen();
            }

            
        }

        public virtual void OnOpen()
        {
            
        }


        protected IEnumerator AutoClose()
        {
            yield return new WaitForSeconds(timeAutoClose);
            Close();
        }

        public float timeAnimAlpha = 0.2f;
        public virtual void Close()
        {
            if (isOpen && !isPlaying)
            {
                isPlaying = true;
                isOpen = false;
                tweener?.Kill();
                tweenerAlpha?.Kill();
                if (coroAutoClose!=null)
                {
                    IEnumeratorTool.instance.StopCoroutine(coroAutoClose);
                }
                transform.gameObject.SetActive(true);
                tweener = transform.DOScale(Vector3.zero, timeAnim);
                tweener.onComplete += () =>
                {
                    isPlaying = false;
                    transform.gameObject.SetActive(false);
                };
                
                
                tweenerAlpha = DOTween.To(()=>1f, (alpha) =>
                {
                    for (int i = 0; i < listSpriteRenderer.Length; i++)
                    {
                        Color c = listSpriteRenderer[i].color;
                        c.a = alpha;
                        listSpriteRenderer[i].color = c;
                    }
                    
                    for (int i = 0; i < listTextMesh.Length; i++)
                    {
                        Color c = listTextMesh[i].color;
                        c.a = alpha;
                        listTextMesh[i].color = c;
                    }
                }, 0, timeAnimAlpha);
            }
        }

        public void ChangeView()
        {
            if (isOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }

    }
}