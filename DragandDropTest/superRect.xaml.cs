using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DragandDropTest
{
    public partial class superRect : UserControl
    {
        public DependencyProperty RectColourLocalProperty =
            DependencyProperty.Register("RectColourLocal", typeof(SolidColorBrush), typeof(superRect), null);
       
        public DependencyProperty RectTextLocalProperty =
            DependencyProperty.Register("RectTextLocal", typeof(string), typeof(superRect), null);


    
        public SolidColorBrush RectColourLocal
        {
            get
            {
                return (SolidColorBrush)GetValue(RectColourLocalProperty);
            }
            set
            {

                SetValue(RectColourLocalProperty, value);
                xRect.Fill = value;
            }
        }

        public string RectTextLocal
        {
            get
            {
                return GetValue(RectTextLocalProperty).ToString();
            }
            set {

                SetValue(RectTextLocalProperty, value);
                xText.Text = value;
            }
        }

        public superRect()
        {
            InitializeComponent();
        }
    }
}
