using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ContractApp.Utilities
{
    public static class ButtonExtensions
    {
        public static readonly DependencyProperty HasDropDownProperty = DependencyProperty.RegisterAttached(
            "HasDropDown",
            typeof(bool),
            typeof(ButtonExtensions),
            new PropertyMetadata(false));

        public static void SetHasDropDown(DependencyObject element, bool value)
        {
            element.SetValue(HasDropDownProperty, value);
        }

        public static bool GetHasDropDown(DependencyObject element)
        {
            return (bool)element.GetValue(HasDropDownProperty);
        }
    }
}
