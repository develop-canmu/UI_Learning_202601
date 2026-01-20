using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.Audio
{
    public struct AudioGroup : IEquatable<AudioGroup>
    {
        private string name;
        /// <summary>ID</summary>
        public string Name
        {
            get { return name; }
        }
        
        public AudioGroup(string name)
        {
            this.name = name;
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return other is AudioGroup audioGroup && Equals(audioGroup);
        }
        
        public bool Equals(AudioGroup other)
        {
            return name == other.name;
        }

        /// <summary>Master</summary>
        public static AudioGroup Master = new AudioGroup("Master");
        /// <summary>BGM</summary>
        public static AudioGroup BGM = new AudioGroup("BGM");
        /// <summary>SE</summary>
        public static AudioGroup SE = new AudioGroup("SE");
        /// <summary>Voice</summary>
        public static AudioGroup Voice = new AudioGroup("Voice");
    }
}
