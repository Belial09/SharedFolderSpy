#region

using System;
using System.ComponentModel;
using System.Reflection;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.AdvancedTreeview.NodeControls
{
    public abstract class BindableControl : NodeControl
    {
        private string _propertyName = "";

        [DefaultValue("")]
        public string DataPropertyName
        {
            get { return _propertyName; }
            set
            {
                if (_propertyName == null)
                    _propertyName = string.Empty;
                _propertyName = value;
            }
        }

        private PropertyInfo GetPropertyInfo(TreeNodeAdv node)
        {
            if (node.Tag != null && !string.IsNullOrEmpty(DataPropertyName))
            {
                var type = node.Tag.GetType();
                return type.GetProperty(DataPropertyName);
            }
            return null;
        }

        public Type GetPropertyType(TreeNodeAdv node)
        {
            if (node.Tag != null && !string.IsNullOrEmpty(DataPropertyName))
            {
                var type = node.Tag.GetType();
                var pi = type.GetProperty(DataPropertyName);
                if (pi != null)
                    return pi.PropertyType;
            }
            return null;
        }

        public object GetValue(TreeNodeAdv node)
        {
            var pi = GetPropertyInfo(node);
            if (pi != null && pi.CanRead)
                return pi.GetValue(node.Tag, null);
            return null;
        }

        public void SetValue(TreeNodeAdv node, object value)
        {
            var pi = GetPropertyInfo(node);
            if (pi != null && pi.CanWrite)
            {
                try
                {
                    pi.SetValue(node.Tag, value, null);
                }
                catch (TargetInvocationException ex)
                {
                    if (ex.InnerException != null)
                        throw new ArgumentException(ex.InnerException.Message, ex.InnerException);
                    throw new ArgumentException(ex.Message);
                }
            }
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(DataPropertyName))
                return GetType().Name;
            return string.Format("{0} ({1})", GetType().Name, DataPropertyName);
        }
    }
}