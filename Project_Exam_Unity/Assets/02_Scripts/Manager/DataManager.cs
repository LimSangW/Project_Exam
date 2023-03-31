using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Manager<DataManager>
{
    private Tables tables = new Tables();
    private Tables Tables
    {
        get
        {
            if (tables == null)
            {
                tables = new Tables();
                LoadTable();
            }

            return tables;
        }
    }

    public override void Init()
    {
        base.Init();
    }

    public override void ClearData()
    {

    }

    public T GetRecord<T>(System.Func<T, bool> selector) where T : TRFoundation
    {
        TableType type = tables.GetTableType(typeof(T));
        return Tables.GetRecord(type, selector);
    }

    public T GetRecord<T>(int index) where T : TRFoundation
    {
        TableType type = tables.GetTableType(typeof(T));
        return Tables.GetRecord<T>(type, x => x.Index == index);
    }

    public List<TRFoundation> GetRecords(TableType type)
    {
        return Tables.GetRecords<TRFoundation>(type);
    }

    public List<T> GetRecords<T>(System.Func<T, bool> selector) where T : TRFoundation
    {
        TableType type = tables.GetTableType(typeof(T));
        return Tables.GetRecords<T>(type, selector);
    }

    public List<T> GetRecords<T>(TableType type) where T : TRFoundation
    {
        return Tables.GetRecords<T>(type);
    }

    public void LoadTable()
    {
        if (tables == null)
            tables = new Tables();

        tables.InitTabeDatas();
    }
}
