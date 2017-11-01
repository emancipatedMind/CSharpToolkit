﻿namespace CSharpToolkit.Algorithms {
    public class BoyerMoore {

        const int ALPHABET_SIZE = 256;

        public int IndexOf(byte[] haystack, byte[] needle) {

            if (needle.Length == 0) 
                return 0;

            int[] charTable = MakeCharTable(needle);
            int[] offsetTable = MakeOffsetTable(needle);
            for (int i = needle.Length - 1; i < haystack.Length;) {
                int j;
                for (j = needle.Length - 1; needle[j] == haystack[i]; --i, --j) {
                    if (j == 0) {
                        return i;
                    }
                }

                i += System.Math.Max(offsetTable[needle.Length - 1 - j], charTable[haystack[i]]);
            }

            return -1;
        }

        private int[] MakeCharTable(byte[] needle) {

            int[] table = new int[ALPHABET_SIZE];
            for (int i = 0; i < table.Length; ++i) {
                table[i] = needle.Length;
            }

            for (int i = 0; i < needle.Length - 1; ++i) {
                table[needle[i]] = needle.Length - 1 - i;
            }

            return table;
        }

        private int[] MakeOffsetTable(byte[] needle) {

            int[] table = new int[needle.Length];
            int lastPrefixPosition = needle.Length;
            for (int i = needle.Length - 1; i >= 0; --i) {

                if (IsPrefix(needle, i + 1))
                    lastPrefixPosition = i + 1;

                table[needle.Length - 1 - i] = lastPrefixPosition - i + needle.Length - 1;
            }

            for (int i = 0; i < needle.Length - 1; ++i) {
                int slen = SuffixLength(needle, i);
                table[slen] = needle.Length - 1 - i + slen;
            }

            return table;
        }

        private bool IsPrefix(byte[] needle, int p) {

            for (int i = p, j = 0; i < needle.Length; ++i, ++j)
                if (needle[i] != needle[j])
                    return false;

            return true;
        }

        private int SuffixLength(byte[] needle, int p) {

            int len = 0;
            for (int i = p, j = needle.Length - 1; i >= 0 && needle[i] == needle[j]; --i, --j)
                len += 1;

            return len;
        }

    }
}
