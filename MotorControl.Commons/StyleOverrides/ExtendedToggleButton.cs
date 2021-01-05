using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MotorControl.Commons.StyleOverrides
{
    public class ExtendedExpander
    {
        public static readonly DependencyProperty ToggleButtonCustomStyleProperty = DependencyProperty.RegisterAttached("ToggleButtonCustomStyle", typeof(Style), typeof(ExtendedExpander));

        public static Style GetToggleButtonCustomStyle(UIElement element) => (Style)element.GetValue(ToggleButtonCustomStyleProperty);
        public static void SetToggleButtonCustomStyle(UIElement element, bool value) => element.SetValue(ToggleButtonCustomStyleProperty, value);
    }
}
