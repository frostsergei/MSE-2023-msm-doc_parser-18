using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserCore
{
    public class Data
    {
        public struct Parameter
        {
            public string Name;
            public string Range;
            public string Description;
        
        }

        private List<Parameter> _parameters = new List<Parameter>();

        public List<Parameter> ReadAll()
        {
            return _parameters;
        }
        public Parameter ReadElem(int index)
        {
            return _parameters[index];
        }
        public void WriteElem(Parameter element)
        {
            _parameters.Add(element);
        }
        public void WriteElem(Parameter element, int idx)
        {
            if(idx < _parameters.Count)
                _parameters[idx] = element;
            else{
                _parameters.AddRange(Enumerable.Repeat(new Parameter(), idx - _parameters.Count - 1));
                _parameters.Add(element);
            }
        }
    }
}
