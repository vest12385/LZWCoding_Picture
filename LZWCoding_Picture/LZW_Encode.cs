using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace LZWEncoder
{
    class LZW_Encode
    {
        private Dictionary<string, int> InitDictionary = new Dictionary<string, int>();
        private string output = string.Empty;
        public LZW_Encode( List<string> ContentString, string path )
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            //List<string> ContentString = Content.Select(c => c.ToString()).ToList();
            List<string> ConstentSort = new List<string>(ContentString);
            ConstentSort.Sort();
            foreach (string ecahWord in ConstentSort)
            {
                dictionary = bulidDictionary(dictionary, ecahWord);
            }
            InitDictionary = new Dictionary<string, int>(dictionary);
            encoder(dictionary, ContentString, path);
        }
        ~LZW_Encode() { }

        public Dictionary<string, int> getDictionary
        {
            get { return InitDictionary; }
        }
        public string getOutput
        {
            get { return output; }
        }

        private Dictionary<string, int> bulidDictionary( Dictionary<string, int> dictionary, string key)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key.ToString(), dictionary.Count() + 1);
            }
            return dictionary;
        }

        private string encoder( Dictionary<string, int> dictionary, List<string> Content, string path)
        {
            int pointer = 0;
            while (pointer < Content.Count())
            {
                if (dictionary.Count > 500)
                    dictionary = new Dictionary<string, int>(InitDictionary);
                List<string> oneWord = new List<string>();
                oneWord.Add(Content[pointer]);
                pointer++;
                while (dictionary.ContainsKey(String.Join(string.Empty, oneWord)) && pointer < Content.Count())
                {
                    oneWord.Add(Content[pointer++].ToString());
                }
                if (dictionary.ContainsKey(String.Join(string.Empty, oneWord)))
                {
                    output += dictionary[String.Join(string.Empty, oneWord)] + " ";
                }
                else
                {
                    string oneWordMinOne = string.Empty;
                    for (int i = 0; i < oneWord.Count() - 1; i++)
                    {
                        oneWordMinOne += oneWord[i];
                    }
                    output += dictionary[oneWordMinOne] + " ";
                    bulidDictionary(dictionary, String.Join(string.Empty, oneWord));
                    pointer--;
                }
                //using (StreamWriter sw = new StreamWriter(path + @"\EncodeDictionary.txt", true, Encoding.Default))
                //{
                //    sw.WriteLine("Now is " + Content[pointer - 1]);
                //    foreach (KeyValuePair<string, int> show in dictionary)
                //    {
                //        sw.WriteLine(show.Key + "  " + show.Value);
                //    }
                //    sw.WriteLine();
                //}
            }
            return output;
        }

    }
}
