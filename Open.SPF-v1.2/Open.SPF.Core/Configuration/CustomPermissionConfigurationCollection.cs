using System;
using System.Configuration;
using System.Text;

namespace Open.SPF.Configuration
{
    public class CustomPermissionConfigurationCollection : ConfigurationElementCollection
    {
        public CustomPermissionConfigurationCollection()
        {
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new CustomPermissionConfigurationElement();
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            // TODO
            return Guid.NewGuid().ToString();
        }

        public CustomPermissionConfigurationElement this[int index]
        {
            get
            {
                return (CustomPermissionConfigurationElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        new public CustomPermissionConfigurationElement this[string key]
        {
            get
            {
                return (CustomPermissionConfigurationElement)BaseGet(key);
            }
        }

        public int IndexOf(CustomPermissionConfigurationElement element)
        {
            return BaseIndexOf(element);
        }

        public void Add(CustomPermissionConfigurationElement element)
        {
            BaseAdd(element);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void Remove(CustomPermissionConfigurationElement element)
        {
            int elementIndex = BaseIndexOf(element);
            if (elementIndex >= 0)
                BaseRemoveAt(elementIndex);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        // There is no key because there are many-to-many relationships between Permission Name and Permitted Role
        //public void Remove(string sourceName)
        //{
        //    BaseRemove(sourceName);
        //}

        public void Clear()
        {
            BaseClear();
        }
    }
}
