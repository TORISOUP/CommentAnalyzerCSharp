using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentAnalyzerCSharp
{

    class Program
    {
        static void Main(string[] args)
        {
            var commentAnalyzer = new CommentAnalyzer();
            var analyzeInfo = commentAnalyzer.AnalyzeComment("wktkして待ってます！");

            Console.WriteLine(analyzeInfo.type);
            Console.WriteLine(analyzeInfo.word);
        }
    }
}
