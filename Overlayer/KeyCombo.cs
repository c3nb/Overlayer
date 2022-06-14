using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Overlayer
{
    public class KeyCombo
    {
        static readonly Dictionary<string, KeyCode> codeDict;
        static KeyCombo()
        {
            codeDict = new Dictionary<string, KeyCode>();
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                codeDict[key.ToString()] = key;
        }
        int curIndex;
        KeyCode[] codes;
        int length;
        public KeyCombo(string combo)
        {
            curIndex = 0;
            length = combo.Length;
            codes = new KeyCode[length];
            combo = combo.ToUpper();
            char[] chars = combo.ToCharArray();
            for (int i = 0; i < length; i++)
            {
                char c = chars[i];
                if (char.IsWhiteSpace(c))
                    codes[i] = KeyCode.Space;
                else codes[i] = codeDict[c.ToString()];
            }
        }
        public KeyCombo(params KeyCode[] codes)
        {
            if (!codes.Any()) return;
            curIndex = 0;
            this.codes = codes;
            length = codes.Length;
        }
        public bool Check()
        {
            if (curIndex == length)
                curIndex = 0;
            if (Input.anyKeyDown && !Input.GetMouseButton(0))
            {
                if (Input.GetKeyDown(codes[curIndex]))
                    curIndex++;
                else
                    curIndex = 0;
            }
            return curIndex == length;
        }
    }
}
