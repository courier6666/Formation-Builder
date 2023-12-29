using System;
using System.Collections.Generic;
using System.Text;

namespace CourseworkWPF.MilitaryFolder
{
    [Serializable]
        public enum RankType
        {
            Enlisted = 0,
            NonCommisioned = 1,
            Warrant = 2,
            Officer = 3
        }
    [Serializable]
        public class Rank
        {
            
            RankType type;
            string s_RankName;
            int n_RankValue;
            string s_imagePath;
            public RankType Type
            {
                get
                {
                    return this.type;
                }
                set
                {
                    this.type = value;
                }
            }

            public string RankName
            {
                get
                {
                    return this.s_RankName;
                }
                set
                {
                    this.s_RankName = value;
                }
            }

            public int RankValue
            {
                get
                {
                    return this.n_RankValue;
                }
                set
                {
                    this.n_RankValue = value;
                }
            }
            public string ImagePath
            {
                get
                {
                    return this.s_imagePath;
                }
                set
                {
                    this.s_imagePath = value;
                }
            }

        private Rank(RankType type, string s_RankName, int n_RankValue, string s_imagePath)
            {
                this.type = type;
                this.s_RankName = s_RankName;
                this.n_RankValue = n_RankValue;
                this.s_imagePath = s_imagePath;
            }
            public static Rank CreateRank(RankType type,  string s_RankName, int n_RankValue, string s_imagePath)
            {
                return new Rank(type, s_RankName, n_RankValue, s_imagePath);
            }
        }
        public class RankFactory
        {
            Dictionary<string, Rank> ranks = new Dictionary<string, Rank>();
            public void AddNewRank(Rank rank)
            {
                ranks.Add(rank.RankName, rank);
            }
            public Rank GetRank(string s_rankName)
            {
                if (!this.ranks.ContainsKey(s_rankName)) return null;
                return ranks[s_rankName];
         
            }
        
            public void RemoveRank(Rank rank)
            {
                this.ranks.Remove(rank.RankName);
            }
            public Dictionary<string, Rank> AllRanks
            {
                get
                {
                    return this.ranks;
                }
                set
                {
                    this.ranks = value;
                }
            }
        }
}
