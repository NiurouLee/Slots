using UnityEngine;

namespace GameModule
{
    public class SpecialContainerRoll<T>:Roll where T :ElementContainer
    {
        public SpecialContainerRoll(Transform inTransform, bool inTopRowHasHighSortOrder, bool inLeftColHasHighSortOrder, string inElementSortLayerName) 
            : base(inTransform,inTopRowHasHighSortOrder,inLeftColHasHighSortOrder,inElementSortLayerName)
        {
            
        }
        
        public override void BuildElementContainer()
        {
            containers = new ElementContainer[containerCount];

            for (var i = 0; i < containerCount; i++)
            {
                GameObject cv = new GameObject("ContainerView");

                cv.SetActive(true);
                
                var constructor = typeof(T).GetConstructor(new[] {typeof(Transform),typeof(string)});

                if (constructor != null)
                {
                    cv.transform.SetParent(transform, false);
                    cv.transform.localPosition = new Vector3(0, containerInitPos[i], 0);
                    var containerView = (T)constructor.Invoke(new object[] {cv.transform, elementSortLayerName});
                    containerView.Initialize(context);
                    containers[i] = containerView;
                }
            }

            shiftRowArrowIndex = 0;
        }
        
    }
}