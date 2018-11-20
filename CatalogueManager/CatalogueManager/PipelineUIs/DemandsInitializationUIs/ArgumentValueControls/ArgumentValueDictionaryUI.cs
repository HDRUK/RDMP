using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using CatalogueLibrary.Data.DataLoad;
using CatalogueLibrary.Repositories;
using ReusableUIComponents;

namespace CatalogueManager.PipelineUIs.DemandsInitializationUIs.ArgumentValueControls
{
    /// <summary>
    /// Allows you to specify the value of an IArugment (the database persistence value of a [DemandsInitialization] decorated Property on a MEF class e.g. a Pipeline components public property that the user can set)
    /// 
    /// <para>This Control is for setting Properties that are of Array types TableInfo[], Catalogue[] etc</para>
    /// </summary>
    [TechnicalUI]
    public partial class ArgumentValueDictionaryUI : UserControl, IArgumentValueUI
    {
        private Type _kType;
        private Type _vType;
        private IDictionary _dictionary;
        private ArgumentValueUIFactory _factory;

        private ArgumentValueUIArgs _args;

        public ArgumentValueDictionaryUI()
        {
            InitializeComponent();

            _factory = new ArgumentValueUIFactory();
        }

        public void SetUp(ArgumentValueUIArgs args)
        {
            _args = args;
            var concreteType = args.Type;

            //get an IDictionary either from the object or a new empty one (e.g. if Value is null)
            _dictionary = (IDictionary)(args.InitialValue??Activator.CreateInstance(concreteType));

            _kType = concreteType.GenericTypeArguments[0];
            _vType = concreteType.GenericTypeArguments[1];

            foreach (DictionaryEntry kvp in _dictionary)
                AddRow(kvp.Key,kvp.Value);

            btnSave.Enabled = false;
        }

        List<object> keys = new List<object>();
        List<object> values = new List<object>();

        Stack<Tuple<Control,Control>> controls = new Stack<Tuple<Control, Control>>();

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddRow(null, null);
        }

        private void AddRow(object key, object val)
        {
            const int uiWidth = 350;
            int element = keys.Count;
            int y = element * 25;
            keys.Add(null);
            values.Add(null);

            var keyArgs = _args.Clone();

            keyArgs.Setter = k =>
            {
                keys[element] = k;
                btnSave.Enabled = true;
            };

            keyArgs.InitialValue = key;
            keyArgs.Type = _kType;

            var keyUI = (Control)_factory.Create(keyArgs);
            keyUI.Dock = DockStyle.None;
            keyUI.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            keyUI.Location = new Point(0, y);
            keyUI.Width = uiWidth;
            panel1.Controls.Add(keyUI);

            var valueArgs = _args.Clone();

            valueArgs.Setter = v =>
            {
                values[element] = v;
                btnSave.Enabled = true;
            };

            valueArgs.InitialValue = val;
            valueArgs.Type = _vType;

            var valueUI = (Control)_factory.Create(valueArgs);
            valueUI.Dock = DockStyle.None;
            valueUI.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            valueUI.Location = new Point(keyUI.Right, y);
            valueUI.Width = uiWidth;

            panel1.Controls.Add(valueUI);
            //they added a row so it's saveable
            btnSave.Enabled = true;
            btnRemove.Enabled = true;

            controls.Push(Tuple.Create(keyUI, valueUI));
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _dictionary.Clear();
                for (int i = 0; i < keys.Count; i++)
                    _dictionary.Add(keys[i], values[i]);

                _args.Setter(_dictionary);
                btnSave.Enabled = false;
            }
            catch (Exception ex)
            {
                ExceptionViewer.Show(ex);
                btnSave.Enabled = true;
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            var popped = controls.Pop();
            panel1.Controls.Remove(popped.Item1);
            panel1.Controls.Remove(popped.Item2);

            keys.RemoveAt(keys.Count - 1);
            values.RemoveAt(values.Count - 1);

            btnRemove.Enabled = keys.Any();
            btnSave.Enabled = true;
        }

    }
}
