﻿using OPC_UA_Client.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OPC_UA_Client.Screens
{

    public partial class AdvancedPage : UserControl
    {
        private OPCServer opcServer = OPCServer.Instance;

        public AdvancedPage()
        {
            InitializeComponent();
        }
    }
}
