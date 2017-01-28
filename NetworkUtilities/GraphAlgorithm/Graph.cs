﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace NetworkUtilities.GraphAlgorithm {
    public class Graph {
        public SubnetworkPointPool[] SubnetworkPointPools { get; private set; }

        public Link[] Links { get; set; }


        private string getDataFromLine(string s, int n) {
            string[] stringSeparator = new string[] {" = ", " "};

            return s.Split(stringSeparator, StringSplitOptions.None)[n];
        }


        public void Load(Link[] links) {
            Links = links;
            var subnetworkPointPools = new List<SubnetworkPointPool>();
            foreach (var link in Links) {
                if (!subnetworkPointPools.Contains(link.Begin))
                    subnetworkPointPools.Add(link.Begin);
                if (!subnetworkPointPools.Contains(link.End))
                    subnetworkPointPools.Add(link.End);
            }
            SubnetworkPointPools = subnetworkPointPools.ToArray();
        }

        public void LoadDeprecated(List<string> textFile ) {
            SubnetworkPointPools = new SubnetworkPointPool[int.Parse(getDataFromLine(textFile[0], 1))];
            if (SubnetworkPointPools.Length == 0) throw new Exception("Zerowa liczba wierzchołków!");
            for (int i = 0; i < SubnetworkPointPools.Length; i++) {
                SubnetworkPointPools[i] = new SubnetworkPointPool(i + 1);
            }

            Links = new Link[int.Parse(getDataFromLine(textFile[1], 1))];
            if (Links.Length == 0) throw new Exception("Zerowa liczba krawędzi!");
            for (int i = 0; i < Links.Length; i++) {
                int edge_id = int.Parse(getDataFromLine(textFile[2 + i], 0));
                int begin_id = int.Parse(getDataFromLine(textFile[2 + i], 1));
                int end_id = int.Parse(getDataFromLine(textFile[2 + i], 2));

                Links[i] = new Link(edge_id, SubnetworkPointPools[begin_id - 1], SubnetworkPointPools[end_id - 1]);
                Links[i].Begin.AddEdgeOut(Links[i]);
            }
        }
        //public void randomizeEdgesWeights()
        //{
        //    Random generator = new Random();
        //    for (int i = 0; i < _links.Length; i++)
        //    {
        //        double randomWeight = generator.NextDouble();
        //        _links[i].Weight = randomWeight;
        //    }
        //}
    }
}