using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LZWDecoder
{
    class LZW_Decode
    {
        private StringBuilder input = new StringBuilder();
        private Dictionary<string, int> initDictionary = new Dictionary<string, int>();
        private string path = string.Empty;
        public LZW_Decode( Dictionary<string, int> dictionary, string output, string path)
        {
            initDictionary = new Dictionary<string, int>(dictionary);
            decoder(dictionary, output, path);
        }
        ~LZW_Decode() { }
        public string getInput
        {
            get { return String.Join(string.Empty, input); }
        }
        public string decoder(Dictionary<string, int> dictionary, string output, string path)
        {
            List<string> codeWord = output.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            int preValue = 0;
            string nowKey = string.Empty;
            string preKey = string.Empty;
            foreach (string oneCodeWord in codeWord)
            {
                if (dictionary.Count > 500)
                    dictionary = new Dictionary<string, int>(initDictionary);
                nowKey = FindKey(dictionary, Convert.ToInt32(oneCodeWord));
                preKey = FindKey(dictionary, preValue);
                if (preKey.Equals("error")) { preKey = string.Empty; }  //一開始時遇到的問題
                if (nowKey.Equals("error"))     //遇到abababababab......這種狀況
                {
                    nowKey = preKey;
                    input.Append(preKey + preKey.Substring(0, preKey.IndexOf(",") + 1));
                }
                else
                    input.Append(nowKey);
                string newKey = preKey + nowKey.Substring(0, nowKey.IndexOf(",") + 1);  //字典在長時，只需用到現在解碼的第一個字母
                if (!dictionary.ContainsKey(newKey))
                {
                    dictionary.Add( newKey, dictionary.Count() + 1 );
                }
                preValue = Convert.ToInt32(oneCodeWord);
                //using (StreamWriter sw = new StreamWriter(path + @"\DecodeDictionary.txt", true, Encoding.Default))
                //{
                //    sw.WriteLine("Now is " + oneCodeWord);
                //    foreach (KeyValuePair<string, int> show in dictionary)
                //    {
                //        sw.WriteLine(show.Key + "  " + show.Value);
                //    }
                //    sw.WriteLine();
                //}
            }

            return input.ToString();
        }
        private string FindKey( Dictionary<string, int> dictionary, int oneCodeWord)
        {
            foreach (KeyValuePair<string, int> Key in dictionary)
            {
                if (Key.Value == oneCodeWord)
                {
                    return Key.Key;
                }
            }
            return "error";
        }
    }
}
