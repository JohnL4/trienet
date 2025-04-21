using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Gma.DataStructures.StringSearch;

namespace SimpleTrieConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var trie = new Trie<int>();
            //You can replace it with other trie data structures too 
            //ITrie<int> trie = new Trie<int>();
            //ITrie<int> trie = new PatriciaSuffixTrie<int>(3);


            try
            {
                //Build-up
                BuildUp("words.txt", trie);
                //BuildUp("words-one-meellion.txt", trie);

                //Look-up
                //LookUp("aaac", trie);
                //LookUp("vpvv", trie );
                //LookUp("abc", trie);
                //LookUp("aaa", trie);
                //LookUp("u", trie);
                //LookUp("i", trie);
                //LookUp("o", trie);
                //LookUp("fox", trie);
                //LookUp("overs", trie);
                //LookUp("porta", trie);
                //LookUp("supercalifragilisticexpialidocious", trie);

                var prefixes = trie.Partition( 2, "" );
                Console.WriteLine( "Prefixes:\t"
                                   + string.Join( "\t", prefixes ) );
                //+ prefixes.Aggregate( new StringBuilder(),
                //                      ( sb, s ) => sb.Append( "\t" ).Append( s ) ) );

            }
            catch (IOException ioException) { Console.WriteLine("Error: {0}", ioException.Message); }
            catch (UnauthorizedAccessException unauthorizedAccessException) { Console.WriteLine("Error: {0}", unauthorizedAccessException.Message); }
    
            Debug.WriteLine( "Done." );
            Console.WriteLine("-------------Press any key to quit--------------");
            Console.ReadKey();
        }

        private static void BuildUp(string fileName, Trie<int> trie)
        {
            IEnumerable<WordAndLine> allWordsInFile = GetWordsFromFile(fileName);
            int i = 1;
            foreach (WordAndLine wordAndLine in allWordsInFile)
            {
                trie.Add(wordAndLine.Word, wordAndLine.Line);
                Debug.Assert( trie.DescendantsCount == i, "trie.DescendantsCount == i" );
                i++;
            }
        }

        private static void LookUp(string searchString, Trie<int> trie)
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Look-up for string '{0}'", searchString);
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var result = trie.Retrieve(searchString).ToArray();
            stopWatch.Stop();

            string matchesText = String.Join(",", result);
            int matchesCount = result.Count();

            if (matchesCount == 0)
            {
                Console.WriteLine("No matches found.\tTime: {0}", stopWatch.Elapsed);
            }
            else
            {
                Console.WriteLine(" {0} matches found. \tTime: {1}\tLines: {2}", matchesCount, stopWatch.Elapsed,
                    matchesText);
            }
        }


        private static IEnumerable<WordAndLine> GetWordsFromFile(string file)
        {
            using (StreamReader reader = File.OpenText(file))
            {
                Console.WriteLine("Processing file {0} ...", file);
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                int lineNo = 0;
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    lineNo++;
                    IEnumerable<string> words = GetWordsFromLine(line);
                    foreach (string word in words)
                    {
                        yield return new WordAndLine { Line = lineNo, Word = word };
                    }
                }
                stopWatch.Stop();
                Console.WriteLine("Lines:{0}\tTime:{1}", lineNo, stopWatch.Elapsed);
            }
        }

        private static IEnumerable<string> GetWordsFromLine(string line)
        {
            var word = new StringBuilder();
            foreach (char ch in line)
            {
                // TODO: The following should really be something like "IsInAlphabet()" or "! IsWhiteSpace", if anything
                // at all for our special case of filename-per-line.  (Filenames can contain just about any character.)
                if (char.IsLetterOrDigit(ch))       
                {
                    word.Append(ch);
                }
                else
                {
                    if (word.Length == 0) continue;
                    yield return word.ToString();
                    word.Clear();
                }
            }
            if (word.Length == 0) yield break;
            yield return word.ToString();
        }

        private struct WordAndLine
        {
            public int Line;
            public string Word;
        }
    }
}
