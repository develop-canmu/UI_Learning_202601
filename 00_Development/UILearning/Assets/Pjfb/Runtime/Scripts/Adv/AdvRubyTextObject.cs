using System;
using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEngine;
using CruFramework.UI;
using TMPro;

namespace Pjfb
{
    [RequireComponent(typeof(RubyTextMeshProUGUI))]
    public class AdvRubyTextObject : AdvTextObject
    {
        private RubyTextMeshProUGUI _rubyText = null;

        private RubyTextMeshProUGUI RubyText
        {
            get
            {
                if(_rubyText == null) 
                {
                    _rubyText = gameObject.GetComponent<RubyTextMeshProUGUI>();
                }
                return _rubyText;
            }
        }
        
        private void Awake()
        {
            
        }

        public override void SetText(string text, Color color)
        {
            RubyText.color = color;
            RubyText.UnditedText = text;
        }
    }
}