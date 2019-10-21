// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Windows.Forms;
using Rdmp.Core.CommandExecution;

namespace Rdmp.UI.ItemActivation.Emphasis
{
    /// <summary>
    /// Event args for an <see cref="EmphasiseRequest"/> (when a TreeView object should be brought to the users attention).
    /// </summary>
    public class EmphasiseEventArgs:EventArgs
    {
        public EmphasiseRequest Request { get; set; }
        public Form FormRequestingActivation { get; set; }

        public EmphasiseEventArgs(EmphasiseRequest request)
        {
            Request = request;
        }
    }
}