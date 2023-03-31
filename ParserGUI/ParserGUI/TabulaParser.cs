using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using System.Collections.Generic;
using Tabula.Extractors;
using Tabula;
using System.Linq;
using Tabula.Detectors;
using UglyToad.PdfPig.Core;
using System;
using ParserGUI;
using System.Xml.Linq;

namespace ParserCore
{
    public class TabulaParser : IParser
    {
        private readonly string _filename;
        private readonly PdfDocument _document;
        private readonly ObjectExtractor _objectExtractor;
        private readonly IExtractionAlgorithm _extractionAlgorithm;
        private readonly ITextParser _cellParser;
        private readonly List<Table> _tables;
        private readonly Dictionary<Table, Dictionary<Cell, string>> _cellStrings = new Dictionary<Table, Dictionary<Cell, string>>();
        private readonly IDetectionAlgorithm _detectionAlgorithm;
        

        public TabulaParser(string filename, ITextParser cellParser)
        {
            _filename = filename;
            _document = PdfDocument.Open(_filename, new ParsingOptions() { ClipPaths = true });
            _objectExtractor = new ObjectExtractor(_document);
            _extractionAlgorithm = new SpreadsheetExtractionAlgorithm();
            _detectionAlgorithm = new SimpleNurminenDetectionAlgorithm();
            _tables = new List<Table>();
            _cellParser = cellParser;
        }
        void IParser.Parse()
        {
            Dictionary<Table, Dictionary<Cell, List<Letter>>> cellLetters = new Dictionary<Table, Dictionary<Cell, List<Letter>>>();
            for (int i = 1; i <= _document.NumberOfPages; i++)
            {
                try
                {
                    var letters = (List<Letter>)_document.GetPage(i).Letters;
                    var tables = ParsePage(i);
                    foreach (var table in tables)
                    {
                        cellLetters.Add(table, FindLettersInCells(letters, table));
                    }
                    _tables.AddRange(tables);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("[ERROR] Parsing page {0} failed: {1}", i, e.Message);
                }
            }
            ParseAllCells(cellLetters);
        }

        int IParser.TableCount
        {
            get
            {
                return _tables.Count;
            }
        }

        List<Table> IParser.GetTables()
        {
            return _tables;
        }

        Table IParser.GetTable(int tableNumber)
        {
            return _tables[tableNumber];
        }

        string IParser.GetCellText(Table table, Cell cell)
        {
            _cellStrings[table].TryGetValue(cell, out string text);
            if (text == null) return "";
            return text;
        }

        private Dictionary<Cell, List<Letter>> FindLettersInCells(List<Letter> letters, Table table)
        {
            Dictionary<Cell, List<Letter>> cellLettersDict = new Dictionary<Cell, List<Letter>>();
            foreach (var row in table.Rows)
            {
                foreach (var cell in row)
                {
                    if (cell.GetText() == "") continue;
                    List<Letter> cellLetters = new List<Letter>();
                    foreach (var letter in letters)
                    {
                        if (IsLetterInsideCell(letter, cell))
                        {
                            cellLetters.Add(letter);
                        }
                    }
                    cellLettersDict.Add(cell, cellLetters);
                }
            }
            return cellLettersDict;
        }

        private bool IsLetterInsideCell(Letter letter, Cell cell)
        {
            return letter.GlyphRectangle.Top <= cell.Top &&
                    letter.GlyphRectangle.Bottom >= cell.Bottom &&
                    letter.GlyphRectangle.Left >= cell.Left &&
                    letter.GlyphRectangle.Right <= cell.Right;
        }

        public List<Table> ParsePage(int pageNumber)
        {
            PageArea page = _objectExtractor.Extract(pageNumber);
            ITableFilter tableFilter = new SimpleTableFilter(page);
            List<TableRectangle> regions = _detectionAlgorithm.Detect(page);
            if (regions.Count == 0) return new List<Table>();
            List<Table> tables = new List<Table>();
            regions.Sort((a, b) => (int)(b.Top - a.Top));
            foreach (var region in regions)
            {
                var extractedTables = _extractionAlgorithm.Extract(page.GetArea(region.BoundingBox));
                tables.AddRange(tableFilter.Filter(extractedTables));
            }

            return tables;
        }

        private void ParseAllCells(Dictionary<Table, Dictionary<Cell, List<Letter>>> cellLetters)
        {
            foreach (var table in cellLetters.AsEnumerable())
            {
                Dictionary<Cell, string> cellStringDict = new Dictionary<Cell, string>();
                foreach (var keyValue in table.Value.AsEnumerable())
                {
                    var letters = keyValue.Value;
                    var text = _cellParser.GetText(letters);
                    cellStringDict.Add(keyValue.Key, text);
                }
                _cellStrings.Add(table.Key, cellStringDict);
            }
        }
   
    }
}