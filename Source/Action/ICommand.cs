﻿using System.Collections.Generic;

namespace CodeM.Common.Orm
{
    public interface ICommand
    {
        void CreateTable(bool force=false);

        bool TryCreateTable(bool force = false);

        void RemoveTable(bool throwError = false);

        bool TryRemoveTable();

        void TruncateTable(bool throwError = false);

        bool TableExists();

        bool TryTruncateTable();

        int GetTransaction();

        bool Save(int? transCode = null);

        bool Delete(bool deleteAll = false);

        bool Delete(int? transCode, bool deleteAll = false);

        bool Update(bool updateAll = false);

        bool Update(int? transCode, bool updateAll = false);

        List<dynamic> Query(int? transCode);

        dynamic QueryFirst(int? transCode);

        long Count(int? transCode);

        bool Exists(int? transCode);
    }
}