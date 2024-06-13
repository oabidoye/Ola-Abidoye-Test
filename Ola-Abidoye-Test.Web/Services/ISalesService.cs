using System.Data;

namespace Ola_Abidoye_Test.Web.Services
{
    public interface ISalesService
    {
        DataTable GetSalesSummaryFile(string filePath);
    }
}
