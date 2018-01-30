using System;
using System.Configuration;
using System.Text;

namespace Open.SPF.Configuration
{
    public class CorePermissionConfigurationCollection : ConfigurationElementCollection
    {
        public CorePermissionConfigurationCollection()
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
            return new CorePermissionConfigurationElement();
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            StringBuilder key = new StringBuilder();
            key.Append(((CorePermissionConfigurationElement)element).PermissionName);
            key.Append("_");
            if (!String.IsNullOrEmpty(((CorePermissionConfigurationElement)element).PermittedRole))
            {
                key.Append(((CorePermissionConfigurationElement)element).PermittedRole);
            }
            else
            {
                key.Append("IsUnrestricted:");
                key.Append(((CorePermissionConfigurationElement)element).IsUnrestricted.ToString());
            }

            return key.ToString();
            //return ((WebPermissionConfigurationElement)element).SourceName;
        }

        public CorePermissionConfigurationElement this[int index]
        {
            get
            {
                return (CorePermissionConfigurationElement)BaseGet(index);
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

        new public CorePermissionConfigurationElement this[string key]
        {
            get
            {
                return (CorePermissionConfigurationElement)BaseGet(key);
            }
        }

        public int IndexOf(CorePermissionConfigurationElement element)
        {
            return BaseIndexOf(element);
        }

        public void Add(CorePermissionConfigurationElement element)
        {
            BaseAdd(element);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void Remove(CorePermissionConfigurationElement element)
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
