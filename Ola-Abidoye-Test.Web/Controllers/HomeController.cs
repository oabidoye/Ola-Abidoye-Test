using Microsoft.AspNetCore.Mvc;
using Ola_Abidoye_Test.Web.Services;
using System.Data;

namespace Ola_Abidoye_Test.Web.Controllers
{
    public class HomeController(ISalesService salesService) : Controller
    {
        private readonly ISalesService _salesService = salesService;

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetSalesList()
        {
            var totalRecord = 0;
            var filterRecord = 0;
            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            var pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            var skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"Data\Sales.csv");

            var salesList = _salesService.GetSalesSummaryFile(filePath)
                                    .AsEnumerable()
                                    .Select(r => r.Table.Columns.Cast<DataColumn>()
                                                                .Select(c => new KeyValuePair<string, object>(c.ColumnName.Trim(), r[c.Ordinal]))
                                                                .ToDictionary(z => z.Key, z => z.Value))
                                    .ToList();

            if (salesList != null)
            {
                totalRecord = salesList.Count;

                // search data when search value found
                if (!string.IsNullOrEmpty(searchValue))
                {
                    salesList = salesList.Where(dictionary => dictionary.Values.Any(Value =>
                    {
                        return Value.ToString().Contains(searchValue, StringComparison.CurrentCultureIgnoreCase);
                    })).ToList();
                }

                // total records after search
                filterRecord = salesList.Count;

                //sort data
                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
                {
                    if (sortColumnDirection == "asc")
                    {
                        salesList = salesList.OrderBy(d => d[sortColumn]).ToList();
                    }
                    else
                    {
                        salesList = salesList.OrderByDescending(d => d[sortColumn]).ToList();
                    }
                }

                //pagination
                salesList = salesList.Skip(skip).Take(pageSize).ToList();
            }

            return Json(new
            {
                //draw = draw,
                recordsTotal = totalRecord,
                recordsFiltered = filterRecord,
                data = salesList
            });
        }
    }
}
