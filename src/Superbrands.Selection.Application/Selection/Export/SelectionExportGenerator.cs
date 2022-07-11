using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using Superbrands.Libs.RestClients.Pim;
using Superbrands.Selection.Application.Options;
using Attribute = Superbrands.Libs.RestClients.Pim.Attribute;

namespace Superbrands.Selection.Application.Selection.Export
{
    class SelectionExportGenerator
    {
        private readonly Domain.Selections.Selection _selection;
        private readonly ExportOptions _exportOptions;
        private readonly Dictionary<int, FileStreamResult> _imagesDictionary;
        private readonly Dictionary<int, Category> _categoriesDict;
        private Dictionary<int, Attribute> _attributesDict;
        private readonly IEnumerable<Product> _products;


        private const int AttributesStartCol = 4;
        private const int SkuCol = 1;
        private const int ImageCol = 2;
        private const int CategoryCol = 3;
        private const int ProductDataStartRow = 2;


        public SelectionExportGenerator(Domain.Selections.Selection selection, IEnumerable<Product> products,
            ExportOptions exportOptions, Dictionary<int, FileStreamResult> imagesDictionary, Dictionary<int,
                Category> categoriesDict)
        {
            _selection = selection;
            _exportOptions = exportOptions;
            _imagesDictionary = imagesDictionary;
            _categoriesDict = categoriesDict;
            _products = products ?? new List<Product>();
        }

        public byte[] Generate()
        {
            using (var pck = new ExcelPackage())
            {
                var ws = pck.Workbook.Worksheets.Add(_selection.Id.ToString());
                FillHeaders(ws);
                AddProductsData(ws);

                return pck.GetAsByteArray();
            }
        }


        private void FillHeaders(ExcelWorksheet ws)
        {
            _attributesDict = _products.SelectMany(x => x.AttributeValues.Select(av => av.Attribute))
                .GroupBy(x => x.Id)
                .ToDictionary(x => x.Key, x => x.First());

            ws.Cells[1, SkuCol].Value = $"SKU";
            ws.Cells[1, ImageCol].Value = $"Фото";
            ws.Cells[1, CategoryCol].Value = $"Категория";

            var col = AttributesStartCol;
            foreach (var headerAttribute in _exportOptions.Attributes)
            {
                ws.Cells[1, col++].Value = headerAttribute.Name;
            }

            ws.Cells[1, 1, 1, col].AutoFitColumns(15, 150);
            ws.Cells[1, 1, 1, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[1, 1, 1, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[1, 1, 1, col].Style.Font.Bold = true;
            ws.Cells[1, 1, 1, col].Style.WrapText = true;
        }

        private void AddProductsData(ExcelWorksheet ws)
        {
            var row = ProductDataStartRow;
            foreach (var group in _products.GroupBy(x => x.ParentId))
            {
                //Заполняем атрибуты для размерных рядов
                foreach (var product in group)
                    FillUnCommonAttributeData(ws, product, row++, group.Count());

                var startMergeRow = row - group.Count();
                //Заполняем атрибуты общие для размерных рядов и мерджим в одну ячейку
                FillCommonAttributeData(ws, group.First(), row, startMergeRow);
                //Заполняем ску фото и категорию 
                FillCustomProductData(ws, group.First(), row, startMergeRow);
            }

            SetStyles(ws);
        }

        private void FillCommonAttributeData(ExcelWorksheet ws, Product product, int row, int startMergeRow)
        {
            foreach (var attr in _exportOptions.Attributes.Where(x => x.IsCommon))
            {
                var col = _exportOptions.Attributes.IndexOf(attr) + AttributesStartCol;
                ws.Cells[startMergeRow, col, row - 1, col].Merge = true;

                var av = product.AttributeValues.FirstOrDefault(x => x.AttributeId == attr.AttributeId);
                SetAttrValueToCell(attr, product, ws, col, startMergeRow);
            }
        }

        private void FillCustomProductData(ExcelWorksheet ws, Product product, int row, int startMergeRow)
        {
            //Мерджим ячейки для фото и категории.
            ws.Cells[startMergeRow, ImageCol, row - 1, ImageCol].Merge = true;
            ws.Cells[startMergeRow, CategoryCol, row - 1, CategoryCol].Merge = true;
            ws.Cells[startMergeRow, CategoryCol].Value = _categoriesDict.TryGetValue(product.CategoryId ?? 1, out var cat) ? cat.Name : "-";

           
            InsertImage(product, ws, startMergeRow, ImageCol);
        }

        private void FillUnCommonAttributeData(ExcelWorksheet ws, Product product, int row, int productsInGroup)
        {
            ws.Cells[row, SkuCol].Value = product.Sku;
            foreach (var attr in _exportOptions.Attributes.Where(x => !x.IsCommon))
            {
                //Устанавливаем высоту в 400 пикселей в сумме для всех ячеек
                if (productsInGroup * 12 < 305)
                    ws.Row(row).Height = 305 / productsInGroup + 1;

                var col = _exportOptions.Attributes.IndexOf(attr) + AttributesStartCol;
                SetAttrValueToCell(attr, product, ws, col, row);
            }
        }

        private void SetStyles(ExcelWorksheet ws)
        {
            var lastCol = _attributesDict.Count + AttributesStartCol;
            ws.Cells[ProductDataStartRow, 1, ws.Dimension.Rows, lastCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[ProductDataStartRow, 1, ws.Dimension.Rows, lastCol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[ProductDataStartRow, 1, ws.Dimension.Rows, lastCol].Style.WrapText = true;
            ws.Cells[ProductDataStartRow, 1, ws.Dimension.Rows, lastCol].AutoFitColumns(15, 150);

            //Устанавливаем ширину в 400 пикселей для картинки
            ws.Column(2).Width = 58;
        }

        private void SetAttrValueToCell(AttributeColumn attr, Product product, ExcelWorksheet ws, int col, int row)
        {
            var av = product.AttributeValues.FirstOrDefault(x => x.AttributeId == attr.AttributeId);
            if (av != null)
                ws.Cells[row, col].Value = (av.StrValue + " " + av.NumValue + " " + av.DateValue + " " + (av.BoolValue != null ? ((bool) av.BoolValue ? "Да" : "Нет") : null) + " " + av.ListValue?.Value).Trim();
            else if (attr.CompositeAttributes != null && attr.CompositeAttributes.Any())
                ws.Cells[row, col].Value = GetProductTextField(product, attr.CompositeAttributes.ToArray());
            else if (_exportOptions.Attributes.Any(x => x.IsComputed && x.Name == attr.Name))
                SetComputedCellValue(ws, attr.Name, row, col);
            else ws.Cells[row, col].Value = "-";
        }

        private void InsertImage(Product product, ExcelWorksheet ws, int row, int col, int maxPhotoSize = 400)
        {
            var fileId = product.ProductFiles.FirstOrDefault(x => x.IsMain && x.FileType == FileType._1)?.FileId;
            if (fileId.HasValue && _imagesDictionary.TryGetValue(fileId.Value, out var file))
            {
                using (var image = new Bitmap(file.FileStream))
                {
                    if (image.HorizontalResolution <= 0.1f || image.VerticalResolution <= 0.1f)
                        image.SetResolution(96, 96);

                    var imageWidth = image.Width;
                    var imageHeight = image.Height;

                    if (imageWidth > maxPhotoSize && imageWidth > imageHeight)
                    {
                        imageWidth = maxPhotoSize;
                        imageHeight = imageHeight * maxPhotoSize / image.Width;
                    }
                    else if (imageHeight > maxPhotoSize)
                    {
                        imageHeight = maxPhotoSize;
                        imageWidth = imageWidth * maxPhotoSize / image.Height;
                    }

                    var pic = ws.Drawings.AddPicture(fileId.ToString(), image);
                    pic.SetPosition(row - 1, 5, col - 1, 5);
                    pic.SetSize(imageWidth, imageHeight);
                }
            }
        }

        private string GetProductTextField(Product product, int[] attributesIds, bool withAttributeName = true)
        {
            return string.Join(";\n", product.AttributeValues.Where(p => attributesIds.Contains(p.AttributeId)).OrderBy(d => attributesIds.ToList().IndexOf(d.AttributeId))
                .Select(p => withAttributeName ? $"{p.Attribute.Name}: {p.StrValue}" : $"{p.StrValue}"));
        }

        private void SetComputedCellValue(ExcelWorksheet ws, string name, int row, int col)
        {
            switch (name)
            {
                case "Отобран":
                    ws.Cells[row, col].Value = "Да";
                    break;
                case "Заказ":
                    ws.Cells[row, col].Value = 0;
                    break;
                case "Сумма":
                {
                    //Оптовая цена
                    var price = _exportOptions.Attributes.FirstOrDefault(x => x.AttributeId == 177);
                    var order = _exportOptions.Attributes.FirstOrDefault(x => x.Name == "Заказ");

                    if (price != null && order != null)
                    {
                        var priceCol = _exportOptions.Attributes.IndexOf(price) + AttributesStartCol;
                        var orderCol = _exportOptions.Attributes.IndexOf(order) + AttributesStartCol;
                        ws.Cells[row, col].Formula = $"IFERROR({ws.Cells[row, priceCol].Address}*{ws.Cells[row, orderCol].Address}, 0)";
                    }

                    break;
                }
            }
        }
    }
}