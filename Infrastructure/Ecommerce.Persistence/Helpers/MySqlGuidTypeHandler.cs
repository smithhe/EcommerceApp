using Dapper;
using System;
using System.Data;

namespace Ecommerce.Persistence.Helpers
{
	//https://stackoverflow.com/questions/5898988/map-string-to-guid-with-dapper/52319934#52319934
	public class MySqlGuidTypeHandler: SqlMapper.TypeHandler<Guid>
	{
		public override void SetValue(IDbDataParameter parameter, Guid guid)
		{
			parameter.Value = guid.ToString();
		}

		public override Guid Parse(object value)
		{
			return new Guid((string)value);
		}
	}
}