// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/02/08/11:47
// Ver : 1.0.0
// Description : TMPTextGradientScale.cs
// ChangeLog :
// **********************************************

using TMPro;
using UnityEngine;

namespace GameModule
{
    public class TMPTextGradientScale : MonoBehaviour
    {
        private TMP_Text m_TextComponent;
        private bool forceUpdate = false;
        public float scaleGradient = 0.95f;
        public float startScale = 1.0f;

        private void Awake()
        {
            m_TextComponent = GetComponent<TMP_Text>();
        }
        
        /// <summary>
        /// 设置ScaleParams
        /// </summary>
        /// <param name="inStartScale">第一个字符的Scale大小</param>
        /// <param name="inScaleGradient">Scale变化的Gradient</param>
        public void UpdateScaleParams(float inStartScale = 1.0f, float inScaleGradient = 0.9f)
        {
            startScale = inStartScale;
            scaleGradient = inScaleGradient;
            forceUpdate = true;
        }
    
        protected void Update()
        {
            if (m_TextComponent == null)
                return;
            
             //if the text and the parameters are the same of the old frame, don't waste time in re-computing everything
             if (!forceUpdate && !m_TextComponent.havePropertiesChanged)
             {
                 return;
             }

             forceUpdate = false;

             //during the loop, vertices represents the 4 vertices of a single character we're analyzing, 
             //while matrix is the roto-translation matrix that will rotate and scale the characters so that they will
             //follow the curve
             Vector3[] vertices;

             //Generate the mesh and get information about the text and the characters
             m_TextComponent.ForceMeshUpdate();

             TMP_TextInfo textInfo = m_TextComponent.textInfo;
             int characterCount = textInfo.characterCount;

             //if the string is empty, no need to waste time
             if (characterCount == 0)
                 return;

             float currentScale = startScale;
             //for each character
             for (int i = 0; i < characterCount; i++)
             {
                 //skip if it is invisible
                 if (!textInfo.characterInfo[i].isVisible)
                     continue;

                 //Get the index of the mesh used by this character, then the one of the material... and use all this data to get
                 //the 4 vertices of the rect that encloses this character. Store them in vertices
                 int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                 int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                 vertices = textInfo.meshInfo[materialIndex].vertices;

                 //Compute the baseline mid point for each character. This is the central point of the character.
                 //we will use this as the point representing this character for the geometry transformations
                 Vector3 charMidBaselinePos =
                     new Vector2((vertices[vertexIndex + 0].x + vertices[vertexIndex + 2].x) / 2,
                         textInfo.characterInfo[i].baseLine);

                 //remove the central point from the vertices point. After this operation, every one of the four vertices 
                 //will just have as coordinates the offset from the central position. This will come handy when will deal with the rotations

                 vertices[vertexIndex + 0] += -charMidBaselinePos;
                 vertices[vertexIndex + 1] += -charMidBaselinePos;
                 vertices[vertexIndex + 2] += -charMidBaselinePos;
                 vertices[vertexIndex + 3] += -charMidBaselinePos;
 
                 vertices[vertexIndex + 1].x *= currentScale;
                 vertices[vertexIndex + 2].x *= currentScale;
                 currentScale *= scaleGradient;
                 vertices[vertexIndex + 0].x *= currentScale;
                 vertices[vertexIndex + 3].x *= currentScale;

                 vertices[vertexIndex + 0] += charMidBaselinePos;
                 vertices[vertexIndex + 1] += charMidBaselinePos;
                 vertices[vertexIndex + 2] += charMidBaselinePos;
                 vertices[vertexIndex + 3] += charMidBaselinePos;
             }

             //Upload the mesh with the revised information
             m_TextComponent.UpdateVertexData();
        }
    }
}