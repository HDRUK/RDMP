// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using BrightIdeasSoftware;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReusableUIComponents
{
    [TypeDescriptionProvider(typeof(AbstractControlDescriptionProvider<PickOneOrCancelDialog<object>, Form>))]
    public partial class PickOneOrCancelDialog<T> : Form
    {
        public T Picked { get; set; } = default(T);

        public PickOneOrCancelDialog(T[] options,string message, Func<T,Image> imageGetter, Func<T,string> nameGetter)
        {
            InitializeComponent();

            label1.Text = message;
            
            
            if(imageGetter != null)
                olvOptions.ImageGetter = (m)=>imageGetter((T)m);

            olvOptions.AspectGetter = (m)=>nameGetter((T)m);

            objectListView1.AddObjects(options);
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Picked = default(T);
            DialogResult = DialogResult.Cancel;
        }

        private void ObjectListView1_ItemActivate(object sender, EventArgs e)
        {
            try
            {
                Picked =  (T)objectListView1.SelectedObject;
            }
            catch
            {
                //someone somehow managed to clear the selection mid activation?
                return;
            }
            
            DialogResult = DialogResult.OK;
            Close();

        }

        private void ObjectListView1_SelectionChanged(object sender, EventArgs e)
        {
            
            try
            {
                Picked =  (T)objectListView1.SelectedObject;
            }
            catch
            {
                //someone somehow managed to clear the selection mid activation?
                return;
            }
        }

        private void TbFilter_TextChanged(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(tbFilter.Text))
                objectListView1.UseFiltering = false;
            else
            {
                objectListView1.UseFiltering = true;
                objectListView1.ModelFilter = new TextMatchFilter(objectListView1,tbFilter.Text);
            }            
        }
    }

}