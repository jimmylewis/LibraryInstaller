﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace Microsoft.Web.LibraryManager.Vsix.UI.Controls
{
    /// <summary>
    /// Custom AutomationPeer for PackageContentsTreeView control
    /// </summary>
    internal class PackageContentsTreeViewAutomationPeer : UserControlAutomationPeer
    {
        public PackageContentsTreeViewAutomationPeer(UserControl owner) : base(owner)
        {
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Tree;
        }
    }
}
