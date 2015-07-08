#region

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design.Behavior;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Glyphs;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Designers
{
    public class RibbonTabDesigner
        : ComponentDesigner
    {
        private Adorner panelAdorner;

        public override DesignerVerbCollection Verbs
        {
            get
            {
                return new DesignerVerbCollection(new[]
                {
                    new DesignerVerb("Add Panel", AddPanel)
                });
            }
        }

        public RibbonTab Tab
        {
            get { return Component as RibbonTab; }
        }

        public void AddPanel(object sender, System.EventArgs e)
        {
            var host = GetService(typeof (IDesignerHost)) as IDesignerHost;

            if (host != null && Tab != null)
            {
                var transaction = host.CreateTransaction("AddPanel" + Component.Site.Name);
                MemberDescriptor member = TypeDescriptor.GetProperties(Component)["Panels"];
                base.RaiseComponentChanging(member);

                var panel = host.CreateComponent(typeof (RibbonPanel)) as RibbonPanel;

                if (panel != null)
                {
                    panel.Text = panel.Site.Name;

                    //Michael Spradlin 07/05/2013 Added Panel Index code so we can tell where a panel is at on the ribbon.
                    panel.Index = Tab.Panels.Count;

                    if (panel.Index == 0)
                    {
                        panel.IsFirstPanel = true;
                    }
                    else
                    {
                        foreach (var pnl in Tab.Panels)
                        {
                            pnl.IsLastPanel = false;
                        }

                        panel.IsLastPanel = true;
                    }

                    Tab.Panels.Add(panel);
                    Tab.Owner.OnRegionsChanged();
                }

                base.RaiseComponentChanged(member, null, null);
                transaction.Commit();
            }
        }

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            panelAdorner = new Adorner();

            //Kevin Carbis - another point where exception is thrown by the designer when current is null
            if (RibbonDesigner.Current != null)
            {
                var bs = RibbonDesigner.Current.GetBehaviorService();

                if (bs == null) return;

                bs.Adorners.AddRange(new Adorner[] {panelAdorner});

                panelAdorner.Glyphs.Add(new RibbonPanelGlyph(bs, this, Tab));
            }
        }
    }
}