using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Extension;
using Foundation.Modal.Result;
using Helper;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace Foundation.Utils
{
    public class ExcelUtil : IDisposable
    {
        private readonly string _filePath;
        private readonly IWorkbook _workbook;
        private ISheet _currentSheet;
        private List<string> _letters;

        public ExcelUtil(string filePath)
        {
            _filePath = filePath;
            if (!Exists()) return;

            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                string fileExtension = Path.GetExtension(filePath);
                _workbook = ExcelHelper.GetWorkbook(fileExtension, stream);
            }

            SelectSheet(0);

            _letters = Enum.GetNames(typeof(Letter)).ToList();
        }

        public bool Exists()
        {
            if (_filePath.IsEmpty()) return false;
            if (!File.Exists(_filePath)) return false;
            return true;
        }

        public ExcelUtil SelectSheet(string sheetName)
        {
            if (_workbook != null && sheetName.IsNotEmpty())
                _currentSheet = _workbook.GetSheet(sheetName);

            return this;
        }

        public ExcelUtil SelectSheet(int sheetIndex)
        {
            if (_workbook != null && sheetIndex >= 0 && sheetIndex < _workbook.NumberOfSheets)
                _currentSheet = _workbook.GetSheetAt(sheetIndex);

            return this;
        }

        /// <summary>
        /// 循环获取数据
        /// </summary>
        /// <param name="startRowIndex"></param>
        /// <param name="setValueAction"></param>
        /// <returns></returns>
        public async Task AsyncCirculateLetterGetValue(int startRowIndex, Func<int, ExcelRowItem, Task> setValueAction)
        {
            if (_currentSheet.LastRowNum < startRowIndex) return;

            for (int currentRowIndex = startRowIndex, dataIndex = 0;
                currentRowIndex <= _currentSheet.LastRowNum;
                currentRowIndex++, dataIndex++)
            {
                var currentRow = _currentSheet.GetRow(currentRowIndex);

                ExcelRowItem rowDictionary = new ExcelRowItem();
                foreach (var currentRowCell in currentRow.Cells)
                {
                    string cellValue = ExcelHelper.GetCellValue(currentRowCell).ToStr();
                    int cellIndex = currentRowCell.ColumnIndex;
                    rowDictionary.Add(cellValue, GetColumnLetter(cellIndex));
                }

                await setValueAction.Invoke(dataIndex, rowDictionary);
            }
        }

        /// <summary>
        /// 循环插入数据
        /// </summary>
        /// <param name="startRowIndex"></param>
        /// <param name="setValueAction"></param>
        public void CirculateLetterSetValue(int startRowIndex, Func<int, ExcelRowItem, bool> setValueAction)
        {
            if (_currentSheet.LastRowNum <= startRowIndex)
                _currentSheet.CreateRow(startRowIndex);

            int currentRowIndex = startRowIndex;
            bool isWhile = false;
            int dataIndex = 0;
            do
            {
                ExcelRowItem item = new ExcelRowItem();
                isWhile = setValueAction.Invoke(dataIndex, item);
                if (isWhile)
                    _currentSheet.CopyRow(currentRowIndex, currentRowIndex + 1);

                SetRowValue(currentRowIndex, item);

                currentRowIndex++;
                dataIndex++;
            } while (isWhile);
        }

        public void SetRowValue(int rowIndex, ExcelRowItem rowItem)
        {
            if (_currentSheet.LastRowNum <= rowIndex)
                _currentSheet.CreateRow(rowIndex);


            foreach (var itemKey in rowItem.Keys)
            {
                var value = rowItem.Get(itemKey);
                SetCellValue(rowIndex, value, itemKey);
            }
        }

        public void SetCellValue(int rowIndex, string value, params Letter[] columnLetters)
        {
            if (_currentSheet.LastRowNum < rowIndex) return;

            IRow row = _currentSheet.GetRow(rowIndex);

            int cellIndex = GetCellIndex(columnLetters);
            if (cellIndex < row.Cells.Count)
            {
                ICell cell = row.Cells[cellIndex];
                cell.SetCellValue(value);
            }
            else
            {
                row.CreateCell(cellIndex).SetCellValue(value);
            }
        }

        private int GetCellIndex(Letter[] letters)
        {
            int index = 0;
            for (int i = 0; i < letters.Length; i++)
            {
                index += i * _letters.Count;
                index += (int) letters[i];
            }

            return index;
        }

        public void CirculateLetterSetValue(int startRowIndex, int dataCount,
            Func<int, string, string, string> getValueAction)
        {
            if (_currentSheet.LastRowNum < startRowIndex) return;

            for (int dataIndex = 0, currentRowIndex = startRowIndex;
                dataIndex < dataCount;
                dataIndex++, currentRowIndex++)
            {
                if (dataIndex < dataCount - 1)
                {
                    _currentSheet.CreateRow(currentRowIndex + 1);
                    _currentSheet.CopyRow(currentRowIndex, currentRowIndex + 1);
                }

                IRow row = _currentSheet.GetRow(currentRowIndex);

                foreach (var currentRowCell in row.Cells)
                {
                    string cellValue = ExcelHelper.GetCellValue(currentRowCell).ToStr();

                    int cellIndex = currentRowCell.ColumnIndex;

                    string indexLetter = GetCellColumnLetter(cellIndex);
                    currentRowCell.SetCellValue(getValueAction.Invoke(dataIndex, indexLetter, cellValue));
                }
            }
        }

        private string GetCellColumnLetter(int cellIndex)
        {
            string indexLetter;
            if (cellIndex < _letters.Count)
            {
                indexLetter = _letters[cellIndex];
            }
            else
            {
                int firstIndex = cellIndex / _letters.Count;
                int twoIndex = cellIndex - firstIndex * _letters.Count;
                indexLetter = _letters[firstIndex - 1] + _letters[twoIndex];
            }

            return indexLetter;
        }

        readonly Regex _placeholderRegex = new Regex("\\{\\{([a-zA-Z0-9_\\u4e00-\\u9fa5]+)\\}\\}");

        public async Task<HandleResult> AsyncFuillImportRowData<T>(IEnumerable<T> data, int startRowIndex,
            Func<T, string, int, Task<string>> getValueFunc)
        {
            if (_currentSheet == null) return HandleResult.Error("未能找到Sheet数据");
            if (data == null) return HandleResult.Error("没有要填充导入的数据");
            if (_currentSheet.LastRowNum < startRowIndex) return HandleResult.Error("起始行超过Sheet最大行数");

            var list = data.ToList();
            for (int dataIndex = 0, currentRowIndex = startRowIndex;
                dataIndex < list.Count;
                dataIndex++, currentRowIndex++)
            {
                if (dataIndex < list.Count - 1)
                {
                    _currentSheet.CreateRow(currentRowIndex + 1);
                    _currentSheet.CopyRow(currentRowIndex, currentRowIndex + 1);
                }


                T item = list[dataIndex];
                IRow row = _currentSheet.GetRow(currentRowIndex);

                foreach (var rowCell in row.Cells)
                {
                    string cellValue = ExcelHelper.GetCellValue(rowCell).ToStr();

                    var macths = _placeholderRegex.Matches(cellValue);
                    if (macths.Count <= 0) continue;

                    foreach (Match macth in macths)
                    {
                        string placeholder = macth.Groups[1].ToStr();

                        var newVal = "";
                        if (macth.Groups.Count == 2)
                            newVal = await getValueFunc.Invoke(item, placeholder, dataIndex);

                        cellValue = cellValue.Replace(macth.ToString(), newVal);
                    }

                    rowCell.SetCellValue(cellValue);
                }
            }

            return HandleResult.Success();
        }

        public HandleResult FuillImportRowData<T>(IEnumerable<T> data, int startRowIndex,
            Func<T, string, int, string> getValueFunc)
        {
            if (_currentSheet == null) return HandleResult.Error("未能找到Sheet数据");
            if (data == null) return HandleResult.Error("没有要填充导入的数据");
            if (_currentSheet.LastRowNum < startRowIndex) return HandleResult.Error("起始行超过Sheet最大行数");

            var list = data.ToList();
            try
            {
                for (int dataIndex = 0, currentRowIndex = startRowIndex;
                    dataIndex < list.Count;
                    dataIndex++, currentRowIndex++)
                {
                    if (dataIndex < list.Count - 1)
                    {
                        _currentSheet.CreateRow(currentRowIndex + 1);
                        _currentSheet.CopyRow(currentRowIndex, currentRowIndex + 1);
                    }


                    T item = list[dataIndex];
                    IRow row = _currentSheet.GetRow(currentRowIndex);

                    foreach (var rowCell in row.Cells)
                    {
                        string cellValue = ExcelHelper.GetCellValue(rowCell).ToStr();

                        var macths = _placeholderRegex.Matches(cellValue);
                        if (macths.Count <= 0) continue;

                        foreach (Match macth in macths)
                        {
                            string placeholder = macth.Groups[1].ToStr();

                            var newVal = "";
                            if (macth.Groups.Count == 2)
                                newVal = getValueFunc.Invoke(item, placeholder, dataIndex);

                            cellValue = cellValue.Replace(macth.ToString(), newVal);
                        }

                        rowCell.SetCellValue(cellValue);
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return HandleResult.Success();
        }

        public byte[] ToBytes()
        {
            MemoryStream memoryStream = new MemoryStream();
            _workbook?.Write(memoryStream);
            _workbook?.Close();
            return memoryStream.ToArray();
        }

        public void Save(string newFilePath = "")
        {
            string filePath = newFilePath.IsEmpty() ? _filePath : newFilePath;
            string folderPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            using (var fileStream =
                new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            {
                _workbook.Write(fileStream);
                Close();
            }
        }

        public void Close()
        {
            _workbook?.Close();
        }


        public void Dispose()
        {
            _workbook?.Close();
        }

        public Letter[] GetColumnLetter(int cellIndex)
        {
            var letters = Enum.GetValues(typeof(Letter));
            if (cellIndex < letters.Length)
            {
                return new[] {Enum.Parse<Letter>(letters.GetValue(cellIndex).ToStr())};
            }

            int firstIndex = cellIndex / letters.Length;
            int twoIndex = cellIndex - firstIndex * letters.Length;

            List<Letter> letterValues = new List<Letter>();
            letterValues.AddRange(GetColumnLetter(firstIndex - 1));
            letterValues.AddRange(GetColumnLetter(twoIndex));

            return letterValues.ToArray();
        }
    }

    public class ExcelRowItem
    {
        private readonly Dictionary<string, string> _row;

        public ExcelRowItem()
        {
            _row = new Dictionary<string, string>();
        }

        /// <summary>
        /// 添加单元格数据
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="columnLetter">单元格字母,至少需要一个</param>
        public void Add(string value, params Letter[] columnLetter)
        {
            if (columnLetter.Length <= 0) return;

            string key = GetKey(columnLetter);

            if (!_row.ContainsKey(key))
                _row.Add(key, value);
        }

        private string GetKey(Letter[] columnLetter)
        {
            return string.Join("", columnLetter);
        }


        public string Get(params Letter[] columnLetter)
        {
            if (columnLetter.Length <= 0) return "";

            string key = GetKey(columnLetter);
            if (!_row.ContainsKey(key)) return "";
            return _row[key];
        }


        private List<Letter[]> _keys;

        public List<Letter[]> Keys
        {
            get
            {
                if (_keys == null || _keys.Count <= 0)
                {
                    _keys = new List<Letter[]>();
                    foreach (var rowKey in _row.Keys)
                    {
                        List<Letter> letters = new List<Letter>();
                        foreach (var c in rowKey.ToCharArray())
                        {
                            letters.Add(Enum.Parse<Letter>(c.ToString()));
                        }

                        _keys.Add(letters.ToArray());
                    }
                }

                return _keys;
            }
        }

        public string this[params Letter[] letters] => Get(letters);
    }

    public enum Letter
    {
        A,
        B,
        C,
        D,
        E,
        F,
        G,
        H,
        I,
        J,
        K,
        L,
        M,
        N,
        O,
        P,
        Q,
        R,
        S,
        T,
        U,
        V,
        W,
        X,
        Y,
        Z
    }
}