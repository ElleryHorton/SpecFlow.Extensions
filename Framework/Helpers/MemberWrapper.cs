using System.Reflection;

namespace SpecFlow.Extensions.PageObjects
{
    public abstract class MemberWrapper
    {
        public abstract string Name { get; }
        public abstract object GetValue(object obj);
        public abstract void SetValue(object obj, object value);
    }

    public class FieldWrapper : MemberWrapper
    {
        private FieldInfo _fieldInfo;
        public FieldWrapper(FieldInfo fieldInfo)
        {
            _fieldInfo = fieldInfo;
        }
        public override string Name { get { return _fieldInfo.Name; } }
        public override object GetValue(object obj)
        {
            return _fieldInfo.GetValue(obj);
        }
        public override void SetValue(object obj, object value)
        {
            _fieldInfo.SetValue(obj, value);
        }
    }

    public class PropertyWrapper : MemberWrapper
    {
        private PropertyInfo _propertyInfo;
        public PropertyWrapper(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }
        public override string Name { get { return _propertyInfo.Name; } }
        public override object GetValue(object obj)
        {
            return _propertyInfo.GetValue(obj);
        }
        public override void SetValue(object obj, object value)
        {
            _propertyInfo.SetValue(obj, value);
        }
    }
}
