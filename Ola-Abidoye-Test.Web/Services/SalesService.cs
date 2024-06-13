using System.Data;
using System.Text;

namespace Ola_Abidoye_Test.Web.Services
{
    public class SalesService : ISalesService
    {
        public DataTable GetSalesSummaryFile(string filePath)
        {
            var salesTable = new DataTable();
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var encoding = Encoding.GetEncoding("Windows-1252");

                using var reader = new StreamReader(filePath, encoding);

                // Get columns from the first line.
                var columnLine = reader.ReadLine();

                if (columnLine != null)
                {
                    var columns = columnLine.Split(',');
                    foreach (string column in columns)
                    {
                        salesTable.Columns.Add(column);
                    }

                    // Get rows from the remaining lines.
                    while (!reader.EndOfStream)
                    {
                        var rowLine = reader.ReadLine();
                        if (rowLine != null)
                        {
                            var rows = rowLine.Split(',');
                            var row = salesTable.NewRow();

                            for (int i = 0; i < columns.Length; i++)
                            {
                                row[i] = rows[i];
                            }

                            salesTable.Rows.Add(row);
                        }
                    }
                }
            }
            catch
            {
                throw;
            }

            return salesTable;
        }
    }
}
