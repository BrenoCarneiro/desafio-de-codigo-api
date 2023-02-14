using System.Text;

namespace LacunaGenetics.Services
{
    public class JobService
    {
        public string DecodeToString(string strandEncoded)
        {
            byte[] bytes = Convert.FromBase64String(strandEncoded);
            string hex = BitConverter.ToString(bytes).Replace("-", "");
            StringBuilder binaryStrand = new StringBuilder();

            foreach (char c in hex.ToCharArray())
            {
                string tempData = string.Empty;
                tempData = Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2);
                while (tempData.Length < 4)
                {
                    tempData = "0" + tempData;
                }
                if (tempData.StartsWith("00"))
                    binaryStrand.Append("A");
                if (tempData.StartsWith("01"))
                    binaryStrand.Append("C");
                if (tempData.StartsWith("11"))
                    binaryStrand.Append("T");
                if (tempData.StartsWith("10"))
                    binaryStrand.Append("G");

                if (tempData.EndsWith("00"))
                    binaryStrand.Append("A");
                if (tempData.EndsWith("01"))
                    binaryStrand.Append("C");
                if (tempData.EndsWith("11"))
                    binaryStrand.Append("T");
                if (tempData.EndsWith("10"))
                    binaryStrand.Append("G");
            }

            return binaryStrand.ToString();
        }

        public string EncodeToBinary(string encoded)
        {
            string result = "0b";
            result += encoded.Replace("A", "00").Replace("C", "01").Replace("T", "11").Replace("G", "10");
            return result;
        }

        public bool CheckGene(string geneEncodede, string strandEncoded)
        {
            string strand = DecodeToString(strandEncoded);
            StringBuilder templateStrand = new StringBuilder();
            if (!strand.StartsWith("CAT"))
            {
                foreach (char c in strand.ToCharArray())
                {
                    if (c == 'G')
                        templateStrand.Append("C");
                    if (c == 'T')
                        templateStrand.Append("A");
                    if (c == 'A')
                        templateStrand.Append("T");
                    if (c == 'C')
                        templateStrand.Append("G");
                }
                strand = templateStrand.ToString();
            }
            string geneDecoded = DecodeToString(geneEncodede);
            string substring;
            int length = geneDecoded.Length;
            bool isActivated = false;
            for (int i = 0; i < (geneDecoded.Length / 2)+1; i++)
            {
                substring = geneDecoded.Substring(i, (length / 2) + i + 1);
                if (strand.Contains(substring))
                {
                    isActivated = true;
                    break;
                }
            }

            return isActivated;
        }
    }
}
