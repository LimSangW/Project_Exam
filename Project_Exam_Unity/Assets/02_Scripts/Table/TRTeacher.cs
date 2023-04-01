using System.IO;
using System.Xml;

public class TRTeacher : TRFoundation
{
    private float playTime;
    private float teacherNum;
    public float PlayTime => playTime;
    public float TeacherNum => teacherNum;

    public override bool ReadRawData(XmlReader reader)
    {
        //형식 유지
        bool result = true;
        reader.ReadStartElement(GetTableName());
        base.ReadRawData(reader);

        //형식 변화
        result &= XmlHelper.ReadFloat(reader, "PlayTime", ref playTime);
        result &= XmlHelper.ReadFloat(reader, "TeacherNum", ref teacherNum);

        //형식 유지
        reader.ReadEndElement();
        return result;
    }

    public override bool Read(BinaryReader reader)
    {
        //형식 유지
        base.Read(reader);

        //형식 변화
        playTime = reader.ReadFloat();
        teacherNum = reader.ReadFloat();

        //형식 유지
        return true;
    }

    public override void Write(BinaryWriter writer)
    {
        //형식 유지
        base.Write(writer);

        //형식 변화
        writer.Write(playTime);
        writer.Write(teacherNum);
    }
}
