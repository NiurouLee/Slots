using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class LinkAnimationNode11026: TransformHolder
    {
        [ComponentBinder("animationNode_1")]
        protected Transform animationNode1;
        
       [ComponentBinder("animationNode_2")]
        protected Transform animationNode2;
        
        [ComponentBinder("animationNode_3")]
        protected Transform animationNode3;
        
        protected Transform[] animationNodes;

        public LinkAnimationNode11026(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this,transform);
            animationNodes = new[] {animationNode1, animationNode2, animationNode3};
        }

        public Vector3 GetIntegralAnimationNodePos(long index)
        {
            return animationNodes[index].transform.position;
        }
    }
}