using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Superbrands.Libs.RestClients.Pim;
using Attribute = Superbrands.Libs.RestClients.Pim.Attribute;


namespace Superbrands.Selection.WebApi.DTOs
{
    public class ExportSelectionResponse : FileContentResult
    {
        private readonly Domain.Selections.Selection _selection;
        private readonly IEnumerable<Product> _products;
        private Dictionary<int, HeaderAttribute> _attributesDict;
        private const int AttributesStartCol = 3;
        private const int ProductDataStartRow = 3;

        public ExportSelectionResponse(Domain.Selections.Selection selection, IEnumerable<Product> products) : base(new byte[1], "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            _selection = selection;
            _products = products ?? new List<Product>();
            FillExcel();
        }

        private void FillExcel()
        {
            using (var pck = new ExcelPackage())
            {
                var ws = pck.Workbook.Worksheets.Add(_selection.Id.ToString());
                AddHeaders(ws);
                AddSelectionData(ws);
                AddProductsData(ws);

                pck.Stream.Flush();
                FileContents = pck.GetAsByteArray();
            }
            //Так как поддерживаются только символы US-ASCII, пришлось конвертить.
            FileDownloadName = $"{ToUnicode(_selection.Id.ToString())}-{DateTime.Now:d}.xlsx";
        }

        private void AddSelectionData(ExcelWorksheet ws)
        {
            ws.Cells[1, 1].Value = $"Выборка: {_selection.Id.ToString()}";
            var lastCol = _attributesDict.Last().Value.ColNumber;
            ws.Cells[1, 1, 1, lastCol].Merge = true;
            ws.Cells[1, 1, 1, lastCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[1, 1, 1, lastCol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[1, 1, 1, lastCol].Style.Font.Bold = true;
        }

        private void AddHeaders(ExcelWorksheet ws)
        {
            ws.Cells[2, 1].Value = $"SKU";
            ws.Cells[2, 2].Value = $"Наименование";
            var col = AttributesStartCol;
            _attributesDict = _products.SelectMany(p => p.AttributeValues.Select(av => av.Attribute))
                .GroupBy(p => p.Id)
                .OrderBy(x => x.Key)
                .ToDictionary(x => x.Key, x => new HeaderAttribute
                {
                    AttributeDto = x.First(),
                    ColNumber = col++
                });

            foreach (var headerAttribute in _attributesDict)
            {
                ws.Cells[2, headerAttribute.Value.ColNumber].Value = headerAttribute.Value.AttributeDto.Name;
            }

            var lastCol = _attributesDict.Last().Value.ColNumber;
            ws.Cells[2, 1, 2, lastCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Cells[2, 1, 2, lastCol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[2, 1, 2, lastCol].Style.Font.Bold = true;
            ws.Cells[1, 1, 2, lastCol].AutoFitColumns(50, 100);
            ws.Cells[2, 1, 2, lastCol].Style.WrapText = true;
        }

        private void AddProductsData(ExcelWorksheet ws)
        {
            var row = ProductDataStartRow;
            foreach (var product in _products)
            {
                ws.Cells[row, 1].Value = product.Sku;
                ws.Cells[row, 2].Value = product.Name;
                foreach (var av in product.AttributeValues)
                {
                    ws.Cells[row, _attributesDict[av.AttributeId].ColNumber].Value = (av.StrValue + " " + av.NumValue + " " + av.DateValue + " " + (av.BoolValue != null ? ((bool) av.BoolValue ? "Да" : "Нет") : null) + " " + av.ListValue?.Value).Trim();
                }

                row++;
            }

            var lastCol = _attributesDict.Last().Value.ColNumber;
            ws.Cells[ProductDataStartRow, 1, row, lastCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[ProductDataStartRow, 1, row, lastCol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[ProductDataStartRow, 1, row, lastCol].Style.WrapText = true;
        }

        private string ToUnicode(string str)
        {
            var latUp = new[] { "A", "B", "V", "G", "D", "E", "Yo", "Zh", "Z", "I", "Y", "K", "L", "M", "N", "O", "P", "R", "S", "T", "U", "F", "Kh", "Ts", "Ch", "Sh", "Shch", "\"", "Y", "'", "E", "Yu", "Ya" };
            var latLow = new[] { "a", "b", "v", "g", "d", "e", "yo", "zh", "z", "i", "y", "k", "l", "m", "n", "o", "p", "r", "s", "t", "u", "f", "kh", "ts", "ch", "sh", "shch", "\"", "y", "'", "e", "yu", "ya" };
            var rusUp = new[] { "А", "Б", "В", "Г", "Д", "Е", "Ё", "Ж", "З", "И", "Й", "К", "Л", "М", "Н", "О", "П", "Р", "С", "Т", "У", "Ф", "Х", "Ц", "Ч", "Ш", "Щ", "Ъ", "Ы", "Ь", "Э", "Ю", "Я" };
            var rusLow = new []{ "а", "б", "в", "г", "д", "е", "ё", "ж", "з", "и", "й", "к", "л", "м", "н", "о", "п", "р", "с", "т", "у", "ф", "х", "ц", "ч", "ш", "щ", "ъ", "ы", "ь", "э", "ю", "я" };
            for (var i = 0; i <= 32; i++)
            {
                str = str.Replace(rusUp[i], latUp[i]);
                str = str.Replace(rusLow[i], latLow[i]);
            }
            return str.Replace(" ", "_");
        }
    }

    internal class HeaderAttribute
    {
        public Attribute AttributeDto;
        public int ColNumber;
    }
}