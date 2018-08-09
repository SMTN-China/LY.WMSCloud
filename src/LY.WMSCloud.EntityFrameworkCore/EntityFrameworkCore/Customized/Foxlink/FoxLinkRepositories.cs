using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using LY.WMSCloud.Customized.Foxlink;
using System.Threading.Tasks;

namespace LY.WMSCloud.EntityFrameworkCore.Customized.Foxlink
{
    /// <summary>
    /// 富港看板仓储
    /// </summary>
    public class FoxLinkRepositories : Core.Customized.Foxlink.IFoxLinkRepositories
    {
        IDbConnection Connection { get; }

        public FoxLinkRepositories(IConfiguration configuration)
        {
            Connection = new Oracle.ManagedDataAccess.Client.OracleConnection(configuration.GetConnectionString("FoxLinkConnection"));
        }

        public async Task<IEnumerable<string>> GeWorkOrder(DateTime startTime)
        {
            return await Connection.QueryAsync<string>("SELECT WORK_ORDER from (SELECT WORK_ORDER,MIN(IN_PDLINE_TIME) StartTime from G_SN_TRAVEL_GE GROUP BY WORK_ORDER ) where StartTime>:StartTime ", new { StartTime = startTime });
        }

        public async Task<IEnumerable<string>> GePROCESS_NAME(string workOrder)
        {
            return await Connection.QueryAsync<string>("SELECT PROCESS_NAME from G_SN_TRAVEL_GE where WORK_ORDER=:WORK_ORDER GROUP BY PROCESS_NAME", new { WORK_ORDER = workOrder });
        }

        public async Task<G_WO_BASE_GE> GetG_WO_BASE_GE(string workOrder)
        {
            return await Connection.QueryFirstOrDefaultAsync<G_WO_BASE_GE>("select * from G_WO_BASE_GE where WORK_ORDER=:WORK_ORDER", new { WORK_ORDER = workOrder });
        }

        public async Task<IEnumerable<G_SN_TRAVEL_GE>> GetG_SN_TRAVEL_GEs(string workOrder, string processName)
        {
            return await Connection.QueryAsync<G_SN_TRAVEL_GE>("select * from G_SN_TRAVEL_GE where WORK_ORDER=:WORK_ORDER and PROCESS_NAME=:PROCESS_NAME", new { WORK_ORDER = workOrder, PROCESS_NAME = processName });
        }

        public async Task<int> GetWorkIndex(string workOrder, DateTime startTime)
        {
            return await Connection.QueryFirstOrDefaultAsync<int>(@"SELECT

    WorkIndex
FROM
    (
        SELECT

            ROWNUM AS WorkIndex,
            WORK_ORDER

        FROM
            (
                SELECT

                    WORK_ORDER,
                    MIN(IN_PDLINE_TIME)

                FROM

                    G_SN_TRAVEL_GE

                WHERE

                    IN_PDLINE_TIME >:IN_PDLINE_TIME

                GROUP BY

                    WORK_ORDER

                ORDER BY

                    MIN(IN_PDLINE_TIME)
            )
    )
WHERE

    WORK_ORDER = :WORK_ORDER", new { WORK_ORDER = workOrder, IN_PDLINE_TIME = startTime });
        }

        public async Task<G_SN_TRAVEL_GE> GetPreviousWorkLastData(DateTime startWorkTime, string lineName)
        {
            return await Connection.QueryFirstOrDefaultAsync<G_SN_TRAVEL_GE>(@"SELECT
	*
FROM
	(
		SELECT
			*
		FROM
			G_SN_TRAVEL_GE
		WHERE
			OUT_PROCESS_TIME < :startWorkTime
		AND OUT_PROCESS_TIME > :startLineTime
		AND PDLINE_NAME = :lineName
		ORDER BY
			OUT_PROCESS_TIME DESC
	)
WHERE
	ROWNUM <= 1", new { lineName, startWorkTime, startLineTime = startWorkTime.Date.AddHours(8) });

        }

        public async Task<IEnumerable<ImportantProcess>> GetImportantProcesses(IEnumerable<ImportantProcess> processes, string workOrder, DateTime startWorkTime)
        {
            foreach (var processe in processes)
            {
                processe.TodayCompleteQty = await Connection.QueryFirstAsync<int>("SELECT COUNT(DISTINCT SERIAL_NUMBER) from G_SN_TRAVEL_GE where WORK_ORDER=:WORK_ORDER and IN_PDLINE_TIME>:IN_PDLINE_TIME and PROCESS_NAME=:PROCESS_NAME and CURRENT_STATUS='0' ",
                    new { WORK_ORDER = workOrder, IN_PDLINE_TIME = startWorkTime, PROCESS_NAME = processe.Name });
            }

            return processes;
        }
    }
}
