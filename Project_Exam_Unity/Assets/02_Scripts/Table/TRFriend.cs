using System.IO;
using System.Xml;

public class TRFriend : TRFoundation
{
    private float playTime;
    private float friendHp;
    private float dontSee;
    private float friendRecoveryDuration;
    
    public float PlayTime => playTime;
    public float FriendHp => friendHp;
    public float DontSee => dontSee;
    public float FriendRecoveryDuration => friendRecoveryDuration;

    public override bool ReadRawData(XmlReader reader)
    {
        //형식 유지
        bool result = true;
        reader.ReadStartElement(GetTableName());
        base.ReadRawData(reader);

        //형식 변화
        result &= XmlHelper.ReadFloat(reader, "PlayTime", ref playTime);
        result &= XmlHelper.ReadFloat(reader, "FriendHp", ref friendHp);
        result &= XmlHelper.ReadFloat(reader, "DontSee", ref dontSee);
        result &= XmlHelper.ReadFloat(reader, "FriendRecoveryDuration", ref friendRecoveryDuration);

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
        friendHp = reader.ReadFloat();
        dontSee = reader.ReadFloat();
        friendRecoveryDuration = reader.ReadFloat();

        //형식 유지
        return true;
    }

    public override void Write(BinaryWriter writer)
    {
        //형식 유지
        base.Write(writer);

        //형식 변화
        writer.Write(playTime);
        writer.Write(friendHp);
        writer.Write(dontSee);
        writer.Write(friendRecoveryDuration);
    }
}