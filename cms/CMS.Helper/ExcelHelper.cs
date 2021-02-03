using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Extension;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace Helper
{
    public class ExcelHelper
    {
        public static object GetCellValue(ICell cell)
        {
            object val = null;
            if (cell != null)
            {
                switch (cell.CellType)
                {
                    case CellType.Unknown:
                        break;
                    case CellType.Numeric:
                        if (DateUtil.IsCellDateFormatted(cell)) //日期类型
                        {
                            val = cell.DateCellValue;
                        }
                        else //其他数字类型
                        {
                            val = cell.NumericCellValue;
                        }

                        break;
                    case CellType.String:
                        val = cell.StringCellValue;
                        break;
                    case CellType.Formula:
                        break;
                    case CellType.Blank:
                        break;
                    case CellType.Boolean:
                        val = cell.BooleanCellValue;
                        break;
                    case CellType.Error:
                        break;
                    default:
                        return null;
                }
            }

            return val;
        }

        public static IWorkbook GetWorkbook(string fileExt, FileStream stream = null)
        {
            return GetWorkbook(fileExt == ".xls" ? ExcelExt.Xls : ExcelExt.Xlsx, stream);
        }

        public static IWorkbook GetWorkbook(ExcelExt ext, FileStream stream = null)
        {
            IWorkbook workbook = null;
            switch (ext)
            {
                case ExcelExt.Xls:
                    workbook = stream == null ? new HSSFWorkbook() : new HSSFWorkbook(stream);
                    break;
                case ExcelExt.Xlsx:
                    workbook = stream == null ? new XSSFWorkbook() : new XSSFWorkbook(stream);
                    break;
            }

            return workbook;
        }

        public static void TemplateExport(string templatePath, string savePath, Dictionary<string, string> data)
        {
            if (!File.Exists(templatePath)) return;
            File.Copy(templatePath, savePath, true);

            IWorkbook workbook = null;
            using (var stream = new FileStream(savePath, FileMode.Open, FileAccess.Read))
            {
                workbook = GetWorkbook(Path.GetExtension(savePath).ToLower(), stream);
            }

            ISheet sheet = workbook?.GetSheetAt(0);

            IRow headRow = sheet?.GetRow(0);
            if (headRow == null) return;

            Regex regPlaceholder = new Regex("^\\{\\{([0-9A-Za-z_]+)\\}\\}$");
            int cellCount = headRow.LastCellNum;
            for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null) continue;
                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                for (int j = 0; j < cellCount; j++)
                {
                    ICell cell = row.GetCell(j);
                    CellType oldCellType = cell.CellType;
                    if (oldCellType != CellType.String) continue;

                    string val = cell.StringCellValue;
                    if (regPlaceholder.IsMatch(val))
                    {
                        var match = regPlaceholder.Match(val);
                        if (match.Groups.Count < 2) continue;

                        var key = match.Groups[1].ToStr();
                        if (!data.ContainsKey(key)) continue;

                        cell.SetCellValue(data[key]);
                    }
                }
            }


            var updateFileStream = new FileStream(savePath, FileMode.Open, FileAccess.Write);
            workbook.Write(updateFileStream);
            workbook.Close();
            updateFileStream.Close();
        }


        public static void Export(string savePath, List<dynamic> data, List<string> ignoreField = null,
            Dictionary<string, string> replaceKeys = null, Func<string, object, string> funcValue = null)
        {
            if (string.IsNullOrWhiteSpace(savePath)) return;
            if (data.Count <= 0) return;

            string dirPath = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            using (var fs = new FileStream(savePath, FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook = GetWorkbook(Path.GetExtension(savePath));
                if (workbook == null) return;

                ISheet sheet = workbook.CreateSheet();

                IRow headRow = sheet.CreateRow(0);

                Dictionary<int, string> columns = new Dictionary<int, string>();
                int keyIndex = 0;
                if (data[0] != null)
                {
                    if (data[0] is IDictionary<string, object> dataItem)
                        foreach (var key in dataItem.Keys)
                        {
                            if (ignoreField != null && ignoreField.Contains(key)) continue;

                            string newKey = key;
                            if (replaceKeys != null && replaceKeys.ContainsKey(key))
                            {
                                newKey = replaceKeys[key];
                            }

                            headRow.CreateCell(keyIndex).SetCellValue(newKey);
                            columns.Add(keyIndex, key);
                            keyIndex++;
                        }
                }

                sheet.AutoSizeColumn(0);

                int rowIndex = 1;
                foreach (var item in data)
                {
                    headRow = sheet.CreateRow(rowIndex);
                    if (item is IDictionary<string, object> dicItem)
                    {
                        foreach (var column in columns)
                        {
                            object oldValue = dicItem[column.Value];
                            string setValue = funcValue == null ? oldValue.ToStr() : funcValue(column.Value, oldValue);

                            headRow.CreateCell(column.Key).SetCellValue(setValue);
                        }
                    }

                    sheet.AutoSizeColumn(rowIndex);
                    rowIndex++;
                }

                workbook.Write(fs);
            }
        }


        private static void JsonToExcel(string excelData, ISheet sheet, ICellStyle cellStyle)
        {
            var table = JsonConvert.DeserializeObject<Table>(excelData);

            FillExcelData(sheet, cellStyle, table);
        }

        private static void FillExcelData(ISheet sheet, ICellStyle cellStyle, Table table)
        {
            // 记录跨行、跨列单元格索引及所跨级数。
            var rowspanStack = new List<RowspanCell>();

            // 记录需要合并的单元格信息。
            var mergeStack = new List<Merge>();

            for (var x = 0; x < table.Rows.Count; x++)
            {
                var row = sheet.CreateRow(x);
                row.Height = 300;
                var currCellIndex = 0;
                foreach (var cell in table.Rows[x].Cells)
                {
                    if (rowspanStack.Count > 0)
                    {
                        foreach (var rowsoan in rowspanStack)
                        {
                            if (rowsoan.Rows > x)
                            {
                                if (rowsoan.Index == currCellIndex)
                                {
                                    row.CreateCell(currCellIndex).SetCellValue(string.Empty);
                                    row.GetCell(currCellIndex).CellStyle = cellStyle;
                                    currCellIndex++;
                                }
                            }
                        }
                    }

                    // 如果跨行
                    if (cell.Rowspan > 0)
                    {
                        rowspanStack.Add(new RowspanCell(currCellIndex, x + cell.Rowspan, x + cell.Rowspan));
                        mergeStack.Add(new Merge(x, x + cell.Rowspan - 1, currCellIndex, currCellIndex, SpanType.Row));
                    }

                    row.CreateCell(currCellIndex).SetCellValue(cell.Text);
                    row.GetCell(currCellIndex).CellStyle = cellStyle;
                    currCellIndex++;

                    if (cell.Colspan <= 0) continue;

                    mergeStack.Add(new Merge(x, x, currCellIndex - 1, currCellIndex + cell.Colspan - 1 - 1,
                        SpanType.Column));
                    for (var i = 0; i < cell.Colspan - 1; i++)
                    {
                        row.CreateCell(currCellIndex).SetCellValue(string.Empty);
                        row.GetCell(currCellIndex).CellStyle = cellStyle;
                        currCellIndex++;
                    }
                }
            }

            // 开始合并单元格
            mergeStack.ForEach(p =>
            {
                var address = new CellRangeAddress(p.StartRowIndex, p.EndRowIndex, p.StartColIndex, p.EndColIndex);
                sheet.AddMergedRegion(address);
            });
        }

        private static ICellStyle GetCellStyle(IWorkbook workbook)
        {
            var cellStyle = workbook.CreateCellStyle();
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            cellStyle.BorderBottom = BorderStyle.Thin;
            cellStyle.BorderBottom = BorderStyle.Thin;
            cellStyle.BottomBorderColor = HSSFColor.Black.Index;
            cellStyle.BorderLeft = BorderStyle.Thin;
            cellStyle.LeftBorderColor = HSSFColor.Black.Index;
            cellStyle.BorderRight = BorderStyle.Thin;
            cellStyle.RightBorderColor = HSSFColor.Black.Index;
            cellStyle.BorderTop = BorderStyle.Thin;
            cellStyle.TopBorderColor = HSSFColor.Black.Index;

            return cellStyle;
        }


        #region 表格信息

        public class JsonTable
        {
            public string JsonData { get; set; }
            public string SheetName { get; set; }
        }

        /// <summary>
        ///     表信息。
        /// </summary>
        public class Table
        {
            public Table()
            {
                Rows = new List<Row>();
            }

            /// <summary>
            ///     行信息。
            /// </summary>
            public List<Row> Rows { get; set; }

            public Row NewRow()
            {
                Rows = Rows ?? new List<Row>();
                Rows.Add(new Row());
                return Rows[Rows.Count - 1];
            }
        }

        /// <summary>
        ///     行信息。
        /// </summary>
        public class Row
        {
            public Row()
            {
                Cells = new List<Cell>();
            }

            /// <summary>
            ///     单元格信息。
            /// </summary>
            public List<Cell> Cells { get; set; }


            public Cell NewCell(string text = "", int rowspan = 0, int colspan = 0)
            {
                Cells = Cells ?? new List<Cell>();
                Cells.Add(new Cell(rowspan, colspan, text));

                return Cells[Cells.Count - 1];
            }
        }

        /// <summary>
        ///     单元格信息。
        /// </summary>
        public class Cell
        {
            public Cell()
            {
            }

            public Cell(int rowspan, int colspan, string text)
            {
                Rowspan = rowspan;
                Colspan = colspan;
                Text = text;
            }

            /// <summary>
            ///     跨行数。
            /// </summary>
            public int Rowspan { get; set; }

            /// <summary>
            ///     跨列数。
            /// </summary>
            public int Colspan { get; set; }

            /// <summary>
            /// 单元格信息
            /// </summary>
            public string Text { get; set; }
        }

        /// <summary>
        ///     跨行单元格信息。
        /// </summary>
        public class RowspanCell
        {
            public RowspanCell()
            {
            }

            public RowspanCell(int index, int rows, int currRowIndex)
            {
                Index = index;
                Rows = rows;
                CurrentRowIndex = currRowIndex;
            }

            /// <summary>
            ///     跨行单元格索引。
            /// </summary>
            public int Index { get; set; }

            /// <summary>
            ///     跨行数。
            /// </summary>
            public int Rows { get; set; }

            /// <summary>
            ///     当前行索引。
            /// </summary>
            public int CurrentRowIndex { get; set; }

            /// <summary>
            ///     递减跨行计数。
            /// </summary>
            public void SetRowDecrease()
            {
                var rows = CurrentRowIndex;
                CurrentRowIndex = rows - 1;
            }
        }

        /// <summary>
        ///     合并单元格信息。
        /// </summary>
        public class Merge
        {
            public Merge()
            {
            }

            public Merge(int startRowIndex, int endRowIndex, int startColIndex, int endColIndex, SpanType type)
            {
                StartRowIndex = startRowIndex;
                EndRowIndex = endRowIndex;
                StartColIndex = startColIndex;
                EndColIndex = endColIndex;
                Type = type;
            }

            /// <summary>
            ///     开始行索引。
            /// </summary>
            public int StartRowIndex { get; set; }

            /// <summary>
            ///     结束行索引。
            /// </summary>
            public int EndRowIndex { get; set; }

            /// <summary>
            ///     开始列索引。
            /// </summary>
            public int StartColIndex { get; set; }

            /// <summary>
            ///     结束列索引。
            /// </summary>
            public int EndColIndex { get; set; }

            /// <summary>
            ///     跨越类型。
            /// </summary>
            public SpanType Type { get; set; }
        }

        /// <summary>
        ///     跨越类型。
        /// </summary>
        public enum SpanType
        {
            Row = 0,
            Column = 1,
            Both = 2
        }

        #endregion
    }

    public enum ExcelExt
    {
        Xls,
        Xlsx
    }

    public class ExcelTable
    {
        private IWorkbook _iWorkbook;

        private ExcelTable()
        {
            Rows = new List<ExcelRow>();
        }

        public ExcelTable(ExcelExt ext) : this()
        {
            _iWorkbook = ExcelHelper.GetWorkbook(ext);
        }

        public ExcelTable(IWorkbook workbook) : this()
        {
            _iWorkbook = workbook;
        }

        public IWorkbook Workbook
        {
            get => _iWorkbook;
            set => _iWorkbook = value;
        }

        /// <summary>
        ///     行信息。
        /// </summary>
        public List<ExcelRow> Rows { get; set; }

        public ExcelRow NewRow()
        {
            Rows.Add(new ExcelRow(_iWorkbook));
            return Rows[Rows.Count - 1];
        }
    }

    public class ExcelRow
    {
        private readonly IWorkbook _iWorkbook;

        public ExcelRow(IWorkbook workbook)
        {
            _iWorkbook = workbook;
            Cells = new List<ExcelCell>();
        }

        /// <summary>
        ///     单元格信息。
        /// </summary>
        public List<ExcelCell> Cells { get; set; }


        public ExcelCell NewCell(string text = "", int rowspan = 0, int colspan = 0)
        {
            Cells.Add(new ExcelCell(_iWorkbook)
            {
                Text = text,
                Rowspan = rowspan,
                Colspan = colspan
            });

            return Cells[Cells.Count - 1];
        }
    }

    public class ExcelCell
    {
        private readonly IWorkbook _iWorkbook;

        public ExcelCell(IWorkbook workbook)
        {
            _iWorkbook = workbook;
        }

        /// <summary>
        ///     跨行数。
        /// </summary>
        public int Rowspan { get; set; }

        /// <summary>
        ///     跨列数。
        /// </summary>
        public int Colspan { get; set; }

        /// <summary>
        /// 单元格信息
        /// </summary>
        public string Text { get; set; }

        private ICellStyle _cellStyle;

        public ICellStyle CellStyle
        {
            get
            {
                if (_cellStyle == null)
                {
                    _cellStyle = _iWorkbook.CreateCellStyle();
                    _cellStyle.Alignment = HorizontalAlignment.Center;
                    _cellStyle.VerticalAlignment = VerticalAlignment.Center;
                    _cellStyle.BorderBottom = BorderStyle.Thin;
                    _cellStyle.BorderBottom = BorderStyle.Thin;
                    _cellStyle.BottomBorderColor = HSSFColor.Black.Index;
                    _cellStyle.BorderLeft = BorderStyle.Thin;
                    _cellStyle.LeftBorderColor = HSSFColor.Black.Index;
                    _cellStyle.BorderRight = BorderStyle.Thin;
                    _cellStyle.RightBorderColor = HSSFColor.Black.Index;
                    _cellStyle.BorderTop = BorderStyle.Thin;
                    _cellStyle.TopBorderColor = HSSFColor.Black.Index;
                }

                return _cellStyle;
            }
        }

        public IFont Font => CellStyle.GetFont(_iWorkbook);
    }
}