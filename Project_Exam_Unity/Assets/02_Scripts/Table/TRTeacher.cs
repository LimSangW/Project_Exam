using System.IO;
using System.Xml;

public class TRTeacher : TRFoundation
{
    private string name;

    public string Name => name;

    public override bool ReadRawData(XmlReader reader)
    {
        //형식 유지
        bool result = true;
        reader.ReadStartElement(GetTableName());
        base.ReadRawData(reader);

        //형식 변화
        result &= XmlHelper.ReadString(reader, "Name", ref name);

        //형식 유지
        reader.ReadEndElement();
        return result;
    }

    public override bool Read(BinaryReader reader)
    {
        //형식 유지
        base.Read(reader);

        //형식 변화
        name = reader.ReadString();

        //형식 유지
        return true;
    }

    public override void Write(BinaryWriter writer)
    {
        //형식 유지
        base.Write(writer);

        //형식 변화
        writer.Write(name);
    }
}
