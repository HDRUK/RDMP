// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FAnsi.Discovery;
using MapsDirectlyToDatabaseTable;
using Rdmp.Core.CommandExecution;
using Rdmp.Core.CommandExecution.AtomicCommands;
using Rdmp.Core.Curation.Data;
using Rdmp.Core.Curation.Data.Defaults;
using Rdmp.UI.ItemActivation;
using Rdmp.UI.ItemActivation.Emphasis;
using Rdmp.UI.Refreshing;
using Rdmp.UI.SimpleDialogs;
using ReusableLibraryCode;
using ReusableLibraryCode.Checks;
using ReusableLibraryCode.Icons.IconProvision;
using ReusableUIComponents;

namespace Rdmp.UI.CommandExecution.AtomicCommands
{
    public abstract class BasicUICommandExecution:BasicCommandExecution,IAtomicCommand
    {
        protected readonly IActivateItems Activator;

        protected BasicUICommandExecution(IActivateItems activator)
        {
            Activator = activator;
        }

        protected void Publish(DatabaseEntity o)
        {
            Activator.RefreshBus.Publish(this,new RefreshObjectEventArgs(o));
        }

        protected void Activate(DatabaseEntity o)
        {
            var cmd = new ExecuteCommandActivate(Activator, o);
            cmd.Execute();
        }

        protected void Emphasise(DatabaseEntity o, int expansionDepth = 0)
        {
            Activator.RequestItemEmphasis(this, new EmphasiseRequest(o, expansionDepth));
        }
        
        protected FileInfo SelectSaveFile(string filter)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = filter;
            if (sfd.ShowDialog() == DialogResult.OK)
                return new FileInfo(sfd.FileName);

            return null;
        }

        protected FileInfo SelectOpenFile(string filter)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = filter;
            if (ofd.ShowDialog() == DialogResult.OK)
                return new FileInfo(ofd.FileName);

            return null;
        }

        internal void SetDefaultIfNotExists(ExternalDatabaseServer newServer, PermissableDefaults permissableDefault, bool askYesNo)
        {
            var defaults = Activator.RepositoryLocator.CatalogueRepository.GetServerDefaults();

            var current = defaults.GetDefaultFor(permissableDefault);
            
            if(current == null)
                if(!askYesNo || YesNo($"Set as the default {permissableDefault} server?", "Set as default"))
                    defaults.SetDefault(permissableDefault,newServer);
        }

        /// <summary>
        /// Prompts user to select 1 of the objects of type T in the list you provide
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="availableObjects"></param>
        /// <param name="initialSearchText"></param>
        /// <param name="allowAutoSelect">True to silently auto select the object if there are only 1 <paramref name="availableObjects"/></param>
        /// <returns></returns>
        protected T SelectOne<T>(IList<T> availableObjects, string initialSearchText = null, bool allowAutoSelect = false) where T : DatabaseEntity
        {
            return SelectOne(availableObjects, out T selected, initialSearchText,allowAutoSelect) ? selected : null;
        }
        
        /// <summary>
        /// Prompts user to select 1 object of type T from all the ones stored in the repository provided
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="repository"></param>
        /// <param name="initialSearchText"></param>
        /// <param name="allowAutoSelect">True to silently auto select the object if there are only 1 compatible object in the <paramref name="repository"/></param>
        /// <returns></returns>
        protected T SelectOne<T>(IRepository repository, string initialSearchText = null, bool allowAutoSelect = false) where T : DatabaseEntity
        {
            return SelectOne(repository.GetAllObjects<T>().ToList(),out T answer,initialSearchText,allowAutoSelect) ? answer: null;
        }

        /// <summary>
        /// Prompts user to select 1 of the objects of type T from all the ones stored in the repository provided, returns true if they made a non null selection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="repository"></param>
        /// <param name="selected"></param>
        /// <param name="initialSearchText"></param>
        /// <param name="allowAutoSelect">True to silently auto select the object if there are only 1 compatible object in the <paramref name="repository"/></param>
        /// <returns></returns>
        protected bool SelectOne<T>(IRepository repository, out T selected, string initialSearchText = null, bool allowAutoSelect = false) where T : DatabaseEntity
        {
            return SelectOne(repository.GetAllObjects<T>().ToList(),out selected,initialSearchText,allowAutoSelect);
        }

        /// <summary>
        /// Prompts user to select 1 of the objects of type T in the list you provide, returns true if they made a non null selection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="availableObjects"></param>
        /// <param name="selected"></param>
        /// <param name="initialSearchText"></param>
        /// <param name="allowAutoSelect">True to silently auto select the object if there are only 1 <paramref name="availableObjects"/></param>
        /// <returns></returns>
        protected bool SelectOne<T>(IList<T> availableObjects, out T selected, string initialSearchText = null, bool allowAutoSelect = false) where T : DatabaseEntity
        {
            selected = (T)Activator.SelectOne("Select One",availableObjects.ToArray(), initialSearchText, allowAutoSelect);
            return selected != null;
        }

        protected DiscoveredTable SelectTable(bool allowDatabaseCreation,string taskDescription)
        {
            var dialog = new ServerDatabaseTableSelectorDialog(taskDescription,true,true);

            dialog.ShowDialog();

            if (dialog.DialogResult != DialogResult.OK)
                return null;

            return dialog.SelectedTable;
        }

        protected bool SelectMany<T>(T[] available, out T[] selected, string initialSearchText = null) where T : DatabaseEntity
        {
            var dialog = new SelectIMapsDirectlyToDatabaseTableDialog(available, false, false);
            dialog.AllowMultiSelect = true;
            dialog.SetInitialFilter(initialSearchText);
            dialog.ShowDialog();
            
            if (dialog.DialogResult != DialogResult.OK)
            {
                selected = null;
                return false;
            }

            selected = dialog.MultiSelected.Cast<T>().ToArray();
            return true;
        }

        protected DiscoveredDatabase SelectDatabase(string taskDescription)
        {
            var dialog = new ServerDatabaseTableSelectorDialog(taskDescription, false, false);

            dialog.ShowDialog();
            
            if (dialog.DialogResult != DialogResult.OK)
                return null;

            return dialog.SelectedDatabase;
        }

        /// <summary>
        /// Prompts the user to type in some text (up to a maximum length).  Returns true if they supplied some text or false if they didn't or it was blank/cancelled etc
        /// </summary>
        /// <param name="header"></param>
        /// <param name="prompt"></param>
        /// <param name="maxLength"></param>
        /// <param name="initialText"></param>
        /// <param name="text"></param>
        /// <param name="requireSaneHeaderText"></param>
        /// <returns></returns>
        protected bool TypeText(string header, string prompt, int maxLength, string initialText, out string text, bool requireSaneHeaderText = false)
        {
            return Activator.TypeText(header, prompt, maxLength, initialText, out text, requireSaneHeaderText);
        }

        /// <inheritdoc cref="TypeText(string, string, int, string, out string,bool)"/>
        protected bool TypeText(string header, string prompt, out string text)
        {
            return TypeText(header, prompt, 500, null, out text);
        }

        /// <summary>
        /// Offers the user a binary choice and returns true if they accept it.  This method is blocking.
        /// </summary>
        /// <param name="text">The question to pose</param>
        /// <param name="caption"></param>
        /// <returns></returns>
        protected bool YesNo(string text,string caption)
        {
            return Activator.YesNo(text,caption);
        }

        /// <summary>
        /// Reports a low visibility error to the <see cref="IActivateItems.GlobalErrorCheckNotifier"/>.  Throws <paramref name="ex"/>
        /// with <paramref name="msg"/> if no global errors handler is registered
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="ex"></param>
        protected void GlobalError(string msg, Exception ex)
        {
            if (Activator?.GlobalErrorCheckNotifier == null)
                throw new Exception(msg,ex);

            Activator.GlobalErrorCheckNotifier.OnCheckPerformed(new CheckEventArgs(msg, CheckResult.Fail, ex));
        }


        /// <summary>
        /// Displays the given message to the user
        /// </summary>
        /// <param name="message"></param>
        protected void Show(string message)
        {
            MessageBox.Show(message);
        }

        /// <summary>
        /// Displays the given message to the user, calling String.Format 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="objects">Objects to use for {0},{1} etc tokens in <paramref name="message"/></param>
        protected void Show(string message, params object[] objects)
        {
            MessageBox.Show(string.Format(message,objects));
        }
        
        /// <summary>
        /// Runs checks on the <paramref name="checkable"/> and calls <see cref="BasicCommandExecution.SetImpossible(string)"/> if there are any failures
        /// </summary>
        /// <param name="checkable"></param>
        protected void SetImpossibleIfFailsChecks(ICheckable checkable)
        {
            try
            {
                checkable.Check(new ThrowImmediatelyCheckNotifier());
            }
            catch (Exception e)
            {

                SetImpossible(ExceptionHelper.ExceptionToListOfInnerMessages(e));
            }
        }

        public virtual Image GetImage(IIconProvider iconProvider)
        {
            return null;
        }
    }
}
