using LinkIT.Data.DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace LinkIT.Data.Repositories
{
    public class DeviceRepository
    {
        private const string ID_COLUMN = "Id";
        private const string TAG_COLUMN = "Tag";
        private const string OWNER_COLUMN = "Owner";
        private const string BRAND_COLUMN = "Brand";
        private const string TYPE_COLUMN = "Type";

        private string _connectionString;

        public DeviceRepository(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException();

            _connectionString = connectionString;
        }

        // To add parameters to a query, see : https://stackoverflow.com/questions/293311/whats-the-best-method-to-pass-parameters-to-sqlcommand
        public IEnumerable<DeviceDto> Get()
        {
            using(var conn = new SqlConnection(_connectionString))
            using(var tx = conn.BeginTransaction())
            {
                var cmd = new SqlCommand { CommandText = string.Format("select * from {0}", TableNames.DEVICE_TABLE) };

                var reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    yield return new DeviceDto
                    {
                        Id = (Guid)reader[ID_COLUMN],
                        Tag = reader[TAG_COLUMN].ToString(),
                        Owner = reader[OWNER_COLUMN].ToString(),
                        Brand = reader[BRAND_COLUMN].ToString(),
                        Type = reader[TYPE_COLUMN].ToString()
                    };
                }

                tx.Commit();
            }
        }
    }
}