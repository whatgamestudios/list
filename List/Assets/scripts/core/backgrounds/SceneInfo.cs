// Copyright (c) Whatgame Studios 2024 - 2026
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Lists {
    public class SceneInfo {
        public string resource;
        public UnityEngine.Color faceColour;
        public UnityEngine.Color outlineColour;
        public string name;
        public string series;
        public string rarity;
        public uint maxSupply;
        public string artist;

        public SceneInfo(string resource, UnityEngine.Color faceColour, UnityEngine.Color outlineColour) {
            this.resource = resource;
            this.faceColour = faceColour;
            this.outlineColour = outlineColour;
        }

        public void SetMetadata(string name, string series, string rarity, uint maxSupply, string artist) {
            this.name = name;
            this.series = series;
            this.rarity = rarity;
            this.maxSupply = maxSupply;
            this.artist = artist;
        }
    }
}