using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ITS.POS.Tools.FormDesigner.Host
{
    /// <summary>
    /// Demonstrates how to write a custom RootDesigner. This designer has a View
    /// of a Graph - it shows the number of components added to the designer
    /// in a pie/bar chart.
    /// </summary>
    public class MyRootDesigner : ComponentDesigner, IRootDesigner, IToolboxUser
    {
        private MyRootControl _rootView;
        public object GetView(System.ComponentModel.Design.ViewTechnology technology)
        {
            _rootView = new MyRootControl(this);
            return _rootView;
        }
        public System.ComponentModel.Design.ViewTechnology[] SupportedTechnologies
        {
            get
            {
                return new ViewTechnology[] { ViewTechnology.Default };
            }
        }

        public void ToolPicked(System.Drawing.Design.ToolboxItem tool)
        {
            _rootView.InvokeToolboxItem(tool);
        }

        public bool GetToolSupported(System.Drawing.Design.ToolboxItem tool)
        {
            return true;
        }

        public new object GetService(Type type)
        {
            return base.GetService(type);
        }


        /// <summary>
        /// This is the View of the RootDesigner.
        /// </summary>
        public class MyRootControl : ScrollableControl
        {
            private MyRootDesigner _rootDesigner;
            private Hashtable _componentInfoTable;
            private int _totalComponents = 0;
            private LinkLabel _linkLabel;



            public MyRootControl(MyRootDesigner rootDesigner)
            {
                _rootDesigner = rootDesigner;
                _componentInfoTable = new Hashtable();
                _linkLabel = new LinkLabel();
                _linkLabel.Text = "Graph Style";
                _linkLabel.Location = new Point(10, 5);
                _linkLabel.Visible = false;
                _linkLabel.Click += new EventHandler(_linkLabel_Click);
                Controls.Add(_linkLabel);
                Resize += new EventHandler(MyRootControl_Resize);
                for (int i = 1; i < _rootDesigner.Component.Site.Container.Components.Count; i++)
                {
                    UpdateTable(_rootDesigner.Component.Site.Container.Components[i].GetType());
                }
                Invalidate();
            }
            protected override void OnPaint(PaintEventArgs pe)
            {
                try
                {
                    if (_componentInfoTable.Count == 0)
                    {
                        _linkLabel.Visible = false;

                        string s = "Add objects from the toolbox on to this custom Graphical RootDesigner";
                        StringFormat sf = new StringFormat();
                        sf.Alignment = StringAlignment.Center;
                        sf.LineAlignment = StringAlignment.Center;

                        pe.Graphics.FillRectangle(new LinearGradientBrush(Bounds, Color.White, Color.Orange, 45F), Bounds);
                        pe.Graphics.DrawString(s, Font, new SolidBrush(Color.Black), Bounds, sf);
                    }
                    else
                    {
                        _linkLabel.Visible = true;

                        pe.Graphics.FillRectangle(new SolidBrush(Color.White), Bounds);
                        pe.Graphics.DrawImage(null, Bounds);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                }
            }


            public void InvokeToolboxItem(System.Drawing.Design.ToolboxItem tool)
            {
                IComponent[] newComponents = tool.CreateComponents(DesignerHost);
                UpdateTable(newComponents[0].GetType());
                Invalidate();
            }
            private void UpdateTable(Type type)
            {
                if (_componentInfoTable[type] == null)
                {
                    ComponentInfo ci = new ComponentInfo();
                    RandomUtil ru = new RandomUtil();

                    ci.Type = type;
                    ci.Color = ru.GetColor();
                    ci.Count = 1;
                    _componentInfoTable.Add(type, ci);
                    _totalComponents++;
                }
                else
                {
                    ComponentInfo ci = (ComponentInfo)_componentInfoTable[type];
                    _componentInfoTable.Remove(type);
                    ci.Count++;
                    _componentInfoTable.Add(type, ci);
                }
            }
            public IDesignerHost DesignerHost
            {
                get
                {
                    return (IDesignerHost)_rootDesigner.GetService(typeof(IDesignerHost));
                }
            }

            public IToolboxService ToolboxService
            {
                get
                {
                    return (IToolboxService)_rootDesigner.GetService(typeof(IToolboxService));
                }
            }

            private class ComponentInfo
            {
                public Type Type;
                public Color Color;
                public int Count;
            }
            private void MyRootControl_Resize(object sender, EventArgs e)
            {
                Invalidate();
            }
            private void _linkLabel_Click(object sender, EventArgs e)
            {
                Invalidate();
            }
        }
    }
}
