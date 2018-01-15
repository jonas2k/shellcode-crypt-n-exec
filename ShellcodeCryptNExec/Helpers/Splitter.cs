using System.Collections.Generic;
using System.Linq;

namespace ShellcodeCryptNExec.Helpers {
    class Splitter {

        public List<string> GetEncodedParts(string s) {

            if (s.Length >= 12) {
                return SplitInThreeEvenParts(s);
            } else {
                return new List<string>();
            }
        }

        private List<string> SplitInThreeEvenParts(string s) {

            List<string> list = new List<string>();

            int stringLength = s.Length;
            int thirdOfLength = stringLength / 3;
            int offset = stringLength % 3;

            list.Add(s.Substring(0, thirdOfLength));
            list.Add(s.Substring(thirdOfLength, thirdOfLength));

            int max = offset == 0 ? thirdOfLength : thirdOfLength + offset;
            list.Add(s.Substring(thirdOfLength * 2, max));
            return EnsureDivisibilityByFour(list);
        }

        private List<string> EnsureDivisibilityByFour(List<string> list) {

            for (int i = 0; i < list.Count; i++) {

                string part = list[i];

                if (!part.Equals(list.Last())) {

                    string nextPart = list[i + 1];

                    while (IsNotDivisableByFour(part)) {

                        char firstCharOfNextPart = nextPart.First();
                        nextPart = nextPart.Substring(1, nextPart.Length - 1);
                        part += firstCharOfNextPart;
                        list[i] = part;
                        list[i + 1] = nextPart;
                    }

                } else {

                    while (IsNotDivisableByFour(part)) {
                        part += "=";
                        list[i] = part;
                    }
                }
            }
            return list;
        }

        private bool IsNotDivisableByFour(string part) {
            return part.Length % 4 != 0;
        }
    }
}