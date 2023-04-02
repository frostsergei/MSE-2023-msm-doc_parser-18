using ParserCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tabula;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace ParserGUI
{
    internal class DocumentContentParser
    {
        struct ContentTableItem
        {
            public string Number;
            public string Title;
            public int Page;
        }


        private PdfDocument _document;
        private ITextParser _textParser;
        private List<string> _parsedLines;
        private static string _contentTableTitle = "содержание";
        private static int _maxPageNumber = 10;
        private int _pageNumberOffset = 0;
        private List<ContentTableItem> _contentItems;
        public DocumentContentParser(PdfDocument document)
        {
            _document = document;
            _textParser = new NearestNeighbourTextParser();
            _parsedLines = new List<string>();
        }
        public List<int> Parse()
        {
            var contentPageNumber = FindContentPage();
            ParseAllContent(contentPageNumber);
            _contentItems = _parsedLines.Select(line => ParseContentTableItem(line)).ToList();
            return GetPages();
        }

        private int FindContentPage()
        {
            var pageNumber = 1;
            while (pageNumber < _maxPageNumber)
            {
                var pageLines = GetPageLines(pageNumber);                
                bool found = false;
                for(int i = 0; i<pageLines.Count; i++)
                {
                    if (pageLines[i].ToLower().Contains(_contentTableTitle))
                    {
                        _pageNumberOffset = pageNumber - ParsePageNumber(pageLines[0]);
                        _parsedLines.AddRange(pageLines.Where((line,index) => index > i));
                        found = true;
                        break;
                    }
                }
                if (found) break;
                pageNumber++;
            }
            return pageNumber;
        }

        private int ParsePageNumber(string line)
        {
            var words = line.Split(' ').ToList();
            var firstWord = words.First(word => !string.IsNullOrWhiteSpace(word));
            int pageNumber;
            if(int.TryParse(firstWord, out int _))
            {
                pageNumber = int.Parse(firstWord);
            }
            else
            {
                var lastWord = words.Last(word => !string.IsNullOrWhiteSpace(word) && int.TryParse(word, out int _));
                if (lastWord == null) throw new Exception("Cannot find page number");
                pageNumber = int.Parse(lastWord);
            }
            return pageNumber;
        }

        private int GetRealPageNumber(int documentPageNumber)
        {
            return documentPageNumber + _pageNumberOffset;
        }

        private ContentTableItem ParseContentTableItem(string line)
        {
            var words = line.Split(' ').ToList();          
            ContentTableItem item = new ContentTableItem()
            {
                Number = char.IsDigit( words[0][0])?words[0]:"",
                Title = string.Join(" ",
                        words.Where(word => !string.IsNullOrWhiteSpace(word) &&
                                            !char.IsDigit(word[0]) && 
                                             word.Count(x => x == '.') < 3
                                             )
                        ),
                Page = GetRealPageNumber(int.Parse(words.Last(word => !string.IsNullOrWhiteSpace(word)).Replace(".", "")))
            };
            return item;
        }

        private List<string> GetPageLines(int pageNumber)
        {
            var page = _document.GetPage(pageNumber);
            var text = _textParser.GetText((List<Letter>)page.Letters);
            var lines = text.Split('\n')
                        .Select(line => line.Replace("\r", ""))
                        .Where(line => !string.IsNullOrWhiteSpace(line))                                             
                        .ToList();
            return lines;
        }

        private void ParseAllContent(int firstPage)
        {
            var firstEntryPage = ParseContentTableItem(_parsedLines[0]).Page;
            for (int i = firstPage + 1; i < firstEntryPage; i++)
            {
                _parsedLines.AddRange(GetPageLines(i).Where((line, index) => index > 0));
            }
        }

        private ContentTableItem GetNextOnTheSameLevel(ContentTableItem item)
        {
            var digits = item.Number.Split('.').Length;
            var currentIndex = _contentItems.IndexOf(item);
            return _contentItems.FirstOrDefault((listItem) =>
            {
                var listDigits = listItem.Number.Split('.').Length;
                return _contentItems.IndexOf(listItem) > currentIndex && listDigits == digits;
            });
        }

        private List<int> GetPages()
        {
            string[] whiteList = new string[] { "ТЕК","НАСТР","БД", "Общесистем", "Вычисляемые", "вычисляемые", "параметр", "настроечн", "Настроечн", "Текущ" };
            string[] blackList = new string[] { "Приложение", "списки", "список", "Списки", "Список", "контр", "Контр","Структур","Ввод","Режим", "режим", "справка", "Справка" };

            var pages = new SortedSet<int>();
            foreach (var item in _contentItems)
            {
                if (whiteList.Any(word => item.Title.Contains(word)) &&
                !blackList.Any(word => item.Title.Contains(word)))
                {
                    var startPage = item.Page;
                    var endPage = GetNextOnTheSameLevel(item).Page;
                    for (int j = startPage; j <= endPage; j++)
                    {
                        pages.Add(j);
                    }
                }       
            }
            foreach (var item in _contentItems)
            {
                if (blackList.Any(word => item.Title.Contains(word)))
                {
                    var startPage = item.Page;
                    var prevIndex = _contentItems.IndexOf(item) - 1;
                    bool keepFirstPage = true;
                    if(prevIndex >= 0 && _contentItems[prevIndex].Number.Split('.').Length < item.Number.Split('.').Length)
                    {
                        keepFirstPage = false;
                    }
                    int endPage;
                    if (_contentItems.IndexOf(item) + 1 >= _contentItems.Count)
                    {
                        endPage = _document.NumberOfPages;
                    }
                    else
                    {
                        endPage = _contentItems[_contentItems.IndexOf(item) + 1].Page;
                    }
                     
                    for (int j = startPage + (keepFirstPage?1:0); j < endPage; j++)
                    {
                        pages.Remove(j);
                    }
                }
            }
            return pages.ToList();
        }

    }
}
