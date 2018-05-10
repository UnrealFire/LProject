using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace LProject
{
    public class Interval
    {
        public readonly int fromH;
        public readonly int fromM;
        public readonly int toH;
        public readonly int toM;

        private const string REGEX = @"с +(?<fromH>\d\d?):(?<fromM>\d\d?) +до +(?<toH>\d\d?):(?<toM>\d\d?)";

        public Interval(string intervalString)
        {         
            MatchCollection matches = new Regex(REGEX).Matches(intervalString);

            if (matches.Count != 1)
            {
                throw new ApplicationException(String.Format("Couldn't match interval value: {0}", intervalString));
            }

            fromH = Int32.Parse(matches[0].Groups["fromH"].Value);
            fromM = Int32.Parse(matches[0].Groups["fromM"].Value);
            toH = Int32.Parse(matches[0].Groups["toH"].Value);
            toM = Int32.Parse(matches[0].Groups["toM"].Value);
        }

        public String ToShortString()
        {
            return $"{fromH} - {toH}";
        }

        public Interval(int _fromH, int _fromM, int _toH, int _toM)
        {
            fromH = _fromH;
            fromM = _fromM;
            toH = _toH;
            toM = _toM;
        }

        public static bool operator ==(Interval a, Interval b)
        {
            return a.fromH == b.fromH &&
                   a.fromM == b.fromM &&
                   a.toH == b.toH &&
                   a.toM == b.toM; 
        }

        public static bool operator !=(Interval a, Interval b)
        {
            return !(a == b);
        }
    }
}
