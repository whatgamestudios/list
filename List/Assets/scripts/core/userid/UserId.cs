// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using System;
using System.Linq;

namespace Lists {
    public class UserId {
        public const string USER_ID = "USER_ID";
        public static (bool, string) GetUserId() {
            string userId = PlayerPrefs.GetString(USER_ID, "NOTSET");
            if (userId == "NOTSET")
            {
                userId = GenerateUserId();
                PlayerPrefs.SetString(USER_ID, userId);
                return (true, userId);
            }
            else
            {
                return (false, userId);
            }
        }

        private static string GenerateUserId() {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int length = 8;
            System.Random random = new System.Random();

            // Using LINQ to generate the set
            string randomString = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            AuditLog.Log($"Generated user id: {randomString}");
            return randomString;
        }
   }
}