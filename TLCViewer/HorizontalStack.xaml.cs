﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TLCViewer
{
    /// <summary>
    /// HorizontalStack.xaml の相互作用ロジック
    /// </summary>
    public partial class HorizontalStack : UserControl
    {
        ThumbButton[] thumbButtons;

        public HorizontalStack()
        {
            thumbButtons = new ThumbButton[2];
            
            if (stack.Children.Contains(thumbButtons[0]))
            {
                stack.Children.Add(thumbButtons[0]);
            }
            

            InitializeComponent();
        }
    }
}
