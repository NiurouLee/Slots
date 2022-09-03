// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/10/15:40
// Ver : 1.0.0
// Description : TextMeshAutoSize.cs
// ChangeLog :
// **********************************************

using System;
using GameModule;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TextMeshAutoSize : MonoBehaviour
#if UNITY_EDITOR
    , ISerializationCallbackReceiver
#endif
{
    private TextGenerator _textGen;
    private string _textInTextMesh;
    [SerializeField,HideInInspector] private Text _textUGUI;
    [SerializeField,HideInInspector] private TextMesh _textMesh;
    [SerializeField,HideInInspector] private float _contentMaxWidth;
    [SerializeField,HideInInspector] private float _originalCharacterSize;

    private void Awake()
    {
        Initialize();
    }

#if UNITY_EDITOR
    private void LateUpdate()
    {
        UpdateTextScale();
    }
    
    public void OnBeforeSerialize()
    {
        Initialize(true);
    }
    
    public void OnAfterDeserialize()
    {
    }
#endif

    private void Initialize(bool forceUpdate=false)
    {
        
        if (!forceUpdate && (_textMesh != null || _textUGUI != null))
            return;
        _textMesh = GetComponent<TextMesh>();
        if (_textMesh == null)
        {
            _textUGUI = GetComponent<Text>();
            _contentMaxWidth = GetUGUITextWidth(_textUGUI);
            _textInTextMesh = _textUGUI.text;
            _originalCharacterSize = _textUGUI.fontSize;   
        }
        else
        {
            _contentMaxWidth = GetTextWidth(_textMesh);
            _textInTextMesh = _textMesh.text;
            _originalCharacterSize = _textMesh.characterSize;   
        }
    }

    public void UpdateTextScale()
    {
        if (_textMesh != null && _textMesh.text != _textInTextMesh)
        {
            UpdateTextMeshFontSize();
        }
        if (_textUGUI != null && _textUGUI.text != _textInTextMesh)
        {
            UpdateUGUITextFontSize();
        }
    }

    private void UpdateTextMeshFontSize()
    {
        var currentWidth = GetTextWidth(_textMesh);
        if (currentWidth > _contentMaxWidth)
        {
            _textMesh.characterSize = _contentMaxWidth / currentWidth * _originalCharacterSize;
        }
        else
        {
            _textMesh.characterSize = _originalCharacterSize;
        }
        _textInTextMesh = _textMesh.text;
    }

    private void UpdateUGUITextFontSize()
    {
        var currentWidth = GetUGUITextWidth(_textUGUI);
        if (currentWidth > _contentMaxWidth)
        {
            _textUGUI.fontSize = Math.Max(1,(int)Math.Floor(_contentMaxWidth / currentWidth * _originalCharacterSize));
        }
        else
        {
            _textUGUI.fontSize = (int)Math.Floor(_originalCharacterSize);
        }
        _textInTextMesh = _textUGUI.text;
    }

    public static float GetTextWidth(TextMesh mesh)
    {
        float width = 0;
        foreach (char symbol in mesh.text)
        {
            CharacterInfo info;
            if (mesh.font.GetCharacterInfo(symbol, out info, mesh.fontSize, mesh.fontStyle))
            {
                width += info.advance;
            }
        }
        
        return width * 0.1f;
    }

    private float GetUGUITextWidth(Text textUGUI)
    {
        if (!textUGUI) return 1;
        if (_textGen == null)
        {
            _textGen = new TextGenerator();
        }
        TextGenerationSettings generationSettings = textUGUI.GetGenerationSettings(textUGUI.rectTransform.rect.size);
        return _textGen.GetPreferredWidth(textUGUI.text, generationSettings);
    }
}