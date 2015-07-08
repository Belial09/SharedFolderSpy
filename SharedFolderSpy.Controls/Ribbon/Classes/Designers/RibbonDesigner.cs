#region

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Collections;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Glyphs;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Interfaces;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Designers
{
    public class RibbonDesigner
        : ControlDesigner
    {
        #region Static

        public static RibbonDesigner Current;

        #endregion

        #region Fields

        private IRibbonElement _selectedElement;
        private Adorner orbAdorner;
        private Adorner quickAccessAdorner;
        private Adorner tabAdorner;

        #endregion

        #region Ctor

        public RibbonDesigner()
        {
            Current = this;
        }

        ~RibbonDesigner()
        {
            if (Current == this)
            {
                Current = null;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the currently selected RibbonElement
        /// </summary>
        public IRibbonElement SelectedElement
        {
            get { return _selectedElement; }
            set
            {
                if (Ribbon == null) return;
                _selectedElement = value;

                var selector = GetService(typeof (ISelectionService)) as ISelectionService;

                if (selector != null && value != null)
                    selector.SetSelectedComponents(new[] {value as Component}, SelectionTypes.Primary);

                if (value is RibbonButton)
                {
                    (value as RibbonButton).ShowDropDown();
                }

                Ribbon.Refresh();
            }
        }

        /// <summary>
        ///     Gets the Ribbon of the designer
        /// </summary>
        public Ribbon Ribbon
        {
            get { return Control as Ribbon; }
        }

        #endregion

        #region Methods

        public override DesignerVerbCollection Verbs
        {
            get
            {
                var verbs = new DesignerVerbCollection();

                verbs.Add(new DesignerVerb("Add Tab", AddTabVerb));


                return verbs;
            }
        }

        public void AddTabVerb(object sender, System.EventArgs e)
        {
            var r = Control as Ribbon;

            if (r != null)
            {
                var host = GetService(typeof (IDesignerHost)) as IDesignerHost;
                if (host == null) return;

                var tab = host.CreateComponent(typeof (RibbonTab)) as RibbonTab;

                if (tab == null) return;

                tab.Text = tab.Site.Name;

                Ribbon.Tabs.Add(tab);

                r.Refresh();
            }
        }

        private void AssignEventHandler()
        {
            //TODO: Didn't work
            //if (SelectedElement == null) return;

            //IEventBindingService binder = GetService(typeof(IEventBindingService)) as IEventBindingService;

            //EventDescriptorCollection evts = TypeDescriptor.GetEvents(SelectedElement);

            ////string id = binder.CreateUniqueMethodName(SelectedElement as Component, evts["Click"]);

            //binder.ShowCode(SelectedElement as Component, evts["Click"]);
        }

        public virtual void CreateItem(Ribbon ribbon, RibbonItemCollection collection, Type t)
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

        private void CreateOrbItem(string collectionName, RibbonItemCollection collection, Type t)
        {
            if (Ribbon == null) return;

            var host = GetService(typeof (IDesignerHost)) as IDesignerHost;
            var transaction = host.CreateTransaction("AddRibbonOrbItem_" + Component.Site.Name);
            MemberDescriptor member = TypeDescriptor.GetProperties(Ribbon.OrbDropDown)[collectionName];
            RaiseComponentChanging(member);

            var item = host.CreateComponent(t) as RibbonItem;

            if (!(item is RibbonSeparator)) item.Text = item.Site.Name;

            collection.Add(item);
            Ribbon.OrbDropDown.OnRegionsChanged();

            RaiseComponentChanged(member, null, null);
            transaction.Commit();

            Ribbon.OrbDropDown.SelectOnDesigner(item);
            Ribbon.OrbDropDown.WrappedDropDown.Size = Ribbon.OrbDropDown.Size;
        }

        /// <summary>
        ///     Creates an Orb's MenuItem
        /// </summary>
        /// <param name="t"></param>
        public void CreteOrbMenuItem(Type t)
        {
            CreateOrbItem("MenuItems", Ribbon.OrbDropDown.MenuItems, t);
        }

        /// <summary>
        ///     Creates an Orb's OptionItem
        /// </summary>
        /// <param name="t"></param>
        public void CreteOrbOptionItem(Type t)
        {
            CreateOrbItem("OptionItems", Ribbon.OrbDropDown.OptionItems, t);
        }

        /// <summary>
        ///     Creates an Orb's RecentItem
        /// </summary>
        /// <param name="t"></param>
        public void CreteOrbRecentItem(Type t)
        {
            CreateOrbItem("RecentItems", Ribbon.OrbDropDown.RecentItems, t);
        }

        private void HitOn(int x, int y)
        {
            if (Ribbon.Tabs.Count == 0 || Ribbon.ActiveTab == null)
            {
                SelectRibbon();
                return;
            }

            if (Ribbon != null)
            {
                if (Ribbon.TabHitTest(x, y))
                {
                    SelectedElement = Ribbon.ActiveTab;
                }
                else
                {
                    #region Tab ScrollTest

                    if (Ribbon.ActiveTab.TabContentBounds.Contains(x, y))
                    {
                        if (Ribbon.ActiveTab.ScrollLeftBounds.Contains(x, y) && Ribbon.ActiveTab.ScrollLeftVisible)
                        {
                            Ribbon.ActiveTab.ScrollLeft();
                            SelectedElement = Ribbon.ActiveTab;
                            return;
                        }

                        if (Ribbon.ActiveTab.ScrollRightBounds.Contains(x, y) && Ribbon.ActiveTab.ScrollRightVisible)
                        {
                            Ribbon.ActiveTab.ScrollRight();
                            SelectedElement = Ribbon.ActiveTab;
                            return;
                        }
                    }

                    #endregion

                    //Check Panel
                    if (Ribbon.ActiveTab.TabContentBounds.Contains(x, y))
                    {
                        RibbonPanel hittedPanel = null;

                        foreach (var panel in Ribbon.ActiveTab.Panels)
                            if (panel.Bounds.Contains(x, y))
                            {
                                hittedPanel = panel;
                                break;
                            }

                        if (hittedPanel != null)
                        {
                            //Check item
                            RibbonItem hittedItem = null;

                            foreach (var item in hittedPanel.Items)
                                if (item.Bounds.Contains(x, y))
                                {
                                    hittedItem = item;
                                    break;
                                }

                            if (hittedItem != null && hittedItem is IContainsSelectableRibbonItems)
                            {
                                //Check subitem
                                RibbonItem hittedSubItem = null;

                                foreach (var subItem in (hittedItem as IContainsSelectableRibbonItems).GetItems())
                                    if (subItem.Bounds.Contains(x, y))
                                    {
                                        hittedSubItem = subItem;
                                        break;
                                    }

                                if (hittedSubItem != null)
                                {
                                    SelectedElement = hittedSubItem;
                                }
                                else
                                {
                                    SelectedElement = hittedItem;
                                }
                            }
                            else if (hittedItem != null)
                            {
                                SelectedElement = hittedItem;
                            }
                            else
                            {
                                SelectedElement = hittedPanel;
                            }
                        }
                        else
                        {
                            SelectedElement = Ribbon.ActiveTab;
                        }
                    }
                    else if (Ribbon.QuickAcessToolbar.SuperBounds.Contains(x, y))
                    {
                        var itemHitted = false;

                        foreach (var item in Ribbon.QuickAcessToolbar.Items)
                        {
                            if (item.Bounds.Contains(x, y))
                            {
                                itemHitted = true;
                                SelectedElement = item;
                                break;
                            }
                        }
                        if (!itemHitted)
                            SelectedElement = Ribbon.QuickAcessToolbar;
                    }
                    else if (Ribbon.OrbBounds.Contains(x, y))
                    {
                        Ribbon.OrbMouseDown();
                    }
                    else
                    {
                        SelectRibbon();

                        Ribbon.ForceOrbMenu = false;
                        if (Ribbon.OrbDropDown.Visible) Ribbon.OrbDropDown.Close();
                    }
                }
            }
        }

        protected override void OnPaintAdornments(PaintEventArgs pe)
        {
            base.OnPaintAdornments(pe);

            using (var p = new Pen(Color.Black))
            {
                p.DashStyle = DashStyle.Dot;

                var host = GetService(typeof (ISelectionService)) as ISelectionService;

                if (host != null)
                {
                    foreach (IComponent comp in host.GetSelectedComponents())
                    {
                        var item = comp as RibbonItem;
                        if (item != null && !Ribbon.OrbDropDown.AllItems.Contains(item))
                        {
                            pe.Graphics.DrawRectangle(p, item.Bounds);
                        }
                    }
                }
            }
        }

        private void SelectRibbon()
        {
            var selector = GetService(typeof (ISelectionService)) as ISelectionService;

            if (selector != null)
                selector.SetSelectedComponents(new Component[] {Ribbon}, SelectionTypes.Primary);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.HWnd == Control.Handle)
            {
                switch (m.Msg)
                {
                    case 0x203: //WM_LBUTTONDBLCLK
                        AssignEventHandler();
                        break;
                    case 0x201: //WM_LBUTTONDOWN
                    case 0x204: //WM_RBUTTONDOWN
                        return;
                    case 0x202: //WM_LBUTTONUP
                    case 0x205: //WM_RBUTTONUP
                        HitOn(WinApi.LoWord((int) m.LParam), WinApi.HiWord((int) m.LParam));
                        return;
                    default:
                        break;
                }
            }


            base.WndProc(ref m);
        }

        #endregion

        #region Site

        public BehaviorService GetBehaviorService()
        {
            return BehaviorService;
        }

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            var changeService = GetService(typeof (IComponentChangeService)) as IComponentChangeService;
            var desigerEvt = GetService(typeof (IDesignerEventService)) as IDesignerEventService;

            changeService.ComponentRemoved += changeService_ComponentRemoved;

            orbAdorner = new Adorner();
            tabAdorner = new Adorner();

            BehaviorService.Adorners.AddRange(new Adorner[] {orbAdorner, tabAdorner});
            if (Ribbon.QuickAcessToolbar.Visible)
            {
                quickAccessAdorner = new Adorner();
                BehaviorService.Adorners.Add(quickAccessAdorner);
                quickAccessAdorner.Glyphs.Add(new RibbonQuickAccessToolbarGlyph(BehaviorService, this, Ribbon));
            }
            else
            {
                quickAccessAdorner = null;
            }
            //orbAdorner.Glyphs.Add(new RibbonOrbAdornerGlyph(BehaviorService, this, Ribbon));
            tabAdorner.Glyphs.Add(new RibbonTabGlyph(BehaviorService, this, Ribbon));
        }

        public void RemoveRecursive(IContainsRibbonComponents item, IDesignerHost service)
        {
            if (item == null || service == null) return;

            foreach (var c in item.GetAllChildComponents())
            {
                if (c is IContainsRibbonComponents)
                {
                    RemoveRecursive(c as IContainsRibbonComponents, service);
                }
                service.DestroyComponent(c);
            }
        }

        /// <summary>
        ///     Catches the event of a component on the ribbon being removed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void changeService_ComponentRemoved(object sender, ComponentEventArgs e)
        {
            var tab = e.Component as RibbonTab;
            var panel = e.Component as RibbonPanel;
            var item = e.Component as RibbonItem;

            var designerService = GetService(typeof (IDesignerHost)) as IDesignerHost;

            RemoveRecursive(e.Component as IContainsRibbonComponents, designerService);

            if (tab != null && Ribbon != null)
            {
                Ribbon.Tabs.Remove(tab);
            }
            else if (panel != null)
            {
                panel.OwnerTab.Panels.Remove(panel);
            }
            else if (item != null)
            {
                if (item.Canvas is RibbonOrbDropDown)
                {
                    Ribbon.OrbDropDown.HandleDesignerItemRemoved(item);
                }
                else if (item.OwnerItem is RibbonItemGroup)
                {
                    (item.OwnerItem as RibbonItemGroup).Items.Remove(item);
                }
                else if (item.OwnerPanel != null)
                {
                    item.OwnerPanel.Items.Remove(item);
                }
                else if (Ribbon != null && Ribbon.QuickAcessToolbar.Items.Contains(item))
                {
                    Ribbon.QuickAcessToolbar.Items.Remove(item);
                }
            }

            SelectedElement = null;

            if (Ribbon != null)
                Ribbon.OnRegionsChanged();
        }

        #endregion
    }
}