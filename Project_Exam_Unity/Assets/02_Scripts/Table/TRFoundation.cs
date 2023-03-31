using UnityEngine;
using System;
using System.Xml;
using System.IO;


[Serializable]
public class TRFoundation
{
    [SerializeField]
    protected int index;

    public int Index { get => index; set => index = value; }
    protected bool hasDevNote = true;

    [AttributeUsage(AttributeTargets.Property)]
    public class DependentIndexFlag : Attribute
    {
        private TableType tableType;
        public TableType TableType => tableType;

        public DependentIndexFlag(TableType tableType)
        {
            this.tableType = tableType;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DependentIndexFlagByEnum : Attribute
    {
        private TableType tableType;
        private int enumValue;
        private string searchPropertyName;

        public TableType TableType => tableType;
        public int EnumValue => enumValue;
        public string SearchPropertyName => searchPropertyName;

        public DependentIndexFlagByEnum(string searchPropertyName, int enumValue, TableType targetTable)
        {
            this.searchPropertyName = searchPropertyName;
            tableType = targetTable;
            this.enumValue = enumValue;
        }
    }
    public string GetTableName()
    {
        return this.GetType().ToString();
    }

    public virtual bool ReadRawData(XmlReader reader)
    {
        bool result = true;
        result &= XmlHelper.ReadInt(reader, "Index", ref index);
        if (hasDevNote)
        {
            string dev = String.Empty;
            result &= XmlHelper.ReadString(reader, "DevNote", ref dev);
        }
        return result;
    }

    public static void ReadStartRoot(XmlReader reader)
    {
        reader.ReadStartElement("Root");
    }

    public static void ReadEndRoot(XmlReader reader)
    {
        reader.ReadEndElement();
    }

    public virtual bool Read(BinaryReader reader)
    {
        index = reader.ReadInt32();
        return true;
    }

    public virtual void Write(BinaryWriter writer)
    {
        writer.Write(index);
    }

#if UNITY_EDITOR
    public virtual bool CheckTableMatch()
    {
        return true;
    }

    public virtual string GetMatchCount()
    {
        return string.Empty;
    }
#endif
}
