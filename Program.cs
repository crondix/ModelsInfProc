using System;
using System.Collections;
using System.Collections.Generic;

class Operator
{
    public int QueueLength
    {
        get
        {
           return Calls.Count;
        }
    }
    public int? now{  get; set; }
    public Queue<int> Calls { get; } = new Queue<int>();
    public bool isFree()
    {
        return now==0|| now == null;
    }
    public bool isQueueFree()
    {
        return Calls.Count < 2;
    }

}
class Program
{
    static void Main()
    {
        Operator[] operators = new Operator[]
        {
            new Operator (),
            new Operator (),
            new Operator (),
            new Operator (),
            new Operator ()
        };
          Random random = new Random();
       
        int callTime() { return random.Next(2, 8); };
        int QuestTime() { return random.Next(10, 50); }
        for(int i=0; i<200; i++)
        {
            int Quest = QuestTime();
            for (int k = 0; k < operators.Length; k++)
            {
                operators[k].now = QuestTime();
            }
        }

      void Tick(Operator[] operators, int callTime)
        {
            for(int i = 0; i < operators.Length; i++)
            {
                if (operators[i].now != 0 || operators[i].now != null)
                {
                    operators[i].now -= callTime;
                    if (operators[i].now <= 0)
                    {
                        if (operators[i].QueueLength != 0)
                        {
                          operators[i].now = operators[i].Calls.Dequeue();
                        }
                            
                    }
                }
              
            }
        }
       
      int  getMinLoadId(){
            int minLoadId = 0;
            for (int i = 0; i < operators.Length; i++)
            {
                if (operators[i].isQueueFree() && operators[i].QueueLength < operators[minLoadId].QueueLength)
                {
                    minLoadId = operators[i].QueueLength;
                }
              
            }
            return minLoadId;
        }





        int test = 0;
        
        for (int i = 0; i < 15; i++)
        {
           test+= ;
        }
        // Выводим полученное случайное значение
        Console.WriteLine("test: " + test);
    }
}