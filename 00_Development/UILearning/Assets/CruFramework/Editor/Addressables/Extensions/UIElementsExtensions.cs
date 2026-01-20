using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CruFramework.Editor.Addressable
{
    public static class UIElementsExtensions
    {
        public static void SetMargin(this IStyle self, int top, int right, int bottom, int left)
        {
            self.marginTop = top;
            self.marginRight = right;
            self.marginBottom = bottom;
            self.marginLeft = left;
        }
        
        public static void SetMargin(this IStyle self, int value)
        {
            self.marginTop = value;
            self.marginRight = value;
            self.marginBottom = value;
            self.marginLeft = value;
        }
        
        public static void SetPadding(this IStyle self, int top, int right, int bottom, int left)
        {
            self.paddingTop = top;
            self.paddingRight = right;
            self.paddingBottom = bottom;
            self.paddingLeft = left;
        }
        
        public static void SetPadding(this IStyle self, int value)
        {
            self.paddingTop = value;
            self.paddingRight = value;
            self.paddingBottom = value;
            self.paddingLeft = value;
        }
        
        public static void SetBorderWidth(this IStyle self, int top, int right, int bottom, int left)
        {
            self.borderTopWidth = top;
            self.borderRightWidth = right;
            self.borderBottomWidth = bottom;
            self.borderLeftWidth = left;
        }
        
        public static void SetBorderWidth(this IStyle self, int value)
        {
            self.borderTopWidth = value;
            self.borderRightWidth = value;
            self.borderBottomWidth = value;
            self.borderLeftWidth = value;
        }
        
        public static void SetBorderColor(this IStyle self, Color top, Color right, Color bottom, Color left)
        {
            self.borderTopColor = top;
            self.borderRightColor = right;
            self.borderBottomColor = bottom;
            self.borderLeftColor = left;
        }
        
        public static void SetBorderColor(this IStyle self, Color value)
        {
            self.borderTopColor = value;
            self.borderRightColor = value;
            self.borderBottomColor = value;
            self.borderLeftColor = value;
        }
        
        public static void SetBorderRadius(this IStyle self, int topLeft, int topRight, int bottomRight, int bottomLeft)
        {
            self.borderTopLeftRadius = topLeft;
            self.borderTopRightRadius = topRight;
            self.borderBottomRightRadius = bottomRight;
            self.borderBottomLeftRadius = bottomLeft;
        }
        
        public static void SetBorderRadius(this IStyle self, int value)
        {
            self.borderTopLeftRadius = value;
            self.borderTopRightRadius = value;
            self.borderBottomRightRadius = value;
            self.borderBottomLeftRadius = value;
        }
    }
}
