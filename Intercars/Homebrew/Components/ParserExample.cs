using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homebrew
{
    public class ParserExample : Parser
    {
        public ParserExample()
        {
            MethodToExecute = Example;
            OnExecuteCompleted += OnEndPars;
            dynamic parameter = new List<string>();
            Parametres.Push(parameter);
            Execute(100);
        }
        public void Example(dynamic obj)
        {
            List<string> asd = (List<string>)obj;
        }
        public void OnEndPars()
        {

        }
    }
}
