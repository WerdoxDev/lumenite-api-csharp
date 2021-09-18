using System;
using System.Collections.Generic;
using System.Linq;

namespace LumeniteApiCsharp
{
    public class Utils
    {
        private static Random _random = new();

        public static string GenerateRandomId(int length = 16, bool uperCase = true)
        {
            string chars = uperCase
                ? "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"
                : "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(x => x[_random.Next(x.Length)]).ToArray());
        }

        public static bool CheckTopic(string topic, string match, params int[] ignores)
        {
            List<string> splitTopic = topic.Split("/").ToList();
            for (int i = 0; i < ignores.Length; i++)
            {
                if(ignores[i] > splitTopic.Count) continue;
                if (i > 0)
                {
                    splitTopic.RemoveAt(ignores[i] - ignores[i - 1]);
                }
                else
                {
                    splitTopic.RemoveAt(ignores[i]);
                }
            }

            var newTopic = string.Join("/", splitTopic);
            return newTopic == match;
        }

        public static Status EmptyStatus(DeviceType type = DeviceType.None)
        {
            Status value = new Status(StatusType.Offline);
            return value;
        }
    }
}