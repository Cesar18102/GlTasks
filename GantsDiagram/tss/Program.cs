using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace tss {

    class Program {

        class Node {

            public static object e = new object();

            public int MSecToProcess { get; private set; }
            public string Name { get; private set; }
            public bool IsProcessed { get; private set; }

            public int Number { get; private set; }
            public int TabNumber { get; private set; }

            private List<Node> ChildNodes = new List<Node>();
            private List<Node> ParentNodes = new List<Node>();
            private const int SegmentDuration = 100;

            public void AddChild(Node N) {

                this.ChildNodes.Add(N);
                N.ParentNodes.Add(this);
            }

            public Node(string Name, double HoursToProcess) {

                this.MSecToProcess = (int)(HoursToProcess * 3600000);
                this.Number = Number;
                this.Name = Name;
                this.TabNumber = TabNumber;
            }

            public void Process() {

                foreach (Node i in ParentNodes)
                    if (!i.IsProcessed)
                        return;

                int SegmentsCount = MSecToProcess / SegmentDuration;


                for (int i = 0; i < SegmentsCount; i++, Thread.Sleep(SegmentDuration)) {

                    lock (e) {

                        Console.SetCursorPosition(i + TabNumber, Number);
                        Console.Write(Name[0].ToString());
                    }
                }

                IsProcessed = true;
                List<Action> ToInvoke = new List<Action>();//

                for(int i = 0; i < ChildNodes.Count; i++) {

                    int MaxNumber = ChildNodes[i].ParentNodes.Max(n => n.Number);

                    ChildNodes[i].Number = MaxNumber + i + 1;
                    ChildNodes[i].TabNumber = TabNumber + SegmentsCount;

                    ToInvoke.Add(ChildNodes[i].Process);//
                    //Thread T = new Thread(ChildNodes[i].Process);
                    //T.Start();
                }

                Parallel.Invoke(ToInvoke.ToArray());
            }

            public bool EndCheck() {

                if (!IsProcessed)
                    return false;

                foreach (Node i in ChildNodes)
                    return i.EndCheck();

                return true;
            }
        }

        static void Main(string[] args) {

            Node root = new Node("ROOT", 1.0 / 3600);

            Node A = new Node("A", 5.0 / 3600);
            Node B = new Node("B", 3.0 / 3600);
            Node C = new Node("END", 1.0 / 3600);

            root.AddChild(A);
            root.AddChild(B);

            A.AddChild(C);
            B.AddChild(C);

            DateTime Start = DateTime.Now;

            Parallel.Invoke(root.Process);//

            //Thread T = new Thread(root.Process);
            //T.Start();

            while (!root.EndCheck()) ;

            DateTime End = DateTime.Now;

            Console.SetCursorPosition(0, 20);
            Console.WriteLine("Processing took " + (End - Start).TotalMilliseconds + " msec");
        }
    }
}
