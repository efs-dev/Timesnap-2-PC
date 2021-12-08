using System;
using System.Collections.Generic;
using Assets.Src.Utils;
using ModestTree;
using UnityEngine;

namespace Src.Scripts.OnScreenKeyboard
{
    public class KeyboardKeyLayoutSetter : MonoBehaviour
    {
        public List<CharKey> Keys;
        public LayoutEnum Layout;

        private void OnEnable()
        {
            DoLayout();
        }

        private void DoLayout()
        {
            var keys = GetKeys();
            Assert.IsEqual(keys.Length, Keys.Count, "Must have enough keys");
            foreach (var result in keys.Zip(Keys))
            {
                result.Item2.KeyValue = result.Item1;
            }
        }

        private string GetKeys()
        {
            switch (Layout)
            {
                case LayoutEnum.Alphabetical:
                    return "abcdefghijklmnopqrstuvwxyz";
                case LayoutEnum.Querty:
                    return "qwertyuiopasdfghjklzxcvbnm";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public enum LayoutEnum
        {
            Alphabetical,
            Querty
        }

        private void OnValidate()
        {
            DoLayout();
        }
    }
}