#region

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Collections;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Designers
{
    internal abstract class RibbonElementWithItemCollectionDesigner
        : ComponentDesigner
    {
        #region Props

        /// <summary>
        ///     Gets a reference to the Ribbon that owns the item
        /// </summary>
        public abstract Ribbon Ribbon { get; }

        /// <summary>
        ///     Gets the collection of items hosted by this item
        /// </summary>
        public abstract RibbonItemCollection Collection { get; }

        /// <summary>
        ///     Overriden. Passes the verbs to the designer
        /// </summary>
        public override DesignerVerbCollection Verbs
        {
            get { return OnGetVerbs(); }
        }

        /// <summary>
        ///     Called when verbs must be retrieved
        /// </summary>
        /// <returns></returns>
        protected virtual DesignerVerbCollection OnGetVerbs()
        {
            return new DesignerVerbCollection(new[]
            {
                new DesignerVerb("Add Button", AddButton),
                new DesignerVerb("Add ButtonList", AddButtonList),
                new DesignerVerb("Add ItemGroup", AddItemGroup),
                new DesignerVerb("Add Separator", AddSeparator),
                new DesignerVerb("Add TextBox", AddTextBox),
                new DesignerVerb("Add ComboBox", AddComboBox),
                new DesignerVerb("Add ColorChooser", AddColorChooser),
                new DesignerVerb("Add CheckBox", AddCheckBox),
                new DesignerVerb("Add UpDown", AddUpDown),
                new DesignerVerb("Add Label", AddLabel),
                new DesignerVerb("Add Host", AddHost)
            });
        }

        #endregion

        #region Methods

        protected virtual void AddButton(object sender, System.EventArgs e)
        {
            CreateItem(typeof (RibbonButton));
        }

        protected virtual void AddButtonList(object sender, System.EventArgs e)
        {
            CreateItem(typeof (RibbonButtonList));
        }

        protected virtual void AddCheckBox(object sender, System.EventArgs e)
        {
            CreateItem(typeof (RibbonCheckBox));
        }

        protected virtual void AddColorChooser(object sender, System.EventArgs e)
        {
            CreateItem(typeof (RibbonColorChooser));
        }

        protected virtual void AddComboBox(object sender, System.EventArgs e)
        {
            CreateItem(typeof (RibbonComboBox));
        }

        protected virtual void AddDescriptionMenuItem(object sender, System.EventArgs e)
        {
            CreateItem(typeof (RibbonDescriptionMenuItem));
        }

        protected virtual void AddHost(object sender, System.EventArgs e)
        {
            CreateItem(typeof (RibbonHost));
        }

        protected virtual void AddItemGroup(object sender, System.EventArgs e)
        {
            CreateItem(typeof (RibbonItemGroup));
        }

        protected virtual void AddLabel(object sender, System.EventArgs e)
        {
            CreateItem(typeof (RibbonLabel));
        }

        protected virtual void AddSeparator(object sender, System.EventArgs e)
        {
            CreateItem(typeof (RibbonSeparator));
        }

        protected virtual void AddTextBox(object sender, System.EventArgs e)
        {
            CreateItem(typeof (RibbonTextBox));
        }

        protected virtual void AddUpDown(object sender, System.EventArgs e)
        {
            CreateItem(typeof (RibbonUpDown));
        }

        /// <summary>
        ///     Creates an item of the speciifed type
        /// </summary>
        /// <param name="t"></param>
        private void CreateItem(Type t)
        {
            CreateItem(Ribbon, Collection, t);
        }

        /// <summary>
        ///     Creates an item of the specified type and adds it to the specified collection
        /// </summary>
        /// <param name="ribbon"></param>
        /// <param name="collection"></param>
        /// <param name="t"></param>
        protected virtual void CreateItem(Ribbon ribbon, RibbonItemCollection collection, Type t)
        {
            var host = GetService(typeof (IDesignerHost)) as IDesignerHost;

            if (host != null && collection != null && ribbon != null)
            {
                var transaction = host.CreateTransaction("AddRibbonItem_" + Component.Site.Name);

                MemberDescriptor member = TypeDescriptor.GetProperties(Component)["Items"];
                base.RaiseComponentChanging(member);

                var item = host.CreateComponent(t) as RibbonItem;

                if (!(item is RibbonSeparator)) item.Text = item.Site.Name;

                collection.Add(item);
                ribbon.OnRegionsChanged();

                base.RaiseComponentChanged(member, null, null);
                transaction.Commit();
            }
        }

        #endregion
    }
}